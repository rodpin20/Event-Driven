using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Coffee_Project;

namespace Coffee_Project.Form_Organize
{
    public partial class Login : Form
    {
        public string LoggedInRole { get; private set; }
        public string LoggedInName { get; private set; }

        public Login()
        {
            InitializeComponent();
          
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Please enter your username.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter your password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string role = "";
            string name = "";

           
            bool success = DatabaseHelper.LoginUser(username, password, out role, out name);

            if (success)
            {
                LoggedInRole = role;
                LoggedInName = name;

                MessageBox.Show($"Welcome, {name}! Login successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Home homeForm = new Home(role, name);
                homeForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid username or password. Please try again.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();
                txtUsername.Focus();
            }
        }
    }
}
