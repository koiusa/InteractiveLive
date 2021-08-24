using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Koiusa.InteractiveRoom
{
    [RequireComponent(typeof(ObjectLoader))]
    public class LiveChatEmitter : MonoBehaviour
    {
        private ObjectLoader _objectLoader;
        private IDictionary<string, Object> _prefabs = null;

        private Canvas canvas;
        private void Awake()
        {
            _prefabs = new Dictionary<string, Object>();
            _objectLoader = gameObject.GetComponent<ObjectLoader>();
            _objectLoader.OnLoaded += GetPrefab;
            canvas = FindObjectsOfType<Canvas>()[0];
        }

        private void GetPrefab(IList<Object> objects)
        {
            foreach (var prefab in objects)
            {
                _prefabs.Add(prefab.name, prefab);
            }
        }

        public void SpawnGameObject(GameObject obj, string liveChatComment, string authorUrl)
        {
            if (_objectLoader.IsLoaded)
            {
                if (obj != null)
                {
                    var speechbubbles = (GameObject)Instantiate(_prefabs.Values.First(), Vector3.zero, Quaternion.identity);
                    var uifollow = speechbubbles.GetComponent<UIFollowTarget>();
                    uifollow.target = obj.transform;
                    uifollow.canvas = canvas;
                    speechbubbles.transform.parent = canvas.transform;
                    speechbubbles.transform.localScale = Vector3.one;
                    speechbubbles.transform.rotation = Quaternion.identity;
                    speechbubbles.transform.Find("Frame/Strings").GetComponent<Text>().text = liveChatComment;
                    if (!string.IsNullOrEmpty(authorUrl))
                    {
                        var icon = speechbubbles.transform.Find("Icon").gameObject.AddComponent<ChannelIcon>();
                        icon.CreateThumbnail(authorUrl);
                    }
                }
            }
        }

        public void DestoroyGameObject()
        {
            if (canvas.transform.hasChanged) {
                foreach (Transform uifollow in canvas.transform)
                {
                    Destroy(uifollow.gameObject);
                }
            }
        }
    }
}
