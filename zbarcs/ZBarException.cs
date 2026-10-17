using System;
using System.Runtime.InteropServices;

namespace zbarcs
{
    /// <summary>
    /// 表示 ZBar 原生错误。
    /// </summary>
    public sealed class ZBarException : Exception
    {
        private const int verbosity = 10;
        private string message;
        private ZBarError code;

        /// <summary>
        /// 从原生错误对象创建异常。
        /// </summary>
        /// <param name="obj">包含 errinfo_t 的原生对象。</param>
        internal ZBarException(IntPtr obj)
        {
            if (obj == IntPtr.Zero)
            {
                this.code = ZBarError.Ok;
                this.message = "Unknown ZBar error.";
                return;
            }

            this.code = (ZBarError)NativeZBar._zbar_get_error_code(obj);
            IntPtr val = NativeZBar._zbar_error_string(obj, verbosity);
            if (IntPtr.Zero == val)
            {
                this.message = "Unknown ZBar error.";
                return;
            }

            string text = Marshal.PtrToStringAnsi(val);
            this.message = string.IsNullOrEmpty(text) ? "Unknown ZBar error." : text;
        }

        /// <summary>
        /// ZBar 返回的错误消息。
        /// </summary>
        public override string Message
        {
            get
            {
                return this.message;
            }
        }

        /// <summary>
        /// ZBar 返回的错误码。
        /// </summary>
        public ZBarError ErrorCode
        {
            get
            {
                return this.code;
            }
        }
    }
}
