using System.Collections.Generic;

namespace Csharp.WestWorld
{
    public struct Telegram
    {
        private const double SmallestDelay = 0.25;

        public EntityNamesEnum Sender { get; private set; }
        public EntityNamesEnum Receiver { get; private set; }
        public MessageTypeEnum Message { get; private set; }
        public long DispatchTime { get; set; }
        public Dictionary<string, string> ExtraInfo { get; private set; }

        public Telegram(
            EntityNamesEnum sender,
            EntityNamesEnum receiver,
            long time,
            MessageTypeEnum message = MessageTypeEnum.Unknown,
            Dictionary<string, string> extraInfo = null)
        {
            Sender = sender;
            Receiver = receiver;
            DispatchTime = time;
            Message = message;
            ExtraInfo = extraInfo;
        }
    }
}
