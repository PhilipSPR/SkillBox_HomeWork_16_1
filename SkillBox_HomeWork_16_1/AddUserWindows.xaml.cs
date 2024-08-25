using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SkillBox_HomeWork_16_1
{
    /// <summary>
    /// Логика взаимодействия для AddUserWindows.xaml
    /// </summary>
    public partial class AddUserWindows : Window
    {
        public AddUserWindows()
        {
            InitializeComponent();
        }

        public AddUserWindows(DataRow row) : this()
        {
            CancelButton.Click += delegate { this.DialogResult = false; };
            AddButton.Click += delegate
            {
                row["LastName"] = LastNameText.Text;
                row["FirstName"] = FirstNameText.Text;
                row["MiddleName"] = MiddleNameText.Text;
                row["PhoneNumber"] = PhoneNumberText.Text;
                row["Email"] = EmailText.Text;
                this.DialogResult = !false;
            };
        }
    }
}
