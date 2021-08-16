using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Koiusa.InteractiveRoom
{
    public class MainThread : MonoBehaviour
    {
        [SerializeField]
        private BlackBoard _blackBoard = null;

        private ChatAction _commentAction = null;
        private ConcurrentQueue<UnityAction> _actionQueue = null;

        // Start is called before the first frame update
        void Awake()
        {
            _actionQueue = new ConcurrentQueue<UnityAction>();
            _blackBoard = gameObject.GetComponent<BlackBoard>();
            _commentAction = new ChatAction(_blackBoard);
        }

        // Update is called once per frame
        void Update()
        {
            if (Keyboard.current.xKey.wasPressedThisFrame)
            {
                _actionQueue.Enqueue(_commentAction.Destroy);
            }
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                _actionQueue.Enqueue(_commentAction.Spawn);
            }
            if (!_actionQueue.IsEmpty)
            {
                UnityAction task;
                if (_actionQueue.TryDequeue(out task))
                {
                    task.Invoke();
                }
            }
        }
    }
}
