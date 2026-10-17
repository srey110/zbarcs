using System;
using System.Collections.Generic;

namespace zbarcs
{
    /// <summary>
    /// 中层图像扫描器接口，用于从二维图像中读取条码。
    /// </summary>
    public class ImageScanner : IDisposable
    {
        private IntPtr handle = IntPtr.Zero;
        private bool cache = false;

        /// <summary>
        /// 创建新的图像扫描器。
        /// </summary>
        public ImageScanner()
        {
            this.handle = NativeZBar.zbar_image_scanner_create();
            if (this.handle == IntPtr.Zero)
            {
                throw new OutOfMemoryException("Failed to create an underlying image scanner.");
            }
        }

        /// <summary>
        /// 扫描图像中的符号。
        /// </summary>
        /// <remarks>
        /// 图像扫描完成后，结果会关联到该图像上，可通过
        /// <see cref="Image.Symbols"/> 获取符号列表。
        /// </remarks>
        /// <param name="image">要扫描的 <see cref="Image"/>。</param>
        /// <returns>图像中解码出的符号数量。</returns>
        public int Scan(Image image)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }
            ThrowIfDisposed();

            int count = NativeZBar.zbar_scan_image(this.handle, image.Handle);
            if (count < 0)
            {
                throw new ArgumentException(
                    "The image format must be Y800 or GREY before it can be scanned.",
                    "image");
            }
            return count;
        }

        /// <summary>
        /// 扫描 System.Drawing.Image 中的符号。
        /// </summary>
        /// <param name="image">要扫描的 <see cref="System.Drawing.Image"/>。</param>
        /// <remarks>
        /// 本方法会把图像转换成 ZBar 需要的格式，并在复制完所有符号后立即释放
        /// 转换出的图像。
        /// </remarks>
        /// <returns>图像中解码出的符号列表。</returns>
        public List<Symbol> Scan(System.Drawing.Image image)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }

            using (Image zimg = new Image(image))
            {
                using (Image grey = zimg.Convert(Image.FourCC('Y', '8', '0', '0')))
                {
                    this.Scan(grey);
                    return new List<Symbol>(grey.Symbols);
                }
            }
        }

        /// <summary>
        /// 启用或禁用帧间结果缓存（默认禁用）。
        /// </summary>
        /// <remarks>
        /// 主要用于视频帧扫描。缓存会过滤连续图像中的重复结果，同时增加一致性
        /// 检查和迟滞；修改该属性也会清空缓存。
        /// </remarks>
        public bool Cache
        {
            get
            {
                ThrowIfDisposed();
                return this.cache;
            }
            set
            {
                ThrowIfDisposed();
                NativeZBar.zbar_image_scanner_enable_cache(this.handle, value ? 1 : 0);
                this.cache = value;
            }
        }

        /// <summary>
        /// 为指定符号类型（传 0 表示全部）设置配置值。
        /// </summary>
        public void SetConfiguration(SymbolType symbology, Config config, int value)
        {
            ThrowIfDisposed();
            if (NativeZBar.zbar_image_scanner_set_config(
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
                if (NativeZBar.zbar_image_scanner_get_config(
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
        /// 解析配置字符串并应用到当前图像扫描器。
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

            return NativeZBar.zbar_image_scanner_set_config(
                this.handle,
                (int)symbology,
                (int)config,
                value) == 0;
        }

        /// <summary>
        /// 从图像扫描器和指定图像中移除之前解码出的结果。
        /// </summary>
        public void RecycleImage(Image image)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }
            ThrowIfDisposed();
            NativeZBar.zbar_image_scanner_recycle_image(this.handle, image.Handle);
        }

        /// <summary>
        /// 获取最近一次扫描图像的解码结果。
        /// </summary>
        /// <remarks>
        /// 返回的符号都是副本，下次扫描图像后仍然有效。
        /// </remarks>
        public IEnumerable<Symbol> Results
        {
            get
            {
                ThrowIfDisposed();
                return new List<Symbol>(
                    SymbolEnumerable.FromSet(NativeZBar.zbar_image_scanner_get_results(this.handle)));
            }
        }

        /// <summary>
        /// 请求通过 D-Bus 发送解码结果。
        /// </summary>
        /// <returns>请求被接受返回 true；D-Bus 不可用时返回 false。</returns>
        public bool RequestDbus(bool enabled)
        {
            ThrowIfDisposed();
            return NativeZBar.zbar_image_scanner_request_dbus(this.handle, enabled ? 1 : 0) == 0;
        }

        private void ThrowIfDisposed()
        {
            Interop.ThrowIfDisposed(this.handle, "ImageScanner");
        }

        // 以下 Dispose 模式参考：
        // 《框架设计指南（Framework Design Guidelines）》第 2 版第 9.4 节

        /// <summary>
        /// 释放当前对象。
        /// </summary>
        /// <param name="disposing">
        /// 从终结器调用时为 false，从 Dispose 调用时为 true。
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.handle != IntPtr.Zero)
            {
                NativeZBar.zbar_image_scanner_destroy(this.handle);
                this.handle = IntPtr.Zero;
            }
            if (disposing)
            {
                // 当前没有需要释放的可终结资源。
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
        ~ImageScanner()
        {
            this.Dispose(false);
        }
    }
}
