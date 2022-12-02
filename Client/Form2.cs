using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        public void startTimer()
        {
            timer1.Interval = 1000;
            timer1.Start();
            timer1.Tick += new EventHandler(this.timer1_Tick!);
        }
        
        public void timer1_Tick(object Sender, EventArgs e)
        {
            doStep();
        }


        public void doStep()
        {
            progressBar1.PerformStep();

            if (progressBar1.Value == progressBar1.Maximum)
            {
                Close();
            }
        }

    }
}
