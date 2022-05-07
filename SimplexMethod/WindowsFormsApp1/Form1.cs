using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        int param_count;
        int limit_count;
        int res_var;
        int art_var;
        float[,] lim_matr;
        List<string> signs;
        List<float> limit_values;
        Form_Rendering renderer;
        Simplex simplex;

        public Form1()
        {
            InitializeComponent();
            param_count = (int)numericUpDown1.Value;
            limit_count = (int)numericUpDown2.Value;
            Reload();
            renderer.Dr_tf(groupBox1, (int)numericUpDown1.Value);
            renderer.Dr_lmt(groupBox2, (int)numericUpDown2.Value, (int)numericUpDown1.Value);
        }

        public void Reload()
        {
            res_var = 0;
            art_var = 0;
            signs = new List<string>();
            limit_values = new List<float>();
            renderer = new Form_Rendering();
        }

        private void var_count()
        {
            foreach (var item in signs)
            {
                if (item == "<=")
                    res_var += 1;
                else if (item == "=")
                    art_var += 1;
                else if (item == ">=")
                {
                    res_var += 1;
                    art_var += 1;  
                }
            }
        }

        private void Standardization()
        {
            lim_matr = new float[limit_count, param_count];
            int i_row = 0;
            int i_column = 0;
            bool check = false;

            foreach (TextBox box in this.groupBox2.Controls.OfType<TextBox>())
            {
                if (box.Name.Contains("lmtB"))
                    limit_values.Add(float.Parse(box.Text));
            }

            foreach (ComboBox box in this.groupBox2.Controls.OfType<ComboBox>())
                signs.Add(box.Text);

            foreach (TextBox box in this.groupBox2.Controls.OfType<TextBox>())
            {
                if (!box.Name.Contains("lmtB"))
                {
                    if (limit_values[i_row] >= 0)
                    {
                        lim_matr[i_row, i_column] = float.Parse(box.Text);
                        i_column += 1;
                    }
                    else
                    {
                        lim_matr[i_row, i_column] = (-1) * float.Parse(box.Text);
                        i_column += 1;
                        check = true;
                    }
                }
                else
                {
                    if (check)
                    {
                        limit_values[i_row] *= (-1);
                        if (signs[i_row] == "<=")
                            signs[i_row] = ">=";
                        else if (signs[i_row] == ">=")
                            signs[i_row] = "<=";
                        check = false;
                    }
                    i_row += 1;
                    i_column = 0;
                }
            }
        }

        private void create_simplex_object()
        {
            Standardization();
            var_count();
            int rows = limit_count;
            int columns = param_count + res_var + art_var;
            float[,] matr = new float[rows, columns];
            string min_max = "";
            
            foreach(ComboBox box in this.groupBox1.Controls.OfType<ComboBox>())
            {
                min_max = box.Text;
            }

            for(int i = 0; i < rows; i++)
            {
                for(int j = 0; j < columns; j++)
                {
                    matr[i, j] = 0;
                }
            }

            for (int i = 0; i < lim_matr.GetLength(0); i++)
            {
                for (int j = 0; j < lim_matr.GetLength(1); j++)
                {
                    matr[i, j] = lim_matr[i, j];
                }
            }

            int i_row = 0;
            int i_column = 0;
            List<float> koef_values = new List<float>();
            foreach (Control item in this.groupBox1.Controls.OfType<TextBox>())
                koef_values.Add(float.Parse(item.Text));

            List<string> koef_names = new List<string>();
            for(int i = 0; i < koef_values.Count; i++)
            {
                koef_names.Add("x" + (i + 1));
            }

            i_row = 0;
            i_column = param_count;
            int counter_S = 0;
            int counter_R = 0;
            List<string> basis_names = new List<string>();
            List<float> basis_values = new List<float>();
            foreach (var item in signs)
            {
                if(item == "<=")
                {
                    counter_S += 1;
                    koef_names.Add("S" + counter_S);
                    koef_values.Add(0);
                    basis_names.Add("S" + counter_S);
                    basis_values.Add(0);
                    matr[i_row, i_column] = 1;
                }
                else if(item == "=")
                {
                    counter_R += 1;
                    koef_names.Add("R" + counter_R);
                    koef_values.Add(1);
                    basis_names.Add("R" + counter_R);
                    basis_values.Add(0);
                    matr[i_row, i_column] = 1;
                }
                else if(item == ">=")
                {
                    counter_S += 1;
                    koef_names.Add("S" + counter_S);
                    koef_values.Add(0);
                    matr[i_row, i_column] = -1;
                    i_column += 1;
                    counter_R += 1;
                    koef_names.Add("R" + counter_R);
                    koef_values.Add(1);
                    basis_names.Add("R" + counter_R);
                    basis_values.Add(0);
                    matr[i_row, i_column] = 1;
                }
                i_row += 1;
                i_column += 1;
            }

            for(int i = 0; i < limit_values.Count(); i++)
                basis_values[i] = limit_values[i];

            simplex = new Simplex(matr, koef_names, koef_values, basis_names, basis_values, min_max, art_var);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            renderer.Del_tf(this.groupBox1);
            renderer.Dr_tf(groupBox1, (int)numericUpDown1.Value);
            renderer.Del_lmt(this.groupBox2);
            renderer.Dr_lmt(groupBox2, (int)numericUpDown2.Value, (int)numericUpDown1.Value);
            param_count = (int)numericUpDown1.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            renderer.Del_lmt(this.groupBox2);
            renderer.Dr_lmt(groupBox2, (int)numericUpDown2.Value, (int)numericUpDown1.Value);
            limit_count = (int)numericUpDown2.Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                create_simplex_object();
                simplex.Calculation();
                textBox1.Text = simplex.GetReport();
                Reload();
            }
            catch(ArgumentOutOfRangeException)
            {
                MessageBox.Show("Заполните пустые поля!");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
