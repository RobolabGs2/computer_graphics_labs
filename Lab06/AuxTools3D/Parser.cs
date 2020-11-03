using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab06.AuxTools3D
{
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

        Node Brackets()
        {
            Pass("(");
            var result = ParseSum();
            Pass(")");
            return result;
        }

        Node ParseSin()
        {
            Pass("sin");
            var value = Brackets();
            return new Sin(value);
        }

        Node ParseCos()
        {
            Pass("cos");
            var value = Brackets();
            return new Cos(value);
        }
        Node ParseExp()
        {
            Pass("exp");
            var value = Brackets();
            return new Exp(value);
        }

        Node ParseX()
        {
            Pass("x");
            return new XValue();
        }

        Node ParseY()
        {
            Pass("y");
            return new YValue();
        }

        Node UnaryMinus()
        {
            Pass("-");
            return new Number(-1) * Term();
        }

        Node ParseNumber()
        {
            string result = "";
            while (char.IsDigit(Get()) || Get() == ',' || Get() == '.')
            {
                result += Get();
                ++idx;
            }
            SkipSpaces();
            return new Number(double.Parse(result));
        }

        Node Term()
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

        Node ParseMult()
        {
            var t = Term();
            while (Get() == '*' || Get() == '/')
            {
                if (PassIfExist("*"))
                    t = t * Term();
                if (PassIfExist("/"))
                    t = t / Term();
            }
            return t;
        }

        Node ParseSum()
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

        public Node Parse(string s)
        {
            idx = 0;
            this.s = s;
            SkipSpaces();
            return ParseSum();
        }
    }
}
