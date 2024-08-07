using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.Collections;
using Unity.Netcode;

namespace Marsion
{
    public class NetworkTool
    {
        public static byte[] Serialize<T>(T obj) where T : class
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();
                bf.Serialize(ms, obj);
                byte[] bytes = ms.ToArray();
                ms.Close();
                return bytes;
            }
            catch (Exception e)
            {
                Managers.Logger.LogError<NetworkTool>("Serialization error : " + e.Message);
                return new byte[0];
            }
        }

        public static T Deserialize<T>(byte[] bytes) where T : class
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();
                ms.Write(bytes, 0, bytes.Length);
                ms.Seek(0, SeekOrigin.Begin);
                T obj = (T)bf.Deserialize(ms);
                ms.Close();
                return obj;
            }
            catch (Exception e)
            {
                Managers.Logger.LogError<NetworkTool>("Deserialization error : " + e.Message);
                return null;
            }
        }

        public static T NetDeserialize<T>(byte[] bytes) where T : INetworkSerializable, new()
        {
            if (bytes == null || bytes.Length == 0)
                return default;

            try
            {
                FastBufferReader reader = new FastBufferReader(bytes, Allocator.Temp);
                reader.ReadNetworkSerializable(out T obj);
                reader.Dispose();
                return obj;
            }
            catch (Exception e)
            {
                Managers.Logger.LogError<NetworkTool>("Deserialization error : " + e.Message);
                return default;
            }
        }
    }
}