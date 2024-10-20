using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Newtonsoft.Json;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Marsion
{
    public class NetworkTool
    {
        public static byte[] Serialize<T>(T obj) where T : class
        {
            try
            {
                string json = JsonSerialize(obj);
                byte[] bytes = Encoding.UTF8.GetBytes(json);
                return bytes;
            }
            catch (Exception e)
            {
                Managers.Logger.LogError<NetworkTool>("Serialization error: " + e.Message);
                return new byte[0];
            }
        }

        public static T Deserialize<T>(byte[] bytes) where T : class
        {
            try
            {
                string json = Encoding.UTF8.GetString(bytes);
                T obj = JsonDeserialize<T>(json);
                return obj;
            }
            catch (Exception e)
            {
                Managers.Logger.LogError<NetworkTool>("Deserialization error: " + e.Message);
                return null;
            }
        }

        public static string JsonSerialize<T>(T obj) where T : class
        {
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            Managers.Logger.Log<NetworkTool>(json, colorName: ColorCodes.Logic);

            return json;
        }

        public static T JsonDeserialize<T>(string json)
        {
            try
            {
                Managers.Logger.Log<NetworkTool>(json, colorName: ColorCodes.Logic);
                // JSON 문자열을 T 타입 객체로 역직렬화
                T obj = JsonConvert.DeserializeObject<T>(json);

                return obj;
            }
            catch (JsonException ex)
            {
                // JSON 파싱 실패 시 예외 처리 및 로그 출력
                Debug.LogError($"JSON Deserialization failed: {ex.Message}");
                return default(T); // 오류 발생 시 null 반환 (참조 타입인 경우)
            }
        }

        public static byte[] NetworkSerialize<T>(T obj, int size = 128) where T : INetworkSerializable
        {
            if (obj == null)
                return new byte[0];

            try
            {
                FastBufferWriter writer = new FastBufferWriter(size, Allocator.Temp, MarsNetwork.MessageSizeMax);
                writer.WriteNetworkSerializable(obj);
                byte[] bytes = writer.ToArray();
                writer.Dispose();
                return bytes;
            }
            catch(Exception e)
            {
                Managers.Logger.LogError<NetworkTool>("Serialization error : " + e.Message);
                return new byte[0];
            }
        }

        public static T NetworkDeserialize<T>(byte[] bytes) where T : INetworkSerializable, new()
        {
            if (bytes == null || bytes.Length == 0)
            {
                Managers.Logger.LogWarning<NetworkTool>("Attempted to deserialize from null or empty byte array.");
                return default;
            }

            try
            {
                using (FastBufferReader reader = new FastBufferReader(bytes, Allocator.Temp))
                {
                    reader.ReadNetworkSerializable(out T obj);
                    reader.Dispose();
                    return obj;
                }
            }
            catch (Exception e)
            {
                Managers.Logger.LogError<NetworkTool>("Deserialization error : " + e.Message);
                return default;
            }
        }
    }
}