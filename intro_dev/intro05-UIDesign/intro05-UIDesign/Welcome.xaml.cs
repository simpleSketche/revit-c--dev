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

namespace intro05_UIDesign
{
    /// <summary>
    /// Interaction logic for Welcome.xaml
    /// </summary>
    public partial class Welcome : Window
    {
        public Welcome()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if(Username.Text == "yangyankun" && Password.Password == "shengri12")
            {
                MessageBox.Show("Login in successfully.");
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Failed Attempt.");
                DialogResult = false;
            }
        }
    }
}
