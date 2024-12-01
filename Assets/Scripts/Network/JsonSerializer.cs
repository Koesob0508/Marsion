using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Marsion
{
    /// <summary>
    ///     큰 데이터는 JSON을 거치도록 지원하는 클래스
    /// </summary>
    public class JsonSerializer
    {
        public static byte[] SerializeBytes<T>(T obj) where T : class
        {
            try
            {
                string json = Serialize(obj);
                byte[] bytes = Encoding.UTF8.GetBytes(json);
                return bytes;
            }
            catch (Exception e)
            {
                Logger.LogError<JsonSerializer>("Serialization error: " + e.Message);
                return new byte[0];
            }
        }

        public static T DeserializeBytes<T>(byte[] bytes) where T : class
        {
            try
            {
                string json = Encoding.UTF8.GetString(bytes);
                T obj = Deserialize<T>(json);
                return obj;
            }
            catch (Exception e)
            {
                Logger.LogError<JsonSerializer>("Deserialization error: " + e.Message);
                return null;
            }
        }

        public static string Serialize<T>(T obj) where T : class
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                Converters = new[] { new ScriptableObjectConverter() }, // ScriptableObject 처리
                Formatting = Formatting.Indented
            };

            string json = JsonConvert.SerializeObject(obj);

            return json;
        }

        public static T Deserialize<T>(string json)
        {
            try
            {
                // JsonSerializerSettings에 ScriptableObjectConverter를 추가하여 역직렬화 시 ScriptableObject 처리
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    Converters = new[] { new ScriptableObjectConverter() } // ScriptableObject 처리
                };

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
    }

    public class ScriptableObjectConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(ScriptableObject).IsAssignableFrom(objectType);
        }

        // 직렬화 시 ScriptableObject 처리
        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (value is ScriptableObject scriptableObject)
            {
                JObject jsonObject = new JObject();
                foreach (var field in scriptableObject.GetType().GetFields())
                {
                    jsonObject[field.Name] = JToken.FromObject(field.GetValue(scriptableObject), serializer);
                }
                jsonObject.WriteTo(writer);
            }
            else
            {
                serializer.Serialize(writer, value);
            }
        }

        // 역직렬화 시 ScriptableObject 처리
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (typeof(ScriptableObject).IsAssignableFrom(objectType))
            {
                var jsonObject = JObject.Load(reader);
                var instance = ScriptableObject.CreateInstance(objectType);
                serializer.Populate(jsonObject.CreateReader(), instance);
                return instance;
            }

            return serializer.Deserialize(reader, objectType);
        }
    }
}