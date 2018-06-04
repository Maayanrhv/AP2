using ImageService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace ImageService.Commands
{
    /// <summary>
    /// deletes a handler from App.config "Handler" values.
    /// </summary>
    class CloseHandlerCommand : ICommand
    {
        /// <summary>
        /// deletes a handler from App.config and returns the new "Handler" value.
        /// </summary>
        /// <param name="args">a list of handlers to delete</param>
        /// <param name="result">whether the deletion went successful or not</param>
        /// <returns>the new "Handler" value in App.config</returns>
        public string Execute(string[] args, out bool result)
        {
            try
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
            } catch(Exception e)
            {
                result = false;
                return Messages.ExceptionInfo(e);
            }
        }
        /// <summary>
        /// replaces the value of a given key in appSettings field in App.config with a new one.
        /// </summary>
        /// <param name="key">a key in appSettings field in App.config</param>
        /// <param name="value">new value to replace the old one</param>
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
