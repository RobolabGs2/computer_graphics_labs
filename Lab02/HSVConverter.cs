﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace Lab02
{
    class HSVConverter: ISolution
    {
        const int scrollRange = 100;

        private List<PictureBox> pictures;
        private List<TrackBar> tracks;
        FastBitmap bitmap;

        public string Name { get; }
        public HSVConverter()
        {
            pictures = new List<PictureBox>();
            tracks = new List<TrackBar>();
            Name = "HSV";
        }

        public void Init(Control container)
        {
            int width = container.Width;
            int height = container.Height;

            for (int i = 0; i < 4; ++i)
            {
                var pict = new PictureBox
                {
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Width = width / 2 - 10,
                    Height = height / 3,
                };

                container.Controls.Add(pict);
                pictures.Add(pict);
            }

            string[] labels = { "Hue", "Saturation", "Brightness" };
            for (int i = 0; i < 3; ++i)
            {
                var track = new TrackBar
                {
                    Width = width - 120,
                    Minimum = 0,
                    Maximum = scrollRange,
                    Value = scrollRange / 2
                };
                var label = new Label { Width = 100, Text = labels[i] };
                track.Scroll += new EventHandler(TrackBarScroll);
                container.Controls.Add(track);
                container.Controls.Add(label);
                tracks.Add(track);
            }
            Button btn = new Button { Text = "Save" };
            btn.Click += new EventHandler(SaveClick);
            container.Controls.Add(btn);
        }

        private void RefrashPictures(PictureBox[] pbs, Func<Color, Color>[] map)
        {
            Bitmap[] bmps = bitmap.Map(map);
            for(int i = 0; i < pbs.Length; ++i)
            {
                pbs[i].Image?.Dispose();
                pbs[i].Image = bmps[i];
            }
        }

        private void TrackBarScroll(object sender, EventArgs e)
        {
            int hue =        tracks[0].Value;
            int saturation = tracks[1].Value;
            int value =      tracks[2].Value;

            Func<Color, Color>[] funcs = {
                col => HueColor(col, hue),
                col => SaturationColor(col, saturation),
                col => ValueColor(col, value),
                col => HSVColor(col, hue, saturation, value),
            };

            for (int i = 0; i < 3; ++i)
                if (sender == tracks[i])
                {
                    RefrashPictures(
                      new PictureBox[] { pictures[i], pictures[3] },
                      new Func<Color, Color>[] { funcs[i], funcs[3] });
                    return;
                }
            RefrashPictures(pictures.ToArray(), funcs);
        }

        static double Scale(double source, double depth)
        {
            if (depth < scrollRange / 2)
                source = source * depth * 2 / scrollRange;
            else
                source += (100 - source) * (depth - scrollRange / 2) * 2 / scrollRange;
            return source;
        }

        static int Hue(Color col, int depth)
        {
            int max = Math.Max(col.R, Math.Max(col.G, col.B));
            int min = Math.Min(col.R, Math.Min(col.G, col.B));
            if (min == max) return 0;
            int result;
            if (max == col.R)
            {
                result = 60 * (col.G - col.B) / (max - min);
                if (col.G < col.B)
                    result += 360;
            }
            else if (max == col.G)
                result = 60 * (col.B - col.R) / (max - min) + 120;
            else
                result = 60 * (col.R - col.G) / (max - min) + 240;
            result = (result + depth * 360 / scrollRange + 190) % 360;
            return result;
        }

        static double Saturation(Color col, int depth)
        {
            int max = Math.Max(col.R, Math.Max(col.G, col.B));
            int min = Math.Min(col.R, Math.Min(col.G, col.B));
            if (max == 0)
                return 0;
            double result = (max - min) * 100 / (double)max;
            return Scale(result, depth);
        }

        static double Value(Color col, int depth)
        {
            double result =  Math.Max(col.R, Math.Max(col.G, col.B)) * 100 / (double) 255;
            return Scale(result, depth);
        }

        static Color HueColor(Color col, int depth)
        {
            return HSV(Hue(col, depth), 100, 100);
        }

        static Color SaturationColor(Color col, int depth)
        {
            return Color.FromArgb(0, (int)Math.Round(Saturation(col, depth) * 255 / 100), 0);
        }

        static Color ValueColor(Color col, int depth)
        {
            return Color.FromArgb(0,(int)Math.Round(Value(col, depth) * 255 / 100), 0);
        }

        static Color HSV(int H, double S, double V)
        {
            int Hi = (H / 60) % 6;
            double Vmin = (100 - S) * V / 100;
            double a = (V - Vmin) * (H % 60) / 60;
            int Vinc = (int)Math.Round((Vmin + a) * 255 / 100);
            int Vdec = (int)Math.Round((V - a) * 255 / 100);
            int iVmin = (int)Math.Round(Vmin * 255 / 100);
            int iV = (int)Math.Round(V * 255 / 100);
            switch (Hi)
            {
                case (0): return Color.FromArgb(iV, Vinc, iVmin);
                case (1): return Color.FromArgb(Vdec, iV, iVmin);
                case (2): return Color.FromArgb(iVmin, iV, Vinc);
                case (3): return Color.FromArgb(iVmin, Vdec, iV);
                case (4): return Color.FromArgb(Vinc, iVmin, iV);
                case (5): return Color.FromArgb(iV, iVmin, Vdec);
                default: return Color.Black;
            }
        }

        static Color HSVColor(Color col, int h, int s, int v)
        {
            return HSV(Hue(col, h), Saturation(col, s), Value(col, v));
        }

        public void Show(FastBitmap bitmap)
        {
            this.bitmap = bitmap;
            TrackBarScroll(null, null);
        }

        private void SaveClick(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "txt files (*.png)|*.png";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fname = saveFileDialog1.FileName;
                if (fname != null)
                {
                    int hue = tracks[0].Value;
                    int saturation = tracks[1].Value;
                    int value = tracks[2].Value;

                    bitmap.Map(new Func<Color, Color>[]{
                        col =>  HSVColor(col, hue, saturation, value)})[0].
                        Save(fname, ImageFormat.Png);
                }
            }
        }
    }
}
