using Microsoft.Win32;

namespace PassThruLoggerControl
{
    class J2534Driver
    {
        private string name;
        private string _path;
        private bool valid;
        private string _key;

        public string key { get { return _key; } }
        public string path { get { return _path; } }

        public J2534Driver(string keyname, RegistryKey driverentry)
        {
            name = keyname;
            _key = keyname;

            _path       = RegistryHelper.GetString(driverentry, "FunctionLibrary");
            valid       = _path != null;
            var vendor  = RegistryHelper.GetString(driverentry, "Vendor");
            var product = RegistryHelper.GetString(driverentry, "Name");

            if (vendor != null && product != null)
            {
                name = product + " {" + vendor + "}";
            }
        }

        public bool IsLogger()
        {
            return path != null && path.EndsWith("PassThruLogger.dll");
        }

        public override string ToString()
        {
            return name;
        }
    }
}
