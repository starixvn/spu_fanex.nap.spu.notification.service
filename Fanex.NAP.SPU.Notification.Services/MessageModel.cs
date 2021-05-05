using System;

namespace Fanex.NAP.SPU.Notification.Services
{
    public class MessageModel
    {
        public MessageModel(string systemName, DateTime timestamp, string environment, string message, string moreInfo = null)
        {
            this.Timestamp = timestamp;
            this.Environment = environment;
            this.Message = message;
            this.SystemName = systemName;
            this.MoreInfo = moreInfo;
        }

        public DateTime Timestamp { get; }

        public string Environment { get; }

        public string Message { get; }

        public string SystemName { get; }

        public string MoreInfo { get; set; }
    }
}
