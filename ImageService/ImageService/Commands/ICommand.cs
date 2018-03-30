﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure.Enums;

namespace ImageService.Commands
{
    public interface ICommand
    {
        string Execute(string[] args, out bool result);          // The Function That will Execute The 
    }
}
