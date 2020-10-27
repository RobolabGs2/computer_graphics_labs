using Lab06.Base3D;
using Lab06.Tools3D.AddItem;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab06.Tools3D.Selection
{
    class Selection : IToolPage
    {
        public Bitmap Image => Properties.Resources.Mouse;
        bool rectActive = false;
        System.Drawing.Point rectStart;
        System.Drawing.Point rectEnd;
        Context context;

        bool cursorActive = false;

        public void Init(ToolTab tab, Context context)
        {
            this.context = context;
            var cursorButton = tab.AddButton(Properties.Resources.Mouse);
            cursorButton.ButtonClick += b => cursorActive = true;
            cursorButton.ButtonDisable += b => cursorActive = false;
            var rectButton = tab.AddButton(Properties.Resources.Rectangle);
            rectButton.ButtonClick += b => rectActive = true;
            rectButton.ButtonDisable += b => rectActive = false;
            context.pictureBox.MouseDown += MouseDown;
            context.pictureBox.MouseMove += MouseMove;
            context.pictureBox.MouseUp += MouseUp;

            tab.AddButton(Properties.Resources.SelectAll, false).ButtonClick += b => {
                if (context.world.entities.Count != context.world.selected.Count)
                    context.world.selected.UnionWith(context.world.entities);
                else
                    context.world.selected.Clear();
                context.Redraw();
            };

            tab.AddButton(Properties.Resources.Trash, false).ButtonClick += b => {
                context.world.entities.ExceptWith(context.world.selected);
                context.world.selected.Clear();
                context.Redraw();
            };

            tab.AddButton(Properties.Resources.Load).ButtonClick += b => {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                saveFileDialog1.Filter = "txt files (*.obj)|*.obj";
                saveFileDialog1.RestoreDirectory = true;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllLines(saveFileDialog1.FileName, Obj.Store(context.world.selected).ToArray());
                }
            };
        }

        private void DrawRectangle(Graphics g)
        {
            Pen pen = new Pen(Constants.borderColore) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
            g.DrawRectangle(pen, Math.Min(rectStart.X, rectEnd.X), Math.Min(rectStart.Y, rectEnd.Y),
                Math.Abs(rectStart.X - rectEnd.X), Math.Abs(rectStart.Y - rectEnd.Y));
            pen.Dispose();
        }

        private void MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            if (rectActive)
            {
                context.Posteffect += DrawRectangle;
                rectStart = new System.Drawing.Point(e.X, e.Y);
            }
            if(cursorActive)
            {
                CursorClick(e.X, e.Y);
                context.Redraw();
            }
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            if (rectActive)
            {
                rectEnd = new System.Drawing.Point(e.X, e.Y);
                context.world.selected.Clear();
                Matrix drawingMatrix = context.DrawingMatrix();
                foreach (var entity in context.world.entities)
                {
                    if (entity.Points().All(p =>
                    {
                        var screenPoint = p * drawingMatrix;
                        if (!context.BeforeScreen(screenPoint.X))
                            return false;
                        return 
                            screenPoint.Y > Math.Min(rectStart.X, rectEnd.X) &&
                            screenPoint.Y < Math.Max(rectStart.X, rectEnd.X) && 
                            screenPoint.Z > Math.Min(rectStart.Y, rectEnd.Y) &&
                            screenPoint.Z < Math.Max(rectStart.Y, rectEnd.Y);
                    }))
                        context.world.selected.Add(entity);
                }
                context.Redraw();
            }
            if (cursorActive)
            {
                CursorClick(e.X, e.Y);
                context.Redraw();
            }
        }

        private void MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            if (rectActive)
            {
                context.Posteffect -= DrawRectangle;
                context.Redraw();
            }
        }

        private void CursorClick(int x, int y)
        {
            context.world.selected.Clear();
            foreach (var entity in context.world.entities)
                if (entity is Polytope poly)
                    if(context.ScreenPointInPolytope(x, y, poly))
                        context.world.selected.Add(entity);
        }
    }
}
