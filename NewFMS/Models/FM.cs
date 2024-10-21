using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

namespace NewFMS.Models
{
    public class FM
    {
        private bool fehler = false;
        private Dictionary<string, double> vars = new Dictionary<string, double>();
        private const string allowed = "abcdefgxyz";
        private string zc;
        private int pos;
        private int klammer;
        private int ebene;
        private double res;

        public FM.Variable[] Variablen
        {
            get
            {
                FM.Variable[] variableArray = new FM.Variable[this.vars.Count];
                int num = 0;
                foreach (KeyValuePair<string, double> keyValuePair in this.vars)
                    variableArray[num++] = new FM.Variable(keyValuePair.Key, keyValuePair.Value);
                return variableArray;
            }
        }

        private bool TryVariable(string Op, out double wert)
        {
            if (this.vars.ContainsKey(Op))
            {
                wert = this.vars[Op];
                return true;
            }
            else
            {
                wert = 0.0;
                return false;
            }
        }

        private int GetKlammer(string Op, int start)
        {
            int num = start;
            for (int startIndex = start; startIndex < Op.Length; ++startIndex)
            {
                switch (Op.Substring(startIndex, 1))
                {
                    case "(":
                        ++this.klammer;
                        break;
                    case ")":
                        --this.klammer;
                        break;
                }
                if (this.klammer == 0)
                {
                    num = startIndex;
                    break;
                }
            }
            return num;
        }

        private double Faculty(double number)
        {
            if (double.IsInfinity(number) || double.IsNaN(number) || number < 0.0 || number % 1.0 != 0.0)
                return double.NaN;
            double num = 1.0;
            for (int index = 0; (double)index < number; ++index)
                num *= (double)(index + 1);
            return num;
        }

        private double Zuteilung(string Op)
        {
            if (Op.Length != 0)
            {
                while (Op.StartsWith("(") && this.GetKlammer(Op, 0) == Op.Length - 1)
                    Op = Op.Substring(1, Op.Length - 2);
                if (double.TryParse(Op, NumberStyles.Float, (IFormatProvider)null, out this.res))
                    return this.res;
                if (Op.Length == 2 && Op == "PI")
                    return Math.PI;
                char[] chArray = new char[2]
        {
          ' ',
          '|'
        };
                if (this.vars.ContainsKey(Op.Trim().Trim("|".ToCharArray())) && this.TryVariable(Op.Trim().Trim("|".ToCharArray()), out this.res))
                    return this.res;
                if (Op.Length > 4 && Op.Substring(3, 1) == "(")
                {
                    this.pos = this.GetKlammer(Op, 3);
                    if (this.pos == Op.Length - 1)
                    {
                        this.zc = Op.Substring(4, this.pos - 4);
                        switch (Op.Substring(0, 3))
                        {
                            case "sqr":
                                return Math.Sqrt(this.Zuteilung(this.zc));
                            case "sin":
                                return Math.Sin(Math.PI * this.Zuteilung(this.zc) / 180.0);
                            case "cos":
                                return Math.Cos(Math.PI * this.Zuteilung(this.zc) / 180.0);
                            case "tan":
                                return Math.Tan(Math.PI * this.Zuteilung(this.zc) / 180.0);
                            case "log":
                                return Math.Log10(this.Zuteilung(this.zc));
                            case "abs":
                                return Math.Abs(this.Zuteilung(this.zc));
                            case "fac":
                                return this.Faculty(this.Zuteilung(this.zc));
                        }
                    }
                }
                this.pos = 0;
                this.ebene = 6;
                this.klammer = 0;
                for (int startIndex = Op.Length - 1; startIndex > -1; --startIndex)
                {
                    switch (Op.Substring(startIndex, 1))
                    {
                        case "(":
                            ++this.klammer;
                            break;
                        case ")":
                            --this.klammer;
                            break;
                        case "+":
                            if (this.klammer == 0 && this.ebene > 0)
                            {
                                this.pos = startIndex;
                                this.ebene = 0;
                                break;
                            }
                            else
                                break;
                        case "-":
                            if (this.klammer == 0 && this.ebene > 1)
                            {
                                this.pos = startIndex;
                                this.ebene = 1;
                                break;
                            }
                            else
                                break;
                        case "*":
                            if (this.klammer == 0 && this.ebene > 2)
                            {
                                this.pos = startIndex;
                                this.ebene = 2;
                                break;
                            }
                            else
                                break;
                        case "%":
                            if (this.klammer == 0 && this.ebene > 3)
                            {
                                this.pos = startIndex;
                                this.ebene = 3;
                                break;
                            }
                            else
                                break;
                        case "/":
                            if (this.klammer == 0 && this.ebene > 4)
                            {
                                this.pos = startIndex;
                                this.ebene = 4;
                                break;
                            }
                            else
                                break;
                        case "^":
                            if (this.klammer == 0 && this.ebene > 5)
                            {
                                this.pos = startIndex;
                                this.ebene = 5;
                                break;
                            }
                            else
                                break;
                    }
                }
                if (this.pos != 0 && this.pos != Op.Length - 1)
                {
                    this.zc = Op.Substring(this.pos, 1);
                    string Op1 = Op.Substring(0, this.pos);
                    string Op2 = Op.Substring(this.pos + 1, Op.Length - (this.pos + 1));
                    switch (this.zc)
                    {
                        case "+":
                            return this.Zuteilung(Op1) + this.Zuteilung(Op2);
                        case "-":
                            return this.Zuteilung(Op1) - this.Zuteilung(Op2);
                        case "*":
                            return this.Zuteilung(Op1) * this.Zuteilung(Op2);
                        case "/":
                            return this.Zuteilung(Op1) / this.Zuteilung(Op2);
                        case "%":
                            return this.Zuteilung(Op1) / 100.0 * this.Zuteilung(Op2);
                        case "^":
                            return Math.Pow(this.Zuteilung(Op1), this.Zuteilung(Op2));
                    }
                }
            }
            this.fehler = true;
            return 0.0;
        }

        public bool Neu(string formel, out double result)
        {
            if (formel == "")
            {
                result = 0.0;
                return false;
            }
            else
            {
                this.fehler = false;
                result = this.Zuteilung(formel);
                return !this.fehler;
            }
        }

        public bool NeuV(FM.Variable var)
        {
            if (this.vars.ContainsKey(var.name))
                this.vars[var.name] = var.wert;
            else
                this.vars.Add(var.name, var.wert);
            return true;
        }

        public bool LschenV()
        {
            this.vars = new Dictionary<string, double>();
            return true;
        }

        public bool LschenV(FM.Variable var)
        {
            if (var.name == "")
                return false;
            else
                return this.vars.Remove(var.name);
        }

        public void WertändernUS(double wert)
        {
            using (Dictionary<string, double>.Enumerator enumerator = this.vars.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    return;
                KeyValuePair<string, double> current = enumerator.Current;
                string key = current.Key;
                this.vars.Remove(current.Key);
                this.vars.Add(key, wert);
            }
        }

        public bool Wertndern(FM.Variable var)
        {
            if (!this.vars.ContainsKey(var.name))
                return false;
            this.vars[var.name] = var.wert;
            return true;
        }

        public struct Variable
        {
            public string name;
            public double wert;

            public Variable(string name, double wert)
            {
                this.name = name;
                this.wert = wert;
            }
        }
    }
}