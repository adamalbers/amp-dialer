using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Net;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Automation;
using System.Text.RegularExpressions;

namespace AMPDialer
{
    class PlaceCall
    {
        public void f_placeCall(NotifyIcon ni)
        {
            string DEST = getDID();
            string PRETTY_PHONE_NUMBER = null;
            string API_KEY = null;
            string DOMAIN_URL = null;
            string SRC = null;
            string AUTO_ANSWER = null;

            try { API_KEY = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\AMPDialer", "API_KEY", null).ToString(); }
            catch { API_KEY = null; }
            try { DOMAIN_URL = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\AMPDialer", "DOMAIN_TXT", null).ToString(); }
            catch { DOMAIN_URL = null; }
            try { SRC = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\AMPDialer", "SRC_TXT", null).ToString(); }
            catch { SRC = null; }
            try { AUTO_ANSWER = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\AMPDialer", "AUTO_ANSWER", "false").ToString(); }
            catch { AUTO_ANSWER = "false"; }

            if (DOMAIN_URL == null || API_KEY == null || SRC == null)
            {
                ni.ShowBalloonTip(5000, "ERROR!", "Please right-click the tray icon and configure your settings first.", ToolTipIcon.Info);
            }
            else if (DEST == "")
            {
                ni.ShowBalloonTip(5000, "ERROR!", "Could not find a phone number to call.", ToolTipIcon.Info);
            }
            else if (DEST.Length <10 || DEST.Length >11)
            {
                ni.ShowBalloonTip(5000, "ERROR!", "Please select a 10 or 11 digit number to dial.", ToolTipIcon.Info);
            }
            else
            {
                if (DEST.Length == 10)
                {
                    PRETTY_PHONE_NUMBER = Regex.Replace(DEST, @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3");
                }
                if (DEST.Length == 11)
                {
                    PRETTY_PHONE_NUMBER = Regex.Replace(DEST, @"(\d{1})(\d{3})(\d{3})(\d{4})", "$1 ($2) $3-$4");
                }
                string URL = "https://" + DOMAIN_URL + "/app/click_to_call/click_to_call.php?domain=" + DOMAIN_URL + "&key=" + API_KEY + "&src=" + SRC + "&dest=" + DEST + "&auto_answer=" + AUTO_ANSWER + "&src_cid_name=" + DEST + "&src_cid_number=" + DEST + "";
                ni.ShowBalloonTip(5000, "Calling...", PRETTY_PHONE_NUMBER, ToolTipIcon.Info);
                var cli = new WebClient();
                string data = cli.DownloadString(URL);
            }
        }

        private string getDID()
        {
            string selectedText = null;
            var element = AutomationElement.FocusedElement;
            if (element != null)
            {
                object pattern;
                if(element.TryGetCurrentPattern(TextPattern.Pattern, out pattern))
                {
                    var tp = (TextPattern) pattern;
                    var sb = new StringBuilder();
                    foreach (var r in tp.GetSelection())
                    {
                        sb.AppendLine(r.GetText(-1));
                    }
                    selectedText = sb.ToString();
                }
            }
            if (selectedText != null)
            {
                //System.Windows.MessageBox.Show("Automation:"+selectedText);
                return new string(selectedText.Where(char.IsDigit).ToArray());  
            }

            //Using CTRL C with clipboard, doesn't always work.
            //Sleep so that the user releases the hotkey before calling the copy function.
            Thread.Sleep(1000);
            Keyboard.SimulateKeyStroke('c', ctrl:true,alt:false,shift:false);
            //SendKeys.Send("^c");
            Thread.Sleep(100);
            if (System.Windows.Clipboard.ContainsText())
            {
                selectedText = System.Windows.Clipboard.GetText();
                if (selectedText != null)
                {
                    //System.Windows.MessageBox.Show("Copy:" + selectedText);
                    return new string(selectedText.Where(char.IsDigit).ToArray());
                }
            }

            return null;
        }



    
    }
}
