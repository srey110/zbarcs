using System;
using System.Runtime.InteropServices;

namespace zbarcs
{
    internal sealed class SafeZBarHandle : SafeHandle
    {
        private readonly NativeHandleType type;
        private readonly SafeZBarHandle dependency;
        private readonly bool ownsDependencyRef;

        internal SafeZBarHandle(IntPtr handle, NativeHandleType type)
            : this(handle, type, null, false)
        {
        }

        internal SafeZBarHandle(
            IntPtr handle,
            NativeHandleType type,
            SafeZBarHandle dependency,
            bool ownsDependencyRef)
            : base(IntPtr.Zero, true)
        {
            this.type = type;
            this.dependency = dependency;
            this.ownsDependencyRef = ownsDependencyRef;
            this.SetHandle(handle);
        }

        public override bool IsInvalid
        {
            get
            {
                return this.handle == IntPtr.Zero;
            }
        }

        protected override bool ReleaseHandle()
        {
            try
            {
                switch (this.type)
                {
                    case NativeHandleType.Image:
                        NativeZBar.zbar_image_destroy(this.handle);
                        break;
                    case NativeHandleType.ImageScanner:
                        NativeZBar.zbar_image_scanner_destroy(this.handle);
                        break;
                    case NativeHandleType.Decoder:
                        NativeZBar.zbar_decoder_destroy(this.handle);
                        break;
                    case NativeHandleType.Scanner:
                        NativeZBar.zbar_scanner_destroy(this.handle);
                        break;
                    default:
                        return false;
                }
                return true;
            }
            finally
            {
                if (this.ownsDependencyRef)
                {
                    this.dependency.DangerousRelease();
                }
            }
        }
    }

    internal enum NativeHandleType
    {
        Image,
        ImageScanner,
        Decoder,
        Scanner
    }
}
