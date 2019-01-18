using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Zstandard.Net
{
    public class PoolingResourceManager : IResourceManager
    {
        private readonly int poolSize;
        private readonly ConcurrentQueue<ZstdStreamSafeHandle> cQueue = new ConcurrentQueue<ZstdStreamSafeHandle>();
        private readonly ConcurrentQueue<ZstdStreamSafeHandle> dQueue = new ConcurrentQueue<ZstdStreamSafeHandle>();

        public PoolingResourceManager(int poolSize)
        {
            this.poolSize = poolSize;
        }

        public void ReleaseHandle(ZstdStreamSafeHandle handle)
        {
            if (handle.IsCompress && cQueue.Count < poolSize)
            {
                cQueue.Enqueue(handle);
            }
            else if (!handle.IsCompress && dQueue.Count < poolSize)
            {
                dQueue.Enqueue(handle);
            }
            else
            {
                handle.Close();
            }
        }

        public ZstdStreamSafeHandle RentHandle(bool isCompression)
        {
            if (isCompression && cQueue.TryDequeue(out var handle))
            {
                return handle;
            }
            else if (!isCompression && dQueue.TryDequeue(out handle))
            {
                return handle;
            }

            return new ZstdStreamSafeHandle(isCompression);
        }

        public byte[] RentMemory(int sizeHint)
        {
            return ArrayPool<byte>.Shared.Rent(sizeHint);
        }

        public void ReturnMemory(byte[] buffer)
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}
