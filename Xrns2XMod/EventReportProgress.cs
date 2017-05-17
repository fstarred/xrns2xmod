using System;

namespace Xrns2XMod
{
    // A delegate type for hooking up change notifications.
    public delegate void ProgressHandler(object sender, EventReportProgressArgs e);

    public enum MsgType 
    { 
        INFO,
        WARNING,
        ERROR 
    };

    public class EventReportProgressArgs : EventArgs
    {
        public EventReportProgressArgs(string message)
        {
            this.message = message;
            this.type = MsgType.INFO;
        }

        public EventReportProgressArgs(string message, MsgType type)
        {
            this.message = message;
            this.type = type;
        }

        public string message;
        public MsgType type;


    }
}
