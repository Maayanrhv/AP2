﻿
namespace ImageService.Infrastructure.Enums
{
    /// <summary>
    /// specifies types of command
    /// </summary>
    public enum CommandEnum : int
    {
        NewFileCommand,
        GetConfigCommand,
        GetLogcommand,
        CloseHandlerCommand,
        CloseAllCommand,
        CloseGUICommand
    }
}