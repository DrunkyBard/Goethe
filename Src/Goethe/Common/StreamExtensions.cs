using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;

namespace Goethe.Common;

public static class StreamExtensions
{
    public static void CopyTo(this Stream stream, long length, Stream destination)
    {
        Debug.Assert(stream.Position + length < stream.Length);

        if (length == 0)
        {
            return;
        }

        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }
        
        var buffer = ArrayPool<byte>.Shared.Rent(81920);

        try
        {
            int read;
            var remainingBytes = length;

            while ((read = stream.Read(buffer, 0, remainingBytes)) > 0)
            {
                destination.Write(buffer, 0, read);
                remainingBytes -= read;
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}