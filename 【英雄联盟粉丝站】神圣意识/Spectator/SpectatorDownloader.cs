using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SAwareness.Spectator
{
    class SpectatorDownloader
    {
        public static String specHtml;

        public static void DownloadAllGameInfo(String gameId, String platformId, String encryptionKey)
        {
            while (!(Boolean)((Dictionary<Object, Object>)SpectatorDownloader.DownloadMetaData(gameId, platformId))["gameEnded"])
            {
                SpectatorDownloader.DownloadGameFiles(gameId, platformId, encryptionKey);
                Thread.Sleep(15000);
            }
        }

        public static Object[] GetFeaturedGames()
        {
            return (Object[])new JavaScriptSerializer().Deserialize<Dictionary<Object, Object>>(new WebClient().DownloadString(specHtml + "featured"))["gameList"];
        }
        public static Dictionary<Object, Object> DownloadMetaData(String gameId, String platformId)
        {
            String metaData = new WebClient().DownloadString(specHtml + "consumer/getGameMetaData/" + platformId + "/" + gameId + "/1/token");
            return new JavaScriptSerializer().Deserialize<Dictionary<Object, Object>>(metaData);
        }
        static Byte[] DownloadFile(String gameId, String platformId, String type, int id)
        {
            try
            {
                String prepend = "get";
                if (type.Equals("Chunk"))
                    prepend += "GameData";
                return new WebClient().DownloadData(specHtml + "consumer/" + prepend + type + "/" + platformId + "/" + gameId + "/" + id + "/token");
            }
            catch
            {
                return new Byte[] { 0xde };
            }
        }
        static Byte[] DownloadChunk(String platformId, String gameId, int chunkId)
        {
            return DownloadFile(gameId, platformId, "getGameDataChunk", chunkId);
        }
        static Byte[] DownloadKeyFrame(String platformId, String gameId, int keyFrame)
        {
            return DownloadFile(gameId, platformId, "getKeyFrame", keyFrame);
        }
        static Byte[] Decrypt(Byte[] encryptKey, Byte[] data)
        {
            return new Blowfish(encryptKey).Decrypt_ECB(data);
        }
        static Byte[] Decompress(Byte[] data)
        {
            GZipStream gzip = new GZipStream(new MemoryStream(data), CompressionMode.Decompress);
            const int size = 4096;
            Byte[] buffer = new Byte[size];
            MemoryStream memory = new MemoryStream();
            int count = 0;
            do
            {
                count = gzip.Read(buffer, 0, size);
                if (count > 0)
                {
                    memory.Write(buffer, 0, count);
                }
            }
            while (count > 0);
            return memory.ToArray();
        }
        public static void DownloadGameFiles(String gameId, String platformId, String encryptionKeyString)
        {
            DownloadGameChunks(gameId, platformId, encryptionKeyString);
            DownloadGameKeyFrames(gameId, platformId, encryptionKeyString);
        }
        public static List<Byte[]> DownloadGameFiles(String gameId, String platformId, String encryptionKeyString, String type)
        {
            List<Byte> encryptionKeyByteList = Decrypt(Encoding.ASCII.GetBytes(gameId), Convert.FromBase64String(encryptionKeyString)).ToList();
            encryptionKeyByteList.RemoveRange(16, encryptionKeyByteList.Count() - 16);
            Byte[] encryptionKeyBytes = encryptionKeyByteList.ToArray();
            Dictionary<Object, Object> metadata = DownloadMetaData(gameId, platformId);
            List<Byte[]> totalBytes = new List<Byte[]>();
            for (int i = (int)metadata["lastChunkId"] - 14; i <= (int)metadata["last" + type + "Id"]; i++)
            {
                if (i < (int)metadata["endStartupChunkId"])
                    continue;
                if (!Directory.Exists(gameId))
                    Directory.CreateDirectory(gameId);
                if (!Directory.Exists(gameId + @"\" + type))
                    Directory.CreateDirectory(gameId + @"\" + type);
                if (File.Exists(gameId + @"\" + type + @"\" + i.ToString()))
                    continue;
                Byte[] encryptedFile = DownloadFile(gameId, platformId, type, i);
                if (encryptedFile.Count() > 5)
                {
                    Byte[] compressedFile = Decrypt(encryptionKeyBytes, encryptedFile);
                    Byte[] chunk = Decompress(compressedFile);
                    totalBytes.Add(chunk);
                    //Console.WriteLine("added : " + i + "/" + (int)metadata["last" + type + "Id"]);
                }
            }
            return totalBytes;
        }
        public static void DownloadGameKeyFrames(String gameId, String platformId, String encryptionKeyString)
        {
            DownloadGameFiles(gameId, platformId, encryptionKeyString, "KeyFrame");
        }
        public static void DownloadGameChunks(String gameId, String platformId, String encryptionKeyString)
        {
            DownloadGameFiles(gameId, platformId, encryptionKeyString, "Chunk");
        }
        public static void DownloadFeaturedGamesChunks()
        {
            Object[] featuredGames = GetFeaturedGames();
            foreach (Dictionary<String, Object> game in featuredGames)
            {
                DownloadGameChunks(((int)game["gameId"]).ToString(),
                                   (String)game["platformId"],
                                   (String)((Dictionary<String, Object>)game["observers"])["encryptionKeyByteList"]);
            }
        }
    }
}
