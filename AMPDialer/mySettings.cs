using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using MOBZystems;

namespace AMPDialer
{
    public partial class mySettings : Form
    {
        public mySettings()
        {
            InitializeComponent();
            GetData();
            string REGROOT = "SOFTWARE\\AMPDialer\\";
            RegistryKey root = Registry.CurrentUser.CreateSubKey(REGROOT);
            string hotkeyString = (string)root.GetValue("Hotkey", "C");
            bool shiftKey = BoolFromString((string)root.GetValue("Shift", "1"));
            bool controlKey = BoolFromString((string)root.GetValue("Control", "1"));
            bool altKey = BoolFromString((string)root.GetValue("Alt", "0"));
            bool windowsKey = BoolFromString((string)root.GetValue("Windows", "0"));
            MOBZystems.Hotkey userHotkey = null;
            // Create a hot key with the settings:
            userHotkey = new MOBZystems.Hotkey(MOBZystems.Hotkey.KeyCodeFromString(hotkeyString), shiftKey, controlKey, altKey, windowsKey);
            // Catch the 'Pressed' event
            userHotkey.Pressed += new HandledEventHandler(UserHotkey_Pressed);
            userHotkey.Register(this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\AMPDialer", "API_KEY", API_KEY_TXT.Text);
            Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\AMPDialer", "DOMAIN_TXT", DOMAIN_TXT.Text);
            Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\AMPDialer", "SRC_TXT", SRC_TXT.Text);
            if(autoAnswer.Checked)
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\AMPDialer", "AUTO_ANSWER", "true");
            else
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\AMPDialer", "AUTO_ANSWER", "false");

            RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (startOnLogon.Checked)
                regKey.SetValue("AMPDialer", Application.ExecutablePath.ToString());
            else
                regKey.DeleteValue("AMPDialer", false);
            this.Close();
        }

        private void GetData()
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            string startRegKey = regKey.GetValue("AMPDialer","null").ToString();
            if (startRegKey != null)
            {
                startOnLogon.Checked = true;
            }
            regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\AMPDialer", true);
            string autoAnswerRegKey = regKey.GetValue("AUTO_ANSWER", "null").ToString();
            if (autoAnswerRegKey != null)
            {
                autoAnswer.Checked = true;
            }
            string API_KEY = null;
            string DOMAIN_URL = null;
            string SRC = null;
            try { API_KEY = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\AMPDialer", "API_KEY", null).ToString(); }
            catch { API_KEY = null; }
            try { DOMAIN_URL = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\AMPDialer", "DOMAIN_TXT", null).ToString(); }
            catch { DOMAIN_URL = null; }
            try { SRC = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\AMPDialer", "SRC_TXT", null).ToString(); }
            catch { SRC = null; }
            if (DOMAIN_URL != null)
            {
                this.DOMAIN_TXT.Text = DOMAIN_URL.ToString();
            }
            if (API_KEY != null)
            {
                this.API_KEY_TXT.Text = API_KEY.ToString();
            }
            if (SRC != null)
            {
                this.SRC_TXT.Text = SRC.ToString();
            }
        }
        /// <summary>
        /// The hotkey was pressed! Handle it
        /// </summary>
        private void UserHotkey_Pressed(object sender, HandledEventArgs e)
        {
            PlaceCall PC = new PlaceCall();
            PC.f_placeCall(ProcessIcon.NotifyIcon);
        }
        private bool BoolFromString(string s)
        {
            if (s == null || s != "1")
                return false;
            else
                return true;
        }

    }
}

