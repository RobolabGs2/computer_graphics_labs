using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab06.AuxTools3D
{
    abstract class Node
    {
        public abstract double Get(double x, double y);
        public abstract Node Dx();
        public abstract Node Dy();

        public static Node operator +(Node n1, Node n2)
        {
            return new Sum(n1, n2);
        }

        public static Node operator *(Node n1, Node n2)
        {
            return new Mult(n1, n2);
        }

        public static Node operator /(Node n1, Node n2)
        {
            return new Div(n1, n2);
        }


        public static Node operator /(Node n, double d)
        {
            return new Div(n, new Number(d));
        }

        public static Node operator -(Node n1, Node n2)
        {
            return new Sum(n1, new Number(-1) * n2);
        }
    }

    class XValue : Node
    {
        public override double Get(double x, double y) => x;

        public override Node Dx()
        {
            return new Number(1);
        }

        public override Node Dy()
        {
            return new Number(0);
        }

    }

    class YValue : Node
    {
        public override double Get(double x, double y) => y;

        public override Node Dx()
        {
            return new Number(0);
        }

        public override Node Dy()
        {
            return new Number(1);
        }
    }

    class Number : Node
    {
        double value;

        public Number(double value)
        {
            this.value = value;
        }

        public override double Get(double x, double y) => value;

        public override Node Dx()
        {
            return new Number(0);
        }

        public override Node Dy()
        {
            return new Number(0);
        }
    }

    class Sum : Node
    {
        Node node1;
        Node node2;

        public Sum(Node node1, Node node2)
        {
            this.node1 = node1;
            this.node2 = node2;
        }

        public override double Get(double x, double y) => node1.Get(x, y) + node2.Get(x, y);

        public override Node Dx()
        {
            return node1.Dx() + node2.Dx();
        }

        public override Node Dy()
        {
            return node1.Dy() + node2.Dy();
        }
    }

    class Mult : Node
    {
        Node node1;
        Node node2;

        public Mult(Node node1, Node node2)
        {
            this.node1 = node1;
            this.node2 = node2;
        }

        public override double Get(double x, double y) => node1.Get(x, y) * node2.Get(x, y);

        public override Node Dx()
        {
            return node1.Dx() * node2 + node1 * node2.Dx();
        }

        public override Node Dy()
        {
            return node1.Dy() * node2 + node1 * node2.Dy();
        }
    }

    class Div : Node
    {
        Node U;
        Node V;

        public Div(Node U, Node V)
        {
            this.U = U;
            this.V = V;
        }

        public override double Get(double x, double y) => U.Get(x, y) / V.Get(x, y);

        private Node Prod(Node u, Node du, Node v, Node dv) => (du * v - dv * u) / (v * v);

        public override Node Dx()
        {
            return Prod(U, U.Dx(), V, V.Dx());
        }

        public override Node Dy()
        {
            return Prod(U, U.Dy(), V, V.Dy());
        }
    }

    class Sin : Node
    {
        Node node;

        public Sin(Node node)
        {
            this.node = node;
        }

        public override double Get(double x, double y) => Math.Sin(node.Get(x, y));

        public override Node Dx()
        {
            return new Cos(node) * node.Dx();
        }

        public override Node Dy()
        {
            return new Cos(node) * node.Dy();
        }
    }

    class Cos : Node
    {
        Node node;

        public Cos(Node node)
        {
            this.node = node;
        }

        public override double Get(double x, double y) => Math.Cos(node.Get(x, y));

        public override Node Dx()
        {
            return new Sin(node) * new Number(-1) * node.Dx();
        }

        public override Node Dy()
        {
            return new Sin(node) * new Number(-1) * node.Dy();
        }
    }

    class Exp : Node
    {
        Node node;

        public Exp(Node node)
        {
            this.node = node;
        }

        public override double Get(double x, double y) => Math.Exp(node.Get(x, y));

        public override Node Dx()
        {
            return this * node.Dx();
        }

        public override Node Dy()
        {
            return this * node.Dy();
        }
    }
}
