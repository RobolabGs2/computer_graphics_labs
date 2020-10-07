using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using System.Windows.Forms;

namespace Lab04.Tools
{
    class Graham : ITool
    {
        public Bitmap image => Properties.Resources.Graham;
        private Context context;

        public Polygon NextToTop(ref Stack<Polygon> s)
        {
            Polygon p = s.Peek();
            s.Pop();
            Polygon res = s.Peek();
            s.Push(p);
            return res;
        }
        public double PolarAngle(double y, double x)
        {
            double res = Math.Atan2(y, x);
            if (res < 0) res += 2 * Math.PI;
            return res;
        }

        public Polygon GrahamScan(List<Polygon> points)
        {
            //Находим точку с минимальной координатой y(самую левую)
            double ymin = points[0].Points[0].Y;
            int min = 0;
            for (int i = 1; i < points.Count(); i++)
            {
                double y = points[i].Points[0].Y;
                if ((y < ymin) || (ymin == y && points[i].Points[0].X < points[min].Points[0].X))
                {
                    ymin = points[i].Points[0].Y;
                    min = i;
                }
            }
            //Делаем самую левую точку первой
            var t = points[0];
            points[0] = points[min];
            points[min] = t;
            var p0 = points[0].Points[0];
            //сортируем в порядке увеличения полярного угла(против часовой) относительно самой левой точки  
            points = points.OrderBy(p => PolarAngle(p.Points[0].Y - p0.Y, p.Points[0].X - p0.X)).ToList();

            Stack<Polygon> S = new Stack<Polygon>();
            S.Push(points[0]); 
            S.Push(points[1]);
            S.Push(points[2]);

            //Если S_i+1 находится не слева от луча S_i-1 -> S_i, то удаляем из списка S_i точку.
            for (int i = 3; i < points.Count(); i++)
            {
                while (S.Peek().Orientation(NextToTop(ref S).Points[0], points[i].Points[0]) != 2)
                    S.Pop();
                S.Push(points[i]);
            }

            //Создаем полигон по точкам
            var res = new Polygon();
            while (S.Count() != 0)
            {
                Polygon p = S.Peek();
                res.Points.Add(p.Points[0]);
                S.Pop();
            }
            res.Points.Reverse();
            return res;
        }

        public bool Active()
        {
            if (context.Selected.Count < 3)
                return false;

            foreach (var p in context.Selected)
                if (p.Points.Count > 1)
                    return false;

            Polygon result = GrahamScan(context.Selected.ToList());

            if (!context.Debug)
                context.Polygons.RemoveWhere(p => context.Selected.Contains(p));
            context.Selected.Clear();
            if (result != null)
            {
                context.Add(result);
                context.Selected.Add(result);
            }
            return false;
        }

        public Matrix Draw(Point start, Point end, Graphics graphics)
        {
            return Matrix.Ident();
        }

        public void Init(Context context)
        {
            this.context = context;
        }
    }
}
