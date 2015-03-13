using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LocalizationCore
{
    public class Painter
    {
        Color BACKGROUND_COLOR = Colors.White;

        public Canvas WorkCanvas { get; private set; }

        public Painter(Canvas canvas)
        {
            WorkCanvas = canvas;
            if (double.IsNaN(canvas.Width) || double.IsNaN(canvas.Height))
                throw new Exception("Please set width and height of canvas");
        }

        public void DrawLine(Point point1, Point point2, Color color, int thickness)
        {
            var line = new Line();
            line.Stroke = new SolidColorBrush(color);
            line.StrokeThickness = thickness;
            line.X1 = point1.X;
            line.Y1 = point1.Y;
            line.X2 = point2.X;
            line.Y2 = point2.Y;

            WorkCanvas.Children.Add(line);
        }

        public void DrawCircle(double radius, Point center, Color personColor, int thickness)
        {
            var ellipse = new Ellipse();
            ellipse.Width = radius * 2;
            ellipse.Height = radius * 2;
            ellipse.Stroke = new SolidColorBrush(personColor);
            ellipse.StrokeThickness = thickness;
            Canvas.SetLeft(ellipse, center.X - radius);
            Canvas.SetTop(ellipse, center.Y - radius);

            WorkCanvas.Children.Add(ellipse);
        }

        public void DrawRectangle(Point topLeft, Point rightBottom, Color color, int thickness, bool filled = false)
        {
            Rectangle rect = new Rectangle();
            rect.Width = rightBottom.X - topLeft.X;
            rect.Height = rightBottom.Y - topLeft.Y;
            if (filled)
                rect.Fill = new SolidColorBrush(color);
            rect.Stroke = new SolidColorBrush(color);
            Canvas.SetLeft(rect, topLeft.X);
            Canvas.SetTop(rect, topLeft.Y);
            
            WorkCanvas.Children.Add(rect);
        }

        public static Color GetGradient(double val, int min, int max)
        {
            if (val > max) val = max;
            if (val < min) val = min;

            var norm = (byte)(255 * (val - min) / (max - min));

            if (norm < 128)
                return Color.FromRgb((byte)(norm * 2), 255, 0);
            else if (norm == 128)
                return Color.FromRgb(255, 255, 0);
            else
                return Color.FromRgb(255, (byte)((256 - norm) * 2), 0);
        }

        public void DrawPolygone(List<Point> list, Color fillColor, Color borderColor)
        {
            Polygon p = new Polygon();
            p.DataContext = list;
            foreach (var point in list)
                p.Points.Add(point);
            p.Fill = new SolidColorBrush(fillColor);
            p.Stroke = new SolidColorBrush(borderColor);
            p.StrokeDashArray = new DoubleCollection(new List<double>() { 4 });

            WorkCanvas.Children.Add(p);
        }

        public void Clear()
        {
            WorkCanvas.Children.Clear();
            DrawRectangle(new Point(0,0), new Point(WorkCanvas.Width-1, WorkCanvas.Height-1), BACKGROUND_COLOR, 1, true);
        }
    }
}
