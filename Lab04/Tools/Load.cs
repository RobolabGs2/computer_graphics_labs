using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Lab04.Tools
{
    class Load: ITool
    {
        public Bitmap image => Properties.Resources.Load;
        private Context ctx;
        public void Init(Context context)
        {
            ctx = context;
        }

        public Matrix Draw(Point start, Point end, Graphics graphics)
        {
            return Matrix.Ident();
        }

        public bool Active()
        {
            using (var dialog = new OpenFileDialog{Title = "Загрузка файла",DefaultExt = ".json"})
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ctx.Polygons = JsonSerializer.Deserialize<List<Polygon>>(File.ReadAllText(dialog.FileName));
                    ctx.Selected.Clear();
                }
            }
            return false;
        }
    }
}
