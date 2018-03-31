using ImageService.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Infrastructure
{
    public class Messages
    {
        // INFO
        public static string ClosingHandler()
        {
            return "Closing Handler";
        }
        public static string HandlesIdSendingCommand(int command)
        {
           try
            {
                return "Handler is Sending Command: " + ((CommandEnum)command).ToString("f");
            } catch (Exception e)
            {
                return "Handler is Sending Command: Unknown";
            }
            
        }
        public static string CommandRanSuccessfully(string path)
        {
            return ("Command Executed Successfully. path: " + path);
        }
        public static string HandlerBeenAssigned(string path)
        {
            return ("Handler has been assigned to path: " + path);
        }

        //ERRORS
        public static string FailedClosingHandler()
        {
            return "Failed in Closing Handler";
        }
        public static string FailedClosingHandler(string reason)
        {
            return ("Failed in Closing Handler. reason: " + reason);
        }
        public static string FailedSendingCommand(string reason)
        {
            return ("Failed in Sending Command. reason: " + reason);
        }
    }
}
