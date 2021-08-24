using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityMemoryMappedFile;

namespace Koiusa.InteractiveRoom
{
    public class MemoryMappedFileController : MonoBehaviour
    {
        [SerializeField]
        private BlackBoard _blackBorad = null;

        [SerializeField]
        private ExternalLinkThread _externalLinkThread = null;

        private MemoryMappedFileServer server;
        private string pipeName = Guid.NewGuid().ToString();

        public MemoryMappedFileServer GetServer
        {
            get { return server; }
        }

        // Use this for initialization
        void Start()
        {
#if UNITY_EDITOR   // For Debug。
            pipeName = "SamplePipeName";
#else
        //Debug.unityLogger.logEnabled = false;
        pipeName = "InteractiveRoomPipe" + Guid.NewGuid().ToString();
#endif
            server = new MemoryMappedFileServer();
            server.ReceivedEvent += Server_Received;
            server.Start(pipeName);
        }

        private void OnApplicationQuit()
        {
            server.ReceivedEvent -= Server_Received;
            server.Stop();
        }

        private async void Server_Received(object sender, DataReceivedEventArgs e)
        {
            Debug.Log(e.CommandType);
            if (e.CommandType == typeof(PipeCommands.SendMessage))
            {
                SendMessage((PipeCommands.SendMessage)e.Data, e);
            }
            else if (e.CommandType == typeof(PipeCommands.MoveObject))
            {
                MoveObject((PipeCommands.MoveObject)e.Data, e);
            }
            else if (e.CommandType == typeof(PipeCommands.GetCurrentPosition))
            {
                await GetCurrentPosition((PipeCommands.GetCurrentPosition)e.Data, e);
            }else if (e.CommandType == typeof(PipeCommands.LiveChatMessage))
            {
                LiveChatMessage((PipeCommands.LiveChatMessage)e.Data, e);
            }
        }

        // Update is called once per frame
        async void Update()
        {
            if (Keyboard.current.kKey.wasPressedThisFrame)
            {
                await server.SendCommandAsync(new PipeCommands.SendMessage { Message = "TestFromServer" });
            }
        }

        private void SendMessage(PipeCommands.SendMessage command, DataReceivedEventArgs e)
        {
            var d = (PipeCommands.SendMessage)e.Data;
            Debug.Log($"[Server]ReceiveFromClient:{d.Message}");
        }

        private void MoveObject(PipeCommands.MoveObject command, DataReceivedEventArgs e)
        {
            _externalLinkThread.BeginInvoke(() => //別スレッドで処理
            {
                var target = _blackBorad.GetObjectEmitter.GetRandamTarget();
                if (target != null)
                {
                    var sphere = UnityEngine.Random.insideUnitSphere * 10;
                    target.GetComponent<Rigidbody>().AddTorque(UnityEngine.Random.rotation.eulerAngles);
                    target.GetComponent<Rigidbody>().AddForce(sphere * command.force);
                }
            });
        }

        private async Task GetCurrentPosition(PipeCommands.GetCurrentPosition command, DataReceivedEventArgs e)
        {
            var target = _blackBorad.GetObjectEmitter.GetRandamTarget();
            if (target != null)
            {
                float x = 0.0f;
                    await _externalLinkThread.InvokeAsync(() => x = target.transform.position.x);
                    await server.SendCommandAsync(new PipeCommands.ReturnCurrentPosition { Data = x }, e.RequestId);
            }
        }

        private void LiveChatMessage(PipeCommands.LiveChatMessage command, DataReceivedEventArgs e)
        {
            var d = (PipeCommands.LiveChatMessage)e.Data;
            _externalLinkThread.BeginInvoke(() => //別スレッドで処理
            {
                Debug.Log($"[Server]ReceiveFromClient:{d.snippet.displayMessage}");
                var Action = new ChatAction(_blackBorad);
                Action.GetLiveChatComment(d.snippet.displayMessage, d.authorDetails.profileImageUrl);
            });
        }
    }
}
