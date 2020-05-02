using System;
using System.Collections.Generic;
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

namespace BatchWriter
{
    /// <summary>
    /// Interaction logic for SteamCredentialsDialog.xaml
    /// </summary>
    public partial class SteamCredentialsDialog : Window
    {
        public SteamCredentialsDialog()
        {
            InitializeComponent();
        }

        public string SteamAccount
        {
            get { return SteamAccountBox.Text; }
            set { SteamAccountBox.Text = value; }
        }

        public string SteamPassword
        {
            get { return PasswordBoxBox.Password; }
            set { PasswordBoxBox.Password = value; }
        }

        public string SteamGuardCode
        {
            get { return SteamGuardBox.Text; }
            set { SteamGuardBox.Text = value; }
        }

        private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OnOK();
        }

        private void OnOK()
        {
            if (String.IsNullOrEmpty(SteamAccount))
            {
                SteamAccountBox.BorderBrush = Brushes.Tomato;
                return;
            }
            //    String.IsNullOrEmpty(SteamPassword) ||
            //    String.IsNullOrEmpty(SteamGuardCode)) return;

            DialogResult = true;
        }

        private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SteamAccountBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) Keyboard.Focus(PasswordBoxBox);
        }

        private void PasswordBoxBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) Keyboard.Focus(SteamGuardBox);
        }

        private void SteamGuardBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) OnOK();
        }
    }
}
