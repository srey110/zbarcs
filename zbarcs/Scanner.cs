using System;

namespace zbarcs
{
    /// <summary>
    /// 低层线性强度采样扫描器，用于识别条边缘并测量宽度。
    /// 可以选择关联一个 <see cref="Decoder"/>。
    /// </summary>
    public class Scanner : IDisposable
    {
        private IntPtr handle = IntPtr.Zero;
        private readonly Decoder decoder;

        /// <summary>
        /// 创建不关联解码器的扫描器。
        /// </summary>
        public Scanner() : this(null)
        {
        }

        /// <summary>
        /// 创建扫描器并关联解码器。
        /// </summary>
        /// <param name="decoder">
        /// 要关联的解码器；传 null 表示不关联。解码器必须在扫描器使用期间保持存活。
        /// </param>
        public Scanner(Decoder decoder)
        {
            this.decoder = decoder;
            IntPtr decoderHandle = decoder == null ? IntPtr.Zero : decoder.Handle;
            this.handle = NativeZBar.zbar_scanner_create(decoderHandle);
            if (this.handle == IntPtr.Zero)
            {
                throw new OutOfMemoryException("Failed to create a ZBar scanner.");
            }
        }

        /// <summary>
        /// 当前关联的解码器，可能为 null。
        /// </summary>
        public Decoder Decoder
        {
            get
            {
                return this.decoder;
            }
        }

        /// <summary>
        /// 清除扫描器全部状态，同时重置关联的解码器。
        /// </summary>
        public SymbolType Reset()
        {
            ThrowIfDisposed();
            return (SymbolType)NativeZBar.zbar_scanner_reset(this.handle);
        }

        /// <summary>
        /// 标记新一轮扫描开始，并把颜色重置为空色。
        /// </summary>
        /// <remarks>
        /// 调用前应先调用 <see cref="Flush"/> 至少两次，以免丢失解码结果。
        /// </remarks>
        public SymbolType NewScan()
        {
            ThrowIfDisposed();
            return (SymbolType)NativeZBar.zbar_scanner_new_scan(this.handle);
        }

        /// <summary>
        /// 冲刷扫描器处理管线。多次调用（最多 3 次）可完全冲刷解码器。
        /// </summary>
        public SymbolType Flush()
        {
            ThrowIfDisposed();
            return (SymbolType)NativeZBar.zbar_scanner_flush(this.handle);
        }

        /// <summary>
        /// 处理下一个采样强度值，强度使用任意相对单位。
        /// </summary>
        public SymbolType ScanY(int y)
        {
            ThrowIfDisposed();
            return (SymbolType)NativeZBar.zbar_scan_y(this.handle, y);
        }

        /// <summary>
        /// 处理 RGB（或 BGR）三元组中的下一个采样。
        /// </summary>
        public SymbolType ScanRgb24(byte red, byte green, byte blue)
        {
            return this.ScanY(red + green + blue);
        }

        /// <summary>
        /// 获取最近一次扫描的宽度。
        /// </summary>
        public uint Width
        {
            get
            {
                ThrowIfDisposed();
                return NativeZBar.zbar_scanner_get_width(this.handle);
            }
        }

        /// <summary>
        /// 获取最近一条边缘的采样位置。
        /// </summary>
        public uint GetEdge(uint offset, int precision)
        {
            ThrowIfDisposed();
            return NativeZBar.zbar_scanner_get_edge(this.handle, offset, precision);
        }

        /// <summary>
        /// 获取最近一次扫描的颜色。
        /// </summary>
        public Color Color
        {
            get
            {
                ThrowIfDisposed();
                return (Color)NativeZBar.zbar_scanner_get_color(this.handle);
            }
        }

        private void ThrowIfDisposed()
        {
            Interop.ThrowIfDisposed(this.handle, "Scanner");
        }

        /// <summary>
        /// 释放当前对象。
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (this.handle != IntPtr.Zero)
            {
                NativeZBar.zbar_scanner_destroy(this.handle);
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
        ~Scanner()
        {
            this.Dispose(false);
        }
    }
}
