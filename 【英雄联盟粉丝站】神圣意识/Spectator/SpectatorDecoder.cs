using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAwareness.Spectator
{
    class SpectatorDecoder
    {
        public static List<Packet> DecodeBytes(Byte[] bytes)
        {
            List<Packet> packets = new List<Packet>();
            Byte marker;
            Int32 offset = 0;
            Single lastTime = 0;
            UInt32 contentLength = 0;
            Byte packetHeader = 0;
            UInt32 blockParam = 0;
            while (offset < bytes.Count() - 5)
            {
                marker = bytes[offset++];
                if (((marker >> 7) & 1) == 1)
                    lastTime += bytes[offset++] / 1000.0f;
                else
                    lastTime = BitConverter.ToSingle(new Byte[4] { bytes[offset++], bytes[offset++], bytes[offset++], bytes[offset++] }, 0);
                if (((marker >> 4) & 1) == 1)
                    contentLength = bytes[offset++];
                else
                    contentLength = BitConverter.ToUInt32(new Byte[4] { bytes[offset++], bytes[offset++], bytes[offset++], bytes[offset++] }, 0);
                if (((marker >> 6) & 1) == 0)
                    packetHeader = bytes[offset++];
                if (((marker >> 5) & 1) == 1)
                {
                    Byte b = bytes[offset++];
                    if (b >> 7 == 1)
                    {
                        b = (byte)(0xff - b);
                        blockParam -= b;
                    }
                    else
                        blockParam += b;
                }
                else
                    blockParam = BitConverter.ToUInt32(new Byte[4] { bytes[offset++], bytes[offset++], bytes[offset++], bytes[offset++] }, 0);
                List<Byte> content = new List<Byte>();
                for (UInt32 j = 0; j < contentLength; j++)
                {
                    content.Add(bytes[offset++]);
                }
                packets.Add(new Packet(blockParam, packetHeader, lastTime, content.ToArray()));
            }
            return packets;
        }
        public static List<Packet> DecodeFile(String gameId, String type, int id)
        {
            Byte[] bytes = File.ReadAllBytes(gameId + @"\" + type + @"\" + id);
            return DecodeBytes(bytes);
        }
        public static List<Packet> DecodeGameChunk(String gameId, int chunkId)
        {
            return DecodeFile(gameId, "Chunk", chunkId);
        }
        public static List<Packet> DecodeGameKeyFrame(String gameId, int chunkId)
        {
            return DecodeFile(gameId, "KeyFrame", chunkId);
        }
    }
}
