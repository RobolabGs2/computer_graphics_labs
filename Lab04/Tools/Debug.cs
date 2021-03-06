﻿using Lab04.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab04.Tools
{
    class Debug : ITool
    {
        public Bitmap image => Resources.Debug;
        Context context;

        public bool Active()
        {
            context.Debug = !context.Debug;
            return false;
        }

        public Matrix Draw(Point start, Point end, Graphics graphics)
        {
            return Matrix.Ident();
        }

        public void Init(Context context)
        {
            this.context = context;
        }
    }
}