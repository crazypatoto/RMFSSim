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
using RMFSSim.RMFS;
using RMFSSim.RMFS.Maps;
using RMFSSim.Windows;
using RMFSSim.RMFS.Visualizations;

namespace RMFSSim
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public RMFSCore RMFS { get; private set; }
        public RMFSVisualization RMFSVisualization { get; private set; }        
        
        public MainWindow()
        {
            InitializeComponent();            
            btn_saveMap.IsEnabled = false;
            btn_saveMapAs.IsEnabled = false;
            toolbar_mapEdit.IsEnabled = false;
            toolbar_simulationControl.IsEnabled = false;
        }

        #region Menu Buttons Events
        #endregion

        #region Toolbar Items Events
        private void btn_newMap_Click(object sender, RoutedEventArgs e)
        {
            var newMapWindow = new NewMapWindow();
            newMapWindow.Owner = this;
            newMapWindow.ShowDialog();

            bool? result = newMapWindow.DialogResult;
            if (result == true)
            {
                this.RMFS = new RMFSCore(newMapWindow.MapWidth, newMapWindow.MapLength);
                this.RMFSVisualization = new RMFSVisualization(this.RMFS, canvas_mapViewer);
                this.RMFSVisualization.DrawMap();
                btn_saveMap.IsEnabled = true;
                btn_saveMapAs.IsEnabled = true;
                toolbar_mapEdit.IsEnabled = true;
                toolbar_simulationControl.IsEnabled = true;
            }
        }

        private void btn_openMap_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            dialog.Title = "Open New Map";
            dialog.DefaultExt = "*.map";
            dialog.Filter = "Map Files (*.map)|*.map";

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                this.RMFS = new RMFSCore(dialog.FileName);
                this.RMFSVisualization = new RMFSVisualization(this.RMFS, canvas_mapViewer);
                this.RMFSVisualization.DrawMap();
                btn_saveMap.IsEnabled = true;
                btn_saveMapAs.IsEnabled = true;
                toolbar_mapEdit.IsEnabled = true;
                toolbar_simulationControl.IsEnabled = true;
            }
        }

        private void btn_saveMap_Click(object sender, RoutedEventArgs e)
        {
            if (this.RMFS == null) return;
            if (this.RMFS.CurrentMap.FilePath == null)
            {
                btn_saveMapAs_Click(null, null);
            }
            else
            {
                this.RMFS.CurrentMap.Save();
            }
        }

        private void btn_saveMapAs_Click(object sender, RoutedEventArgs e)
        {
            if (this.RMFS == null) return;
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            dialog.Title = "Save Current Map";
            dialog.DefaultExt = "*.map";
            dialog.Filter = "Map Files (*.map)|*.map";

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                this.RMFS.CurrentMap.SaveToFile(dialog.FileName);
            }
        }


        private void btn_addNone_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_addWall_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_addStorage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_addPickStation_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_addReplenishmentStation_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_addChargeStation_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_addAGV_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_addPod_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Add poD");
        }

        private void btn_startSim_Click(object sender, RoutedEventArgs e)
        {
            btn_startSim.Background = new SolidColorBrush(Colors.Green);
            toolbar_mapEdit.IsEnabled = false;
        }

        private void btn_pauseSim_Click(object sender, RoutedEventArgs e)
        {
            btn_pauseSim.Background = new SolidColorBrush(Colors.Orange);
        }

        private void btn_stopSim_Click(object sender, RoutedEventArgs e)
        {
            btn_stopSim.Background = new SolidColorBrush(Colors.Red);
            toolbar_mapEdit.IsEnabled = true;
        }

        private void btn_slowDown_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_speedUp_Click(object sender, RoutedEventArgs e)
        {
            var target = canvas_mapViewer.Children.OfType<Ellipse>().
                SingleOrDefault(child => child.Tag.ToString() == "None_003-003");
            if (target != null)
            {
                target.Fill = Brushes.Red;
            }
        }
        #endregion

        #region MapViewer Canvas Events
        private void canvas_mapViewer_MouseDown(object sender, MouseButtonEventArgs e)
        {

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

        #region Fucntions
        #endregion

    }
}
