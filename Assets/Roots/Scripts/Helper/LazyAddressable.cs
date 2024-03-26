#if OFF
#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Cysharp.Threading.Tasks
{
    public static class LazyAddressable
    {
        #region addressable

        public static UniTask<T> LoadAddressable<T>(
            object key)
            where T : class
        {
            var completionSource = new UniTaskCompletionSource<T>();
            Addressables.LoadAssetAsync<T>(key)
                .Completed += x => completionSource.TrySetResult(x.Result);
            return completionSource.Task;
        }

        public static UniTask<IList<T>> LoadAddressables<T>(
            object key,
            Action<T> callback)
            where T : class
        {
            var completionSource = new UniTaskCompletionSource<IList<T>>();
            Addressables.LoadAssetsAsync(key, callback)
                .Completed += x => completionSource.TrySetResult(x.Result);
            return completionSource.Task;
        }

        public static UniTask<T> LoadAddressable<T>(
            this IResourceLocation location)
        {
            var completionSource = new UniTaskCompletionSource<T>();
            Addressables.LoadAssetAsync<T>(location)
                .Completed += x => completionSource.TrySetResult(x.Result);
            return completionSource.Task;
        }

        public static UniTask<T> LoadAddressable<T>(
            this AssetReference assetReference)
            where T : class
        {
            var completionSource = new UniTaskCompletionSource<T>();
            assetReference.LoadAssetAsync<T>()
                .Completed += x => completionSource.TrySetResult(x.Result);
            return completionSource.Task;
        }

        public static void LoadAddressable<T>(
            this AssetReference assetReference,
            Action<T> callback)
            where T : class
        {
            assetReference.LoadAssetAsync<T>()
                .Completed += x => callback?.Invoke(x.Result);
        }

        public static UniTask<SceneInstance> LoadSceneAddressable(
            this AssetReference assetReference)
        {
            var completionSource = new UniTaskCompletionSource<SceneInstance>();
            assetReference.LoadSceneAsync()
                .Completed += x => completionSource.TrySetResult(x.Result);
            return completionSource.Task;
        }

        public static UniTask<SceneInstance> LoadSceneAddressable(
            object key,
            LoadSceneMode mode = LoadSceneMode.Single,
            bool activateOnLoad = true)
        {
            var completionSource = new UniTaskCompletionSource<SceneInstance>();
            Addressables.LoadSceneAsync(key, mode, activateOnLoad)
                .Completed += x => completionSource.TrySetResult(x.Result);
            return completionSource.Task;
        }

        /// <summary>
        /// Use ReleaseInstance to release resources
        /// </summary>
        /// <param name="assetReference"></param>
        /// <param name="parent"></param>
        /// <param name="instantiateInWorldSpace"></param>
        /// <returns></returns>
        public static UniTask<GameObject> InstantiateAddressable(
            this AssetReference assetReference,
            Transform parent,
            bool instantiateInWorldSpace = false)
        {
            var completionSource = new UniTaskCompletionSource<GameObject>();
            assetReference.InstantiateAsync(parent, instantiateInWorldSpace)
                .Completed += x => completionSource.TrySetResult(x.Result);
            return completionSource.Task;
        }

        /// <summary>
        /// Use ReleaseInstance to release resources
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parent"></param>
        /// <param name="instantiateInWorldSpace"></param>
        /// <returns></returns>
        public static UniTask<GameObject> InstantiateAddressable(
            string key,
            Transform parent = null,
            bool instantiateInWorldSpace = false)
        {
            var completionSource = new UniTaskCompletionSource<GameObject>();
            Addressables.InstantiateAsync(key, parent, instantiateInWorldSpace)
                .Completed += x => completionSource.TrySetResult(x.Result);
            return completionSource.Task;
        }

        public static UniTask<GameObject> InstantiateAddressable(
            this IResourceLocation location,
            Transform parent = null,
            bool instantiateInWorldSpace = false)
        {
            var completionSource = new UniTaskCompletionSource<GameObject>();
            Addressables.InstantiateAsync(location, parent, instantiateInWorldSpace)
                .Completed += x => completionSource.TrySetResult(x.Result);
            return completionSource.Task;
        }

        public static UniTask<IList<IResourceLocation>> LoadResourceLocationAddressables(
            object key,
            Type type = null)
        {
            var completionSource = new UniTaskCompletionSource<IList<IResourceLocation>>();
            Addressables.LoadResourceLocationsAsync(key, type)
                .Completed += x => completionSource.TrySetResult(x.Result);
            return completionSource.Task;
        }

        public static bool IsNullOrEmpty(
            this AssetReference assetReference)
        {
            return assetReference == null || !assetReference.RuntimeKeyIsValid();
        }

        #endregion
    }
}

#endif
#endif