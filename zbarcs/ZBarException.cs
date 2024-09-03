using System;
using System.Runtime.InteropServices;

namespace zbarcs
{
    public sealed class ZBarException : Exception
    {
        private const int verbosity = 10;
        private string message;
        private ZBarError code;
        internal ZBarException(IntPtr obj)
        {
            this.code = (ZBarError)NativeZBar._zbar_get_error_code(obj);
            IntPtr val = NativeZBar._zbar_error_string(obj, verbosity);
            if (IntPtr.Zero == val)
            {
                this.message = "Unkown";
                return;
            }
            this.message = Marshal.PtrToStringAnsi(val);
        }
        /// <value>
        /// Error message from ZBar
        /// </value>
        public override string Message
        {
            get
            {
                return this.message;
            }
        }
        /// <value>
        /// Error code of this exception, from ZBar
        /// </value>
        public ZBarError ErrorCode
        {
            get
            {
                return this.code;
            }
        }
    }
}