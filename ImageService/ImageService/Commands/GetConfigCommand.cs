using ImageService.Communication;
using ImageService.Infrastructure.Enums;
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
                configs[0] = ConfigurationManager.AppSettings.Get("Handler");
                configs[1] = ConfigurationManager.AppSettings.Get("OutputDir");
                configs[2] = ConfigurationManager.AppSettings.Get("SourceName");
                configs[3] = ConfigurationManager.AppSettings.Get("LogName");
                configs[4] = ConfigurationManager.AppSettings.Get("ThumbnailSize");

                CommunicationProtocol commandSendArgs = new CommunicationProtocol(
                    (int)CommandEnum.GetConfigCommand, configs);
                result = true;
                return commandSendArgs.parseToJson();
            }
            catch (Exception ex)
            {
                result = false;
                return ex.ToString();
            }
        }
    }
}
