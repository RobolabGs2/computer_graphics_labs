using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab06
{
    public class ToolTabs : TabControl
    {
        Bitmap[] Images { get; }
        public ToolTabs(IEnumerable<ToolTab> tabs, IEnumerable<Bitmap> images)
        {
            TabPages.AddRange(tabs.ToArray());
            Dock = DockStyle.Fill;
            this.SizeMode = TabSizeMode.Fixed;
            this.ItemSize = new System.Drawing.Size(36, 34);
            this.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.DrawItem += tabControlDrawItem;
            this.Images = images.ToArray();
        }

        private const int TCM_ADJUSTRECT = 0x1328;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == TCM_ADJUSTRECT)
            {
                RECT rect = (RECT)(m.GetLParam(typeof(RECT)));
                rect.Left = Left - Margin.Left;
                rect.Right = Right + Margin.Right;
                rect.Top = Top - Margin.Top;
                rect.Bottom = Bottom + Margin.Bottom;
                Marshal.StructureToPtr(rect, m.LParam, true);
            }

            base.WndProc(ref m);
        }

        private void tabControlDrawItem(object sender, DrawItemEventArgs e)
        {
            using (Brush br = new SolidBrush(Constants.backColore))
                e.Graphics.FillRectangle(br, e.Bounds);
            
            var bmp = Images[e.Index];
            e.Graphics.DrawImage(bmp, e.Bounds, new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
        }

        private struct RECT
        {
            public int Left, Top, Right, Bottom;
        }
    }

    public class ToolTab : TabPage
    {
        FlowLayoutPanel panel;
        List<TabButton> buttons = new List<TabButton>();
        public Action ClearEvents = () => { };  // TODO: его надо где-то вызвать

        public ToolTab()
        {
            BackColor = Constants.backColore;
            panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(panel);
        }
    
        public TabButton AddButton(Bitmap image, bool fixedColor = true)
        {
            TabButton p = new TabButton
            {
                Image = image,
                Size = image.Size,
                BackColor = Color.Transparent,
                Margin = new Padding(20, 20, 0, 0),
            };

            p.MouseDown += (o, s) => {
                buttons.ForEach(b => { 
                    if (b.ButtonEnable) { 
                        b.BackColor = Color.Transparent;
                        b.ButtonEnable = false;
                        b.ButtonDisable(b);
                    } });
                p.BackColor = Constants.textColore;
                p.ButtonEnable = fixedColor;
                p.ButtonClick(p);
            };

            if(!fixedColor)
                p.MouseUp += (o, s) => p.BackColor = Color.Transparent;
            panel.Controls.Add(p);
            buttons.Add(p);
            return p;
        }
    }

    public class TabButton: PictureBox
    {
        public Action<TabButton> ButtonClick = t => { };
        public Action<TabButton> ButtonDisable = t => { };
        public bool ButtonEnable { get; set; } = false;
    }
}
