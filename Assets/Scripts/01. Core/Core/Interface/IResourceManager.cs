using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Marsion
{
    public interface IResourceManager
    {
        T Load<T>(string path) where T : Object;
        T[] LoadAll<T>(string path) where T : Object;
        IEnumerator LoadAddressableAssets<T>(string label, System.Action<IList<T>> onLoaded) where T : Object;
        GameObject Instantiate(string path, Transform parent = null);
        T Instantiate<T>(string path, Transform parent = null);
        void Destroy(GameObject go);
    }
}