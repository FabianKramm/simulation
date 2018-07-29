using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simulation.Util.Dialog
{
    public partial class InputDialog : Form
    {
        public string ResultText
        {
            get;
            private set;
        }

        public InputDialog(string title, string textboxText)
        {
            this.InitializeComponent();
            this.Text = title;
            this.richTextBox1.Text = textboxText;
        }

        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            ResultText = richTextBox1.Text.Trim();
        }

        private void tblLayout_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
