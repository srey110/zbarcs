using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace zbarcs
{
    /// <summary>
    /// 表示 ZBar 中的图像。
    /// </summary>
    public class Image : IDisposable
    {
        /// <summary>
        /// 非托管 ZBar 图像句柄。
        /// </summary>
        private IntPtr handle = IntPtr.Zero;

        /// <summary>
        /// 从非托管图像指针创建托管包装。
        /// </summary>
        /// <remarks>该资源由当前 Image 实例负责管理。</remarks>
        /// <param name="handle">非托管 ZBar 图像指针。</param>
        /// <param name="incRef">是否增加引用计数。</param>
        internal Image(IntPtr handle, bool incRef)
        {
            if (handle == IntPtr.Zero)
            {
                throw new ArgumentException("Can't create an image from a null pointer.", "handle");
            }

            this.handle = handle;
            if (incRef)
            {
                NativeZBar.zbar_image_ref(this.handle, 1);
            }
        }

        /// <summary>
        /// 创建未初始化的图像。
        /// </summary>
        /// <remarks>
        /// 新建图像尚未初始化，必须自行设置宽度、高度、格式和数据。
        /// </remarks>
        public Image()
        {
            this.handle = NativeZBar.zbar_image_create();
            if (this.handle == IntPtr.Zero)
            {
                throw new OutOfMemoryException("Failed to create a new ZBar image.");
            }
        }

        /// <summary>
        /// 从 System.Drawing.Image 创建 ZBar 图像。
        /// </summary>
        /// <param name="image">要转换的图像。</param>
        /// <remarks>
        /// 转换结果为 BGR3 格式；扫描前应通过 <see cref="Convert"/> 转为
        /// GREY/Y800，因为 ZBar 只读取灰度图像。
        /// 透明区域会合成到白色背景上；无论源图 DPI 是多少，都按原始像素
        /// 尺寸逐像素复制。
        /// </remarks>
        public Image(System.Drawing.Image image) : this()
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }

            int width = image.Width;
            int height = image.Height;
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentException("The source image has an invalid size.", "image");
            }

            byte[] data = new byte[checked(width * height * 3)];
            using (Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.PageUnit = GraphicsUnit.Pixel;
                    graphics.CompositingMode = CompositingMode.SourceOver;
                    graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                    graphics.PixelOffsetMode = PixelOffsetMode.Half;
                    graphics.Clear(System.Drawing.Color.White);
                    graphics.DrawImage(
                        image,
                        new Rectangle(0, 0, width, height),
                        new Rectangle(0, 0, width, height),
                        GraphicsUnit.Pixel);
                }

                BitmapData bitmapData = bitmap.LockBits(
                    new Rectangle(0, 0, width, height),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format24bppRgb);
                try
                {
                    ReadRows(bitmapData, data, width, height);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }
            }

            this.Size = new Size(width, height);
            this.Format = FourCC('B', 'G', 'R', '3');
            this.Data = data;
        }

        /// <summary>
        /// 获取非托管图像资源指针。
        /// </summary>
        internal IntPtr Handle
        {
            get
            {
                ThrowIfDisposed();
                return this.handle;
            }
        }

        /// <summary>
        /// 将当前图像转换为 System.Drawing.Bitmap。
        /// </summary>
        /// <returns>当前图像的 <see cref="System.Drawing.Bitmap"/> 表示。</returns>
        public Bitmap ToBitmap()
        {
            ThrowIfDisposed();

            int width = checked((int)this.Width);
            int height = checked((int)this.Height);
            if (width <= 0 || height <= 0 || this.DataLength == 0)
            {
                throw new InvalidOperationException("The image has no pixel data to convert.");
            }

            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            try
            {
                using (Image bgr = this.Convert(FourCC('B', 'G', 'R', '3')))
                {
                    byte[] data = bgr.Data;
                    int rowBytes = width * 3;
                    if (data.Length < rowBytes * height)
                    {
                        throw new InvalidOperationException("The converted image data is incomplete.");
                    }

                    BitmapData bitmapData = bitmap.LockBits(
                        new Rectangle(0, 0, width, height),
                        ImageLockMode.WriteOnly,
                        PixelFormat.Format24bppRgb);
                    try
                    {
                        WriteRows(bitmapData, data, width, height);
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }
                }

                return bitmap;
            }
            catch
            {
                bitmap.Dispose();
                throw;
            }
        }

        /// <summary>
        /// 获取或设置图像宽度。该操作不影响图像数据。
        /// </summary>
        public uint Width
        {
            get
            {
                ThrowIfDisposed();
                return NativeZBar.zbar_image_get_width(this.handle);
            }
            set
            {
                ThrowIfDisposed();
                NativeZBar.zbar_image_set_size(this.handle, value, this.Height);
            }
        }

        /// <summary>
        /// 获取或设置图像高度。该操作不影响图像数据。
        /// </summary>
        public uint Height
        {
            get
            {
                ThrowIfDisposed();
                return NativeZBar.zbar_image_get_height(this.handle);
            }
            set
            {
                ThrowIfDisposed();
                NativeZBar.zbar_image_set_size(this.handle, this.Width, value);
            }
        }

        /// <summary>
        /// 获取或设置图像像素尺寸。
        /// </summary>
        /// <remarks>
        /// 该操作会把裁剪区域重置为整幅图像，但不影响图像数据。
        /// </remarks>
        public Size Size
        {
            get
            {
                ThrowIfDisposed();
                uint width;
                uint height;
                unsafe
                {
                    NativeZBar.zbar_image_get_size(this.handle, &width, &height);
                }
                return new Size(checked((int)width), checked((int)height));
            }
            set
            {
                ThrowIfDisposed();
                if (value.Width < 0 || value.Height < 0)
                {
                    throw new ArgumentOutOfRangeException("value", "The image size cannot be negative.");
                }
                NativeZBar.zbar_image_set_size(this.handle, (uint)value.Width, (uint)value.Height);
            }
        }

        /// <summary>
        /// 获取或设置要扫描的图像矩形区域。
        /// </summary>
        /// <remarks>
        /// ZBar 会把该矩形裁剪到图像边界内。
        /// </remarks>
        public Rectangle Crop
        {
            get
            {
                ThrowIfDisposed();
                uint x;
                uint y;
                uint width;
                uint height;
                unsafe
                {
                    NativeZBar.zbar_image_get_crop(this.handle, &x, &y, &width, &height);
                }
                return new Rectangle(
                    checked((int)x),
                    checked((int)y),
                    checked((int)width),
                    checked((int)height));
            }
            set
            {
                ThrowIfDisposed();
                if (value.X < 0 || value.Y < 0 || value.Width < 0 || value.Height < 0)
                {
                    throw new ArgumentOutOfRangeException("value", "The crop rectangle cannot be negative.");
                }
                NativeZBar.zbar_image_set_crop(
                    this.handle,
                    (uint)value.X,
                    (uint)value.Y,
                    (uint)value.Width,
                    (uint)value.Height);
            }
        }

        /// <summary>
        /// 获取或设置图像采样数据的 FourCC 格式码。
        /// </summary>
        /// <remarks>
        /// 修改格式不会转换数据；如需转换请调用 <see cref="Convert"/>。
        /// FourCC 的生成方式见 <see cref="FourCC(char, char, char, char)"/>。
        /// </remarks>
        public uint Format
        {
            get
            {
                ThrowIfDisposed();
                return NativeZBar.zbar_image_get_format(this.handle);
            }
            set
            {
                ThrowIfDisposed();
                NativeZBar.zbar_image_set_format(this.handle, value);
            }
        }

        /// <summary>
        /// 获取或设置与图像关联的“序列号”（页号/帧号）。
        /// </summary>
        public uint SequenceNumber
        {
            get
            {
                ThrowIfDisposed();
                return NativeZBar.zbar_image_get_sequence(this.handle);
            }
            set
            {
                ThrowIfDisposed();
                NativeZBar.zbar_image_set_sequence(this.handle, value);
            }
        }

        /// <summary>
        /// 获取图像采样数据的字节长度。
        /// </summary>
        public uint DataLength
        {
            get
            {
                ThrowIfDisposed();
                return NativeZBar.zbar_image_get_data_length(this.handle);
            }
        }

        /// <summary>
        /// 获取或设置图像关联的数据。
        /// </summary>
        /// <remarks>
        /// 设置时会把数据复制到非托管内存，之后由 ZBar 持有该副本；
        /// 获取时会把原生数据复制到托管数组。
        /// </remarks>
        public byte[] Data
        {
            get
            {
                ThrowIfDisposed();
                uint length = NativeZBar.zbar_image_get_data_length(this.handle);
                if (length == 0)
                {
                    return new byte[0];
                }

                IntPtr pData = NativeZBar.zbar_image_get_data(this.handle);
                if (pData == IntPtr.Zero)
                {
                    throw new InvalidOperationException("The image data pointer is null.");
                }
                return Interop.CopyBytes(pData, length);
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                ThrowIfDisposed();

                if (value.Length == 0)
                {
                    NativeZBar.zbar_image_set_data(this.handle, IntPtr.Zero, 0, null);
                    return;
                }

                IntPtr data = Marshal.AllocHGlobal(value.Length);
                try
                {
                    Marshal.Copy(value, 0, data, value.Length);
                    NativeZBar.zbar_image_set_data(
                        this.handle,
                        data,
                        (uint)value.Length,
                        CleanupHandler);
                }
                catch
                {
                    Marshal.FreeHGlobal(data);
                    throw;
                }
            }
        }

        /// <summary>
        /// 数据清理回调。以静态字段持有引用，避免托管委托被回收后原生代码
        /// 仍在使用它。
        /// </summary>
        private static readonly NativeZBar.zbar_image_cleanup_handler CleanupHandler =
            new NativeZBar.zbar_image_cleanup_handler(ReleaseAllocatedUnmanagedMemory);

        private static void ReleaseAllocatedUnmanagedMemory(IntPtr image)
        {
            IntPtr pData = NativeZBar.zbar_image_get_data(image);
            if (pData != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(pData);
            }
        }

        /// <summary>
        /// 释放原生图像数据并解除清理回调。
        /// </summary>
        public void ClearData()
        {
            ThrowIfDisposed();
            NativeZBar.zbar_image_set_data(this.handle, IntPtr.Zero, 0, null);
        }

        /// <summary>
        /// 获取或设置与图像关联的用户数据。
        /// </summary>
        public IntPtr UserData
        {
            get
            {
                ThrowIfDisposed();
                return NativeZBar.zbar_image_get_userdata(this.handle);
            }
            set
            {
                ThrowIfDisposed();
                NativeZBar.zbar_image_set_userdata(this.handle, value);
            }
        }

        /// <summary>
        /// 获取当前图像关联的解码结果符号。
        /// </summary>
        public IEnumerable<Symbol> Symbols
        {
            get
            {
                ThrowIfDisposed();
                return new List<Symbol>(
                    SymbolEnumerable.FromSet(NativeZBar.zbar_image_get_symbols(this.handle)));
            }
        }

        /// <summary>
        /// 转换图像格式。支持的格式请参考 ZBar 文档。
        /// </summary>
        /// <remarks>
        /// 受格式约束，转换后的图像尺寸可能向上取整。
        /// FourCC 的生成方式见 <see cref="FourCC(char, char, char, char)"/>。
        /// </remarks>
        /// <param name="format">目标 FourCC 格式。</param>
        /// <returns>
        /// 返回新的 <see cref="Image"/>，包含按目标格式转换后的采样数据；
        /// 原图像不受影响。
        /// </returns>
        public Image Convert(uint format)
        {
            ThrowIfDisposed();
            if (this.DataLength == 0)
            {
                throw new InvalidOperationException("The image has no data to convert.");
            }
            IntPtr img = NativeZBar.zbar_image_convert(this.handle, format);
            if (img == IntPtr.Zero)
            {
                throw new NotSupportedException("The image could not be converted to the requested format.");
            }
            return new Image(img, false);
        }

        /// <summary>
        /// 转换图像格式并按指定尺寸裁剪/填充。
        /// </summary>
        /// <remarks>
        /// 目标尺寸大于原图时，会复制最后一行/列补齐；目标尺寸小于原图时，
        /// 会丢弃右侧/底部多出的行/列。图像不会被缩放。
        /// </remarks>
        public Image ConvertResize(uint format, uint width, uint height)
        {
            ThrowIfDisposed();
            if (this.DataLength == 0)
            {
                throw new InvalidOperationException("The image has no data to convert.");
            }
            if (format == this.Format && (width != this.Width || height != this.Height))
            {
                throw new NotSupportedException(
                    "ZBar cannot resize an image without changing its format.");
            }
            IntPtr img = NativeZBar.zbar_image_convert_resize(this.handle, format, width, height);
            if (img == IntPtr.Zero)
            {
                throw new NotSupportedException("The image could not be converted to the requested format and size.");
            }
            return new Image(img, false);
        }

        /// <summary>
        /// 把原始图像数据导出到文件，用于调试。
        /// </summary>
        /// <param name="fileBase">
        /// 基础文件名；实际文件会在其后追加 ".XXXX.zimg"，其中 XXXX 为格式的
        /// FourCC。
        /// </param>
        public void Write(string fileBase)
        {
            if (fileBase == null)
            {
                throw new ArgumentNullException("fileBase");
            }
            ThrowIfDisposed();

            int result = NativeZBar.zbar_image_write(this.handle, fileBase);
            if (result != 0)
            {
                throw new IOException("Failed to write the image (error " + result + ").");
            }
        }

        /// <summary>
        /// 由四个字符生成 FourCC 码。
        /// </summary>
        /// <remarks>
        /// FourCC 的更多信息见 FourCC.org。
        /// </remarks>
        public static uint FourCC(char c0, char c1, char c2, char c3)
        {
            if (c0 > 0xff || c1 > 0xff || c2 > 0xff || c3 > 0xff)
            {
                throw new ArgumentOutOfRangeException("c0", "FourCC characters must fit in one byte.");
            }
            return (uint)c0 | ((uint)c1) << 8 | ((uint)c2) << 16 | ((uint)c3) << 24;
        }

        /// <summary>
        /// 由四字符字符串生成 FourCC 码。
        /// </summary>
        public static uint FourCC(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (value.Length != 4)
            {
                throw new ArgumentException("A FourCC value must contain exactly four characters.", "value");
            }
            return FourCC(value[0], value[1], value[2], value[3]);
        }

        /// <summary>
        /// 将位图数据逐行读入托管数组，自动处理 stride 填充。
        /// </summary>
        private static void ReadRows(BitmapData bitmapData, byte[] data, int width, int height)
        {
            int rowBytes = width * 3;
            if (bitmapData.Stride == rowBytes)
            {
                Marshal.Copy(bitmapData.Scan0, data, 0, rowBytes * height);
                return;
            }

            if (bitmapData.Stride < 0)
            {
                throw new NotSupportedException("Bottom-up bitmaps are not supported.");
            }

            for (int y = 0; y < height; y++)
            {
                IntPtr source = IntPtr.Add(bitmapData.Scan0, y * bitmapData.Stride);
                Marshal.Copy(source, data, y * rowBytes, rowBytes);
            }
        }

        /// <summary>
        /// 将托管数组逐行写入位图，自动处理 stride 填充。
        /// </summary>
        private static void WriteRows(BitmapData bitmapData, byte[] data, int width, int height)
        {
            int rowBytes = width * 3;
            if (bitmapData.Stride == rowBytes)
            {
                Marshal.Copy(data, 0, bitmapData.Scan0, rowBytes * height);
                return;
            }

            if (bitmapData.Stride < 0)
            {
                throw new NotSupportedException("Bottom-up bitmaps are not supported.");
            }

            for (int y = 0; y < height; y++)
            {
                IntPtr destination = IntPtr.Add(bitmapData.Scan0, y * bitmapData.Stride);
                Marshal.Copy(data, y * rowBytes, destination, rowBytes);
            }
        }

        private void ThrowIfDisposed()
        {
            Interop.ThrowIfDisposed(this.handle, "Image");
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
                NativeZBar.zbar_image_destroy(this.handle);
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
        ~Image()
        {
            this.Dispose(false);
        }
    }
}
