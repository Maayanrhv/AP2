using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Communication
{
    public class ClientServerArgsParser
    {
        public static ServiceInfoEventArgs Parse(CommunicationProtocol e)
        {
            //TODO: enother implementation, without ifs
            CommandEnum ce = (CommandEnum)e.Command_Id;
            string[] args = e.Command_Args;
            if (args == null)
            {
                return null;
            }
            ServiceInfoEventArgs siea = new ServiceInfoEventArgs();
            if (ce == CommandEnum.CloseGUICommand)
            {

            }
            else if (ce == CommandEnum.GetConfigCommand)
            {
                ClientServerArgsParser.GetConfigCommand(siea, args);
            }
            else if (ce == CommandEnum.CloseHandlerCommand)
            {
                ClientServerArgsParser.CloseHandlerCommand(siea, args);
            }
            else if (ce == CommandEnum.GetLogcommand)
            {
                ClientServerArgsParser.GetLogcommand(siea, args);
            }
            return siea;
        }

        private static void GetConfigCommand(ServiceInfoEventArgs siea, string[] args)
        {
            int indx;
            Dictionary<string, string> d = new Dictionary<string, string>();
            //string[] subs;
            foreach (string str in args)
            {
                indx = str.IndexOf(' ');
                d.Add(str.Substring(0, indx), str.Substring(indx + 1));
            }

            //string handlers;
            //if (d.TryGetValue("Handler", out handlers))
            //{
            //    d.Remove("Handler");
            //    string[] hanlrs_list = handlers.Split(';');
            //    siea.removed_Handlers = hanlrs_list.ToList<string>();
            //}
            siea.config_Map = d;
        }
        private static void CloseHandlerCommand(ServiceInfoEventArgs siea, string[] args)
        {
            List<string> handlers = new List<string>();
            foreach (string handler in args)
            {
                handlers.Add(handler);
            }
            siea.removed_Handlers = handlers;
        }
        private static void GetLogcommand(ServiceInfoEventArgs siea, string[] args)
        {
            List<Couple> logs = new List<Couple>();
            foreach (string log in args)
            {
                int indx = log.IndexOf(' ');
                string[] subs = { log.Substring(0, indx), log.Substring(indx + 1) };
                int res;
                Int32.TryParse(subs[0], out res);
                MessageTypeEnum mt = (MessageTypeEnum)res;
                logs.Add(new Couple(mt, subs[1]));
            }
            siea.logs_List = logs;
        }
    }
}
