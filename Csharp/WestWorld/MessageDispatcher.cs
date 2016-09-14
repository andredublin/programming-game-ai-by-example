using System;
using System.Collections.Generic;
using System.Linq;

namespace Csharp.WestWorld
{
    public class MessageDispatcher
    {
        public const long SEND_MESSAGE_IMMEDIATELY = 0;

        private HashSet<Telegram> _priorityQueue = new HashSet<Telegram>();

        private MessageDispatcher() { }
        private static MessageDispatcher _instance;

        public static MessageDispatcher GetInstance()
        {
            if (_instance == null)
            {
                _instance = new MessageDispatcher();
            }

            return _instance;
        }

        public void Discharge(BaseGameEntity receiver, Telegram telegram)
        {
            if (!receiver.HandleMessage(telegram))
            {
                Console.WriteLine("Message not handled");
            }
        }

        public void DispatchMessage(
            long delay,
            EntityNamesEnum senderId,
            EntityNamesEnum receiverId,
            MessageTypeEnum message,
            Dictionary<string, string> extraInfo)
        {
            var sender = GameManager.EntityMgr().GetEntityFromId(senderId);
            var receiver = GameManager.EntityMgr().GetEntityFromId(receiverId);

            if (receiver == null)
            {
                Console.WriteLine(string.Format("Warning! No receiver with Id of {0} found", receiverId));
                return;
            }

            var telegram = new Telegram(sender.Id, receiver.Id, 0, message, extraInfo);

            if (delay <= 0)
            {
                Console.WriteLine(
                    string.Format(
                        "Instant telegram dispatched at time: {0} by {1} for {2}. Message is {3}",
                        DateTime.UtcNow,
                        sender.Id,
                        receiver.Id,
                        message));

                Discharge(receiver, telegram);
            }
            else
            {
                var currentTime = DateTime.UtcNow.Ticks;
                telegram.DispatchTime = currentTime + delay;
                _priorityQueue.Add(telegram);
                Console.WriteLine(
                    string.Format(
                        "Delayed telegram from {0} recorded at time {1} for {2}. Message is {3}",
                        sender.Id,
                        DateTime.UtcNow,
                        receiver.Id,
                        message));
            }
        }

        public void DisplayDelayedMessages()
        {
            var currentTime = DateTime.UtcNow.Ticks;

            while (_priorityQueue.Count > 0 &&
                (_priorityQueue.First().DispatchTime < currentTime) &&
                (_priorityQueue.First().DispatchTime > 0))
            {
                var telegram = _priorityQueue.First();
                var receiver = GameManager.EntityMgr().GetEntityFromId(telegram.Receiver);

                Console.WriteLine(
                    string.Format(
                        "Queued telegram ready for dispatch: Sent to {0}. Message is {1}",
                        receiver.Id,
                        telegram.Message));

                Discharge(receiver, telegram);
                _priorityQueue.Remove(telegram);
            }
        }
    }
}
