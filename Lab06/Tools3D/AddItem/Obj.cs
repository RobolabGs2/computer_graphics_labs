using Lab06.Base3D;
using Lab06.Materials3D;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Lab06.Tools3D.AddItem
{
    static class Obj
    {
        public static Entity Parse(string file)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(file));
            Polytope current = new Polytope();
            Dictionary<string, BaseMaterial> mtl = new Dictionary<string, BaseMaterial>();
            foreach (var s in File.ReadAllLines(file))
            {
                if (String.IsNullOrEmpty(s))
                    continue;
                var lines = Regex.Split(s, @"\s").Where(l => !String.IsNullOrEmpty(l)).ToList();
                string flag = lines[0];
                if (flag == "v")
                {
                    current.Add(new Base3D.Point
                    {
                        X = double.Parse(lines[1]),
                        Y = double.Parse(lines[2]),
                        Z = double.Parse(lines[3])
                    });
                }
                else
                if (flag == "vn")
                {
                    current.AddNormal(new Base3D.Point
                    {
                        X = double.Parse(lines[1]),
                        Y = double.Parse(lines[2]),
                        Z = double.Parse(lines[3])
                    });
                }
                else
                if (flag == "vt")
                {
                    current.Addtexture((
                        double.Parse(lines[1]),
                        double.Parse(lines[2])
                    ));
                }
                else
                if (flag == "f")
                {
                    var first = lines[1].Split('/');
                    var vertexes = lines.Skip(1).Select(l => Int32.Parse(l.Split('/')[0]) - 1).ToArray();
                    var textures = (first.Length > 1 && !String.IsNullOrEmpty(first[1])) ? 
                        lines.Skip(1).Select(l => Int32.Parse(l.Split('/')[1]) - 1).ToArray() : null;
                    var normals = (first.Length > 2 && !String.IsNullOrEmpty(first[2])) ? 
                        lines.Skip(1).Select(l => Int32.Parse(l.Split('/')[2]) - 1).ToArray() : null;

                    current.Add(new Polygon(vertexes, textures, normals));
                }
                else
                if (flag == "mtllib")
                {
                    try
                    {
                        var new_mtl = LoadMtl(Regex.Replace(s, @"mtllib\s*(.*\S)\s*$", "$1"));
                        mtl = new_mtl;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show($"Не вышло загрузить текстуры: {e.Message}", "Окошко-всплывашка", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
                else
                if (flag == "usemtl")
                {
                    string matName = Regex.Replace(s, @"usemtl\s*(.*\S)\s*$", "$1");
                    if(mtl.ContainsKey(matName))
                        current.Matreial = mtl[matName];
                }
            }
            return current;
        }

        private static Dictionary<string, BaseMaterial> LoadMtl(string file)
        {
            Dictionary<string, BaseMaterial> materials = new Dictionary<string, BaseMaterial>();
            string currentName = "";
            BaseMaterial currentMat = new SolidMaterial();

            foreach (var s in File.ReadAllLines(file))
            {
                if (String.IsNullOrEmpty(s))
                    continue;
                var lines = Regex.Split(s, @"\s").Where(l => !String.IsNullOrEmpty(l)).ToList();
                string flag = lines[0];

                if(flag == "newmtl")
                {
                    materials.Add(currentName, currentMat);
                    currentName = Regex.Replace(s, @"newmtl\s*(.*\S)\s*$", "$1");
                    currentMat = new SolidMaterial();
                }
                else
                if (flag == "map_Kd")
                {
                    var imageName = Regex.Replace(s, @"map_Kd\s*(.*\S)\s*$", "$1");
                    var img = new CSharpImageLibrary.ImageEngineImage(imageName);   
                    currentMat = new TextureMaterial(img);
                }
                else
                if (flag == "Kd")
                {
                    currentMat = new SolidMaterial(Color.FromArgb(0,
                        (int)(double.Parse(lines[1]) * 255),
                        (int)(double.Parse(lines[2]) * 255),
                        (int)(double.Parse(lines[3]) * 255)));
                }
            }
            materials.Add(currentName, currentMat);
            return materials;
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

                    foreach (Base3D.Point p in poly.points)
                        yield return $"v {p.X} {p.Y} {p.Z}";
                    foreach (Base3D.Point p in poly.normals)
                        yield return $"vn {p.X} {p.Y} {p.Z}";

                    foreach (Polygon p in poly.polygons)
                    {
                        string result = "f";
                        if(p.normals == null)
                            foreach(int v in p.indexes)
                                result += $" {v + startPoint}";
                        else
                            for(int i = 0; i < p.indexes.Length; ++i)
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
