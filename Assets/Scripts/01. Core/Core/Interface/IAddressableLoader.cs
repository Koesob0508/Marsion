using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Marsion
{
    public interface IAddressableLoader
    {
        IEnumerator LoadAssetsAsync<T>(string label, Action<IList<T>> onLoaded) where T : UnityEngine.Object;
    }

    public class DefaultAddressableLoader : IAddressableLoader
    {
        public IEnumerator LoadAssetsAsync<T>(string label, Action<IList<T>> onLoaded) where T : UnityEngine.Object
        {
            AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(label, null);
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                onLoaded?.Invoke(handle.Result);
            }
            else
            {
                Logger.LogError<DefaultAddressableLoader>($"Failed to load Addressables with label: {label}");
            }
        }
    }
}