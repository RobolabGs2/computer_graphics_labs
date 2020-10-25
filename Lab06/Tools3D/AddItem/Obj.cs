using Lab06.Base3D;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace Lab06.Tools3D.AddItem
{
    static class Obj
    {
        public static Entity Parse(string file)
        {
            Polytope current = new Polytope();
            foreach (var s in File.ReadAllLines(file))
            {
                if (String.IsNullOrEmpty(s))
                    continue;
                var lines = s.Split(' ').Where(l => !String.IsNullOrEmpty(l)).ToList();
                string flag = lines[0];
                if (flag == "v")
                {
                    current.Add(new Point
                    {
                        X = double.Parse(lines[1].Replace(".", ",")),
                        Y = double.Parse(lines[2].Replace(".", ",")),
                        Z = double.Parse(lines[3].Replace(".", ","))
                    });
                }
                else
                if (flag == "vn")
                {
                    current.AddNormal(new Point
                    {
                        X = double.Parse(lines[1].Replace(".", ",")),
                        Y = double.Parse(lines[2].Replace(".", ",")),
                        Z = double.Parse(lines[3].Replace(".", ","))
                    });
                }
                else
                if (flag == "f")
                {
                    if(lines[1].Split('/').Length  == 3)
                        current.Add(new Polygon(
                            lines.Skip(1).Select(l => Int32.Parse(l.Split('/')[0]) - 1).ToList(),
                            lines.Skip(1).Select(l => Int32.Parse(l.Split('/')[2]) - 1).ToList()));
                    else
                        current.Add(new Polygon(lines.Skip(1).Select(l => Int32.Parse(l.Split('/')[0]) - 1).ToList()));
                }
            }
            return current;
        }
    }
}
