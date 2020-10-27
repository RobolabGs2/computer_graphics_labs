﻿using System;
using System.Windows.Forms;
using Lab06.Base3D;

namespace Lab06.Tools3D.AddItem
{
    class MultiItem<TotalEntity, PartialEntity>
        where TotalEntity : Entity
        where PartialEntity : Entity
    {
        PartialEntity entity;
        TotalEntity totalEntity;
        Point location;
        Context context;
        Func<TotalEntity> newTotalEntity;
        Action<TotalEntity, PartialEntity> append;
        Func<PartialEntity> newPartialEntity;

        MultiItem(Func<TotalEntity> newTotalEntity,
            Action<TotalEntity, PartialEntity> append,
            Func<PartialEntity> newPartialEntity, Context context)
        {
            this.newPartialEntity = newPartialEntity;
            this.append = append;
            this.newTotalEntity = newTotalEntity;
            this.context = context;
        }

        private void New()
        {
            totalEntity = newTotalEntity();
            context.world.entities.Add(totalEntity);
            Next();
        }
        private void Next()
        {
            entity = newPartialEntity();
            context.world.selected.Add(entity);
            location = new Point();
        }
        private void Restart()
        {
            Delete();
            New();
        }
        public static void AddButton(TabButton button,
            Func<TotalEntity> newTotalEntity,
            Action<TotalEntity, PartialEntity> append,
            Func<PartialEntity> newPartialEntity, Context context)
        {
            var item = new MultiItem<TotalEntity, PartialEntity>(newTotalEntity, append, newPartialEntity, context);
            button.ButtonClick += b =>
            {
                item.New();
                context.KeyUp += item.Keys;
                context.pictureBox.MouseClick += item.MouseClick;
                context.pictureBox.MouseMove += item.MouseMove;
            };

            button.ButtonDisable += b =>
            {
                item.Flush();
                context.KeyUp -= item.Keys;
                context.pictureBox.MouseClick -= item.MouseClick;
                context.pictureBox.MouseMove -= item.MouseMove;
                context.Redraw();
            };
        }

        private void Keys(object sender, KeyEventArgs args)
        {
            switch (args.KeyCode)
            {
                case System.Windows.Forms.Keys.Escape:
                    Restart();
                    break;
                case System.Windows.Forms.Keys.Enter:
                    Flush();
                    New();
                    break;
            }
            context.Redraw();
        }

        private void Flush()
        {
            context.world.selected.Remove(entity);
            context.world.selected.Add(totalEntity);
        }

        private void MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            append(totalEntity, entity);
            context.world.selected.Remove(entity);
            Next();
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
            context.world.entities.Remove(totalEntity);
            context.world.selected.Remove(entity);
        }
    }
}