using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Xrns2XModUI
{
    public partial class DonationBox : UserControl
    {
        private ToolTip toolTip;

        public DonationBox()
        {
            InitializeComponent();
            toolTip = new ToolTip();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=zenon66%40gmail%2ecom&lc=GB&item_name=Zenon&currency_code=EUR&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted");
        }

        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
            toolTip.SetToolTip(pictureBox2, "Make a very little donation if you feel this tool used to be useful for you\n" +
            "in any way, indeed would make me really happy :). \n" +
            "Maybe I'll look forward to work for some other formats to convert");
        }

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }
    }
}
