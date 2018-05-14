using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ImageService.Communication
{
    public class CommunicationProtocol
    {
        #region Members
        private int CommandId;
        private string[] CommandArgs;
        #endregion

        public CommunicationProtocol(int cmndId, string[] args)
        {
            CommandId = cmndId;
            CommandArgs = args;
        }

        //Getters & Setters
        public int Command_Id { get; set; }
        public string[] Command_Args { get; set; }

        public string parseToJson()
        {
            JObject cmdObj = new Newtonsoft.Json.Linq.JObject();
            cmdObj["CommandId"] = CommandId;
            JArray args = new JArray(CommandArgs);
            cmdObj["CommandArgs"] = args;
            return cmdObj.ToString();
        }

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
