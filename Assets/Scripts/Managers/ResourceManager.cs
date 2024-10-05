using UnityEngine;

namespace Marsion
{
    public class ResourceManager
    {
        public T Load<T>(string path) where T : Object
        {
            return Resources.Load<T>(path);
        }

        public T[] LoadAll<T>(string path) where T : Object
        {
            return Resources.LoadAll<T>(path);
        }

        public GameObject Instantiate(string path, Transform parent = null)
        {
            GameObject original = Load<GameObject>($"{path}");

            if(original == null)
            {
                Managers.Logger.Log<ResourceManager>($"Failed to load prefab : {path}");
                return null;
            }

            GameObject go = Object.Instantiate(original, parent);

            int index = go.name.IndexOf("(Clone)");

            if (index > 0)
                go.name = go.name.Substring(0, index);

            return go;
        }

        public T Instantiate<T>(string path, Transform parent = null)
        {
            GameObject original = Load<GameObject>($"{path}");

            if (original == null)
            {
                Managers.Logger.Log<ResourceManager>($"Failed to load prefab : {path}");
                return default;
            }

            GameObject go = Object.Instantiate(original, parent);

            int index = go.name.IndexOf("(Clone)");

            if (index > 0)
                go.name = go.name.Substring(0, index);

            return go.GetComponent<T>();
        }

        public void Destroy(GameObject go)
        {
            if (go == null) return;

            Object.Destroy(go);
        }
    }
}