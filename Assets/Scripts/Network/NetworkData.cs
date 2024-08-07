using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Netcode;
using UnityEngine.Rendering.Universal;

namespace Marsion
{
    public class SerializedData
    {
        private FastBufferReader reader;
        private INetworkSerializable data;
        private byte[] bytes;

        public SerializedData(FastBufferReader r) { reader = r; data = null; }
        public SerializedData(INetworkSerializable d) { data = d; }

        public string GetString()
        {
            reader.ReadValueSafe(out string msg);
            return msg;
        }

        public T Get<T>() where T : INetworkSerializable, new()
        {
            if(data != null)
            {
                return (T)data;
            }
            else if(bytes != null)
            {
                data = NetworkTool.NetDeserialize<T>(bytes);
                return (T)data;
            }
            else
            {
                reader.ReadNetworkSerializable(out T val);
                data = val;
                return val;

            }
        }

        public void PreRead()
        {
            int size = reader.Length - reader.Position;
            bytes = new byte[size];
            reader.ReadBytesSafe(ref bytes, size);
        }
    }

    public class NetworkGameData : INetworkSerializable
    {
        public GameData gameData;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if(serializer.IsReader)
            {
                int size = 0;
                serializer.SerializeValue(ref size);
                if(size > 0)
                {
                    byte[] bytes = new byte[size];
                    serializer.SerializeValue(ref bytes);
                    gameData = NetworkTool.Deserialize<GameData>(bytes);
                }
            }

            if(serializer.IsWriter)
            {
                byte[] bytes = NetworkTool.Serialize(gameData);
                int size = bytes.Length;
                serializer.SerializeValue(ref size);
                if (size > 0)
                    serializer.SerializeValue(ref bytes);
            }
        }
    }
}