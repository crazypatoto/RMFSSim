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

namespace RMFSSim.Windows
{
    /// <summary>
    /// NewMapWindow.xaml 的互動邏輯
    /// </summary>
    public partial class NewMapWindow : Window
    {
        public int MapWidth { get; private set; }
        public int MapLength { get; private set; }
        public NewMapWindow()
        {
            InitializeComponent();
        }

        private void button_create_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.MapLength = int.Parse(textbox_length.Text);
                this.MapWidth = int.Parse(textbox_width.Text);
                this.DialogResult = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid Input!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }            
        }

        private void button_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
