using System.Collections.Generic;
using Unity.Netcode;

namespace Marsion
{
    public class SerializedData
    {
        private FastBufferReader reader;
        private INetworkSerializable data;
        private byte[] bytes;

        public SerializedData(FastBufferReader r) { reader = r; data = null; }
        public SerializedData(INetworkSerializable d) { data = d; }

        public int GetInt()
        {
            reader.ReadValueSafe(out int value);
            return value;
        }

        public string GetString()
        {
            reader.ReadValueSafe(out string value);
            return value;
        }

        public T Get<T>() where T : INetworkSerializable, new()
        {
            reader.ReadNetworkSerializable(out T value);
            return value;
        }
    }

    public class SerializedUlong : INetworkSerializable
    {
        public ulong value;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref value);
        }
    }

    public class SerializedString : INetworkSerializable
    {
        public string value;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref value);
        }
    }

    public class SerializedGameData : INetworkSerializable
    {
        public IGameData GameData;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            // 역직렬화
            if (serializer.IsReader)
            {
                // 먼저 크기를 읽습니다.
                int size = 0;
                serializer.SerializeValue(ref size);

                // 크기가 0보다 큰 경우 바이트 배열을 읽습니다.
                if (size > 0)
                {
                    byte[] bytes = new byte[size];
                    serializer.SerializeValue(ref bytes);
                    GameData = NetworkTool.Deserialize<DefaultGameData>(bytes);
                }
            }

            // 직렬화
            if (serializer.IsWriter)
            {
                byte[] bytes = NetworkTool.Serialize(GameData);
                int size = bytes.Length;

                // 크기를 먼저 직렬화합니다.
                serializer.SerializeValue(ref size);

                // 크기가 0보다 큰 경우 바이트 배열을 직렬화합니다.
                if (size > 0)
                {
                    serializer.SerializeValue(ref bytes);
                }
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

    public class SerializedDrawnCardData : INetworkSerializable
    {
        public ulong PlayerID;
        public string CardUID;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref PlayerID);
            serializer.SerializeValue(ref CardUID);
        }
    }

    public class SerializedTrySpawnCardData : INetworkSerializable
    {
        public string CardUID;
        public int Index;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref CardUID);
            serializer.SerializeValue(ref Index);
        }
    }

    public class SerializedPlayCardResultData : INetworkSerializable
    {
        public bool Succeeded;
        public ulong PlayerID;
        public string CardUID;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Succeeded);
            serializer.SerializeValue(ref PlayerID);
            serializer.SerializeValue(ref CardUID);
        }
    }

    public class SerializedSpawnCardResultData : INetworkSerializable
    {
        public bool Succeeded;
        public ulong PlayerID;
        public string CardUID;
        public int Index;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Succeeded);
            serializer.SerializeValue(ref PlayerID);
            serializer.SerializeValue(ref CardUID);
            serializer.SerializeValue(ref Index);
        }
    }

    public class SerializedTryAttackData : INetworkSerializable
    {
        public ulong AttackPlayerID;
        public string AttackerUID;
        public ulong DefendPlayerID;
        public string DefenderUID;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref AttackPlayerID);
            serializer.SerializeValue(ref AttackerUID);
            serializer.SerializeValue(ref DefendPlayerID);
            serializer.SerializeValue(ref DefenderUID);
        }
    }

    public class SerializedAttackCardResultData : INetworkSerializable
    {
        public bool Succeeded;
        public ulong AttackPlayerID;
        public string AttackerUID;
        public ulong DefendPlayerID;
        public string DefenderUID;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Succeeded);
            serializer.SerializeValue(ref AttackPlayerID);
            serializer.SerializeValue(ref AttackerUID);
            serializer.SerializeValue(ref DefendPlayerID);
            serializer.SerializeValue(ref DefenderUID);
        }
    }

    public class SerializedDeadCardsData : INetworkSerializable
    {
        public List<string> DeadCards = new();

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            int count = DeadCards.Count;
            serializer.SerializeValue(ref count);

            if (serializer.IsReader)
            {
                DeadCards = new List<string>(count);
                for (int i = 0; i < count; i++)
                {
                    string value = "";
                    serializer.SerializeValue(ref value);
                    DeadCards.Add(value);
                }
            }
            else
            {
                foreach (var value in DeadCards)
                {
                    string item = value;
                    serializer.SerializeValue(ref item);
                }
            }
        }
    }

    public class SerializedDraftState : INetworkSerializable
    {
        public bool isComplete;
        public int count;
        public string portraitID;
        public string[] deck;
        public string[] selections;
        public string[] subSelections;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref isComplete);
            serializer.SerializeValue(ref count);
            serializer.SerializeValue(ref portraitID);
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

    public class StringContainer : INetworkSerializable
    {
        public string SomeText;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsWriter)
            {
                serializer.GetFastBufferWriter().WriteValueSafe(SomeText);
            }
            else
            {
                serializer.GetFastBufferReader().ReadValueSafe(out SomeText);
            }
        }
    }
}