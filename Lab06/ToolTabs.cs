using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

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
            this.Deselected += (sender, args) =>
            {
                var toolTab = (ToolTab) args.TabPage;
                toolTab.TabDeselected();
                toolTab.ClearEvents();
            };
            this.Selecting += (sender, args) =>
            {
                var toolTab = (ToolTab) args.TabPage;
                toolTab.TabSelected();
            };
        }

        private const int TCM_ADJUSTRECT = 0x1328;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == TCM_ADJUSTRECT)
            {
                RECT rect = (RECT) (m.GetLParam(typeof(RECT)));
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
        FlowLayoutPanel panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Constants.backColore,
        };

        List<TabButton> buttons = new List<TabButton>();

        SplitContainer mainContainer = new SplitContainer
        {
            Orientation = Orientation.Horizontal,
            Dock = DockStyle.Fill,
            BackColor = Constants.backColore,
        };

        public Control Settings => mainContainer.Panel2;
        public Action TabSelected = () => { };
        public Action TabDeselected = () => { };
        public Action ClearEvents = () => { };

        public ToolTab()
        {
            BackColor = Constants.backColore;
            Controls.Add(mainContainer);
            mainContainer.Panel1.Controls.Add(panel);
            ClearEvents += () => buttons.ForEach(b =>
            {
                if (b.ButtonEnable) b.ButtonDisable(b);
            });
            mainContainer.Panel2.Padding = new Padding(10);
        }

        public TabButton AddButton(Bitmap image, bool fixedButton = true)
        {
            TabButton p = new TabButton
            {
                Image = image,
                Size = image.Size,
                BackColor = Color.Transparent,
                Margin = new Padding(20, 20, 0, 0),
            };

            p.MouseDown += (o, s) =>
            {
                buttons.ForEach(b =>
                {
                    if (b.ButtonEnable) b.ButtonDisable(b);
                });
                if (s.Button.HasFlag(MouseButtons.Right) && p.ButtonEnable)
                    p.ButtonDisable(p);
                if (s.Button.HasFlag(MouseButtons.Left))
                    p.ButtonClick(p);
            };

            if (!fixedButton)
                p.MouseUp += (o, s) => p.ButtonDisable(p);
            panel.Controls.Add(p);
            buttons.Add(p);
            return p;
        }
    }

    public class TabButton : PictureBox
    {
        public Action<TabButton> ButtonClick = t => { };
        public Action<TabButton> ButtonDisable = t => { };
        public bool ButtonEnable { get; set; } = false;

        public TabButton()
        {
            ButtonClick += t =>
            {
                BackColor = Constants.textColore;
                ButtonEnable = true;
            };
            ButtonDisable += t =>
            {
                BackColor = Color.Transparent;
                ButtonEnable = false;
            };
        }
    }
}