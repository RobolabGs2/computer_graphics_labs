using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab05.LSystem
{
    static class Parser
    {
        public static Turtle Parse(string[] input)
        {
            Dictionary<char, string> rules = new Dictionary<char, string>();
            Dictionary<char, Action<State, Graphics>> actions = new Dictionary<char, Action<State, Graphics>>();

            try
            {
                foreach (string s in input)
                    ParseLine(s, rules, actions);
            }
            catch
            {
                InformationMessage("Повторное объявление правила");
            }
            return new Turtle(rules, actions);
        }

        static Regex rule = new Regex(@"^(\S)\s*=>\s*(\S*)\s*$");
        static Regex move = new Regex(@"^(\S)\s*=\s*move\s*(-?\d+)\s*$");
        static Regex moveRange = new Regex(@"^(\S)\s*=\s*move\s*(-?\d+)\s*-\s*(-?\d+)\s*$");
        static Regex rotate = new Regex(@"^(\S)\s*=\s*rotate\s*(-?\d+)\s*$");
        static Regex rotateRange = new Regex(@"^(\S)\s*=\s*rotate\s*(-?\d+)\s*-\s*(-?\d+)\s*$");
        static Regex push = new Regex(@"^(\S)\s*=\s*push\s*$");
        static Regex pop = new Regex(@"^(\S)\s*=\s*pop\s*$");
        static Regex color = new Regex(@"^\s*(\w*)\s*$");
        static Regex colorPair = new Regex(@"^\s*(\w*)\s*-\s*(\w*)\s*$");
        static Regex intPair = new Regex(@"^\s*(\d+)\s*-\s*(\d+)\s*$");
        static Regex intString = new Regex(@"^\s*\d+\s*$");
        static Regex spaces = new Regex(@"^\s*$");
        static Random random = new Random();

        private static double RandonRange(double a, double b)
        {
            if (a > b)
                (a, b) = (b, a); 
            return random.NextDouble() * (b - a) + a;
        }

        private static Color GetColor(string s)
        {
            Color result = Color.FromName(s);
            if (result.A != 0)
                return result;
            InformationMessage($"Неизвестный цвет: {s}");
            return Color.Black;
        }

        public static (Color, Color) Parsecolor(string s)
        {
            if (color.IsMatch(s))
            {
                Color result = GetColor(s);
                return (result, result);
            } 
            if (colorPair.IsMatch(s))
            {
                GroupCollection match = colorPair.Matches(s)[0].Groups;
                return (GetColor(match[1].Value), GetColor(match[2].Value));
            }
            else
                InformationMessage($"Цвет должен быть в формате:\r\n<цвет>\r\n<цвет начала>-<цвет конца>");
            return (Color.Black, Color.Black);
        }

        public static (int, int) ParsPair(string s)
        {
            if (intString.IsMatch(s))
            {
                return (Int32.Parse(s), Int32.Parse(s));
            }
            if (intPair.IsMatch(s))
            {
                GroupCollection match = colorPair.Matches(s)[0].Groups;
                return (Int32.Parse(match[1].Value), Int32.Parse(match[2].Value));
            }
            else
                InformationMessage($"ожидолось число или диапазон: {s}");
            return (0, 0);
        }

        private static void ParseLine(string s, Dictionary<char, string> rules, 
            Dictionary<char, Action<State, Graphics>> actions)
        {
            if (rule.IsMatch(s))
            {
                GroupCollection match = rule.Matches(s)[0].Groups;
                rules.Add(match[1].Value[0], match[2].Value);
            }
            else
            if (move.IsMatch(s))
            {
                GroupCollection match = move.Matches(s)[0].Groups;
                actions.Add(match[1].Value[0], (state, g) => state.Move(g, Int32.Parse(match[2].Value)));
            }
            else
            if (moveRange.IsMatch(s))
            {
                GroupCollection match = moveRange.Matches(s)[0].Groups;
                actions.Add(match[1].Value[0], (state, g) =>
                    state.Move(g, RandonRange(Int32.Parse(match[2].Value), Int32.Parse(match[3].Value))));
            }
            else
            if (rotate.IsMatch(s))
            {
                GroupCollection match = rotate.Matches(s)[0].Groups;
                actions.Add(match[1].Value[0], (state, g) => state.Rotate(Int32.Parse(match[2].Value)));
            }
            else
            if (rotateRange.IsMatch(s))
            {
                GroupCollection match = rotateRange.Matches(s)[0].Groups;
                actions.Add(match[1].Value[0], (state, g) =>
                    state.Rotate(RandonRange(Int32.Parse(match[2].Value), Int32.Parse(match[3].Value))));
            }
            else
            if (push.IsMatch(s))
            {
                GroupCollection match = push.Matches(s)[0].Groups;
                actions.Add(match[1].Value[0], (state, g) => state.Push());
            }
            else
            if (pop.IsMatch(s))
            {
                GroupCollection match = pop.Matches(s)[0].Groups;
                actions.Add(match[1].Value[0], (state, g) => state.Pop());
            }
            else
            if (spaces.IsMatch(s))
            { }
            else
                InformationMessage($"Строка с непонятно чем:\r\n{s}");
        }

        public static void InformationMessage(string s)
        {
            MessageBox.Show(s, "Окошко-всплывашка", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
