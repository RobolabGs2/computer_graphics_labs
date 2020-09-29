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
    class Save: ITool
    {
        public Bitmap image => Properties.Resources.Save;
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
            using (var dialog = new SaveFileDialog{Title = "Сохранение файла",DefaultExt = ".json"})
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var json = JsonSerializer.Serialize(ctx.Polygons);
                    File.WriteAllText(dialog.FileName, json);
                }
            }
            return false;
        }
    }
}
