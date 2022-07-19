using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace RecreationCenter
{
    public partial class loginForm : Form
    {
        public static XmlSerializer xmlSerializer;
        public static List<LoginClass> credentials;
        public loginForm()
        {
            InitializeComponent();
            xmlSerializer = new XmlSerializer(typeof(List<LoginClass>));
            try
            {
                FileStream fileStream = new FileStream("D:/Islington College/Semester 5 and 6/App Dev/Coursework1/Login.xml", FileMode.Open, FileAccess.Read);


                credentials = (List<LoginClass>)xmlSerializer.Deserialize(fileStream);
                fileStream.Close();

            }
            catch (Exception e)
            {
                credentials = new List<LoginClass>();


            }

           
        }

        //This method is a click event for the button to show admin page or employee page where username and password are also validated.

        private void loginButton_Click(object sender, EventArgs e)
        {
            Boolean isLoggedIn = false;


            foreach (var detail in credentials)
            {
                //validation for employee
                if (detail.UserType.Equals("employee") & detail.Username.Equals(usernameTextbox.Text) & detail.Password.Equals(passwordTextbox.Text))
                {
                    isLoggedIn = true;
                    MessageBox.Show("Logged in successfully as employee.");
                    MainForm main = new MainForm();
                    this.Hide();
                    main.Show();
                    main.mainPriceButton.Visible = false;
                    main.mainReportButton.Visible = false;


                }
                //validatiopn for admin
                else if (detail.UserType.Equals("admin") & detail.Username.Equals(usernameTextbox.Text) & detail.Password.Equals(passwordTextbox.Text))
                {
                    isLoggedIn = true;
                    MessageBox.Show("Logged in successfully as Admin.");
                    MainForm main = new MainForm();
                    this.Hide();
                    main.ShowDialog();

                }
                else
                {
                    Console.WriteLine(detail.Username);
                }


            }
            if (isLoggedIn == false)
            {
                //displaying error if the username or password doesnt match the given credentials
                MessageBox.Show("Invalid Userame/Password. Please enter correct Username or Password.");
            }


        }

        //This method fixes the size of the form and doesn't let the user maximize it.
        private void loginForm_Load(object sender, EventArgs e)
        {
            MaximizeBox = false;
        }
    } }
