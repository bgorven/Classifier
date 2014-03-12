using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDetect
{
    public struct rectangle
    {
        private static readonly System.Windows.DependencyProperty rectLeftProperty = System.Windows.Controls.Canvas.LeftProperty;
        private static readonly System.Windows.DependencyProperty rectTopProperty = System.Windows.Controls.Canvas.TopProperty;

        public float x, y, w, h;

        public int Left { get { return (int)x; } set { x = value; } }
        public int Top { get { return (int)y; } set { y = value; } }
        public int Width { get { return (int)w; } set { w = value; } }
        public int Height { get { return (int)h; } set { h = value; } }

        public rectangle(int x, int y, int w, int h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }
        public rectangle(double x, double y, double w, double h)
        {
            this.x = (float)x;
            this.y = (float)y;
            this.w = (float)w;
            this.h = (float)h;
        }
        public System.Windows.Shapes.Shape initializeShape(System.Windows.Shapes.Shape shape, double scaleX, double scaleY)
        {
            shape.SetValue(rectLeftProperty, x * scaleX);
            shape.SetValue(rectTopProperty, y * scaleY);
            shape.Width = w * scaleX;
            shape.Height = h * scaleY;

            shape.Stroke = System.Windows.Media.Brushes.AliceBlue;
            shape.StrokeThickness = 3;

            return shape;
        }
    }
}
