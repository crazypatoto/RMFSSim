using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using RMFSSim.RMFS;
using RMFSSim.RMFS.Maps;
using RMFSSim.RMFS.AGVs;
using System.Windows;
using System.Windows.Input;

namespace RMFSSim.RMFS.Visualizations
{
    public class RMFSVisualization
    {
        #region Color Dictionaries

        public static Dictionary<MapNode.NodeTypes, Color> NodeTypeColorDict = new Dictionary<MapNode.NodeTypes, Color>
        {
            {MapNode.NodeTypes.None, Color.FromRgb(240,240,240)},
            {MapNode.NodeTypes.PickStation, Color.FromRgb(240,240,240)},
            {MapNode.NodeTypes.ReplenishmentStation, Color.FromRgb(240,240,240)},
            {MapNode.NodeTypes.ChargeStation,Color.FromRgb(240,240,240)},
            {MapNode.NodeTypes.Stroage, Color.FromRgb(240,240,240)},
            {MapNode.NodeTypes.Buffer, Color.FromRgb(240,240,240)},
        };
        #endregion

        private readonly RMFSCore _rmfs;
        private readonly Canvas _canvas;
        private Point _initialMousePosition;
        private readonly MatrixTransform _transform = new MatrixTransform();
        private const float _scaleFactor = 1.1f;
        private const double _spacingRatio = 0.3;
        private const double _selectionStrokeWidthRatio = 0.1;
        private double _cellSize;        
        private Rectangle _selectedNodeRect = null;
        public RMFSVisualization(RMFSCore rmfs, Canvas canvas)
        {
            _rmfs = rmfs;
            _canvas = canvas;
            _canvas.MouseDown += canvas_MouseDown;
            _canvas.MouseMove += canvas_MouseMove;
            _canvas.MouseWheel += canvas_MouseWheel;
            _cellSize = Math.Min(canvas.ActualWidth / (rmfs.CurrentMap.Width + rmfs.CurrentMap.Width * _spacingRatio + _spacingRatio),
                canvas.ActualHeight / (rmfs.CurrentMap.Length + rmfs.CurrentMap.Length * _spacingRatio + _spacingRatio));

        }

        #region Canvas Events        

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DetermineSelectedNode(e.OriginalSource);
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                _initialMousePosition = _transform.Inverse.Transform(e.GetPosition(_canvas));
            }
        }
        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DetermineSelectedNode(e.OriginalSource);
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                Point mousePosition = _transform.Inverse.Transform(e.GetPosition(_canvas));
                Vector delta = Point.Subtract(mousePosition, _initialMousePosition);
                var translate = new TranslateTransform(delta.X, delta.Y);
                _transform.Matrix = translate.Value * _transform.Matrix;

                foreach (UIElement child in _canvas.Children)
                {
                    child.RenderTransform = _transform;
                }
            }
        }
        private void canvas_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            float scaleFactor = _scaleFactor;
            if (e.Delta < 0)
            {
                scaleFactor = 1f / scaleFactor;
            }

            Point mousePostion = e.GetPosition(_canvas);
            Matrix scaleMatrix = _transform.Matrix;
            scaleMatrix.ScaleAt(scaleFactor, scaleFactor, mousePostion.X, mousePostion.Y);
            _transform.Matrix = scaleMatrix;

            foreach (UIElement child in _canvas.Children)
            {
                double x = Canvas.GetLeft(child);
                double y = Canvas.GetTop(child);

                double sx = x * scaleFactor;
                double sy = y * scaleFactor;

                Canvas.SetLeft(child, sx);
                Canvas.SetTop(child, sy);

                child.RenderTransform = _transform;
            }

        }
        private void DetermineSelectedNode(object originalSource)
        {
            if (originalSource is Rectangle)
            {
                if (_selectedNodeRect != null)
                {
                    _selectedNodeRect.Stroke = null;
                }
                var target = (Rectangle)originalSource;
                target.Stroke = new SolidColorBrush(Colors.Red);
                target.StrokeThickness = _cellSize * _selectionStrokeWidthRatio;
                _selectedNodeRect = target;
            }
        }
        #endregion



        public void ClearMap()
        {
            _canvas.Children.Clear();
        }

        public void DrawMap()
        {
            for (int y = 0; y < _rmfs.CurrentMap.Length; y++)
            {
                for (int x = 0; x < _rmfs.CurrentMap.Width; x++)
                {
                    var currentNode = _rmfs.CurrentMap.AllNodes[y, x];
                    Rectangle rectangle = new Rectangle
                    {
                        Tag = currentNode,
                        Height = _cellSize,
                        Width = _cellSize,
                    };
                    rectangle.Fill = new SolidColorBrush(NodeTypeColorDict[currentNode.Type]);
                    _canvas.Children.Add(rectangle);
                    Canvas.SetLeft(rectangle, x * (_cellSize + _cellSize * _spacingRatio) + _cellSize * _spacingRatio);
                    Canvas.SetTop(rectangle, y * (_cellSize + _cellSize * _spacingRatio) + _cellSize * _spacingRatio);
                }
            }
        }
    }
}
