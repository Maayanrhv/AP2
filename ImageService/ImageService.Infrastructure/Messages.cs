using ImageService.Infrastructure.Enums;
using System;
using System.Net.Sockets;

namespace ImageService.Infrastructure
{
    /// <summary>
    /// built-in messages to send the logger to present in the EventLog of the service.
    /// </summary>
    public class Messages
    {
        // INFO messages
        public static string ClosingHandler()
        {
            return "Closing Handler";
        }
        public static string HandlesIdSendingCommand(int command)
        {
            try
            {
                return "Handler is Sending Command: " + ((CommandEnum)command).ToString("f");
            }
            catch (Exception e)
            {
                return "Handler is Sending Command: Unknown\n" + e.ToString();
            }

        }
        public static string CommandRanSuccessfully(string path)
        {
            return ("Command Executed Successfully. path: " + path);
        }

        public static string CommandRanSuccessfully(CommandEnum ce)
        {
            return ("Command Executed Successfully: " + Enum.GetName(typeof(CommandEnum), ce));
        }

        public static string ClosedHandlerSuccessfully(string path)
        {
            return ("Closed Handler Successfully. path: " + path);
        }

        public static string HandlerBeenAssigned(string path)
        {
            return ("Handler has been assigned to path: " + path);
        }

        public static string ServerWaitsForConnections()
        {
            return ("Server is waiting for connections...");
        }

        public static string ServerGotNewClientConnection()
        {
            return ("Server got new client connection");
        }

        public static string ServerStopped()
        {
            return ("Server stopped");
        }

        public static string ServerClosedConnections()
        {
            return ("Server closed connections");
        }
        public static string ClientClosedConnection()
        {
            return ("A Client Closed Connection With Server");
        }

        //ERROR messages
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
        public static string ExceptionInfo(Exception e)
        {
            string exp = "Exception occured: " + e.Message +
                "\n - Data: " + e.Data.ToString() +
                "\n - Source: " + e.Source +
                "\n - InnerException: " + e.InnerException +
                "\n - TargetSite: " + e.TargetSite +
                "\n - HResult: " + e.HResult +
                "\n - StackTrace: " + e.StackTrace +
                "\n - HelpLink: " + e.HelpLink;
            return exp;
        }
        public static string CouldntFindDateTime()
        {
            return "Couldn't find Date Time of the file. ";
        }
        public static string FailedToCreateFolder()
        {
            return "failed to create folder. ";
        }
        public static string CouldntDeleteFile()
        {
            return "Couldn't delete the file. ";
        }
        public static string ServerCouldntAcceptClient()
        {
            return "Server Couldn't Accept Client. ";
        }
        public static string ErrorRecievingMessageFromClient()
        {
            return "Server Error Recieving Message From Client.";
        }
        public static string ErrorHandlingClient()
        {
            return "Server Error Handling Client.";
        }
        public static string FailedExecutingCommand(CommandEnum ce)
        {
            return ("Command Failed to Execute: " + Enum.GetName(typeof(CommandEnum), ce));
        }
        public static string ErrorSendingConfigAndLogDataToClient(Exception e)
        {
            return ("Error While Sending Config And Log Data To Client.\nError Message: " + e);
        }
    }
}
