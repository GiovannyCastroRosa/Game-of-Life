using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game_of_Life
{
    public partial class Options_Dialog : Form
    {
        public Options_Dialog()
        {
            InitializeComponent();
        }

        public int GetTimer()
        {
            return (int)numericUpDown1.Value;
        }
        public int GetNumberWidth()
        {
            return (int)numericUpDown2.Value;
        }

        public int GetNumberHeight()
        {
            return (int)numericUpDown3.Value;
        }

        public void SetTimer(int timer)
        {
            numericUpDown1.Value = timer;
        }
        public void SetNumberWidth(int width)
        {
            numericUpDown2.Value = width;
        }

        public void SetNumberHeight(int height)
        {
            numericUpDown3.Value = height;
        }
    }
}
