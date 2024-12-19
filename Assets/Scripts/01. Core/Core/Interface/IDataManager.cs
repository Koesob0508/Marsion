using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Marsion
{
    public interface IIdentifiable
    {
        string ID { get; }
    }

    public interface IDataManager
    {
        void Init();
        IEnumerator LoadFromAddressables<T>(string label) where T : Object, IIdentifiable;
        Dictionary<string, T> GetDictionary<T>() where T : Object, IIdentifiable;
    }
}