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

        private int _editModeFlag = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region Menu Buttons Events
        #endregion

        #region Toolbar Items Events
        private void btn_newMap_Click(object sender, RoutedEventArgs e)
        {
            CreateNewMap();
        }

        private void btn_openMap_Click(object sender, RoutedEventArgs e)
        {
            OpenExistingMap();
        }

        private void btn_saveMap_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentMap();
        }

        private void btn_saveMapAs_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentMapAs();
        }
        private void btn_selectMode_Click(object sender, RoutedEventArgs e)
        {
            ChangeEditingMode(0);
        }

        private void btn_addNone_Click(object sender, RoutedEventArgs e)
        {
            ChangeEditingMode(1);
        }


        private void btn_addStorage_Click(object sender, RoutedEventArgs e)
        {
            ChangeEditingMode(2);
        }

        private void btn_addPickStation_Click(object sender, RoutedEventArgs e)
        {
            ChangeEditingMode(3);
        }

        private void btn_addReplenishmentStation_Click(object sender, RoutedEventArgs e)
        {
            ChangeEditingMode(4);
        }

        private void btn_addChargeStation_Click(object sender, RoutedEventArgs e)
        {
            ChangeEditingMode(5);
        }
        private void btn_bidirection_Click(object sender, RoutedEventArgs e)
        {

            ChangeEditingMode(6);
        }
        private void btn_onedirection_Click(object sender, RoutedEventArgs e)
        {
            ChangeEditingMode(7);
        }
        private void btn_nopassing_Click(object sender, RoutedEventArgs e)
        {
            ChangeEditingMode(8);
        }
        private void btn_nopassing_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            foreach (var mapEdge in this.RMFS.CurrentMap.AllEdges)
            {
                mapEdge.Direction = MapEdge.Directions.None;
                this.RMFSVisualization.UpdateMapEdge(mapEdge);
            }
        }
        private void btn_addAGV_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btn_addPod_Click(object sender, RoutedEventArgs e)
        {
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

        private void btn_viewDetail_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion



        #region Fucntions        
        private void CreateNewMap()
        {
            var newMapWindow = new NewMapWindow();
            newMapWindow.Owner = this;
            newMapWindow.ShowDialog();

            bool? result = newMapWindow.DialogResult;
            if (result == true)
            {
                StartNewRMFS(new RMFSCore(newMapWindow.MapWidth, newMapWindow.MapLength));
            }
        }
        private void OpenExistingMap()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            dialog.Title = "Open New Map";
            dialog.DefaultExt = "*.map";
            dialog.Filter = "Map Files (*.map)|*.map";

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                StartNewRMFS(new RMFSCore(dialog.FileName));
            }
        }
        private void StartNewRMFS(RMFSCore rmfsCore)
        {
            this.RMFS = rmfsCore;
            this.RMFSVisualization?.Dispose();
            this.RMFSVisualization = new RMFSVisualization(this.RMFS, canvas_mapViewer);
            this.RMFSVisualization.NodeSelectedEvent += OnNodeSelected;
            this.RMFSVisualization.EdgeSelectedEvent += OnEdgeSelected;
            this.RMFSVisualization.DrawNewMap();
            menuItem_saveMap.IsEnabled = true;
            menuItem_saveMapAs.IsEnabled = true;
            btn_saveMap.IsEnabled = true;
            btn_saveMapAs.IsEnabled = true;
            toolbar_mapEdit.IsEnabled = true;
            toolbar_instanceControl.IsEnabled = true;
            toolbar_simulationControl.IsEnabled = true;
            tb_mapSN.Text = this.RMFS.CurrentMap.SerialNumber;
            tb_mapWidth.Text = this.RMFS.CurrentMap.Width.ToString();
            tb_mapLength.Text = this.RMFS.CurrentMap.Length.ToString();
            ChangeEditingMode(0);
        }
        private void SaveCurrentMap()
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

        private void SaveCurrentMapAs()
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

        /// <summary>
        /// Change editing mode.
        /// </summary>        
        /// <param name="modeFlag"> 0: Seletcion Mode, 1: None Node, 2:Storage Node, 3: Pick Station Node, 4: Replenishment Station Node, 5: Charge Station Node</param>
        private void ChangeEditingMode(int modeFlag)
        {
            btn_selectMode.Background = null;
            btn_addNone.Background = null;
            btn_addStorage.Background = null;
            btn_addPickStation.Background = null;
            btn_addReplenishmentStation.Background = null;
            btn_addChargeStation.Background = null;
            btn_bidirection.Background = null;
            btn_onedirection.Background = null;
            btn_nopassing.Background = null;
            switch (modeFlag)
            {
                case 0:
                    btn_selectMode.Background = new SolidColorBrush(Color.FromRgb(168, 175, 191));
                    break;
                case 1:
                    btn_addNone.Background = new SolidColorBrush(Color.FromRgb(168, 175, 191));
                    break;
                case 2:
                    btn_addStorage.Background = new SolidColorBrush(RMFSVisualization.NodeTypeColorDict[MapNode.NodeTypes.Stroage]);
                    break;
                case 3:
                    btn_addPickStation.Background = new SolidColorBrush(RMFSVisualization.NodeTypeColorDict[MapNode.NodeTypes.PickStation]);
                    break;
                case 4:
                    btn_addReplenishmentStation.Background = new SolidColorBrush(RMFSVisualization.NodeTypeColorDict[MapNode.NodeTypes.ReplenishmentStation]);
                    break;
                case 5:
                    btn_addChargeStation.Background = new SolidColorBrush(RMFSVisualization.NodeTypeColorDict[MapNode.NodeTypes.ChargeStation]);
                    break;
                case 6:
                    btn_bidirection.Background = new SolidColorBrush(Color.FromRgb(168, 175, 191));
                    break;
                case 7:
                    btn_onedirection.Background = new SolidColorBrush(Color.FromRgb(168, 175, 191));
                    break;
                case 8:
                    btn_nopassing.Background = new SolidColorBrush(Color.FromRgb(168, 175, 191));
                    break;
            }
            if (modeFlag < 6)
            {
                this.RMFSVisualization.SetSelectionMode(0);
            }
            else
            {
                this.RMFSVisualization.SetSelectionMode(1);
            }
            _editModeFlag = modeFlag;
        }
        private void OnNodeSelected(object sender, RMFSVisualization.NodeSelectedEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                switch (_editModeFlag)
                {
                    case 1:
                        e.SelectedNode.Type = MapNode.NodeTypes.None;
                        break;
                    case 2:
                        e.SelectedNode.Type = MapNode.NodeTypes.Stroage;
                        break;
                    case 3:
                        e.SelectedNode.Type = MapNode.NodeTypes.PickStation;
                        break;
                    case 4:
                        e.SelectedNode.Type = MapNode.NodeTypes.ReplenishmentStation;
                        break;
                    case 5:
                        e.SelectedNode.Type = MapNode.NodeTypes.ChargeStation;
                        break;
                }
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                if (_editModeFlag > 0)
                {
                    e.SelectedNode.Type = MapNode.NodeTypes.None;
                }
            }
            this.RMFSVisualization.UpdateMapNode(e.SelectedNode);
        }

        private void OnEdgeSelected(object sender, RMFSVisualization.EdgeSelectedEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                switch (_editModeFlag)
                {
                    case 6:
                        e.SelectedEdge.Direction = MapEdge.Directions.LeavingAndEntering;
                        break;
                    case 7:
                        if ((e.SelectedNode1.Location.X + e.SelectedNode1.Location.Y) < (e.SelectedNode2.Location.X + e.SelectedNode2.Location.Y))
                        {
                            e.SelectedEdge.Direction = MapEdge.Directions.Leaving;
                        }
                        else
                        {
                            e.SelectedEdge.Direction = MapEdge.Directions.Entering;
                        }
                        break;
                    case 8:
                        e.SelectedEdge.Direction = MapEdge.Directions.None;
                        break;
                }
                this.RMFSVisualization.UpdateMapEdge(e.SelectedEdge);
            }
        }
        #endregion
        
    }
}
