using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab05.LSystem
{
    class Turtle
    {
        Dictionary<char, string> rules;
        Dictionary<char, Action<State, Graphics>> actions;

        State state;
        (Color, Color) colors = (Color.Black, Color.Black);
        (int, int) widths = (0, 0);
        (int, int) scales = (0, 0);
        (int, int) rotations = (0, 0);
        int maxDepth = 0;

        public Turtle(Dictionary<char, string> rules, Dictionary<char, Action<State, Graphics>> actions)
        {
            this.rules = rules;
            this.actions = actions;
        }

        private int GetAverageColorC(float k, int c1, int c2)
        {
            return (int)Math.Round(c1 * k + c2 * (1 - k));
        }

        private Color GetAverageColor(float k, int depth)
        {
            return Color.FromArgb(
                GetAverageColorC(k, colors.Item1.A, colors.Item2.A),
                GetAverageColorC(k, colors.Item1.R, colors.Item2.R),
                GetAverageColorC(k, colors.Item1.G, colors.Item2.G),
                GetAverageColorC(k, colors.Item1.B, colors.Item2.B));
        }

        private void Parse(Graphics g, char c, int depth)
        {
            if (depth > 0)
                if (rules.ContainsKey(c))
                {
                    foreach (char c1 in rules[c])
                        Parse(g, c1, depth - 1);
                    return;
                }

            float k = maxDepth == 0 ? 1 : depth / (float)maxDepth;
            state.SetRotateError(GetAverageColorC(k, rotations.Item1, rotations.Item2));
            state.SetScaleError(GetAverageColorC(k, scales.Item1, scales.Item2));
            state.SetLine(GetAverageColor(k, depth), GetAverageColorC(k, widths.Item1, widths.Item2));
            if (actions.ContainsKey(c))
                actions[c](state, g);
        }

        public void Parse(Graphics g, string s, int depth, Point start, (Color, Color) colors, 
            (int, int) widths, (int, int) scales, (int, int) rotations)
        {
            maxDepth = depth;
            this.colors = colors;
            this.widths = widths;
            this.scales = scales;
            this.rotations = rotations;
            state = new State(start);
            foreach (char c in s)
                Parse(g, c, depth);
        }
    }
}
