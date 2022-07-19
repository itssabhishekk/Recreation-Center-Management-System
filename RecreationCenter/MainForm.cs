using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace RecreationCenter
{
    public partial class MainForm : Form
    {
        public static XmlSerializer priceSerializer;
        public static List<Price> priceList;


        public static XmlSerializer entrySerializer;
        public static List<Entry> entry;
        public MainForm()
        {
            InitializeComponent();
            mainTab.Appearance = TabAppearance.FlatButtons;
            mainTab.ItemSize = new Size(0, 1);
            mainTab.SizeMode = TabSizeMode.Fixed;

            foreach (TabPage tab in mainTab.TabPages)
            {
                tab.Text = "";
            }
            priceSerializer = new XmlSerializer(typeof(List<Price>));
            try
            {
                FileStream fileStream = new FileStream("D:/Islington College/Semester 5 and 6/App Dev/Coursework1/Price.xml", FileMode.Open, FileAccess.Read);
                //while running on a different computer, make sure to change the path of the filestream

                priceList = (List<Price>)priceSerializer.Deserialize(fileStream);
                fileStream.Close();

            }
            catch (Exception e)
            {
                priceList = new List<Price>();

            }

            BindingSource bsprice = new BindingSource();
            bsprice.DataSource = priceList;
            dataGridViewTicketPrice.DataSource = bsprice;

            entrySerializer = new XmlSerializer(typeof(List<Entry>));
            try
            {
                FileStream fileStream = new FileStream("D:/Islington College/Semester 5 and 6/App Dev/Coursework1/Entry.xml", FileMode.Open, FileAccess.Read);

                entry = (List<Entry>)entrySerializer.Deserialize(fileStream);
                fileStream.Close();

            }
            catch (Exception e)
            {
                entry = new List<Entry>();

            }
            BindingSource bs = new BindingSource();
            bs.DataSource = entry;
            dataGridViewEntry.DataSource = bs;


            BindingSource checkOut = new BindingSource();
            checkOut.DataSource = entry;
            dataGridView2.DataSource = checkOut;

            UpdateVisitorIDs();
            GetID();
            EnteredTime();
            totalAmountTextbox.Clear();
        }

        private void adultTextbox_TextChanged(object sender, EventArgs e)
        {

        }

        //This method is a click event for the button to save the prices that the admin input to the xml file.
        private void saveButton_Click(object sender, EventArgs e)
        {
            FileStream fileStream = new FileStream("D:/Islington College/Semester 5 and 6/App Dev/Coursework1/Price.xml", FileMode.Create, FileAccess.Write);
            Price pricing = new Price();

            if (textBoxCount.Text != "" && childrenWeekdaysTextbox.Text != "" && childrenWeekendsTextbox.Text != "" && adultWeekdaysTextbox.Text != "" && adultWeekendsTextbox.Text != "" && comboBoxHr.Text != "")
            {
                pricing.Persons = int.Parse(textBoxCount.Text);
                pricing.Hours = int.Parse(comboBoxHr.Text);
                pricing.ChildPriceOnWeekdays = int.Parse(childrenWeekdaysTextbox.Text);
                pricing.AdultPriceOnWeekdays = int.Parse(adultWeekdaysTextbox.Text);
                pricing.ChildPriceOnWeekends = int.Parse(childrenWeekendsTextbox.Text);
                pricing.AdultPriceOnWeekends = int.Parse(adultWeekendsTextbox.Text);



                Boolean check = true;

                foreach (var detail in priceList)
                {

                    if (detail.Persons.Equals(pricing.Persons) & detail.Hours.Equals(pricing.Hours))
                    {
                        MessageBox.Show("Duplicate data");
                        check = false;
                    }
                }
                if (check == true)
                {
                    priceList.Add(pricing);
                    priceSerializer.Serialize(fileStream, priceList);
                    MessageBox.Show("Price added");
                    BindingSource bsprice = new BindingSource();
                    bsprice.DataSource = priceList;
                    dataGridViewTicketPrice.DataSource = bsprice;
                }
            }
            else {
                MessageBox.Show("Please fill all the fields.");
            }
            fileStream.Close();
        }

        //This method is a click event for the button to check-in the visitors that the admin/employee input. 
        private void checkInButton_Click(object sender, EventArgs e)
        {
 
            FileStream newfileStream = new FileStream("D:/Islington College/Semester 5 and 6/App Dev/Coursework1/Entry.xml", FileMode.Create, FileAccess.Write);
            Entry value = new Entry();

            //validation done so that none of the fields are kept empty.
            if (visitorIdTextBox.Text != "" && nameTextbox.Text != "" && childrenTextbox.Text != "" && adultTextbox.Text != "")
            {

                value.EntryDate = entryDatePicker.Value;
                value.ID = int.Parse(visitorIdTextBox.Text);
                value.Name = nameTextbox.Text;
                value.Child = int.Parse(childrenTextbox.Text);
                value.Adult = int.Parse(adultTextbox.Text);
                value.EntryTime = entryTimePicker.Value;
                value.VisitorsNum = value.Child + value.Adult;
                value.Exit = false;

                Boolean check = true;

                foreach (var data in entry)
                {

                    if (data.ID.Equals(value.ID))
                    {
                        MessageBox.Show("Duplicate data");
                        check = false;
                    }
                }
                if (check == true)
                {
                    entry.Add(value);
                    entrySerializer.Serialize(newfileStream, entry);
                    MessageBox.Show("Data added");
                    UpdateVisitorIDs();
                    GetID();
                    EnteredTime();
                    BindingSource bs = new BindingSource();
                    bs.DataSource = entry;
                    dataGridViewEntry.DataSource = bs;

                }
            }

            else
            {
                MessageBox.Show("Please fill all the fields.");            
            }
            newfileStream.Close();
        }

        //This method is a click event for the button to confirm the visitors that wants to check out. This method calculates and display total hours, total number of visitor and total cost to be paid.
        private void confirmButton_Click(object sender, EventArgs e)
        {
            try
            {

                int visitorID = int.Parse(visitorIdComboBox.Text);

                foreach (var data in entry)
                {
                    if (data.ID.Equals(visitorID))
                    {
                        var tempTime = exitTimePicker.Value;

                        //Calculating total hours spent by visitor for price 
                        var visitingHour = (tempTime - data.EntryTime).Hours;
                        var remainingMinutes = (tempTime - data.EntryTime).Minutes;

                        if (remainingMinutes > 30)
                        {
                            visitingHour += 1;
                        }

                        noOfVisitorTextbox2.Text = data.VisitorsNum.ToString();
                        entryTimePicker2.Text = data.EntryTime.ToShortTimeString();

                        totalHoursTextbox.Text = visitingHour.ToString();


                        var CalculatedAmount = 0;
                        var WeekDayNumber = (int)data.EntryDate.DayOfWeek;
                        Console.WriteLine(WeekDayNumber);

                        foreach (var pricing in priceList)
                        {
                            if ((data.VisitorsNum == pricing.Persons) & (totalHoursTextbox.Text.Equals(pricing.Hours.ToString())))
                            {
                                if (WeekDayNumber == 6 || WeekDayNumber == 0)
                                {

                                    CalculatedAmount = data.Adult * pricing.AdultPriceOnWeekends + data.Child * pricing.ChildPriceOnWeekends;
                                    break;
                                }
                                else
                                {
                                    CalculatedAmount = data.Adult * pricing.AdultPriceOnWeekdays + data.Child * pricing.ChildPriceOnWeekends;
                                    break;
                                }
                            }


                        }
                        totalAmountTextbox.Text = CalculatedAmount.ToString();
                        checkOutButton.Enabled = true;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Please select a bill number to proceed.");
            }

        }

        //This method is a click event for the button to switch to the check-in page of the application.
        private void mainCheckInButton_Click(object sender, EventArgs e)
        {
            mainTab.SelectTab(checkInPage);

        }

        //This method is a click event for the button to switch to the check-out page of the application.
        private void mainCheckOutButton_Click(object sender, EventArgs e)
        {
            mainTab.SelectTab(checkOutPage);
        }

        //This method is a click event for the button to switch to the Ticket price page of the application.
        private void mainPriceButton_Click(object sender, EventArgs e)
        {
            mainTab.SelectTab(ticketPricePage);
        }

        //This method is a click event for the button to switch to the Report page of the application.
        private void mainReportButton_Click(object sender, EventArgs e)
        {
            mainTab.SelectTab(reportPage);
        }

        //This method is a click event for the button to log out from the application.
        private void logoutButton_Click(object sender, EventArgs e)
        {
            loginForm login = new loginForm();
            MessageBox.Show("Logged out successfully.");
            this.Hide();
            login.Show();
            this.Close();


        }

        //This method fixes the size of the form and doesn't let the user maximize it.
        private void MainForm_Load(object sender, EventArgs e)
        {
            MaximizeBox = false;
        }


        //This method is a click event for the button to check-out the visitors after getting all the costs, hours and numbers. 
        private void checkOutButton_Click(object sender, EventArgs e)
        {
            FileStream fileStreamTicketPrices = new FileStream("D:/Islington College/Semester 5 and 6/App Dev/Coursework1/Entry.xml", FileMode.Open, FileAccess.Write);
            Entry checkOutDetails = new Entry();

            int visitorID = int.Parse(visitorIdComboBox.Text);


            Boolean check = true;


            foreach (var data in entry)
            {
                if (data.ID.Equals(visitorID))
                {
                    data.EntryTime = entryTimePicker.Value;
                    data.TotalHours = int.Parse(totalHoursTextbox.Text);
                    data.ExitTime = exitTimePicker.Value;
                    data.TotalAmount = int.Parse(totalAmountTextbox.Text);
                    data.Exit = true;

                    BindingSource checkOutGBbs = new BindingSource();
                    checkOutGBbs.DataSource = entry;
                    dataGridViewEntry.DataSource = checkOutGBbs;

                    check = false;
                }
            }
            if (check == false)
            {
                entry.Append(checkOutDetails);
                entrySerializer.Serialize(fileStreamTicketPrices, entry);
                MessageBox.Show("Vistor checked out.");
                BindingSource checkInGBbs = new BindingSource();
                checkInGBbs.DataSource = entry;
                dataGridViewEntry.DataSource = checkInGBbs;
                UpdateVisitorIDs();
                GetID();
                EnteredTime();
                totalAmountTextbox.Clear();

            }
            fileStreamTicketPrices.Close();
        }

        //This method adds the visitor ID from the check-in page's textbox to the dropdown of the visitor ID in checkout page.
        private void UpdateVisitorIDs()
        {
            visitorIdComboBox.Items.Clear();
            var visitorIDList = entry.Where(a => a.Exit == false);
            foreach (var count in visitorIDList.Select(a => a.ID).Distinct())
            {
                visitorIdComboBox.Items.Add(count);
            }
        }

        //This method auto increases the visitor ID and keeps it in default in the textbox.
        private void GetID()
        {
            int ID = 0;

            try
            {
                ID = entry.Select(a => a.ID).Max();
            }
            catch
            {

            }
            ID += 1;
            visitorIdTextBox.Text = ID.ToString();
        }

        //This method selects the current time to check out when the visitor wants to check out.
        public void EnteredTime()
        {
            entryTimePicker.Value = DateTime.Now;
        }

        //This method deletes the selected price.
        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (dataGridViewTicketPrice.SelectedRows.Count == 0)
            {
                MessageBox.Show("No data selected.");
                return;
            }
            DataGridViewRow selectedRow = dataGridViewTicketPrice.SelectedRows[0];

            var delRowGroup = selectedRow.Cells[0].Value.ToString();
            var delRowHour = selectedRow.Cells[1].Value.ToString();
            priceList.RemoveAll(x => x.Persons == int.Parse(delRowGroup) & x.Hours == int.Parse(delRowHour));
            FileStream fileStreamDelPricing = new FileStream("D:/Islington College/Semester 5 and 6/App Dev/Coursework1/Price.xml", FileMode.Create, FileAccess.Write);
            priceSerializer.Serialize(fileStreamDelPricing, priceList);
            MessageBox.Show("Data Deleted");

            BindingSource bsDelPrice = new BindingSource();
            bsDelPrice.DataSource = priceList;
            dataGridViewTicketPrice.DataSource = bsDelPrice;

            fileStreamDelPricing.Close();
        }

        //this method updates the selected price.
        private void editButton_Click(object sender, EventArgs e)
        {
            comboBoxHr.Visible = false;
            updateHourTextbox.Visible = true;
            textBoxCount.ReadOnly = true;
            updateHourTextbox.ReadOnly = true;

            if (dataGridViewTicketPrice.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a price to edit");
                return;
            }
            DataGridViewRow selectedRow = dataGridViewTicketPrice.SelectedRows[0];


            textBoxCount.Text = selectedRow.Cells[0].Value.ToString();
            updateHourTextbox.Text = selectedRow.Cells[1].Value.ToString();
            childrenWeekdaysTextbox.Text = selectedRow.Cells[2].Value.ToString();
            childrenWeekendsTextbox.Text = selectedRow.Cells[4].Value.ToString();
            adultWeekdaysTextbox.Text = selectedRow.Cells[3].Value.ToString();
            adultWeekendsTextbox.Text = selectedRow.Cells[5].Value.ToString();

            saveChangeButton.Visible = true;
            editButton.Visible = false;
        }

        //This method saves the changes after updating the price.
        private void saveChangeButton_Click(object sender, EventArgs e)
        {
            FileStream fileStreamUpdatePrice = new FileStream("D:/Islington College/Semester 5 and 6/App Dev/Coursework1/Price.xml", FileMode.Create, FileAccess.Write);
            Price pricing = new Price();

            foreach (var listVar in priceList)
            {
                if (listVar.Persons == int.Parse(textBoxCount.Text) & listVar.Hours == int.Parse(updateHourTextbox.Text))
                {
                    listVar.ChildPriceOnWeekdays = int.Parse(childrenWeekdaysTextbox.Text);
                    listVar.ChildPriceOnWeekends = int.Parse(childrenWeekendsTextbox.Text);
                    listVar.AdultPriceOnWeekdays = int.Parse(adultWeekdaysTextbox.Text);
                    listVar.AdultPriceOnWeekends = int.Parse(adultWeekendsTextbox.Text);
                }

                priceList.Append(listVar);
                priceSerializer.Serialize(fileStreamUpdatePrice, priceList);
                MessageBox.Show("Price Updated.");

                BindingSource bsUpdatePrice = new BindingSource();
                bsUpdatePrice.DataSource = priceList;
                dataGridViewTicketPrice.DataSource = bsUpdatePrice;

                saveChangeButton.Visible = false;
                editButton.Visible = true;

                comboBoxHr.Visible = true;
                updateHourTextbox.Visible = false;
                textBoxCount.ReadOnly = false;

                break;
            }

            fileStreamUpdatePrice.Close();
        }

        //This method generates report as well charts.
        private void ReportGeneration(object sender, EventArgs e)
        {
            //generating daily report
            var reportDate = reportDatePicker.Value;
            var details = entry.Where(a => a.EntryDate.Date == reportDate.Date & a.Exit == true).ToList();
            var last = details.GroupBy(b => b.VisitorsNum).Select(c => new { c.Key, childVisitors = c.Sum(d => d.VisitorsNum), adultVisitors = c.Sum(d => d.Adult), totalVisitors = c.Sum(d => d.VisitorsNum) });

            BindingSource DailyVisitorRepBS = new BindingSource();
            DailyVisitorRepBS.DataSource = last;
            dailyReportGridView.DataSource = DailyVisitorRepBS;

            //generating weekly report
            var ReportGenerationWeek = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(reportDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday);
            var VisitorsList = entry.Where(a => CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(a.EntryDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday) == ReportGenerationWeek & a.Exit == true).ToList();
            var week = VisitorsList.GroupBy(b => b.EntryDate.DayOfWeek.ToString()).Select(b => new { day = b.Key, totalVisitors = b.Sum(c => c.VisitorsNum), totalEarning = b.Sum(c => c.TotalAmount) });

            BindingSource DailyEarningRepBS = new BindingSource();
            DailyEarningRepBS.DataSource = week.ToList();
            weeklyReportGridView.DataSource = DailyEarningRepBS;

            //generating chart
            visitorWeeklyChart.ChartAreas[0].AxisX.Title = "Day";
            priceWeeklyChart.ChartAreas[0].AxisX.Title = "Day";

            visitorWeeklyChart.DataSource = week.ToList();
            visitorWeeklyChart.Series[0].XValueMember = "day";
            visitorWeeklyChart.Series[0].YValueMembers = "totalVisitors";


            priceWeeklyChart.DataSource = week.ToList();
            priceWeeklyChart.Series[0].XValueMember = "day";
            priceWeeklyChart.Series[0].YValueMembers = "totalEarning";
        }

        //This method sorts the data via Bubble Sort Algorithm.
        private void BubbleSort(bool ascending, string columnName)
        {
            // create a list of the data
            List<Object> data = new List<Object>();
            BindingSource ebs = (BindingSource)weeklyReportGridView.DataSource;

            foreach (Object item in ebs)
            {
                data.Add(item);
            }

            // sort the list
            for (int i = 0; i < data.Count; i++)
            {
                // loop through the list again
                for (int j = 0; j < data.Count - 1; j++)
                {
                    // get the values from both sides
                    object leftValue = data[j].GetType().GetProperty(columnName).GetValue(data[j], null);
                    object rightValue = data[j + 1].GetType().GetProperty(columnName).GetValue(data[j + 1], null);

                    bool compare;
                    if (ascending)
                    {
                        compare = (int)leftValue > (int)rightValue;
                    }
                    else
                    {
                        compare = (int)leftValue < (int)rightValue;
                    }

                    // compare the objects based on data type
                    if (compare)
                    {
                        // swap the values
                        var temp = data[j];
                        data[j] = data[j + 1];
                        data[j + 1] = temp;
                    }
                }
            }

            // clear the bindingsource
            ebs.Clear();

            // add the sorted data back to the bindingsource
            ebs.DataSource = data;

            weeklyReportGridView.DataSource = ebs;
        }

        //This method is a click event for the button to sort the weekly report via total earnings.
        private void priceSortButton_Click(object sender, EventArgs e)
        {
            BubbleSort(true, "totalEarning");

        }


        //This method is a click event for the button to sort the weekly report via total visitors.
        private void visitorSortButton_Click(object sender, EventArgs e)
        {
            BubbleSort(true, "totalVisitors");
        }
    }
}
