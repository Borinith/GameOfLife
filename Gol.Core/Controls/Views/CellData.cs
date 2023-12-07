using System;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Gol.Core.Controls.Views
{
    internal class CellData
    {
        private readonly Canvas _canvas;

        private readonly MonoLifeView _parent;

        /// <summary>
        ///     Constructor for <see cref="CellData" />.
        /// </summary>
        public CellData(int x, int y, Canvas canvas, MonoLifeView parent)
        {
            _canvas = canvas;
            _parent = parent;
            X = x;
            Y = y;
        }

        /// <summary>
        ///     Is cell is black.
        /// </summary>
        public bool IsBlack => _canvas.Children.Contains(Rectangle);

        /// <summary>
        ///     X value.
        /// </summary>
        private int X { get; }

        /// <summary>
        ///     Y value.
        /// </summary>
        private int Y { get; }

        /// <summary>
        ///     Cell rectangle reference.
        /// </summary>
        private Rectangle? Rectangle { get; set; }

        /// <summary>
        ///     Set rectangle value.
        /// </summary>
        public void DrawRectangle()
        {
            if (Rectangle == null)
            {
                Rectangle = new Rectangle
                {
                    Fill = _parent.CellBrush,
                    Width = _parent.CellSize,
                    Height = _parent.CellSize
                };
            }
            else if (_canvas.Children.Contains(Rectangle))
            {
                return;
            }

            _canvas.Children.Add(Rectangle);
            Canvas.SetLeft(Rectangle, X * _parent.CellSize);
            Canvas.SetTop(Rectangle, Y * _parent.CellSize);
            Panel.SetZIndex(Rectangle, -1);
        }

        /// <summary>
        ///     Clear saved rectangle.
        /// </summary>
        public void ClearRectangle()
        {
            if (Rectangle == null)
            {
                throw new ArgumentException("Rectangle is not drawn here");
            }

            _canvas.Children.Remove(Rectangle);
        }
    }
}