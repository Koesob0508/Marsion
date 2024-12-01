using UnityEngine;

namespace Marsion
{
    public interface IResourceLoader
    {
        T Load<T>(string path) where T : Object;
        T[] LoadAll<T>(string path) where T : Object;
    }
}