using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdsFileTypePlus
{
    public static class StreamExtension
    {
        public static void WriteUInt32(this System.IO.Stream stream, UInt32 uint32)
        {
            stream.WriteByte((byte)(uint32 & 0xff));
            stream.WriteByte((byte)((uint32 >> 8) & 0xff));
            stream.WriteByte((byte)((uint32 >> 16) & 0xff));
            stream.WriteByte((byte)((uint32 >> 24) & 0xff));
        }
    }
}
