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
    public class RMFSVisualization : IDisposable
    {
        #region Color Dictionaries

        public static Dictionary<MapNode.NodeTypes, Color> NodeTypeColorDict = new Dictionary<MapNode.NodeTypes, Color>
        {
            {MapNode.NodeTypes.None, Color.FromRgb(240,240,240)},
            {MapNode.NodeTypes.PickStation, Color.FromRgb(240,27,77)},
            {MapNode.NodeTypes.ReplenishmentStation, Color.FromRgb(240,112,27)},
            {MapNode.NodeTypes.ChargeStation,Color.FromRgb(240,240,27)},
            {MapNode.NodeTypes.Stroage, Color.FromRgb(27,80,240)},
        };
        #endregion

        public EventHandler<NodeSelectedEventArgs> NodeSelectedEvent;
        public EventHandler<EdgeSelectedEventArgs> EdgeSelectedEvent;

        private bool _disposed;
        private readonly RMFSCore _rmfs;
        private readonly Canvas _canvas;
        private Point _initialMousePosition;
        private readonly MatrixTransform _transform = new MatrixTransform();
        private const double _scaleFactor = 1.1;
        private const double _spacingRatio = 0.1;
        private const double _arrowStrokeWidthRatio = 0.02;
        private double _cellSize;
        private Rectangle _selectedNodeRect = null;
        private Rectangle _selectedEdgeRect = null;
        private MapNode _selectedEdgeNode1 = null;
        private MapNode _selectedEdgeNode2 = null;

        /// <summary>
        /// Selection Mode. (0: Node, 1: Edge)
        /// </summary>
        public int SelectionMode { get; private set; }
        public RMFSVisualization(RMFSCore rmfs, Canvas canvas)
        {
            _rmfs = rmfs;
            _canvas = canvas;
            _canvas.Children.Clear();
            _canvas.MouseDown += canvas_MouseDown;
            _canvas.MouseMove += canvas_MouseMove;
            _canvas.MouseWheel += canvas_MouseWheel;
            _cellSize = Math.Min(canvas.ActualWidth / (rmfs.CurrentMap.Width + rmfs.CurrentMap.Width * _spacingRatio + _spacingRatio),
                canvas.ActualHeight / (rmfs.CurrentMap.Length + rmfs.CurrentMap.Length * _spacingRatio + _spacingRatio));

            _selectedNodeRect = new Rectangle
            {
                Tag = null,
                Height = _cellSize * (1.0 + 2.0 * _spacingRatio),
                Width = _cellSize * (1.0 + 2.0 * _spacingRatio),
                IsHitTestVisible = false,
                Stroke = new SolidColorBrush(Colors.Red),
                StrokeThickness = _cellSize * _spacingRatio,
                StrokeDashArray = new DoubleCollection { 1 },
            };
            _canvas.Children.Add(_selectedNodeRect);
            Canvas.SetZIndex(_selectedNodeRect, 2);

            this.SelectionMode = 0;
        }
        ~RMFSVisualization()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                _canvas.MouseDown -= canvas_MouseDown;
                _canvas.MouseMove -= canvas_MouseMove;
                _canvas.MouseWheel -= canvas_MouseWheel;
                _canvas.Children.Clear();
            }
            _disposed = true;
        }

        #region Canvas Events        

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Right)
            {
                DetermineSelectedNode(e.OriginalSource, e.ChangedButton);
            }
            else if (e.ChangedButton == MouseButton.Middle)
            {
                _initialMousePosition = _transform.Inverse.Transform(e.GetPosition(_canvas));
            }
        }
        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DetermineSelectedNode(e.OriginalSource, MouseButton.Left);
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                DetermineSelectedNode(e.OriginalSource, MouseButton.Right);
            }
            if (e.MiddleButton == MouseButtonState.Pressed)
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
            double scaleFactor = _scaleFactor;
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

        #endregion

        private void DetermineSelectedNode(object originalSource, MouseButton changedButton)
        {
            if (originalSource is Rectangle)
            {
                var target = originalSource as Rectangle;
                if (target.Tag is MapNode)
                {
                    var selectedNode = target.Tag as MapNode;
                    _selectedNodeRect.Tag = target;
                    Canvas.SetLeft(_selectedNodeRect, Canvas.GetLeft(target) - _cellSize * Math.Sqrt(_transform.Value.Determinant) * _spacingRatio);
                    Canvas.SetTop(_selectedNodeRect, Canvas.GetTop(target) - _cellSize * Math.Sqrt(_transform.Value.Determinant) * _spacingRatio);
                    if (this.SelectionMode == 0)
                    {
                        OnNodeSelectedEvent(new NodeSelectedEventArgs(selectedNode, changedButton));
                    }
                    else if (this.SelectionMode == 1)
                    {
                        if (_selectedEdgeNode1 == null)
                        {
                            _selectedEdgeNode1 = selectedNode;
                        }
                        else if (_selectedEdgeNode2 == null)
                        {
                            if (selectedNode.IsNeighbourNode(_selectedEdgeNode1))
                            {
                                _selectedEdgeNode2 = selectedNode;
                                var selectedEdge = _rmfs.CurrentMap.GetEdgeByNodes(_selectedEdgeNode1, _selectedEdgeNode2);
                                if (_selectedEdgeRect != null)
                                {
                                    _canvas.Children.Remove(_selectedEdgeRect);
                                }
                                _selectedEdgeRect = new Rectangle
                                {
                                    Tag = _canvas.Children.OfType<Path>().First(path => path.Tag == selectedEdge),
                                    IsHitTestVisible = false,
                                    Stroke = new SolidColorBrush(Colors.Red),
                                    StrokeThickness = _cellSize * _spacingRatio,
                                    RenderTransform = _transform
                                };
                                if (_selectedEdgeNode1.Location.Y == _selectedEdgeNode2.Location.Y)
                                {
                                    _selectedEdgeRect.Width = _cellSize * (2.0 + 3.0 * _spacingRatio);
                                    _selectedEdgeRect.Height = _cellSize * (1.0 + 2.0 * _spacingRatio);
                                }
                                else
                                {
                                    _selectedEdgeRect.Width = _cellSize * (1.0 + 2.0 * _spacingRatio);
                                    _selectedEdgeRect.Height = _cellSize * (2.0 + 3.0 * _spacingRatio);
                                }
                                _canvas.Children.Add(_selectedEdgeRect);
                                var targetEdgeArrow = _canvas.Children.OfType<Path>().FirstOrDefault(path => path.Tag == selectedEdge);
                                Canvas.SetLeft(_selectedEdgeRect, Canvas.GetLeft(targetEdgeArrow) - _cellSize * Math.Sqrt(_transform.Value.Determinant) * (0.5 + 0.5 * _spacingRatio));
                                Canvas.SetTop(_selectedEdgeRect, Canvas.GetTop(targetEdgeArrow) - _cellSize * Math.Sqrt(_transform.Value.Determinant) * (0.5 + 0.5 * _spacingRatio));
                                Canvas.SetZIndex(_selectedEdgeRect, 2);
                                OnEdgeSelectedEvent(new EdgeSelectedEventArgs(selectedEdge, _selectedEdgeNode1, _selectedEdgeNode2, changedButton));
                                _selectedEdgeNode1 = _selectedEdgeNode2;
                                _selectedEdgeNode2 = null;
                            }
                            else
                            {                                
                                if(_selectedEdgeNode1 != selectedNode)
                                {
                                    _selectedEdgeNode1 = selectedNode;
                                    if (_selectedEdgeRect != null)
                                    {
                                        _canvas.Children.Remove(_selectedEdgeRect);
                                    }
                                }                                
                            }
                        }
                    }
                }
            }
        }

        public void FitToCanvas()
        {

        }

        public void ClearMap()
        {
            _canvas.Children.Clear();
        }

        public void DrawNewMap()
        {
            // Draw Map Nodes
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
                    Canvas.SetZIndex(rectangle, 1);
                }
            }

            // Map Map Edges
            for (int y = 0; y < 2 * _rmfs.CurrentMap.Length - 1; y++)
            {
                for (int x = 0; x < _rmfs.CurrentMap.Width; x++)
                {
                    var edge = _rmfs.CurrentMap.AllEdges[y, x];
                    var edgeDirection = edge.Direction;
                    var geometryGroup = new GeometryGroup();
                    var arrowEnds = ArrowLineGeometryGenerator.ArrowEnds.None;
                    if ((edgeDirection & MapEdge.Directions.Leaving) > 0)
                    {
                        arrowEnds |= ArrowLineGeometryGenerator.ArrowEnds.End;
                    }
                    if ((edgeDirection & MapEdge.Directions.Entering) > 0)
                    {
                        arrowEnds |= ArrowLineGeometryGenerator.ArrowEnds.Start;
                    }

                    if (y % 2 == 0)
                    {
                        // Draw horizaontal arrows                       
                        if (x < _rmfs.CurrentMap.Width - 1)
                        {
                            geometryGroup.Children.Add(ArrowLineGeometryGenerator.GenerateArrowGeometry(arrowEnds, new Point(_cellSize * _spacingRatio / 2.0, _cellSize * _spacingRatio / 2.0), new Point(_cellSize * (1.0 + _spacingRatio * 1.5), _cellSize * _spacingRatio / 2.0), 45, 0.2 * (_cellSize * (1.0 + _spacingRatio))));
                        }
                    }
                    else
                    {
                        // Draw vertical arrows
                        if (y < 2 * _rmfs.CurrentMap.Length - 2)
                        {
                            geometryGroup.Children.Add(ArrowLineGeometryGenerator.GenerateArrowGeometry(arrowEnds, new Point(_cellSize * _spacingRatio / 2.0, _cellSize * _spacingRatio / 2.0), new Point(_cellSize * _spacingRatio / 2.0, _cellSize * (1.0 + _spacingRatio * 1.5)), 45, 0.2 * (_cellSize * (1.0 + _spacingRatio))));
                        }
                    }
                    var path = new Path()
                    {
                        Data = geometryGroup,
                        Tag = edge,
                        StrokeThickness = _cellSize * _arrowStrokeWidthRatio,
                        IsHitTestVisible = false                // Make edge arrow not selectable
                    };
                    if (edgeDirection == MapEdge.Directions.None)
                    {
                        path.Fill = null;
                        path.Stroke = null;
                    }
                    else
                    {
                        path.Fill = Brushes.Black;
                        path.Stroke = Brushes.Black;
                    }                    
                    Canvas.SetLeft(path, (x + 0.5) * (_cellSize * (1 + _spacingRatio)));
                    Canvas.SetTop(path, (y / 2 + 0.5) * (_cellSize * (1 + _spacingRatio)));
                    Canvas.SetZIndex(path, 2);
                    _canvas.Children.Add(path);
                }
            }
        }

        public void UpdateMapNode(MapNode mapNode)
        {
            var targetNodeRect = _canvas.Children.OfType<Rectangle>().FirstOrDefault(rect => rect.Tag == mapNode);
            if (targetNodeRect == null) return;
            targetNodeRect.Fill = new SolidColorBrush(NodeTypeColorDict[mapNode.Type]);
        }

        public void UpdateMapEdge(MapEdge mapEdge)
        {
            var targetEdgePath = _canvas.Children.OfType<Path>().FirstOrDefault(path => path.Tag == mapEdge);
            if (targetEdgePath == null) return;
            targetEdgePath.Data = null;

            var edgeDirection = mapEdge.Direction;
            var geometryGroup = new GeometryGroup();
            var arrowEnds = ArrowLineGeometryGenerator.ArrowEnds.None;
            if ((edgeDirection & MapEdge.Directions.Leaving) > 0)
            {
                arrowEnds |= ArrowLineGeometryGenerator.ArrowEnds.End;
            }
            if ((edgeDirection & MapEdge.Directions.Entering) > 0)
            {
                arrowEnds |= ArrowLineGeometryGenerator.ArrowEnds.Start;
            }

            var coordinates = _rmfs.CurrentMap.CoordinatesOfEdge(mapEdge);
            if (coordinates.Item2 % 2 == 0)
            {
                // Draw horizaontal arrows                       
                geometryGroup.Children.Add(ArrowLineGeometryGenerator.GenerateArrowGeometry(arrowEnds, new Point(_cellSize * _spacingRatio / 2.0, _cellSize * _spacingRatio / 2.0), new Point(_cellSize * (1.0 + _spacingRatio * 1.5), _cellSize * _spacingRatio / 2.0), 45, 0.2 * (_cellSize * (1.0 + _spacingRatio))));
            }
            else
            {
                // Draw vertical arrows
                geometryGroup.Children.Add(ArrowLineGeometryGenerator.GenerateArrowGeometry(arrowEnds, new Point(_cellSize * _spacingRatio / 2.0, _cellSize * _spacingRatio / 2.0), new Point(_cellSize * _spacingRatio / 2.0, _cellSize * (1.0 + _spacingRatio * 1.5)), 45, 0.2 * (_cellSize * (1.0 + _spacingRatio))));
            }

            targetEdgePath.Data = geometryGroup;
            if (edgeDirection == MapEdge.Directions.None)
            {
                targetEdgePath.Fill = null;
                targetEdgePath.Stroke = null;
            }
            else
            {
                targetEdgePath.Fill = Brushes.Black;
                targetEdgePath.Stroke = Brushes.Black;
            }
        }

        public void SetSelectionMode(int mode)
        {
            if (mode == 1)
            {
                this.SelectionMode = 1;
            }
            else
            {
                if (_selectedEdgeRect != null)
                {
                    _canvas.Children.Remove(_selectedEdgeRect);
                }
                this.SelectionMode = 0;
            }
        }
        protected virtual void OnNodeSelectedEvent(NodeSelectedEventArgs e)
        {
            EventHandler<NodeSelectedEventArgs> raiseEvent = NodeSelectedEvent;
            if (raiseEvent != null)
            {
                raiseEvent(this, e);
            }
        }

        protected virtual void OnEdgeSelectedEvent(EdgeSelectedEventArgs e)
        {
            EventHandler<EdgeSelectedEventArgs> raiseEvent = EdgeSelectedEvent;
            if (raiseEvent != null)
            {
                raiseEvent(this, e);
            }
        }

        #region Custom Events
        public class NodeSelectedEventArgs : EventArgs
        {
            public NodeSelectedEventArgs(MapNode selectedNode, MouseButton changedButton)
            {
                this.SelectedNode = selectedNode;
                this.ChangedButton = changedButton;
            }
            public MapNode SelectedNode { get; }
            public MouseButton ChangedButton { get; }
        }

        public class EdgeSelectedEventArgs : EventArgs
        {
            public EdgeSelectedEventArgs(MapEdge selectedEdge, MapNode selectedNode1, MapNode selectedNode2, MouseButton changedButton)
            {
                this.SelectedEdge = selectedEdge;
                this.SelectedNode1 = selectedNode1;
                this.SelectedNode2 = selectedNode2;
                this.ChangedButton = changedButton;
            }
            public MapEdge SelectedEdge { get; }
            public MapNode SelectedNode1 { get; }
            public MapNode SelectedNode2 { get; }
            public MouseButton ChangedButton { get; }
        }

        #endregion
    }
}
