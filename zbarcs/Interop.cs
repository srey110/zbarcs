using System;
using System.Runtime.InteropServices;
using System.Text;

namespace zbarcs
{
    /// <summary>
    /// 托管 ZBar 封装共用的辅助方法。
    /// </summary>
    internal static class Interop
    {
        // zbar 返回的是字节数据。二维码解码器使用 UTF-8，部分传统码制则使用
        // ASCII 或 Latin-1；对非法 UTF-8 回退到 Latin-1，避免二进制负载被
        // 静默损坏。
        private static readonly Encoding Utf8 = new UTF8Encoding(false, true);
        private static readonly Encoding Latin1 = Encoding.GetEncoding(28591);

        /// <summary>
        /// 从非托管内存复制指定长度的字节。
        /// </summary>
        /// <param name="pointer">非托管数据指针。</param>
        /// <param name="length">字节数。</param>
        /// <returns>复制后的托管字节数组。</returns>
        internal static byte[] CopyBytes(IntPtr pointer, uint length)
        {
            if (length == 0)
            {
                return new byte[0];
            }
            if (pointer == IntPtr.Zero)
            {
                throw new InvalidOperationException("The native data pointer is null.");
            }
            if (length > int.MaxValue)
            {
                throw new OutOfMemoryException("The native data is too large to copy.");
            }

            byte[] data = new byte[(int)length];
            Marshal.Copy(pointer, data, 0, data.Length);
            return data;
        }

        /// <summary>
        /// 将字节数据解码为文本，优先使用 UTF-8，失败时回退到 Latin-1。
        /// </summary>
        internal static string DecodeText(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return string.Empty;
            }

            try
            {
                return Utf8.GetString(data);
            }
            catch (DecoderFallbackException)
            {
                return Latin1.GetString(data);
            }
        }

        /// <summary>
        /// 对象已释放时抛出 <see cref="ObjectDisposedException"/>。
        /// </summary>
        internal static void ThrowIfDisposed(IntPtr handle, string objectName)
        {
            if (handle == IntPtr.Zero)
            {
                throw new ObjectDisposedException(objectName);
            }
        }
    }
}
