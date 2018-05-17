using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using ImageService.Logging;
using ImageService.Server;
using ImageService.Controller;
using ImageService.Modal;
using System.Configuration;
using System.IO;

namespace ImageService
{
    /// <summary>
    /// the service manage images in specified directories: 
    /// images will be copied to a directory named "outputDir", organized by years
    /// and months.In addition, a "thumbNail" directory will have the same images and the
    /// same structure, but with a fixed and uniform image size.
    /// </summary>
    public partial class ImageService : ServiceBase
    {
        private int eventId = 1;
        private ImageServer m_imageServer;
        private IImageServiceModal m_modal;
        private IImageController m_controller;
        private ILoggingService m_logging;

        /// <summary>
        /// constructor 
        /// </summary>
        /// <param name="args">arguments</param>
        public ImageService(string[] args)
        {
            InitializeComponent();
            string eventSourceName = ConfigurationManager.AppSettings["SourceName"];
            string logName = ConfigurationManager.AppSettings["LogName"];
            if (args.Count() > 0)
            {
                eventSourceName = args[0];
            }
            if (args.Count() > 1)
            {
                logName = args[1];
            }
            eventLog1 = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists(eventSourceName))
            {
                System.Diagnostics.EventLog.CreateEventSource(eventSourceName, logName);
            }
            eventLog1.Source = eventSourceName;
            eventLog1.Log = logName;
        }

        /// <summary>
        /// start function of the service:
        /// -    creats output directory
        /// -    creats server, modal, conntroler and logger.
        /// </summary>
        /// <param name="args">arguments</param>
        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("In OnStart");
            // Set up a timer to trigger every minute.  
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60000; // 60 seconds  
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();
            timer.Enabled = true;
            // Update the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            this.m_logging = new LoggingService();
            this.m_logging.AddEvent(OnMsg);

            string outputFolder = CreateOutputDirFolder();
            int thumbnailSize = int.Parse(ConfigurationManager.AppSettings["ThumbnailSize"]);
            this.m_modal = new ImageServiceModal(outputFolder, thumbnailSize);
            this.m_controller = new ImageController(this.m_modal);

            Console.WriteLine("creating Server");
            this.m_imageServer = new ImageServer(this.m_controller, this.m_logging);
            this.m_imageServer.Start();
        }

        /// <summary>
        /// creats the OutputDir directory.
        /// </summary>
        /// <returns></returns>
        private string CreateOutputDirFolder()
        {
            string path = ConfigurationManager.AppSettings["OutputDir"];
            string newDirPath = path + "\\OutputDir";
            if (!Directory.Exists(newDirPath))
            {
                DirectoryInfo di = Directory.CreateDirectory(newDirPath);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
            return newDirPath;
        }

        /// <summary>
        /// event handler for timer event.
        /// contains default message that is printed while the service tuns.
        /// </summary>
        /// <param name="sender">who called OnTimer func</param>
        /// <param name="args">arguments</param>
        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            // Insert monitoring activities here.  
            eventLog1.WriteEntry("Monitoring the System", EventLogEntryType.Information, eventId++);
        }

        /// <summary>
        /// closing the server and then the service.
        /// </summary>
        protected override void OnStop()
        {
            eventLog1.WriteEntry("In onStop.");
            m_imageServer.CloseHandlers();
            // Update the service state to Stop Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            this.m_imageServer.Stop();
        }


        protected override void OnContinue()
        {
            eventLog1.WriteEntry("In OnContinue.");
        }

        /// <summary>
        ///  event handler for logger. this functinon will print to the eventLog
        /// every message that the logger recieves.
        /// </summary>
        /// <param name="o">who called OnMsg func</param>
        /// <param name="msg">message object</param>
        protected void OnMsg(object o, MessageRecievedEventArgs msg)
        {
            eventLog1.WriteEntry(msg.Status.ToString() + " " + msg.Message);
        }

        /// <summary>
        ///  setting state.
        /// </summary>
        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public int dwServiceType;
            public ServiceState dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        };

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);
    }
}
