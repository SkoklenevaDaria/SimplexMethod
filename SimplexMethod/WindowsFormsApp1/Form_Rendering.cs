using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace WindowsFormsApp1
{
    class Form_Rendering
    {
        private List<string> min_max = new List<string>() { "min", "max" };
        private List<string> eq = new List<string>() { "<=", "=", ">=" };

        public void Dr_tf(GroupBox groupBox, int param_count)
        {
            int h = 0;
            for(int i = 0; i < param_count; i++)
            {
                groupBox.Controls.Add(createLabel(18 + h, 60, "tf" + i, "x" + (i + 1)));
                groupBox.Controls.Add(createTextBox(12 + h, 76, "tf" + i));
                h += 39;
            }
            h -= 10;
            groupBox.Controls.Add(createLabel(18 + h, 76, "tf->", @"-->"));
            h += 19;
            groupBox.Controls.Add(createComboBox(18 + h, 76, "tf_cb", min_max));
        }

        public void Del_tf(GroupBox groupBox)
        {
            for(int i = 0; i < groupBox.Controls.Count; i++)
            {
                if (groupBox.Controls[i].Name.Contains("tf"))
                {
                    groupBox.Controls.Remove(groupBox.Controls[i]);
                    i--;
                }
            }
        }

        public void Dr_lmt(GroupBox groupBox, int limit_count, int param_count)
        {
            int h_hor = 0;
            int h_vert = 26;
            for(int i = 0; i < param_count; i++)
            {
                groupBox.Controls.Add(createLabel(18 + h_hor, 65, "lmt" + i, "x" + (i + 1)));
                h_hor += 39;
            }
            h_hor += 66;
            groupBox.Controls.Add(createLabel(18 + h_hor, 65, "lmtB", "B"));
            h_hor = 0;

            for(int i = 0; i < limit_count; i++)
            {
                for(int j = 0; j < param_count; j++)
                {
                    groupBox.Controls.Add(createTextBox(9 + h_hor, 65 + h_vert, "lmt" + j));
                    h_hor += 39;
                }
                groupBox.Controls.Add(createComboBox(9 + h_hor, 65 + h_vert, "lmt" + i, eq));
                h_hor += 65;
                groupBox.Controls.Add(createTextBox(9 + h_hor, 65 + h_vert, "lmtB" + "_" + i));
                h_vert += 26;
                h_hor = 0;
            }
        }

        public void Del_lmt(GroupBox groupBox)
        {
            for(int i = 0; i < groupBox.Controls.Count; i++)
            {
                if (groupBox.Controls[i].Name.Contains("lmt"))
                {
                    groupBox.Controls.Remove(groupBox.Controls[i]);
                    i--;
                }
            }
        }

        private TextBox createTextBox(int X, int Y, string pr)
        {
            TextBox tb = new TextBox();
            tb.Name = "textbox_" + pr;
            tb.Width = 33;
            tb.Height = 20;
            tb.Location = new Point(X, Y);

            return tb;
        }

        private Label createLabel(int X, int Y, string pr, string text)
        {
            Label label = new Label();
            label.Name = "label_" + pr;
            label.Text = text;
            label.Width = 19;
            label.Height = 13;
            label.Location = new Point(X, Y);

            return label;
        }

        private ComboBox createComboBox(int X, int Y, string pr, List<string> list)
        {
            ComboBox box = new ComboBox();
            box.Name = "combobox_" + pr;
            box.Width = 60;
            box.Height = 21;
            box.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (string item in list)
                box.Items.Add(item);
            box.Location = new Point(X, Y);

            return box;
        }
    }
}
