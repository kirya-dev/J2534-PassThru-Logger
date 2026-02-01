using Microsoft.Win32;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PassThruLoggerControl
{
    class RegistryHelper
    {
        public static string GetString(RegistryKey key, string name)
        {
            var value = key.GetValue(name);

            return (value == null || key.GetValueKind(name) != RegistryValueKind.String)
                ? null
                : (string)value;
        }

        public static void ScanDrivers(List<J2534Driver> drivers)
        {
            var reg32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            var passthruList = reg32.OpenSubKey(@"Software\PassThruSupport.04.04\\", false);

            string loggerPath = System.IO.Directory.GetCurrentDirectory() + "\\PassThruLogger.dll";
            string foundLoggerPath = null;
            string cofigAppPath = System.IO.Directory.GetCurrentDirectory() + "\\PassThruLoggerControl.exe";
            string msgTitle = "PassThruLogger.dll registration helper";

            foreach (var v in passthruList.GetSubKeyNames())
            {
                var driver = new J2534Driver(v, passthruList.OpenSubKey(v));
                if (driver.IsLogger())
                {
                    foundLoggerPath = driver.path;
                    continue;
                }
                drivers.Add(driver);
            }

            if (!System.IO.File.Exists(loggerPath))
            {
                MessageBox.Show("Not found PassThruLogger.dll in current directory.", msgTitle);
                return;
            }

            if (loggerPath == foundLoggerPath)
            {
                return;
            }

            MessageBox.Show("PassThruLogger is incorrerly registred as PassThru interface in Windows Registry!", msgTitle);

            try
            {
                var key = reg32.CreateSubKey(@"Software\PassThruSupport.04.04\PassThruLogger\");
                key.SetValue("Vendor",            "GitHub",         RegistryValueKind.String);
                key.SetValue("Name",              "PassThruLogger", RegistryValueKind.String);
                key.SetValue("FunctionLibrary",   loggerPath,       RegistryValueKind.String);
                key.SetValue("ConfigApplication", cofigAppPath,     RegistryValueKind.String);
                foreach (string param in new string[] {"DeviceSerial"})
                {
                    key.SetValue(param, 0, RegistryValueKind.DWord);
                }
                foreach (string param in new string[]{"SCI_A_ENGINE", "SCI_A_TRANS", "SCI_B_ENGINE", "SCI_B_TRANS", "J2610_PS", "ISO9141", "ISO9141_PS", "ISO14230", "ISO14230_PS", "GM_UART_PS", "J1850VPW", "J1850VPW_PS", "J1850PWM", "J1850PWM_PS", "CAN", "CAN_PS", "ISO15765", "ISO15765_PS", "SW_CAN_PS", "SW_ISO15765_PS", "HONDA_DIAG_PS"})
                {
                    key.SetValue(param, 1, RegistryValueKind.DWord);
                }
                MessageBox.Show("PassThruLogger successfully registred!", msgTitle);
            }
            catch (System.UnauthorizedAccessException)
            {
                MessageBox.Show("Run as admin for registration", msgTitle);
            }
        }
    }
}