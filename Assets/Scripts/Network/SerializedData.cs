using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Netcode;
using UnityEngine.Rendering.Universal;

namespace Marsion.Logic
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

    public class SerializedGameData : INetworkSerializable
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

    public class SerializedCardData : INetworkSerializable
    {
        public string UID;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref UID);
        }
    }

    public class SerializedDeckBuildState : INetworkSerializable
    {
        public string[] deck;
        public string[] selections;
        public string[] subSelections;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            // Serialize deck array
            SerializeStringArray(ref deck, serializer);
            // Serialize selections array
            SerializeStringArray(ref selections, serializer);
            // Serialize subSelections array
            SerializeStringArray(ref subSelections, serializer);
        }

        private void SerializeStringArray<T>(ref string[] array, BufferSerializer<T> serializer) where T : IReaderWriter
        {
            // If serializing, write the length of the array
            if (serializer.IsWriter)
            {
                int length = array != null ? array.Length : 0;
                serializer.SerializeValue(ref length);

                // Serialize each element of the array
                for (int i = 0; i < length; i++)
                {
                    string element = array[i];
                    serializer.SerializeValue(ref element);
                }
            }
            else // If deserializing, read the length and allocate array
            {
                int length = 0;
                serializer.SerializeValue(ref length);

                array = new string[length];

                // Deserialize each element of the array
                for (int i = 0; i < length; i++)
                {
                    string element = null;
                    serializer.SerializeValue(ref element);
                    array[i] = element;
                }
            }
        }
    }
}