using Gol.Core.Controls.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Gol.Core.Controls.Views
{
    /// <summary>
    ///     Mono life grid control.
    /// </summary>
    public class MonoLifeView : StackPanel
    {
        /// <summary>
        ///     Dependency property for <see cref="IsReadOnly" /> property.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty;

        /// <summary>
        ///     Dependency property for <see cref="LineBrush" /> property.
        /// </summary>
        public static readonly DependencyProperty LineBrushProperty;

        /// <summary>
        ///     Dependency property for <see cref="CellSize" /> property.
        /// </summary>
        public static readonly DependencyProperty CellSizeProperty;

        /// <summary>
        ///     Dependency property for <see cref="MonoLifeGrid" /> property.
        /// </summary>
        public static readonly DependencyProperty MonoLifeGridProperty;

        /// <summary>
        ///     Dependency property for <see cref="CellBrush" /> property.
        /// </summary>
        public static readonly DependencyProperty CellBrushProperty;

        private CellData[,] _cellGrid = new CellData[0, 0];

        private int _currentX = -1;

        private int _currentY = -1;

        private bool _mouseMoving;

        /// <summary>
        ///     Static constructor for <see cref="MonoLifeView" />.
        /// </summary>
        static MonoLifeView()
        {
            CellBrushProperty = DependencyProperty.Register(
                nameof(CellBrush),
                typeof(Brush),
                typeof(MonoLifeView),
                new PropertyMetadata(Brushes.GreenYellow));

            IsReadOnlyProperty = DependencyProperty.Register(
                nameof(IsReadOnly),
                typeof(bool),
                typeof(MonoLifeView),
                new PropertyMetadata(default(bool)));

            LineBrushProperty = DependencyProperty.Register(
                nameof(LineBrush),
                typeof(Brush),
                typeof(MonoLifeView),
                new PropertyMetadata(new BrushConverter().ConvertFrom("#2d2d2d")));

            CellSizeProperty = DependencyProperty.Register(
                nameof(CellSize),
                typeof(int),
                typeof(MonoLifeView),
                new PropertyMetadata(10));

            MonoLifeGridProperty = DependencyProperty.Register(
                nameof(MonoLifeGrid),
                typeof(MonoLifeGrid<bool>),
                typeof(MonoLifeView),
                new PropertyMetadata(
                    default(MonoLifeGrid<bool>),
                    (source, args) =>
                        ((MonoLifeView)source).MonoLifeGridModelChanged((MonoLifeGrid<bool>?)args.OldValue)));
        }

        /// <summary>
        ///     Constructor for <see cref="MonoLifeView" />.
        /// </summary>
        public MonoLifeView()
        {
            CanvasCommon = new Canvas();
            Children.Add(CanvasCommon);
        }

        /// <summary>
        ///     Cell brush color.
        /// </summary>
        public Brush CellBrush
        {
            get => (Brush)GetValue(CellBrushProperty);

            set => SetValue(CellBrushProperty, value);
        }

        /// <summary>
        ///     Is read only grid.
        /// </summary>
        /// <remarks>Prevent mouse marking.</remarks>
        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);

            set => SetValue(IsReadOnlyProperty, value);
        }

        /// <summary>
        ///     Line brush color.
        /// </summary>
        public Brush LineBrush
        {
            get => (Brush)GetValue(LineBrushProperty);

            set => SetValue(LineBrushProperty, value);
        }

        /// <summary>
        ///     Cell size (pixels).
        /// </summary>
        public int CellSize
        {
            get => (int)GetValue(CellSizeProperty);

            set => SetValue(CellSizeProperty, value);
        }

        /// <summary>
        ///     Mono grid model.
        /// </summary>
        public MonoLifeGrid<bool>? MonoLifeGrid
        {
            get => (MonoLifeGrid<bool>?)GetValue(MonoLifeGridProperty);

            set => SetValue(MonoLifeGridProperty, value);
        }

        private Canvas? CanvasCommon { get; }

        /// <summary>
        ///     Creates color with corrected brightness.
        /// </summary>
        /// <param name="color">Color to correct.</param>
        /// <param name="correctionFactor">
        ///     The brightness correction factor. Must be between -1 and 1.
        ///     Negative values produce darker colors.
        /// </param>
        /// <returns>
        ///     Corrected <see cref="Color" /> structure.
        /// </returns>
        private static Color ChangeColorBrightness(Color color, float correctionFactor)
        {
            float red = color.R;
            float green = color.G;
            float blue = color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return Color.FromArgb(color.A, (byte)red, (byte)green, (byte)blue);
        }

        /// <inheritdoc />
        protected override void OnMouseUp(MouseButtonEventArgs args)
        {
            base.OnMouseUp(args);

            if (args.LeftButton == MouseButtonState.Released && !_mouseMoving && !IsReadOnly)
            {
                var cell = GetMouseCell(args);
                ProcessMouseSelection(cell);
            }
        }

        /// <inheritdoc />
        protected override void OnMouseMove(MouseEventArgs args)
        {
            base.OnMouseMove(args);

            if (args.LeftButton == MouseButtonState.Pressed && !IsReadOnly)
            {
                _mouseMoving = true;
                var cell = GetMouseCell(args);

                if (cell.X == _currentX && cell.Y == _currentY)
                {
                    return;
                }

                ProcessMouseSelection(cell, true);
                _currentX = cell.X;
                _currentY = cell.Y;
            }

            if (args.LeftButton == MouseButtonState.Released)
            {
                _currentY = _currentX = -1;
                _mouseMoving = false;
            }
        }

        private void ProcessMouseSelection(IntPoint cell, bool isOnlyDraw = false)
        {
            if (0 <= cell.X && cell.X < MonoLifeGrid!.Width && 0 <= cell.Y && cell.Y < MonoLifeGrid.Height)
            {
                var current = MonoLifeGrid[cell.X, cell.Y];
                var setValue = true;

                if (current)
                {
                    if (!isOnlyDraw)
                    {
                        _cellGrid[cell.X, cell.Y].ClearRectangle();
                    }
                    else
                    {
                        setValue = false;
                    }
                }
                else
                {
                    DrawSquare(cell.X, cell.Y);
                }

                if (setValue)
                {
                    MonoLifeGrid[cell.X, cell.Y] = !current;
                }
            }
        }

        private IntPoint GetMouseCell(MouseEventArgs args)
        {
            var position = args.GetPosition(this);
            int x = (int)(position.X / CellSize), y = (int)(position.Y / CellSize);

            return new IntPoint(x, y);
        }

        private void MonoLifeGridModelChanged(MonoLifeGrid<bool>? lastGrid)
        {
            if (CanvasCommon == null)
            {
                return;
            }

            var lastLifeId = lastGrid?.LifeId;

            if (Dispatcher != null && Dispatcher.CheckAccess())
            {
                GridRender(lastLifeId);
            }
            else
            {
                Dispatcher?.Invoke(() => GridRender(lastLifeId));
            }
        }

        private void GridRender(Guid? lastLifeId)
        {
            if (MonoLifeGrid == null || MonoLifeGrid.Height == 0 || MonoLifeGrid.Width == 0)
            {
                return;
            }

            var isNewGrid = !(lastLifeId.HasValue && lastLifeId == MonoLifeGrid?.LifeId);

            if (isNewGrid)
            {
                CanvasCommon!.Children.Clear();
                SquareGrid();
                _cellGrid = new CellData[MonoLifeGrid!.Width, MonoLifeGrid.Height];
                Width = MonoLifeGrid.Width * CellSize;
                Height = MonoLifeGrid.Height * CellSize;
            }

            for (var i = 0; i < MonoLifeGrid!.Width; i++)
            {
                Canvas canvasRow = null!;

                if (isNewGrid)
                {
                    canvasRow = new Canvas();
                }

                for (var j = 0; j < MonoLifeGrid.Height; j++)
                {
                    CellData cell;

                    if (isNewGrid)
                    {
                        cell = new CellData(i, j, canvasRow, this);
                        _cellGrid[i, j] = cell;
                    }
                    else
                    {
                        cell = _cellGrid[i, j];
                    }

                    if (MonoLifeGrid[i, j] && !cell.IsBlack)
                    {
                        DrawSquare(i, j);
                    }
                    else if (!MonoLifeGrid[i, j] && cell.IsBlack)
                    {
                        cell.ClearRectangle();
                    }
                }

                if (isNewGrid)
                {
                    CanvasCommon!.Children.Add(canvasRow);
                }
            }
        }

        private void DrawSquare(int x, int y)
        {
            _cellGrid[x, y].DrawRectangle();
        }

        private void SquareGrid()
        {
            var mainColor = LineBrush;
            var tenthLine = new SolidColorBrush(ChangeColorBrightness(((SolidColorBrush)mainColor).Color, -0.6f));

            // Rows
            for (var i = 0; i <= MonoLifeGrid!.Height; i++)
            {
                var horizontalLine = CreateLine(0, i * CellSize, MonoLifeGrid.Width * CellSize, i * CellSize);
                horizontalLine.Stroke = i % 10 == 0 ? LineBrush : tenthLine;
                CanvasCommon!.Children.Add(horizontalLine);
            }

            // Columns
            for (var j = 0; j <= MonoLifeGrid.Width; j++)
            {
                var verticalLine = CreateLine(j * CellSize, 0, j * CellSize, MonoLifeGrid.Height * CellSize);
                verticalLine.Stroke = j % 10 == 0 ? LineBrush : tenthLine;
                CanvasCommon!.Children.Add(verticalLine);
            }
        }

        private static Line CreateLine(int x1, int y1, int x2, int y2)
        {
            return new Line
            {
                X1 = x1,
                X2 = x2,
                Y1 = y1,
                Y2 = y2,
                StrokeThickness = 1
            };
        }
    }
}