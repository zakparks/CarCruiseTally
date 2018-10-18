using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;

//TODO - add inputs for children's choice votes

namespace CarCruiseTally
{
    /// <summary>
    /// Interaction logic for InputForm.xaml
    /// </summary>
    public partial class InputForm
    {
        /// <summary>
        ///  list of all the ballots submitted
        /// </summary>
        private readonly List<Ballot> _ballots = new List<Ballot>();

        /// <summary>
        /// Current ordered list of all the unique cars
        /// </summary>
        private List<int> CurrentOrderedTally { get; set; }

        /// <summary>
        /// List of cars that have won something else 
        /// </summary>
        private List<int> WonOtherAward { get; set; }

        public InputForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Save input data as a ballot
        /// </summary>
        /// <param name="sender">object snder</param>
        /// <param name="e">event arguments</param>
        private void btn_submit_Click(object sender, RoutedEventArgs e)
        {
            // get information from the UI
            if (txt_carNumbers.Text == "")
            {
                return;
            }

            int[] carNumbers = Array.ConvertAll(txt_carNumbers.Text.Split(), int.Parse).Distinct().ToArray();
            int carNum = txt_CarNo.Text == "" ? 0 : int.Parse(txt_CarNo.Text);

            // save the data
            Ballot b = new Ballot(carNumbers, carNum);
            _ballots.Add(b);

            // clear the form
            ClearForm();

            // calculate current totals
            TallyCurrentBallots();
        }

        /// <summary>
        /// Calculate the cars that are in the lead based on the current ballots
        /// </summary>
        /// <param name="exclude">Optionally remove any cars in the exclude box, eg cars that won other awards</param>
        private void TallyCurrentBallots([Optional] bool exclude)
        {
            // generate a list of votes descending
            CurrentOrderedTally = _ballots.SelectMany(a => a.Top10).GroupBy(b => b).OrderByDescending(c => c.Count()).Select(d => d.Key).ToList();
            CurrentOrderedTally.RemoveAll(m => m == 0);

            // remove any cars in the exclude box, eg cars that won other awards
            if (exclude)
            {
                WonOtherAward = Array.ConvertAll(txt_Exclude.Text.Split(), int.Parse).ToList();
                CurrentOrderedTally = CurrentOrderedTally.Except(WonOtherAward).ToList();
            }

            // Get the top 20
            string top20display = "";
            for (int i = 0; i < CurrentOrderedTally.Count; i++)
            {
                //add to the string
                top20display += CurrentOrderedTally[i] + ", ";

                // insert a special character to denote that 20 cars have been selected. 
                // Sometimes things are changed after calculation and it helps to have more than 10 cars displayed
                if (i == 10)
                {
                    top20display += " | ";
                }
            }

            // add strings to the interface
            txt_curWinners.Text = top20display.Substring(0, top20display.Length - 2);
        }

        /// <summary>
        /// Clear all the input forms
        /// </summary>
        /// <param name="sender">object snder</param>
        /// <param name="e">event arguments</param>
        private void btn_clear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        /// <summary>
        /// Clear all the input forms
        /// </summary>
        private void ClearForm()
        {
            // clear the form
            txt_carNumbers.Clear();

            // reset focus
            txt_carNumbers.Focus();
        }

        /// <summary>
        /// Saves all the ballot data to a .csv file in case its needed later
        /// </summary>
        /// <param name="sender">object snder</param>
        /// <param name="e">event arguments</param>
        private void btn_export_Click(object sender, RoutedEventArgs e)
        {
            // create the save file dialog
            StringBuilder sb = new StringBuilder();
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Comma Seperated Values (*.csv)|*.csv",
                RestoreDirectory = true
            };

            // add the header line with the column titles
            sb.AppendLine("Car #, Top 10");

            // build all the rows for the csv
            foreach (var bal in _ballots)
            {
                sb.AppendLine(bal.CarNumber + "," + string.Join(" ", bal.Top10.Select(i => i.ToString())));
            }

            // open the save file dialog and save the file
            sfd.ShowDialog();
            if (sfd.FileName != "")
            {
                using (StreamWriter sw = new StreamWriter(sfd.FileName))
                {
                    sw.Write(sb);
                }
            }
        }

        /// <summary>
        /// Exit the program
        /// </summary>
        /// <param name="sender">object snder</param>
        /// <param name="e">event arguments</param>
        private void btn_exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Update the interface and recalculate the ballots
        /// </summary>
        /// <param name="sender">object snder</param>
        /// <param name="e">event arguments</param>
        private void btn_exclude_Click(object sender, RoutedEventArgs e)
        {
            TallyCurrentBallots(true);
        }

        /// <summary>
        /// Remove the last ballot entered
        /// </summary>
        /// <param name="sender">object snder</param>
        /// <param name="e">event arguments</param>
        private void btn_undoPrev_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Delete the previously entered ballot?", "Undo", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                _ballots.Remove(_ballots.Last());
            }
        }

        /// <summary>
        /// Trims trailing whitespace after a textbox is left
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
        private void txt_lostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            TextBox tx = (TextBox)sender;
            tx.Text = tx.Text.Replace("  ", " ");
            tx.Text = tx.Text.TrimEnd(' ');
        }
    }
}