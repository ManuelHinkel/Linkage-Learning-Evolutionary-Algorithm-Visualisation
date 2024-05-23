using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.ViewModels.Controls
{
    public enum MessagePriority
    {
        ALL,
        INTERESTING,
        IMPORTANT,
    }

    public class Message
    {
        public string Content { get; set; }
        public MessagePriority Priority { get; set; }

        public string Color
        {
            get => GlobalManager.MESSAGE_COLORS[(int)Priority];
        }

        public Message(string content, MessagePriority priority = MessagePriority.ALL)
        {
            Content = content;
            Priority = priority;
        }
    }
    public class MessageBox: ReactiveObject
    {
        public ObservableCollection<Message> Messages { get; private set; } = new ObservableCollection<Message>();

        private IList<Message> _messages = new List<Message>();

        private int _messagePriorityIndex;
        public int MessagePriorityIndex
        {
            get => _messagePriorityIndex;
            set
            {
                this.RaiseAndSetIfChanged(ref _messagePriorityIndex, value);
                RecalculateMessages();
            }
        }

        public void Clear()
        {
            Messages.Clear();
            _messages.Clear();
        }

        public void Insert(int index, Message message)
        {
            _messages.Insert(index, message);
            if((int)message.Priority >= MessagePriorityIndex)
            {
                Messages.Insert(index, message);
            }
        }

        private void RecalculateMessages()
        {
            Messages.Clear();

            foreach(Message message in _messages)
            {
                if ((int)message.Priority >= MessagePriorityIndex)
                {
                    Messages.Add(message);
                }
            }
        }
    }
}
