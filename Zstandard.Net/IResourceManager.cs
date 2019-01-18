using System.Runtime.InteropServices;

namespace Zstandard.Net
{
    public interface IResourceManager
    {
        ZstdStreamSafeHandle RentHandle(bool isCompress);
        void ReleaseHandle(ZstdStreamSafeHandle handle);
        byte[] RentMemory(int sizeHint);
        void ReturnMemory(byte[] buffer);
    }
}
