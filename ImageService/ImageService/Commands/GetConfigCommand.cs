using ImageService.Communication;
using ImageService.Infrastructure.Enums;
using Newtonsoft.Json;
using System;
using System.Configuration;

namespace ImageService.Commands
{
    /// <summary>
    /// called when a new Client connetcted to Server. the command retrieves the App.config 
    /// content and returns it.
    /// </summary>
    class GetConfigCommand : ICommand
    {
        /// <summary>
        /// returns the App.config content.
        /// </summary>
        /// <param name="args">not in use. can be null</param>
        /// <param name="result">whether  reading the App.config went successful or not</param>
        /// <returns>the App.config content in a json serialized format, ready to be sent via tcp</returns>
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
