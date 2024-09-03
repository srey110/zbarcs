using System;
using System.Runtime.InteropServices;

namespace zbarcs
{
    public static class ZBar
    {
        /// <value>
        /// retrieve runtime library version information.
        /// </value>
        public static Version Version
        {
            get
            {
                uint major = 0;
                uint minor = 0;
                uint patch = 0;
                unsafe
                {
                    if (0 != NativeZBar.zbar_version(&major, &minor, &patch))
                    {
                        throw new Exception("Failed to get ZBar version.");
                    }
                }
                return new Version((int)major, (int)minor, (int)patch);
            }
        }
        /// <summary>
        /// set global library debug level.
        /// </summary>
        /// <param name="verbosity"></param>
        public static void SetVerbosity(int verbosity)
        {
            NativeZBar.zbar_set_verbosity(verbosity);
        }
        /// <summary>
        /// increase global library debug level
        /// </summary>
        public static void IncVerbosity()
        {
            NativeZBar.zbar_increase_verbosity();
        }
        /// <summary>
        /// retrieve string name for addon encoding.
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public static string AddonName(SymbolType sym)
        {
            IntPtr val = NativeZBar.zbar_get_addon_name((int)sym);
            if (IntPtr.Zero == val)
            {
                throw new Exception("Failed to get ZBar addon name.");
            }
            return Marshal.PtrToStringAnsi(val);
        }
        /// <summary>
        /// retrieve string name for symbol encoding.
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public static string Name(SymbolType sym)
        {
            IntPtr val = NativeZBar.zbar_get_symbol_name((int)sym);
            if (IntPtr.Zero == val)
            {
                throw new Exception("Failed to get ZBar symbol name.");
            }
            return Marshal.PtrToStringAnsi(val);
        }
        /// <summary>
        /// retrieve string name for configuration setting.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string Name(Config config)
        {
            IntPtr val = NativeZBar.zbar_get_config_name((int)config);
            if (IntPtr.Zero == val)
            {
                throw new Exception("Failed to get ZBar config name.");
            }
            return Marshal.PtrToStringAnsi(val);
        }
        /// <summary>
        /// retrieve string name for modifier.
        /// </summary>
        /// <param name="modifier"></param>
        /// <returns></returns>
        public static string Name(Modifier modifier)
        {
            IntPtr val = NativeZBar.zbar_get_modifier_name((int)modifier);
            if (IntPtr.Zero == val)
            {
                throw new Exception("Failed to get ZBar modifier name.");
            }
            return Marshal.PtrToStringAnsi(val);
        }
        /// <summary>
        /// retrieve string name for orientation.
        /// </summary>
        /// <param name="orientation"></param>
        /// <returns></returns>
        public static string Name(Orientation orientation)
        {
            IntPtr val = NativeZBar.zbar_get_orientation_name((int)orientation);
            if (IntPtr.Zero == val)
            {
                throw new Exception("Failed to get ZBar orientation name.");
            }
            return Marshal.PtrToStringAnsi(val);
        }
        /// <summary>
        /// parse a configuration string of the form "[symbology.]config[=value]".
        /// the config must match one of the recognized names.
        /// </summary>
        /// <param name="config_string"></param>
        /// <param name="symbology"></param>
        /// <param name="config"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ParseConfig(string config_string, out SymbolType symbology, out Config config, out int value)
        {
            unsafe
            {
                int sym = 0;
                int cfg = 0;
                int val = 0;
                if (0 != NativeZBar.zbar_parse_config(config_string, &sym, &cfg, &val))
                {
                    symbology = SymbolType.None;
                    config = Config.Enable;
                    value = 0;
                    return false;
                }
                symbology = (SymbolType)sym;
                config = (Config)cfg;
                value = val;
            }
            return true;            
        }
    }
}