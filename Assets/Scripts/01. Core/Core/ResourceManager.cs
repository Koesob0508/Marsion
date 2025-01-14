using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Marsion
{
    public class ResourceManager : IResourceManager
    {
        private readonly IResourceLoader _resourceLoader;
        private readonly IAddressableLoader _addresableLoader;

        private readonly Dictionary<System.Type, IDictionary> _resourceDictionaries = new();

        public ResourceManager(IResourceLoader resourceLoader, IAddressableLoader addressableLoader)
        {
            _resourceLoader = resourceLoader;
            _addresableLoader = addressableLoader;
        }

        public void Init()
        {
            Logger.Log<ResourceManager>("Resource manager initialized", colorName: ColorCodes.CommonManager);

            LoadSO<CardSO>("CardSO");
            LoadSO<PortraitSO>("PortraitSO");
        }

        public void LoadSO<T>(string path = "") where T : Object, IIdentifiable
        {
            if(string.IsNullOrEmpty(path))
            {
                Logger.LogError<ResourceManager>($"Path is null or empty for {typeof(T).Name}", colorName: ColorCodes.CommonManager);
                return;
            }

            EnsureTypeRegistered<T>();

            var dictionary = (Dictionary<string, T>)_resourceDictionaries[typeof(T)];

            T[] assets = LoadAll<T>(path);

            foreach (var asset in assets)
            {
                if (!dictionary.ContainsKey(asset.ID))
                {
                    dictionary[asset.ID] = asset;
                }
            }
        }

        public IDictionary<string, T> GetDictionary<T>() where T : Object, IIdentifiable
        {
            if (_resourceDictionaries.TryGetValue(typeof(T), out var dictionary))
            {
                return dictionary as Dictionary<string, T>;
            }
            return null;
        }


        private void EnsureTypeRegistered<T>() where T : UnityEngine.Object, IIdentifiable
        {
            if (!_resourceDictionaries.ContainsKey(typeof(T)))
            {
                Logger.Log<ResourceManager>($"Registering type : {typeof(T).Name}", colorName: ColorCodes.CommonManager);
                _resourceDictionaries[typeof(T)] = new Dictionary<string, T>();
            }
        }

        public T Load<T>(string path) where T : Object => _resourceLoader.Load<T>(path);
        public T[] LoadAll<T>(string path) where T : Object => _resourceLoader.LoadAll<T>(path);
        public IEnumerator LoadAddressableAssets<T>(string label, System.Action<IList<T>> onLoaded) where T : Object
        => _addresableLoader.LoadAssetsAsync(label, onLoaded);

        public GameObject Instantiate(string path, Transform parent = null)
        {
            GameObject original = Load<GameObject>($"{path}");

            if(original == null)
            {
                LogLoadFailure(path);
                return null;
            }

            return InstantiateInternal(original, parent);
        }

        public T Instantiate<T>(string path, Transform parent = null)
        {
            GameObject original = Load<GameObject>($"{path}");

            if (original == null)
            {
                LogLoadFailure(path);
                return default;
            }

            GameObject instance = InstantiateInternal(original, parent);
            return instance.GetComponent<T>();
        }

        public void Destroy(GameObject go)
        {
            if (go == null) return;
            Object.Destroy(go);
        }

        // private method
        private GameObject InstantiateInternal(GameObject original, Transform parent)
        {
            GameObject instance = Object.Instantiate(original, parent);

            int index = instance.name.IndexOf("(Clone)");
            if (index > 0)
                instance.name = instance.name.Substring(0, index);

            return instance;
        }

        private void LogLoadFailure(string path)
        {
            Logger.Log<ResourceManager>($"Failed to load prefab : {path}");
        }
    }
}