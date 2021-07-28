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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ColumnGrid
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : Window
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        private void BtnEnter_Click(object sender, RoutedEventArgs e)
        {
            var inputvalue = Convert.ToInt32(userInput.Text);
            var inputvalue2 = Convert.ToInt32(userInput2.Text);
            var inputvalue3 = float.Parse(userInput3.Text);
            if (userInput.Text != null && inputvalue > 0 && userInput2.Text != null && inputvalue2 > 0
                && userInput3.Text != null && inputvalue3 > 0)
            {
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Invalid Input");
                DialogResult = false;
            }
        }
    }
}
