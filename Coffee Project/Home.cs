using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Coffee_Project
{
    public partial class Home : Form
    {
        private string userRole;
        private string userName;

        public Home()
        {
            InitializeComponent();
        }

        public Home(string role, string name) : this()
        {
            userRole = role;
            userName = name;
            UpdateWelcomeMessage();
        }

        private void UpdateWelcomeMessage()
        {
            // Update the title to show welcome message with user name and role
            if (!string.IsNullOrEmpty(userName))
            {
                lblTitle.Text = $"Welcome, {userName}!";
                lblSubTitle.Text = $"You are logged in as: {userRole}\r\nExperience the rich taste of Philippine cuisine with our carefully crafted coffee, tea, pastries, and traditional Filipino dishes.";
            }
        }
    }
}
