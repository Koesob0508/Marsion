using System.Collections.Generic;
using UnityEngine;

namespace Marsion.Tool
{
    [System.Serializable]
    public class MyDictionary<K, V>
    {
        [SerializeField]
        private List<K> keys = new List<K>();

        [SerializeField]
        private List<V> values = new List<V>();

        // Dictionary에 새로운 키-값 쌍을 추가합니다.
        public void Add(K key, V value)
        {
            keys.Add(key);
            values.Add(value);
        }

        // 키에 해당하는 값을 Dictionary에서 제거합니다.
        public bool Remove(K key)
        {
            int index = keys.IndexOf(key);
            if (index >= 0)
            {
                keys.RemoveAt(index);
                values.RemoveAt(index);
                return true;
            }
            return false; // 키를 찾지 못한 경우
        }

        // 현재 리스트를 Dictionary로 변환하여 반환합니다.
        public Dictionary<K, V> ToDictionary()
        {
            Dictionary<K, V> dict = new Dictionary<K, V>();
            for (int i = 0; i < keys.Count; i++)
            {
                dict[keys[i]] = values[i];
            }
            return dict;
        }

        // 키에 해당하는 값이 있는지 확인하고, 값을 반환합니다.
        public bool TryGetValue(K key, out V value)
        {
            int index = keys.IndexOf(key);
            if (index >= 0)
            {
                value = values[index];
                return true;
            }
            value = default(V);
            return false;
        }

        // Dictionary를 비웁니다.
        public void Clear()
        {
            keys.Clear();
            values.Clear();
        }
    }
}