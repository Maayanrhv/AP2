using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

using ImageService.Modal;

namespace ImageService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun = new ServiceBase[] { new ImageService(args) };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
