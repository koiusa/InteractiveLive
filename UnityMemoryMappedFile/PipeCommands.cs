using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UnityMemoryMappedFile
{
    public partial class PipeCommands
    {
        public static Type GetCommandType(string commandStr)
        {

            var commands = typeof(PipeCommands).GetNestedTypes(System.Reflection.BindingFlags.Public);
            foreach (var command in commands)
            {
                if (command.Name == commandStr) return command;
            }
            return null;
        }

        #region Types
        public enum NotifyLogTypes
        {
            Exception = 0,
            Error = 1,
            Assert = 2,
            Warning = 3,
            Log = 4,
        }
        #endregion

        #region Commands
        public class SendMessage
        {
            public string Message { get; set; }
            public string url { get; set; }
        }
        public class MoveObject
        {
            public float force { get; set; }
        }

        public class GetCurrentPosition
        {

        }

        public class ReturnCurrentPosition
        {
            public float Data { get; set; }
        }
        #region WPF
        public class StatusStringChangedRequest
        {
            public bool doSend { get; set; }
        }
        public class ExitControlPanel { }

        public class SaveSettings { public string Path { get; set; } }
        public class LoadSettings { public string Path { get; set; } }
        public class LogNotify
        {
            public string condition { get; set; }
            public string stackTrace { get; set; }
            public NotifyLogTypes type { get; set; }
            public int errorCount { get; set; }
        }
        #endregion
        #endregion
    }

    [Serializable]
    public class KeyConfig
    {
        public static string Language = "Japanese";
    }
}
