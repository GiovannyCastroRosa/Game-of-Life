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
    public partial class ModalDialog : Form
    {
        public ModalDialog()
        {
            InitializeComponent();
        }
        
        public int GetNumber()
        {
            return (int)numericUpDown1.Value;
        }

        public void SetNumber(int number)
        {
            numericUpDown1.Value = number;
        }

        private void buttonRandomize_Click(object sender, EventArgs e)
        {
            Random rand = new Random();
            int seed = rand.Next(-10000000,10000000);
            SetNumber(seed);
            
        }
    }
}
