using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using System;
using System.Collections.Generic;

namespace ImageService.Communication
{
    public class ClientServerArgsParser
    {
        /// <summary>
        /// parsing from CommunicationProtocol to ServiceInfoEventArgs.
        /// </summary>
        public static ServiceInfoEventArgs Parse(CommunicationProtocol e)
        {
            CommandEnum ce = (CommandEnum)e.Command_Id;
            string[] args = e.Command_Args;
            if (args == null)
            {
                return null;
            }
            ServiceInfoEventArgs siea = new ServiceInfoEventArgs();
             if (ce == CommandEnum.GetConfigCommand)
            {
                ClientServerArgsParser.GetConfigCommand(siea, args);
            }
            else if (ce == CommandEnum.CloseHandlerCommand)
            {
                ClientServerArgsParser.CloseHandlerCommand(siea, args);
            }
            else if (ce == CommandEnum.GetLogCommand)
            {
                ClientServerArgsParser.GetLogCommand(siea, args);
            }
            return siea;
        }

        /// <summary>
        /// filling the ConfigMap property
        /// </summary>
        /// <param name="siea">ServiceInfoEventArgs object to fill</param>
        /// <param name="args">CommunicationProtocol arguments</param>
        private static void GetConfigCommand(ServiceInfoEventArgs siea, string[] args)
        {
            int indx;
            Dictionary<string, string> d = new Dictionary<string, string>();
            foreach (string str in args)
            {
                indx = str.IndexOf(' ');
                d.Add(str.Substring(0, indx), str.Substring(indx + 1));
            }
            siea.ConfigMap = d;
        }

        /// <summary>
        /// filling the RemovedHandlers property
        /// </summary>
        /// <param name="siea">ServiceInfoEventArgs object to fill</param>
        /// <param name="args">CommunicationProtocol arguments</param>
        private static void CloseHandlerCommand(ServiceInfoEventArgs siea, string[] args)
        {
            List<string> handlers = new List<string>();
            foreach (string handler in args)
            {
                handlers.Add(handler);
            }
            siea.RemovedHandlers = handlers;
        }

        /// <summary>
        /// filling the LogsList property
        /// </summary>
        /// <param name="siea">ServiceInfoEventArgs object to fill</param>
        /// <param name="args">CommunicationProtocol arguments</param>
        private static void GetLogCommand(ServiceInfoEventArgs siea, string[] args)
        {
            List<Log> logs = new List<Log>();
            foreach (string log in args)
            {
                int indx = log.IndexOf(' ');
                string[] subs = { log.Substring(0, indx), log.Substring(indx + 1) };
                MessageTypeEnum mt;
                try
                {
                    mt = (MessageTypeEnum)Enum.Parse(typeof(MessageTypeEnum), subs[0]);
                    logs.Add(new Log(mt, subs[1]));
                } catch (Exception)
                {
                    mt = MessageTypeEnum.INFO;
                    logs.Add(new Log(mt, log));
                }  
            }
            logs.Reverse();
            siea.LogsList = logs;
        }
    }
}
