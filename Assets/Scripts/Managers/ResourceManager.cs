using UnityEngine;

namespace Marsion
{
    public class ResourceManager : IResourceManager
    {
        private readonly IResourceLoader _loader;

        public ResourceManager(IResourceLoader loader)
        {
            _loader = loader;
        }

        public T Load<T>(string path) where T : Object
        {
            return _loader.Load<T>(path);
        }

        public T[] LoadAll<T>(string path) where T : Object
        {
            return _loader.LoadAll<T>(path);
        }

        public GameObject Instantiate(string path, Transform parent = null)
        {
            GameObject original = Load<GameObject>($"{path}");

            if (original == null)
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