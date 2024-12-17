using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Marsion
{
    public class DataManager : IDataManager
    {
        private readonly IResourceManager _resourceManager;
        private readonly Dictionary<Type, IDictionary> _dataDictionaries = new();

        public DataManager(IResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
        }

        // 데이터 초기화 메서드
        public void Init()
        {
            Logger.Log<DataManager>("Data initialized", colorName: ColorCodes.CommonManager);

            Load<CardSO>("CardSO");
            Load<PortraitSO>("PortraitSO");
        }

        // 제네릭 Load 메서드 (Object 타입을 상속하는 경우에 대응)
        public void Load<T>(string path = "") where T : UnityEngine.Object, IIdentifiable
        {
            if(string.IsNullOrEmpty(path))
            {
                Logger.LogError<DataManager>($"Path is null or empty for {typeof(T).Name}", colorName: ColorCodes.CommonManager);
                return;
            }

            EnsureTypeRegistered<T>();

            var dictionary = (Dictionary<string, T>)_dataDictionaries[typeof(T)];

            T[] assets = _resourceManager.LoadAll<T>(path);

            foreach(var asset in assets)
            {
                if(!dictionary.ContainsKey(asset.ID))
                {
                    dictionary[asset.ID] = asset;
                }
            }
        }

        public IEnumerator LoadFromAddressables<T>(string label) where T : UnityEngine.Object, IIdentifiable
        {
            if (string.IsNullOrEmpty(label))
            {
                Logger.LogError<DataManager>($"Label is null or empty for {typeof(T).Name}", colorName: ColorCodes.CommonManager);
                yield break;
            }

            EnsureTypeRegistered<T>();
        }

        // 특정 타입의 딕셔너리를 가져오는 메서드
        public Dictionary<string, T> GetDictionary<T>() where T : UnityEngine.Object, IIdentifiable
        {
            if (_dataDictionaries.TryGetValue(typeof(T), out var dictionary))
            {
                return dictionary as Dictionary<string, T>;
            }
            return null;
        }

        private void EnsureTypeRegistered<T>() where T : UnityEngine.Object, IIdentifiable
        {
            if(!_dataDictionaries.ContainsKey(typeof(T)))
            {
                Logger.Log<DataManager>($"Registering type : {typeof(T).Name}", colorName: ColorCodes.CommonManager);
                _dataDictionaries[typeof(T)] = new Dictionary<string, T>();
            }
        }
    }
}