using System.Collections.Generic;
using UnityEngine;

namespace Marsion.Tool
{
    public class MyDictionary<K, V>
    {
        public List<K> Keys = new List<K>();

        public List<V> Values = new List<V>();

        public int Count => Keys.Count;

        // Dictionary에 새로운 키-값 쌍을 추가합니다.
        public void Add(K key, V value)
        {
            Keys.Add(key);
            Values.Add(value);
        }

        // 키에 해당하는 값을 Dictionary에서 제거합니다.
        public bool Remove(K key)
        {
            int index = Keys.IndexOf(key);
            if (index >= 0)
            {
                Keys.RemoveAt(index);
                Values.RemoveAt(index);
                return true;
            }
            return false; // 키를 찾지 못한 경우
        }

        // 현재 리스트를 Dictionary로 변환하여 반환합니다.
        public Dictionary<K, V> ToDictionary()
        {
            Dictionary<K, V> dict = new Dictionary<K, V>();
            for (int i = 0; i < Keys.Count; i++)
            {
                dict[Keys[i]] = Values[i];
            }
            return dict;
        }

        // 키에 해당하는 값이 있는지 확인하고, 값을 반환합니다.
        public bool TryGetValue(K key, out V value)
        {
            int index = Keys.IndexOf(key);
            if (index >= 0)
            {
                value = Values[index];
                return true;
            }
            value = default(V);
            return false;
        }

        // Dictionary를 비웁니다.
        public void Clear()
        {
            Keys.Clear();
            Values.Clear();
        }
    }
}