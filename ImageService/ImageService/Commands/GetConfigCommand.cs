using ImageService.Communication;
using ImageService.Infrastructure.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    class GetConfigCommand : ICommand
    {
        public string Execute(string[] args, out bool result)
        {
            try
            {
                string[] configs = new string[5];
                configs[0] = "Handler " + ConfigurationManager.AppSettings.Get("Handler");
                configs[1] = "OutputDir " + ConfigurationManager.AppSettings.Get("OutputDir");
                configs[2] = "SourceName " + ConfigurationManager.AppSettings.Get("SourceName");
                configs[3] = "LogName " + ConfigurationManager.AppSettings.Get("LogName");
                configs[4] = "ThumbnailSize " + ConfigurationManager.AppSettings.Get("ThumbnailSize");

                CommunicationProtocol commandSendArgs = new CommunicationProtocol(
                    (int)CommandEnum.GetConfigCommand, configs);
                result = true;
                //return commandSendArgs.parseToJson();
                return JsonConvert.SerializeObject(commandSendArgs);
            }
            catch (Exception ex)
            {
                result = false;
                return ex.ToString();
            }
        }
    }
}
