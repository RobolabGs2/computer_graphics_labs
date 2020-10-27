using Lab06.Base3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab06.Tools3D.AddItem
{
    class Item
    {
        Entity entity;
        Point location;
        Context context;
        Func<Entity> func;

        Item(Func<Entity> func, Context context)
        {
            this.func = func;
            this.entity = func();
            this.context = context;
            context.world.entities.Add(entity);
        }

        public static void AddButton(TabButton button, Func<Entity> func, Context context)
        {
            Item item = null;
            button.ButtonClick += b =>
            {
                item = new Item(func, context);
                context.pictureBox.MouseClick += item.MouseClick;
                context.pictureBox.MouseMove += item.MouseMove;
            };

            button.ButtonDisable += b =>
            {
                context.pictureBox.MouseClick -= item.MouseClick;
                context.pictureBox.MouseMove -= item.MouseMove;
                item.Delete();
                context.Redraw();
            };
        }

        private void MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            context.world.selected.Add(entity);
            entity = func();
            location = new Point();
            //MouseMove(sender, new MouseEventArgs(MouseButtons.Left, 0, e.X, e.Y, 0));
            context.world.entities.Add(entity);
            context.Redraw();
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.None)
                return;
            var point = context.ScreenToXY(e.X, e.Y);
            if (!point.front)
                return;
            entity.Apply(Matrix.Move(point.p - location));
            location = point.p;
            context.Redraw();
        }

        private void Delete()
        {
            context.world.entities.Remove(entity);
        }
    }
}