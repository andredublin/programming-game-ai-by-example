using System;
using System.Collections.Generic;
using System.Linq;

namespace Csharp.WestWorld
{
    public class MessageDispatcher
    {
        public const long SendMessageImmediately = 0;

        private readonly HashSet<Telegram> _priorityQueue;

        private MessageDispatcher()
        {
            _priorityQueue = new HashSet<Telegram>();
        }
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
                Console.WriteLine($"Warning! No receiver with Id of {receiverId} found");
                return;
            }

            var telegram = new Telegram(sender.Id, receiver.Id, 0, message, extraInfo);

            if (delay <= 0)
            {
                Console.WriteLine($"Instant telegram dispatched at time: {DateTime.UtcNow.Ticks} by {sender.Id} for {receiver.Id}. Message is {message}");

                Discharge(receiver, telegram);
            }
            else
            {
                var currentTime = DateTime.UtcNow.Ticks;
                telegram.DispatchTime = currentTime + delay;
                _priorityQueue.Add(telegram);
                Console.WriteLine($"Delayed telegram from {sender.Id} recorded at time {DateTime.UtcNow.Ticks} for {receiver.Id}. Message is {message}");
            }
        }

        public void DisplayDelayedMessages()
        {
            var currentTime = DateTime.UtcNow.Ticks;

            while (_priorityQueue.Count > 0 &&
                _priorityQueue.First().DispatchTime < currentTime &&
                _priorityQueue.First().DispatchTime > 0)
            {
                var telegram = _priorityQueue.First();
                var receiver = GameManager.EntityMgr().GetEntityFromId(telegram.Receiver);

                Console.WriteLine($"Queued telegram ready for dispatch: Sent to {receiver.Id}. Message is {telegram.Message}");

                Discharge(receiver, telegram);
                _priorityQueue.Remove(telegram);
            }
        }
    }
}
