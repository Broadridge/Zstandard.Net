using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Zstandard.Net
{
    public class ZstdStreamSafeHandle : SafeHandle
    {

        public ZstdStreamSafeHandle(bool isCompress)
            : base(IntPtr.Zero, true)
        {
            this.IsCompress = isCompress;
            this.handle = isCompress
                ? ZstandardInterop.ZSTD_createCStream()
                : ZstandardInterop.ZSTD_createDStream();
        }

        public bool IsCompress { get; }

        public override bool IsInvalid => this.handle == IntPtr.Zero;

        protected override bool ReleaseHandle()
        {
            var handle = Interlocked.Exchange(ref this.handle, IntPtr.Zero);
            if (handle != IntPtr.Zero)
            {
                if (IsCompress)
                {
                    ZstandardInterop.ZSTD_freeCStream(this.handle);
                }
                else
                {
                    ZstandardInterop.ZSTD_freeDStream(this.handle);
                }
            }

            return true;
        }
    }
}
