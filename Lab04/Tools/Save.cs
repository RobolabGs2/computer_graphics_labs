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
    class Save : ITool
    {
        public Bitmap image => Properties.Resources.Save;
        private Context context;

        public void Init(Context context)
        {
            this.context = context;
        }

        public Matrix Draw(Point start, Point end, Graphics graphics)
        {
            return Matrix.Ident();
        }

        public bool Active()
        {
            using (var dialog = new SaveFileDialog
            {
                Title = "Сохранение в файл", 
                DefaultExt = ".cgjson",
                Filter = "Polygons in JSON (*.cgjson)|*.cgjson"
            })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllText(dialog.FileName, JsonSerializer.Serialize(context.Polygons));
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(
                            $"Не удалось сохранить в файл '${dialog.FileName}': ${e.Message}.", "Окошко-всплывашка",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }

            return false;
        }
    }
}