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
using System.Diagnostics;

namespace RMFSSim
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        #region Menu Buttons Events
        #endregion

        #region Toolbar Items Events
        #endregion

        #region MapViewer Events
        private void canvas_mapViewer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("MOuse Donw");
        }

        private void canvas_mapViewer_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void canvas_mapViewer_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void canvas_mapViewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {

        }
        #endregion

        #region Fcuntions
        #endregion

    }
}
