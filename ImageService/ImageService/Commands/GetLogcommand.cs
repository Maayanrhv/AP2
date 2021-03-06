﻿using ImageService.Communication;
using ImageService.Infrastructure.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;

namespace ImageService.Commands
{
    /// <summary>
    /// called when a new Client connetcted to Server. the command retrieves
    /// all the current existing logs since Service had started.
    /// </summary>
    public class GetLogCommand : ICommand
    {
        /// <summary>
        /// returns all existing logs since Service had started.
        /// </summary>
        /// <param name="args">not in use. can be null</param>
        /// <param name="result">whether  reading the logs went successful or not</param>
        /// <returns>all existing logs since Service had started</returns>
        public string Execute(string[] args, out bool result)
        {
            bool stopGetLogs = false;
            string[] logsArray;
            List<string> logsList = new List<string>();
            string logName = ConfigurationManager.AppSettings["LogName"];
            EventLog log = new EventLog(logName, "."); // "." is the local computer
            EventLogEntryCollection entries = log.Entries;
            int size = entries.Count;
            int i;
            // iterate from end to beggining.
            for (i = size -1; i > 0; i--)
            {
                EventLogEntry entry = entries[i];
                if (stopGetLogs)
                    break;
                string msg = entry.Message;
                //TODO: change "In OnStart" to a parameter.
                if (msg.Contains("In OnStart"))
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