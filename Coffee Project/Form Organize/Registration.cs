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
    public partial class Registration : Form
    {
        public Registration()
        {
            InitializeComponent();
            InitializeEventHandlers();
        }

        private void InitializeEventHandlers()
        {
       
        }

       
        private void btnRegister_Click_1(object sender, EventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Please enter a username.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter your name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter a password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            if (cmbRole.SelectedItem == null || string.IsNullOrWhiteSpace(cmbRole.SelectedItem.ToString()))
            {
                MessageBox.Show("Please select a role (Admin or Employee).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbRole.Focus();
                return;
            }

            string username = txtUsername.Text.Trim();
            string name = txtName.Text.Trim();
            string password = txtPassword.Text;
            string role = cmbRole.SelectedItem.ToString();

            // Register user to database
            bool success = DatabaseHelper.RegisterUser(username, name, password, role);

            if (success)
            {
               
                txtUsername.Clear();
                txtName.Clear();
                txtPassword.Clear();
                cmbRole.SelectedIndex = -1;
            }
        }

        private void btnLogin_Click_1(object sender, EventArgs e)
        {
            Login loginForm = new Login();
            loginForm.Show();
            this.Hide();
        }
    }
}
