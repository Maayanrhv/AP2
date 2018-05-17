using ImageService.Communication;
using ImageService.Infrastructure.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    public class GetLogCommand : ICommand
    {
        public string Execute(string[] args, out bool result)
        {
            bool stopGetLogs = false;
            string[] logsArray;
            List<string> logsList = new List<string>();
            string logName = ConfigurationManager.AppSettings["LogName"];
            EventLog log = new EventLog(logName, "."); // "." is the local computer
            EventLogEntryCollection entries = log.Entries;

            foreach (EventLogEntry entry in entries)
            {
                if (stopGetLogs)
                    break;
                string msg = entry.Message;
                //TODO: change "In OnStart" to a parameter.
                if (msg.Equals("In OnStart"))
                    stopGetLogs = true;
                logsList.Add("" + msg);
            }

            string convertEachString = JsonConvert.SerializeObject(logsList);
            if (convertEachString == null || !logsList.Any())
            {
                result = false;
                return null;
            }
            // if list is not empty
                logsArray = logsList.ToArray();
                CommunicationProtocol commandSendArgs = new CommunicationProtocol(

                    (int)CommandEnum.GetLogCommand, logsArray);
                result = true;
                return JsonConvert.SerializeObject(commandSendArgs);
        }
    }
}