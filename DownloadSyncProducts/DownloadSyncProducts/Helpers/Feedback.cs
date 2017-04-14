using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DownloadSyncProducts.Helpers
{
    public static class Feedback
    {
        public static void WriteLine(string msg, TextBox txtBox) {
            txtBox.Text += string.Format("{0}-------------------------------{1}{2}", Environment.NewLine, Environment.NewLine, msg);
            txtBox.Refresh();
        }
    }
}
