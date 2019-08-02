using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyStartup
{
    public partial class ModifyIntervalForm : Form
    {
        public Action<double> ModifyIntervalAction = null;

        public ModifyIntervalForm()
        {
            InitializeComponent();
        }

        private void ModifyIntervalForm_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            double interval = Convert.ToDouble(this.textBox.Text);
            ModifyIntervalAction?.Invoke(interval);
            this.Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
