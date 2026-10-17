using System;
using System.Runtime.InteropServices;

namespace zbarcs
{
    /// <summary>
    /// ZBar 全局接口。
    /// </summary>
    public static class ZBar
    {
        /// <summary>
        /// 获取运行时库版本信息。
        /// </summary>
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
        /// 设置全局库调试级别。
        /// </summary>
        /// <param name="verbosity">调试级别，数值越大输出越多。</param>
        public static void SetVerbosity(int verbosity)
        {
            NativeZBar.zbar_set_verbosity(verbosity);
        }

        /// <summary>
        /// 提高全局库调试级别，例如对应 -vvvv 参数。
        /// </summary>
        public static void IncVerbosity()
        {
            NativeZBar.zbar_increase_verbosity();
        }

        /// <summary>
        /// 获取附加码（addon）编码的名称。
        /// </summary>
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
        /// 获取符号编码的名称。
        /// </summary>
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
        /// 获取配置项的名称。
        /// </summary>
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
        /// 获取修饰符的名称。
        /// </summary>
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
        /// 获取方向的名称。
        /// </summary>
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
        /// 解析形如 "[symbology.]config[=value]" 的配置字符串。
        /// 配置名必须能被识别；未指定 symbology 时返回 0，未指定 value 时按 1 处理。
        /// </summary>
        /// <param name="config_string">配置字符串。</param>
        /// <param name="symbology">解析出的符号类型。</param>
        /// <param name="config">解析出的配置项。</param>
        /// <param name="value">解析出的配置值。</param>
        /// <returns>解析成功返回 true。</returns>
        public static bool ParseConfig(string config_string, out SymbolType symbology, out Config config, out int value)
        {
            if (config_string == null)
            {
                throw new ArgumentNullException("config_string");
            }
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
