using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Koiusa.InteractiveRoom
{
    /// <summary>
    /// Load‚µ‚½Prefab‚ðInstantiate‚·‚é
    /// </summary>
    [RequireComponent(typeof(ObjectLoader))]
    public class ObjectEmitter : MonoBehaviour
    {
        [SerializeField]
        private int _limitCount = 10;

        private ObjectLoader _objectLoader;

        private IDictionary<string, Object> _prefabs = null;
        private IList<GameObject> _gameObjects = null;
        public GameObject GetRandamTarget()
        {
            if (_gameObjects.Count > 0)
            {
                var r = new System.Random();
                return _gameObjects[r.Next(0, _gameObjects.Count)];
            }
            else
            {
                return null;
            }
        }

        private void Awake()
        {
            _gameObjects = new List<GameObject>();
            _prefabs = new Dictionary<string, Object>();
            _objectLoader = gameObject.GetComponent<ObjectLoader>();
            _objectLoader.OnLoaded += GetPrefab;
        }

        private void GetPrefab(IList<Object> objects)
        {
            foreach (var prefab in objects)
            {
                _prefabs.Add(prefab.name, prefab);
            }
        }

        private void ClampGameObject(int limit)
        {
            while (_gameObjects.Count > limit)
            {
                Destroy(_gameObjects[0]);
                _gameObjects.RemoveAt(0);
            }
        }


        public void SpawnGameObject(PrefabContext context)
        {
            if (_objectLoader.IsLoaded)
            {
                _gameObjects.Add((GameObject)Instantiate(_prefabs[context.key], context.position, context.rotate));
                ClampGameObject(_limitCount);
            }
        }

        public void DestroyGameObject()
        {
            ClampGameObject(0);
        }

        public string GetRondomName()
        {
            return _objectLoader.Objects[GetPrefabIndex()].name;
        }

        public IList<Object> GamePrefabs
        {
            get { return _objectLoader.Objects; }
        }
        public GameObject GetSpoawnObject
        {
            get { return _gameObjects.Last(); }
        }


        private int GetPrefabIndex()
        {
            return (int)Mathf.Floor(Random.Range(0, _objectLoader.Objects.Count));
        }
    }
}
