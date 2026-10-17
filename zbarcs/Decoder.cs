using System;

namespace zbarcs
{
    /// <summary>
    /// 低层条宽流解码器，用于识别符号并提取编码数据。
    /// </summary>
    public class Decoder : IDisposable
    {
        private IntPtr handle = IntPtr.Zero;

        /// <summary>
        /// 创建新的解码器。
        /// </summary>
        public Decoder()
        {
            this.handle = NativeZBar.zbar_decoder_create();
            if (this.handle == IntPtr.Zero)
            {
                throw new OutOfMemoryException("Failed to create a ZBar decoder.");
            }
        }

        internal IntPtr Handle
        {
            get
            {
                ThrowIfDisposed();
                return this.handle;
            }
        }

        /// <summary>
        /// 处理输入流中的下一个条/空宽度。
        /// </summary>
        /// <remarks>
        /// 一次扫描的第一个值是空宽度，之后条和空交替出现。
        /// </remarks>
        public SymbolType DecodeWidth(uint width)
        {
            ThrowIfDisposed();
            return (SymbolType)NativeZBar.zbar_decode_width(this.handle, width);
        }

        /// <summary>
        /// 清除解码器全部状态，并冲刷尚未完成的符号。
        /// </summary>
        public void Reset()
        {
            ThrowIfDisposed();
            NativeZBar.zbar_decoder_reset(this.handle);
        }

        /// <summary>
        /// 标记新一轮扫描开始。
        /// </summary>
        /// <remarks>
        /// 会清除符号内部状态并把颜色重置为空色；已经部分解码的符号状态保留。
        /// </remarks>
        public void NewScan()
        {
            ThrowIfDisposed();
            NativeZBar.zbar_decoder_new_scan(this.handle);
        }

        /// <summary>
        /// 为指定符号类型（传 0 表示全部）设置配置值。
        /// </summary>
        public void SetConfiguration(SymbolType symbology, Config config, int value)
        {
            ThrowIfDisposed();
            if (NativeZBar.zbar_decoder_set_config(
                    this.handle,
                    (int)symbology,
                    (int)config,
                    value) != 0)
            {
                throw new ArgumentException(
                    "The configuration does not apply to the specified symbology or the value is out of range.");
            }
        }

        /// <summary>
        /// 为指定符号类型设置布尔配置。
        /// </summary>
        public void SetConfiguration(SymbolType symbology, Config config, bool value)
        {
            SetConfiguration(symbology, config, value ? 1 : 0);
        }

        /// <summary>
        /// 获取指定符号类型的配置值。
        /// </summary>
        /// <returns>配置项的当前值。</returns>
        public int GetConfiguration(SymbolType symbology, Config config)
        {
            ThrowIfDisposed();
            int value;
            unsafe
            {
                if (NativeZBar.zbar_decoder_get_config(
                        this.handle,
                        (int)symbology,
                        (int)config,
                        &value) != 0)
                {
                    throw new ArgumentException(
                        "The configuration does not apply to the specified symbology.");
                }
            }
            return value;
        }

        /// <summary>
        /// 获取指定符号类型的布尔配置位掩码。
        /// </summary>
        /// <returns>当前已启用配置的位掩码。</returns>
        public uint GetConfigs(SymbolType symbology)
        {
            ThrowIfDisposed();
            return NativeZBar.zbar_decoder_get_configs(this.handle, (int)symbology);
        }

        /// <summary>
        /// 解析配置字符串并应用到当前解码器。
        /// </summary>
        /// <param name="configString">
        /// 形如 "[symbology.]config[=value]" 的字符串。
        /// </param>
        /// <returns>解析并应用成功返回 true。</returns>
        public bool ParseConfig(string configString)
        {
            if (configString == null)
            {
                throw new ArgumentNullException("configString");
            }
            ThrowIfDisposed();

            SymbolType symbology;
            Config config;
            int value;
            if (!ZBar.ParseConfig(configString, out symbology, out config, out value))
            {
                return false;
            }

            return NativeZBar.zbar_decoder_set_config(
                this.handle,
                (int)symbology,
                (int)config,
                value) == 0;
        }

        /// <summary>
        /// 下一个传给 <see cref="DecodeWidth"/> 的元素的颜色。
        /// </summary>
        public Color Color
        {
            get
            {
                ThrowIfDisposed();
                return (Color)NativeZBar.zbar_decoder_get_color(this.handle);
            }
        }

        /// <summary>
        /// 最近一次解码出的数据，按 UTF-8 优先解码。
        /// </summary>
        public string Data
        {
            get
            {
                return Interop.DecodeText(this.DataBytes);
            }
        }

        /// <summary>
        /// 最近一次解码出的原始字节。
        /// </summary>
        public byte[] DataBytes
        {
            get
            {
                ThrowIfDisposed();
                IntPtr pData = NativeZBar.zbar_decoder_get_data(this.handle);
                uint length = NativeZBar.zbar_decoder_get_data_length(this.handle);
                return Interop.CopyBytes(pData, length);
            }
        }

        /// <summary>
        /// 最近一次解码出的二进制数据长度；没有新数据时为 0。
        /// </summary>
        public uint DataLength
        {
            get
            {
                ThrowIfDisposed();
                return NativeZBar.zbar_decoder_get_data_length(this.handle);
            }
        }

        /// <summary>
        /// 最近一次解码出的符号类型；没有新数据时为 <see cref="SymbolType.None"/>。
        /// </summary>
        public SymbolType Type
        {
            get
            {
                ThrowIfDisposed();
                return (SymbolType)NativeZBar.zbar_decoder_get_type(this.handle);
            }
        }

        /// <summary>
        /// 最近一次解码符号的修饰符位掩码。
        /// </summary>
        public uint Modifiers
        {
            get
            {
                ThrowIfDisposed();
                return NativeZBar.zbar_decoder_get_modifiers(this.handle);
            }
        }

        /// <summary>
        /// 最近一次解码方向：1 表示正向，-1 表示反向，0 表示未知或不适用。
        /// </summary>
        public int Direction
        {
            get
            {
                ThrowIfDisposed();
                return NativeZBar.zbar_decoder_get_direction(this.handle);
            }
        }

        /// <summary>
        /// 获取或设置与当前解码器关联的用户数据。
        /// </summary>
        public IntPtr UserData
        {
            get
            {
                ThrowIfDisposed();
                return NativeZBar.zbar_decoder_get_userdata(this.handle);
            }
            set
            {
                ThrowIfDisposed();
                NativeZBar.zbar_decoder_set_userdata(this.handle, value);
            }
        }

        private void ThrowIfDisposed()
        {
            Interop.ThrowIfDisposed(this.handle, "Decoder");
        }

        /// <summary>
        /// 释放当前对象。
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (this.handle != IntPtr.Zero)
            {
                NativeZBar.zbar_decoder_destroy(this.handle);
                this.handle = IntPtr.Zero;
            }
        }

        /// <summary>
        /// 释放当前对象持有的资源。
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 终结器。
        /// </summary>
        ~Decoder()
        {
            this.Dispose(false);
        }
    }
}
