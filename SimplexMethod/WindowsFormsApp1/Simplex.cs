using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    class Simplex
    {
        private List<string> koef_names = new List<string>();
        private List<float> koef_values = new List<float>();
        private List<string> basis_names = new List<string>();
        private List<float> basis_values = new List<float>();
        private List<float> estimates = new List<float>();
        private float z_value = 0;
        private float[,] l_matr;
        private bool min_max;
        private int art_count;
        private string report = "";
        private bool error = false;

        public Simplex(float[,] l_matr, List<string> koef_names, List<float> koef_values, List<string> basis_names, List<float> basis_values, string min_max, int art_count)
        {
            this.l_matr = l_matr;
            this.koef_names = koef_names;
            this.koef_values = koef_values;
            this.basis_names = basis_names;
            this.basis_values = basis_values;
            if (min_max == "min")
                this.min_max = false;
            else
                this.min_max = true;
            this.art_count = art_count;
        }

        public void Calculation()
        {
            if (art_count != 0)
            {
                if (R_simplex())
                {
                    full_reculc(ref l_matr, koef_names, koef_values);
                    report += "\r\nZ = ";

                    for (int i = 0; i < koef_values.Count; i++)
                    {
                        report += string.Format("{0} * {1} + ", koef_values[i], koef_names[i]);
                    }

                    report = report.Remove(report.Length - 2);
                    if (min_max)
                        report += "--> max\r\n\r\n";
                    else
                        report += "--> min\r\n\r\n";
                    report += CreateReport("Z", koef_values, estimates, ref z_value);
                    Average_simplex();
                }
            }
            else
            {
                func_reculc(koef_values, koef_names, estimates, ref z_value);
                report += CreateReport("Z", koef_values, estimates, ref z_value);
                Average_simplex();
            }

            if (!error)
            {
                report += "\r\nЗначения переменных:\r\n";
                report += "----------------------------------\r\n";
                for (int i = 0; i < basis_names.Count; i++)
                {
                    report += string.Format("{0} = {1:f2}\r\n", basis_names[i], basis_values[i]);
                }
                report += "\r\nЗначения целевой функции:\r\n";
                report += "-----------------------------------------\r\n";
                report += string.Format("Z = {0:f2}", z_value);
            }
        }

        private string CreateReport(string stage, List<float> koef_values, List<float> estimates, ref float z_value)
        {
            string report = "";
            report += "базис\t|";
            foreach (var item in koef_names)
                report += ("\t" + item);
            report += " \t|\tрешение\r\n";
            for(int i = 0; i < basis_names.Count(); i++)
            {
                report += (basis_names[i] + "\t|");
                for(int j = 0; j < koef_values.Count(); j++)
                {
                    report += string.Format("\t{0:f2}", l_matr[i, j]);
                }
                report += string.Format("\t|\t{0:f2}", basis_values[i]);
                report += "\r\n";
            }
            report += string.Format("{0}\t|", stage);
            foreach (var item in estimates)
                report += string.Format("\t{0:f2}", item);
            report += string.Format("\t|\t{0:f2}\r\n", z_value);
            report += "\r\n";

            return report;
        }

        public string GetReport()
        {
            return report;
        }

        private bool Break_condition()
        {
            foreach (float num in estimates)
            {
                if (num > 0 && !min_max)
                    return false;
                else if (num < 0 && min_max)
                    return false;
            }
            return true;
        }

        private void Average_simplex()
        {
            int lead_column;
            int lead_row = 0;
            float lead_value;
            int iterator;
            float min;
            float buf_value;
            bool check = Break_condition();

            while (!check)
            {
                iterator = 0;
                min = float.MaxValue;

                if (!min_max)
                    lead_column = estimates.IndexOf(estimates.Max());
                else
                    lead_column = estimates.IndexOf(estimates.Min());

                for (int i = 0; i < basis_values.Count; i++)
                {
                    if (l_matr[iterator, lead_column] > 0)
                    {
                        if (basis_values[i] / l_matr[iterator, lead_column] < min)
                        {
                            min = basis_values[i] / l_matr[iterator, lead_column];
                            lead_row = i;
                        }
                    }
                    iterator += 1;
                }

                if (min.CompareTo(float.MaxValue) == 0)
                {
                    MessageBox.Show("Обнаружено неограниченное решение!");
                    report += "Обнаружено неограниченное решение!";
                    error = true;
                    break;
                }

                lead_value = l_matr[lead_row, lead_column];

                basis_names[lead_row] = koef_names[lead_column];

                for (int i = 0; i < l_matr.GetLength(1); i++)
                    l_matr[lead_row, i] /= lead_value;
                basis_values[lead_row] /= lead_value;

                for (int i = 0; i < l_matr.GetLength(0); i++)
                {
                    if (i != lead_row)
                    {
                        buf_value = l_matr[i, lead_column];
                        for (int j = 0; j < l_matr.GetLength(1); j++)
                        {
                            l_matr[i, j] -= buf_value * l_matr[lead_row, j];
                        }
                        basis_values[i] -= buf_value * basis_values[lead_row];
                    }
                }

                func_reculc(koef_values, koef_names, estimates, ref z_value);
                report += CreateReport("Z", koef_values, estimates, ref z_value);

                check = Break_condition();
            }
        }

        private void full_reculc(ref float[,] l_matr, List<string> koef_names, List<float> koef_values)
        {
            int rows = l_matr.GetLength(0);
            int columns = l_matr.GetLength(1) - art_count;
            float[,] buf_matr = new float[rows, columns];
            List<int> art_index = new List<int>();

            int limit = koef_names.Count();

            for (int i = (limit - 1); i != 0; i--)
            {
                if (koef_names[i].Contains('R'))
                {
                    art_index.Add(i);
                    koef_names.RemoveAt(i);
                    koef_values.RemoveAt(i);
                }
            }
            art_index.Reverse();

            int i_column = 0;

            for (int i = 0; i < l_matr.GetLength(0); i++)
            {
                for (int j = 0; j < l_matr.GetLength(1); j++)
                {
                    if (!art_index.Contains(j))
                    {
                        buf_matr[i, i_column] = l_matr[i, j];
                        i_column++;
                    }
                }
                i_column = 0;
            }
            l_matr = buf_matr;

            func_reculc(koef_values, koef_names, estimates, ref z_value);
        }

        private void func_reculc(List<float> buf_koef_values, List<string> koef_names, List<float> buf_func, ref float buf_solution)
        {
            int i_column = 0;
            float r_value = 0;

            buf_func.Clear();

            for (int i = 0; i < buf_koef_values.Count(); i++)
            {
                for (int j = 0; j < koef_names.Count(); j++)
                {
                    if (basis_names.Contains(koef_names[j]))
                        r_value += buf_koef_values[j] * l_matr[basis_names.IndexOf(koef_names[j]), i_column];
                }
                i_column += 1;
                r_value -= buf_koef_values[i];
                buf_func.Add(r_value);
                r_value = 0;
            }

            buf_solution = 0;

            for (int i = 0; i < koef_names.Count(); i++)
            {
                if (basis_names.Contains(koef_names[i]))
                    buf_solution += buf_koef_values[i] * basis_values[basis_names.IndexOf(koef_names[i])];
            }
        }

        private bool find_R(List<string> names)
        {
            foreach (string item in names)
            {
                if (item.Contains("R"))
                    return false;
            }
            return true;
        }

        private bool R_simplex()
        {
            float R_solution = 0;
            List<float> R_func = new List<float>();
            List<float> R_koef_values = new List<float>(koef_values);

            report += "R = ";

            for (int i = 0; i < koef_names.Count; i++)
            {
                if (koef_names[i].Contains("x") || koef_names[i].Contains("S"))
                    R_koef_values[i] = 0;
                else
                {
                    R_koef_values[i] = 1;
                    report += string.Format("{0} + ", koef_names[i]);
                }
            }

            report = report.Remove(report.Length - 2);
            report += "--> min\r\n\r\n";

            func_reculc(R_koef_values, koef_names, R_func, ref R_solution);
            report += CreateReport("R", R_koef_values, R_func, ref R_solution);

            int lead_column;
            int lead_row = 0;
            float lead_value;
            int iterator;
            float min;
            float buf_value;
            bool check = find_R(basis_names);
            while (R_solution != 0.0 && !check)
            {
                if (R_func.Max() <= 0.0)
                {
                    MessageBox.Show("Решение отсутствует!");
                    report += "Решение отсутствует!";
                    error = true;
                    return false;
                }

                iterator = 0;
                min = float.MaxValue;
                lead_column = R_func.IndexOf(R_func.Max());

                for (int i = 0; i < basis_values.Count; i++)
                {
                    if (l_matr[iterator, lead_column] > 0)
                    {
                        if (basis_values[i] / l_matr[iterator, lead_column] < min)
                        {
                            min = basis_values[i] / l_matr[iterator, lead_column];
                            lead_row = i;
                        }
                    }
                    iterator += 1;
                }

                if (min.CompareTo(float.MaxValue) == 0)
                {
                    MessageBox.Show("Обнаружено неограниченное решение!");
                    report += "Обнаружено неограниченное решение!";
                    error = true;
                    return false;
                }

                lead_value = l_matr[lead_row, lead_column];

                basis_names[lead_row] = koef_names[lead_column];

                for (int i = 0; i < l_matr.GetLength(1); i++)
                    l_matr[lead_row, i] /= lead_value;
                basis_values[lead_row] /= lead_value;

                for (int i = 0; i < l_matr.GetLength(0); i++)
                {
                    if (i != lead_row)
                    {
                        buf_value = l_matr[i, lead_column];
                        for (int j = 0; j < l_matr.GetLength(1); j++)
                        {
                            l_matr[i, j] -= buf_value * l_matr[lead_row, j];
                        }
                        basis_values[i] -= buf_value * basis_values[lead_row];
                    }
                }

                func_reculc(R_koef_values, koef_names, R_func, ref R_solution);
                report += CreateReport("R", R_koef_values, R_func, ref R_solution);

                check = find_R(basis_names);
            }
            return true;
        }
    }
}
