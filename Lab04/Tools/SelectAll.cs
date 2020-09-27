﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab04.Properties;

namespace Lab04.Tools
{
    class SelectAll: ITool
    {
        public Bitmap image => Resources.SelectAll;
        private Context _context;
        public void Init(Context context)
        {
            _context = context;
        }
        public Matrix Draw(Point start, Point end, Graphics graphics)
        {
            return Matrix.Ident();
        }

        public bool Active()
        {
            if (_context.Selected.Count == _context.Polygons.Count)
            {
                _context.Selected.Clear();
                return false;
            }
            _context.Selected.Clear();
            _context.Selected.AddRange(_context.Polygons);
            return false;
        }
    }
}
