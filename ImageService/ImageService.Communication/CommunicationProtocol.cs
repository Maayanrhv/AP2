using System.Linq;
using Newtonsoft.Json.Linq;

namespace ImageService.Communication
{
    /// <summary>
    /// the format in which data is transformed between the Server and it's clients.
    /// </summary>
    public class CommunicationProtocol
    {

        #region Members
        private int CommandId;
        private string[] CommandArgs;
        #endregion

        # region Getters & Setters
        public int Command_Id {
            get
            {
                return CommandId;
            }
            set
            {
                CommandId = value;
            }
        }
        public string[] Command_Args
        {
            get
            {
                return CommandArgs;
            }
            set
            {
                CommandArgs = value;
            }
        }
        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="cmndId">command id - which comand is transferred</param>
        /// <param name="args">command arguments</param>
        public CommunicationProtocol(int cmndId, string[] args)
        {
            CommandId = cmndId;
            CommandArgs = args;
        }

        /// <summary>
        /// constructor
        /// </summary>
        public CommunicationProtocol() { }

        /// <summary>
        /// parsing to Json- compress the content of the object to string representation.
        /// </summary>
        /// <returns>transferred data in string jason format</returns>
        public string parseToJson()
        {
            JObject cmdObj = new Newtonsoft.Json.Linq.JObject();
            cmdObj["CommandId"] = CommandId;
            JArray args = new JArray(CommandArgs);
            cmdObj["CommandArgs"] = args;
            return cmdObj.ToString();
        }

        /// <summary>
        /// undo "parseToJson".
        /// </summary>
        /// <param name="str">transferred data in string jason format</param>
        /// <returns>the content of the object in CommunicationProtocol representation</returns>
        public static CommunicationProtocol ParseFromJson(string str)
        {
            JObject cmdObj = JObject.Parse(str);
            int CommandID = (int)cmdObj["CommandId"];
            JArray arr = (JArray)cmdObj["CommandArgs"];
            string[] array = arr.Select(c => (string)c).ToArray();
            CommunicationProtocol msg = new CommunicationProtocol(CommandID, array);
            return msg;
        }
    }
}
