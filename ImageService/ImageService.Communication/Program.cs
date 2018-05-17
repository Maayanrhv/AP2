using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Communication
{
   

    class Program
    {
        static void Main(string[] args)
        {
            Debug.WriteLine("");
            Debug.WriteLine("                  start                   ");
            Debug.WriteLine("");
            string[] argss = { "OutputDir C:/Users/   djoff/Pictures /workService", "SourceName ImageServiceSource",
                "LogName ImageServiceLog", "ThumbnailSize 120",
            "Handler C:/Users/djoff/Pictures/workService/watFile;C:/Users/djoff/Pictures/workService/folowed"};

            string[] argss2 = { "0 C:/Users/djoff/Pictures/workService/watFile","1 C:/Users/djoff/Pictures/workService/folowed" };
            CommunicationProtocol cp = new CommunicationProtocol((int)CommandEnum.GetLogCommand, argss2);

            ServiceInfoEventArgs info = ClientServerArgsParser.Parse(cp);

            int e;
            e = 0;

        }
    }
}
