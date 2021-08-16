using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
namespace Koiusa.InteractiveRoom
{
    /// <summary>
    /// AssetLabel‚ÅPrefab‚ðLoad‚·‚é
    /// </summary>
    public class ObjectLoader : MonoBehaviour
    {
        [SerializeField]
        private AssetLabelReference _labelReference = null;

        private IList<AsyncOperationHandle<IList<Object>>> _handles;
        private IList<Object> _objects;
        private bool _isLoaded = false;

        public delegate void OnCompleteDelegate(IList<Object> objects);
        public OnCompleteDelegate OnLoaded;

        private void Awake()
        {
            _handles = new List<AsyncOperationHandle<IList<Object>>>();
            _objects = new List<Object>();
            LoadAssets(_labelReference);
        }

        private void OnDestroy()
        {
            foreach (var handle in _handles)
            {
                Addressables.ReleaseInstance(handle);
            }
        }

        private async void LoadAssets(AssetLabelReference label)
        {
            var handle = Addressables.LoadAssetsAsync<Object>(label, OnLoadPrefab);
            await handle.Task;
            Debug.Log("OnLoaded");
            _handles.Add(handle);
            _isLoaded = true;
            OnLoaded?.Invoke(_objects);
        }

        private void OnLoadPrefab(Object prefab)
        {
            _objects.Add(prefab);
            Debug.Log("OnLoadPrefaab:" + prefab.name);
        }

        public bool IsLoaded
        {
            get{ return _isLoaded; }
        }

        public IList<Object> Objects
        {
            get { return _objects; }
        }
    }
}
