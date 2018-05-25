using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Infrastructure
{
    class Program
    {

        static void Main(string[] args)
        {
            EventLog[] eventLogs = EventLog.GetEventLogs();
            foreach (EventLog e in eventLogs)
            {
                if (e.LogDisplayName.Equals("ImageServiceLog"))
                {
                    e.Clear();
                    break;
                }
            }

        }
    }
}
