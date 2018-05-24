using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    class CloseHandlerCommand : ICommand
    {
        public string Execute(string[] args, out bool result)
        {
            // deletes all handlers in args
            string handlersFromConfig = ConfigurationManager.AppSettings.Get("Handler");
            string[] handlers = handlersFromConfig.Split(';');
            List<string> handlersList = handlers.ToList<string>();
            foreach(string handlerToDel in args)
            {
                handlersList.Remove(handlerToDel);
            }
            StringBuilder sb = new StringBuilder();
            foreach (string h in handlersList)
            {
                sb.Append(h);
                sb.Append(';');
            }
            string newHandlers;
            if (sb.Length == 0)
            {
                newHandlers = "";
            } else
            {
                sb.Remove(sb.Length - 1, 1);
                newHandlers = sb.ToString();
            }

            UpdateSetting("Handler", newHandlers);

            result = true;
            return newHandlers;
        }

        private void UpdateSetting(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove(key);
            config.AppSettings.Settings.Add(key, value);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

    }
}
