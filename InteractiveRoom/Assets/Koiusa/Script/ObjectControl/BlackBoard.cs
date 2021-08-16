using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Koiusa.InteractiveRoom
{
    public class BlackBoard : MonoBehaviour
    {
        [SerializeField]
        private MemoryMappedFileController _controlWPFWindow = null;

        [SerializeField]
        private ObjectEmitter _objectEmitter = null;

        [SerializeField]
        private LiveChatEmitter _liveChatEmitter = null;
        public ObjectEmitter GetObjectEmitter
        {
            get { return _objectEmitter; }
        }

        public LiveChatEmitter GetLiveChatEmitter
        {
            get { return _liveChatEmitter; }
        }
    }
}
