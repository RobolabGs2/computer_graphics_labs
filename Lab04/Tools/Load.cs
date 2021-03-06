﻿using System;
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
    class Load : ITool
    {
        public Bitmap image => Properties.Resources.Load;
        private Context context;
        private readonly string defaultFileName;

        public Load(string defaultFileName = null)
        {
            this.defaultFileName = defaultFileName;
        }

        public void Init(Context context)
        {
            this.context = context;
            if (defaultFileName != null)
                TryLoadPolygons(defaultFileName);
        }

        public Matrix Draw(Point start, Point end, Graphics graphics)
        {
            return Matrix.Ident();
        }

        public bool Active()
        {
            using (var dialog = new OpenFileDialog
            {
                Title = "Загрузка из файла",
                DefaultExt = ".cgjson",
                Filter = "Polygons in JSON (*.cgjson)|*.cgjson"
            })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                    TryLoadPolygons(dialog.FileName);
            }

            return false;
        }

        private HashSet<Polygon> GetFromFile(string filename)
        {
            return JsonSerializer.Deserialize<HashSet<Polygon>>(File.ReadAllText(filename));
        }

        private void TryLoadPolygons(string filename)
        {
            try
            {
                context.Polygons = GetFromFile(filename);
                context.Selected.Clear();
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    $"Не удалось загрузить файл '${filename}': ${e.Message}.", "Окошко-всплывашка",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}