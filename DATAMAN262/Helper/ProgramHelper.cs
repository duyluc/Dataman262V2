using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATAMAN262.Helper
{
    public class ProgramHelper
    {
        static public void LogErr(String log, Exception t)
        {
            System.Windows.Forms.MessageBox.Show($"Log: {log} -- Source: {t.Source.ToString()} -- Error: {t.Message}" );
        }

        static public void LogErr(String log)
        {
            System.Windows.Forms.MessageBox.Show($"Error: {log}");
        }

        static public string ArrangeIp(List<System.Windows.Forms.TextBox> group)
        {
            string ip = "";
            for(int i = 0; i < 4; i++)
            {
                if (group[i].Text.Length > 3) return "";
                if (string.IsNullOrEmpty(group[i].Text)) return "";
                ip += group[i].Text;
                if (i < 3)
                {
                    ip += ".";
                }
            }
            return ip;
        }
    }

}
