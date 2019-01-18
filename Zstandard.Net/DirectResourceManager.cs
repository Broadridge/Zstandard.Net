using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Zstandard.Net
{
    public class DirectResourceManager : IResourceManager
    {
        public static IResourceManager Instance = new DirectResourceManager();

        public void ReleaseHandle(ZstdStreamSafeHandle handle)
        {
            handle.Close();
        }

        public ZstdStreamSafeHandle RentHandle(bool isCompression)
        {
            return new ZstdStreamSafeHandle(isCompression);
        }

        public byte[] RentMemory(int sizeHint)
        {
            return new byte[sizeHint];
        }

        public void ReturnMemory(byte[] buffer)
        {
        }
    }
}
