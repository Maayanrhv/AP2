
using System.ComponentModel;

namespace ImageService.Logging
{
    /// <summary>
    /// specifies type of message
    /// </summary>
    public enum MessageTypeEnum : int
    {
        [Description("INFO")]
        INFO,
        [Description("WARNING")]
        WARNING,
        [Description("FAIL")]
        FAIL
    }
}