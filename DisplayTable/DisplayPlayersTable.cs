using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DisplayTable
{
    public partial class DisplayPlayersTable : Form
    {

        public DisplayPlayersTable()
        {
            InitializeComponent();
        }

        //connects the entity to the database
        private BaseballDatabaseApp.BaseballEntities dbcontext =
            new BaseballDatabaseApp.BaseballEntities();

        private void DisplayPlayersTable_Load(object sender, EventArgs e)
        {
            //turn the add and delete buttons on
            addItemButton.Enabled = true;
            deleteItemButton.Enabled = true;
            //run refresh method
            RefreshPlayers();
        }

        private void RefreshPlayers()
        {
            //if the data is empty, dont display anything
            if (dbcontext != null)
            {
                dbcontext.Dispose();
            }

            dbcontext = new BaseballDatabaseApp.BaseballEntities();

            //display the data, sort by last name and then first name
            dbcontext.Players
            .OrderBy(player => player.LastName)
            .ThenBy(player => player.FirstName)
            .Load();

            //connect the binding source to the Players database
            playerBindingSource.DataSource = dbcontext.Players.Local;
            playerBindingSource.MoveFirst();
            searchTextBox.Clear();
            minBatAverage.Clear();
            maxBatAverage.Clear();
        }

        private void playerBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            Validate();
            playerBindingSource.EndEdit();

            //validate the entries and then save the data
            try
            {
                dbcontext.SaveChanges();
            }
            catch (DbEntityValidationException)
            {
                MessageBox.Show("Entries cannot be empty", "Entity Validation Exception");
            }
            RefreshPlayers();
        }

        private void batAverageButton_Click(object sender, EventArgs e)
        {
            string minBatAvg = minBatAverage.Text;
            string maxBatAvg = maxBatAverage.Text;

            //if either min or max contains something that is not a digit,
            //display the message
            if (!minBatAvg.Any(Char.IsDigit) || (!maxBatAvg.Any(Char.IsDigit)))
                {
                MessageBox.Show("Can only choose batting averages that are numbers.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                minBatAverage.Clear();
                maxBatAverage.Clear();
            }
            else
            {
                var minAverage = Decimal.Parse(minBatAvg);
                var maxAverage = Decimal.Parse(maxBatAvg);

                //search for players with batting averages with the min and max
                //declared by the user
                var battingAverage =
                    from player in dbcontext.Players
                    where (player.BattingAverage >= minAverage) && (player.BattingAverage <= maxAverage)
                    orderby player.BattingAverage
                    select player;

                //display the batting averages
                playerBindingSource.DataSource = battingAverage.ToList();

                //disable the add and delete buttons
                addItemButton.Enabled = false;
                deleteItemButton.Enabled = false;
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            //enable add and delete button and refresh the data
            addItemButton.Enabled = true;
            deleteItemButton.Enabled = true;
            RefreshPlayers();
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
             string userSearch = searchTextBox.Text;

            //make sure user is searching for something
            if (userSearch.Length <= 0)
            {
                //if nothing is typed in, present this message to user
                //and have them type it in again
                MessageBox.Show("Must enter in a last name to search", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                //last names can only contain letters, if not, display message
                if ((userSearch.Any(Char.IsDigit)) || (userSearch.Any(Char.IsSymbol)) || (userSearch.Any(Char.IsPunctuation)))
                {
                    MessageBox.Show("Last Names can only contain letters. Please search again.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    searchTextBox.Clear();
                }
                else
                {
                    //search for last name
                    var lastNameQuery =
                        from player in dbcontext.Players
                        where player.LastName.StartsWith(userSearch)
                        orderby player.LastName, player.FirstName
                        select player;

                    playerBindingSource.DataSource = lastNameQuery.ToList();

                    addItemButton.Enabled = false;
                    deleteItemButton.Enabled = false;
                }
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            //close application
            Close();
        }

        private void deleteItemButton_Click(object sender, EventArgs e)
        {

        }

        private void addItemButton_Click(object sender, EventArgs e)
        {

        }
    }
}
