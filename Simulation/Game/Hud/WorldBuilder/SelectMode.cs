using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simulation.Game.Hud.WorldBuilder
{
    public partial class SelectMode : Form
    {
        public PlacementMode ResultMode
        {
            get;
            private set;
        }

        public SelectMode()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ResultMode = PlacementMode.BlockPlacement;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ResultMode = PlacementMode.AmbientObjectPlacement;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ResultMode = PlacementMode.AmbientHitableObjectPlacement;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            ResultMode = PlacementMode.LivingEntityPlacement;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
