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
                        X = double.Parse(lines[1]),
                        Y = double.Parse(lines[2]),
                        Z = double.Parse(lines[3])
                    });
                }
                else
                if (flag == "vn")
                {
                    current.AddNormal(new Point
                    {
                        X = double.Parse(lines[1]),
                        Y = double.Parse(lines[2]),
                        Z = double.Parse(lines[3])
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

        public static IEnumerable<string> Store(IEnumerable<Entity> entities)
        {
            int startPoint = 1;
            int startNorm = 1;
            int polyCounter = 1;
            foreach (Entity e in entities)
            {
                if(e is Polytope poly)
                {
                    yield return "s off";

                    yield return $"o Polytope_{polyCounter}";
                    yield return $"g Polytope_{polyCounter++}";

                    foreach (Point p in poly.points)
                        yield return $"v {p.X} {p.Y} {p.Z}";
                    foreach (Point p in poly.normals)
                        yield return $"vn {p.X} {p.Y} {p.Z}";

                    foreach (Polygon p in poly.polygons)
                    {
                        string result = "f";
                        if(p.normals == null)
                            foreach(int v in p.indexes)
                                result += $" {v + startPoint}";
                        else
                            for(int i = 0; i < p.indexes.Count; ++i)
                                result += $" {p.indexes[i] + startPoint}//{p.normals[i] + startNorm}";
                        yield return result;
                    }

                    startNorm += poly.normals.Count;
                    startPoint += poly.points.Count;
                }
            }
            yield break;
        }
    }
}
