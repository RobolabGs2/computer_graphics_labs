using Lab06.Base3D;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab06.Tools3D.Graph
{
    public class Graph : IToolPage
    {
        public Bitmap Image => Properties.Resources.Graph;

        public void Init(ToolTab tab, Context context)
        {
            Parser p = new Parser();
            var panel = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 7,
                Dock = DockStyle.Top,
                ForeColor = Constants.borderColore,
                AutoSize = true,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                Font = new Font("Consalas", 12)
            };

            var funcText = new TextBox { Dock = DockStyle.Fill, Text = "(x *x + sin(y) *  y) / 5"};
            panel.Controls.AddRange(new Control[] { new Label { Text = "Z = " }, funcText });
            var XStart = new TextBox { Dock = DockStyle.Fill, Text = "-5" };
            panel.Controls.AddRange(new Control[] { new Label { Text = "X от" }, XStart });
            var XEnd = new TextBox { Dock = DockStyle.Fill, Text = "5" };
            panel.Controls.AddRange(new Control[] { new Label { Text = "X до" }, XEnd });
            var XStep = new TextBox { Dock = DockStyle.Fill, Text = "1" };
            panel.Controls.AddRange(new Control[] { new Label { Text = "X Шаг" }, XStep });
            var YStart = new TextBox { Dock = DockStyle.Fill, Text = "-5" };
            panel.Controls.AddRange(new Control[] { new Label { Text = "Y от" }, YStart });
            var YEnd = new TextBox { Dock = DockStyle.Fill, Text = "5" };
            panel.Controls.AddRange(new Control[] { new Label { Text = "Y до" }, YEnd });
            var YStep = new TextBox { Dock = DockStyle.Fill, Text = "1" };
            panel.Controls.AddRange(new Control[] { new Label { Text = "Y Шаг" }, YStep });
            tab.Settings.Controls.Add(panel);

            tab.AddButton(Properties.Resources.Graph, false).ButtonClick +=
                b =>
                {
                    try
                    {
                        context.world.entities.Add(MakeGraph(p.Parse(funcText.Text),
                            double.Parse(XStart.Text),
                            double.Parse(XEnd.Text),
                            double.Parse(XStep.Text),
                            double.Parse(YStart.Text),
                            double.Parse(YEnd.Text),
                            double.Parse(YStep.Text)));
                        context.Redraw();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show($"Ошибка: \"{e.Message}\"", "Окошко-всплывашка", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                };

        }

        class Parser
        {
            string s;
            int idx;
            
            char Get()
            {
                if (idx >= s.Length)
                    return '\0';
                return s[idx];
            }

            void SkipSpaces()
            {
                while (Get() != '\0' && char.IsWhiteSpace(Get()))
                    ++idx;
            }


            void Pass(string target)
            {
                foreach (var c in target)
                {
                    if (char.ToLower(Get()) != char.ToLower(c))
                        throw new Exception($"Ожидалось {target}"); ;
                    ++idx;
                }
                SkipSpaces();
            }

            bool PassIfExist(string target)
            {
                foreach (var c in target)
                {
                    if (char.ToLower(Get()) != char.ToLower(c))
                        return false;
                    ++idx;
                }
                SkipSpaces();
                return true;
            }

            INode Brackets()
            {
                Pass("(");
                var result = ParseSum();
                Pass(")");
                return result;
            }

            INode ParseSin()
            {
                Pass("sin");
                var value = Brackets();
                return new Sin(value);
            }

            INode ParseCos()
            {
                Pass("cos");
                var value = Brackets();
                return new Cos(value);
            }
            INode ParseExp()
            {
                Pass("exp");
                var value = Brackets();
                return new Exp(value);
            }

            INode ParseX()
            {
                Pass("x");
                return new XValue();
            }

            INode ParseY()
            {
                Pass("y");
                return new YValue();
            }

            INode UnaryMinus()
            {
                Pass("-");
                return new Number(-1) * Term();
            }

            INode ParseNumber()
            {
                string result = "";
                while(char.IsDigit(Get()) || Get() == ',' || Get() == '.')
                {
                    result += Get();
                    ++idx;
                }
                SkipSpaces();
                return new Number(double.Parse(result));
            }

            INode Term()
            {
                if (Get() == '(')
                    return Brackets();
                if (char.ToLower(Get()) == 's')
                    return ParseSin();
                if (char.ToLower(Get()) == 'c')
                    return ParseCos();
                if (char.ToLower(Get()) == 'e')
                    return ParseExp();
                if (char.ToLower(Get()) == 'x')
                    return ParseX();
                if (char.ToLower(Get()) == 'y')
                    return ParseY();
                if (char.ToLower(Get()) == '-')
                    return UnaryMinus();
                if (char.ToLower(Get()) == '+')
                    return Term();
                if (char.IsDigit(Get()))
                    return ParseNumber();
                throw new Exception($"Непонятный терм, начинается на {Get()}");
            }

            INode ParseMult()
            {
                var t = Term();
                while(Get() == '*' || Get() == '/')
                {
                    if (PassIfExist("*"))
                        t = t * Term();
                    if (PassIfExist("/"))
                        t = t / Term();
                }
                return t;
            }

            INode ParseSum()
            {
                var t = ParseMult();
                while (Get() == '+' || Get() == '-')
                {
                    if (PassIfExist("+"))
                        t = t + ParseMult();
                    if (PassIfExist("-"))
                        t = t - ParseMult();
                }
                return t;
            }

            public INode Parse(string s)
            {
                idx = 0;
                this.s = s;
                SkipSpaces();
                return ParseSum();
            }
        }

        Entity MakeGraph(INode node, double xStart, double xEnd, double xStep, double yStart, double yEnd, double yStep)
        {
            var result = new Polytope();
            int xDelta = 0;
            int yDelta = 0;
            for (double i = xStart; i <= xEnd; i += xStep)
            {
                xDelta += 1;
                yDelta = 0;
                for (double j = yStart; j <= yEnd; j += yStep)
                {
                    yDelta += 1;
                    result.Add(new Base3D.Point { X = i, Y = j, Z = node.Get(i, j) });
                    result.AddNormal(new Base3D.Point { X = -node.Dx().Get(i, j), Y = -node.Dy().Get(i, j), Z = 1 }.Normal());
                    result.AddNormal(new Base3D.Point { X = node.Dx().Get(i, j), Y = node.Dy().Get(i, j), Z = -1 }.Normal());
                }
            }

            for (int i = 0; i < xDelta - 1; ++i)
                for (int j = 0; j < yDelta - 1; ++j)
                {
                    result.Add(new Polygon(new int[]{
                        j + i * yDelta,
                        j + (i + 1) * yDelta,
                        (j + 1) + (i + 1) * yDelta,
                        (j + 1) + i * yDelta},
                        new int[]{
                        2 * (j + i * yDelta),
                        2 * (j + (i + 1) * yDelta),
                        2 * ((j + 1) + (i + 1) * yDelta),
                        2 * ((j + 1) + i * yDelta)}
                    ));

                    result.Add(new Polygon(new int[]{
                        j + i * yDelta,
                        (j + 1) + i * yDelta,
                        (j + 1) + (i + 1) * yDelta,
                        j + (i + 1) * yDelta},
                        new int[]{
                        2 * (j + i * yDelta) + 1,
                        2 * ((j + 1) + i * yDelta) + 1,
                        2 * ((j + 1) + (i + 1) * yDelta) + 1,
                        2 * (j + (i + 1) * yDelta) + 1}
                    ));
                }

            return result;
        }
    }

    abstract class INode
    {
        public abstract double Get(double x, double y);
        public abstract INode Dx();
        public abstract INode Dy();

        public static INode operator +(INode n1, INode n2)
        {
            return new Sum(n1, n2);
        }

        public static INode operator *(INode n1, INode n2)
        {
            return new Mult(n1, n2);
        }

        public static INode operator /(INode n1, INode n2)
        {
            return new Div(n1, n2);
        }


        public static INode operator /(INode n, double d)
        {
            return new Div(n, new Number(d));
        }

        public static INode operator -(INode n1, INode n2)
        {
            return new Sum(n1, new Number(-1) * n2);
        }
    }

    class XValue : INode
    {
        public override double Get(double x, double y) => x;

        public override INode Dx()
        {
            return new Number(1);
        }

        public override INode Dy()
        {
            return new Number(0);
        }
    }

    class YValue : INode
    {
        public override double Get(double x, double y) => y;

        public override INode Dx()
        {
            return new Number(0);
        }

        public override INode Dy()
        {
            return new Number(1);
        }
    }

    class Number : INode
    {
        double value;

        public Number(double value)
        {
            this.value = value;
        }

        public override double Get(double x, double y) => value;

        public override INode Dx()
        {
            return new Number(0);
        }

        public override INode Dy()
        {
            return new Number(0);
        }
    }

    class Sum : INode
    {
        INode node1;
        INode node2;

        public Sum(INode node1, INode node2)
        {
            this.node1 = node1;
            this.node2 = node2;
        }

        public override double Get(double x, double y) => node1.Get(x, y) + node2.Get(x, y);

        public override INode Dx()
        {
            return node1.Dx() + node2.Dx();
        }

        public override INode Dy()
        {
            return node1.Dy() + node2.Dy();
        }
    }

    class Mult : INode
    {
        INode node1;
        INode node2;

        public Mult(INode node1, INode node2)
        {
            this.node1 = node1;
            this.node2 = node2;
        }

        public override double Get(double x, double y) => node1.Get(x, y) * node2.Get(x, y);

        public override INode Dx()
        {
            return node1.Dx() * node2 + node1 * node2.Dx();
        }

        public override INode Dy()
        {
            return node1.Dy() * node2 + node1 * node2.Dy();
        }
    }

    class Div : INode
    {
        INode U;
        INode V;

        public Div(INode U, INode V)
        {
            this.U = U;
            this.V = V;
        }

        public override double Get(double x, double y) => U.Get(x, y) / V.Get(x, y);

        private INode Prod(INode u, INode du, INode v, INode dv) => (du * v - dv * u) / (v * v);

        public override INode Dx()
        {
            return Prod(U, U.Dx(), V, V.Dx());
        }

        public override INode Dy()
        {
            return Prod(U, U.Dy(), V, V.Dy());
        }
    }

    class Sin : INode
    {
        INode node;

        public Sin(INode node)
        {
            this.node = node;
        }

        public override double Get(double x, double y) => Math.Sin(node.Get(x, y));

        public override INode Dx()
        {
            return new Cos(node) * node.Dx();
        }

        public override INode Dy()
        {
            return new Cos(node) * node.Dy();
        }
    }

    class Cos : INode
    {
        INode node;

        public Cos(INode node)
        {
            this.node = node;
        }

        public override double Get(double x, double y) => Math.Cos(node.Get(x, y));

        public override INode Dx()
        {
            return new Sin(node) * new Number(-1) * node.Dx();
        }

        public override INode Dy()
        {
            return new Sin(node) * new Number(-1) * node.Dy();
        }
    }

    class Exp : INode
    {
        INode node;

        public Exp(INode node)
        {
            this.node = node;
        }

        public override double Get(double x, double y) => Math.Exp(node.Get(x, y));

        public override INode Dx()
        {
            return this * node.Dx();
        }

        public override INode Dy()
        {
            return this * node.Dy();
        }
    }

}
