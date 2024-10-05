using System;
using System.Collections;
using System.Collections.Generic;

namespace Marsion
{
    public class DataManager
    {
        // 각 타입별로 리스트와 딕셔너리를 관리하기 위해 Dictionary 사용
        private Dictionary<Type, IList> dataLists;
        private Dictionary<Type, IDictionary> dataDictionaries;

        public List<CardSO> CardList { get { return GetList<CardSO>(); } }
        public Dictionary<string, CardSO> CardDictionary { get { return GetDictionary<CardSO>(); } }

        // 데이터 초기화 메서드
        public void Init()
        {
            dataLists = new Dictionary<Type, IList>();
            dataDictionaries = new Dictionary<Type, IDictionary>();

            Load<CardSO>("CardSO");
        }

        // 제네릭 Load 메서드 (Object 타입을 상속하는 경우에 대응)
        public void Load<T>(string path = "") where T : UnityEngine.Object, IIdentifiable
        {
            // 해당 타입에 대한 리스트 및 딕셔너리 확보
            if (!dataLists.ContainsKey(typeof(T)))
            {
                dataLists[typeof(T)] = new List<T>();
                dataDictionaries[typeof(T)] = new Dictionary<string, T>();
            }

            var list = (List<T>)dataLists[typeof(T)];
            var dictionary = (Dictionary<string, T>)dataDictionaries[typeof(T)];

            // 리소스 로드 및 리스트에 추가
            T[] loadedItems = Managers.Resource.LoadAll<T>(path);
            list.AddRange(loadedItems);

            // ID를 키로 딕셔너리에 추가
            foreach (var item in list)
            {
                dictionary.Add(item.ID, item); // T 타입이 IIdentifiable 인터페이스를 구현한다고 가정
            }
        }

        // 특정 타입의 리스트를 가져오는 메서드
        public List<T> GetList<T>() where T : UnityEngine.Object
        {
            if (dataLists.TryGetValue(typeof(T), out IList list))
            {
                return (List<T>)list;
            }
            return null;
        }

        // 특정 타입의 딕셔너리를 가져오는 메서드
        public Dictionary<string, T> GetDictionary<T>() where T : UnityEngine.Object
        {
            if (dataDictionaries.TryGetValue(typeof(T), out IDictionary dictionary))
            {
                return (Dictionary<string, T>)dictionary;
            }
            return null;
        }
    }

    // ID를 기준으로 Dictionary에 추가하기 위해 IIdentifiable 인터페이스 정의
    public interface IIdentifiable
    {
        string ID { get; }
    }
}