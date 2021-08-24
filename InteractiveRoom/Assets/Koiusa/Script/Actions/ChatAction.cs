using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Koiusa.InteractiveRoom
{
    public class ChatAction
    {
        private BlackBoard _blackBorad = null;

        public ChatAction(BlackBoard aBlackBoard)
        {
            _blackBorad = aBlackBoard;
        }

        public void GetLiveChatComment(string comments, string authorUrl)
        {
            Spawn();
            var obj = _blackBorad.GetObjectEmitter.GetSpoawnObject;
            _blackBorad.GetLiveChatEmitter.SpawnGameObject(obj,comments, authorUrl);
        }

        public void Spawn()
        {
            var context = new PrefabContext();
            context.key = SimulateComment();
            var sphere = Random.insideUnitSphere * 10;
            context.position = new Vector3(sphere.x, Random.Range(0.0f, 10.0f), sphere.z);
            context.rotate = Random.rotation;
            _blackBorad.GetObjectEmitter.SpawnGameObject(context);
        }

        public void Destroy()
        {
            _blackBorad.GetObjectEmitter.DestroyGameObject();
            _blackBorad.GetLiveChatEmitter.DestoroyGameObject();
        }

        private string SimulateComment()
        {
            string comment = _blackBorad.GetObjectEmitter.GetRondomName();
            Debug.Log("SimulateComment:" + comment);
            return comment;
        }
    }
}
