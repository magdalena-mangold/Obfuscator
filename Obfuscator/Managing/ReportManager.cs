using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Obfuscator.Managing
{
    class ReportManager
    {
        private RichTextBox report;
        public RichTextBox Report
        {
            get { return report; }
            set { report = value; }
        }

        public ReportManager(RichTextBox reportWindow)
        {
            Report = reportWindow;
        }

        public void AddLine(string line)
        {
            Report.AppendText(line + "\r");
        }
    }
}
