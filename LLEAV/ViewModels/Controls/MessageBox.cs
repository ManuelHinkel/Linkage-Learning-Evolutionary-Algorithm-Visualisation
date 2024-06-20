using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LLEAV.ViewModels.Controls
{
    /// <summary>
    /// Enumeration for the different message priorities.
    /// </summary>
    public enum MessagePriority
    {
        ALL,
        INTERESTING,
        IMPORTANT,
    }

    /// <summary>
    /// Represents a message with content and priority.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Gets or sets the content of the message.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the priority of the message.
        /// </summary>
        public MessagePriority Priority { get; set; }

        /// <summary>
        /// Gets the color associated with the message priority.
        /// </summary>
        public string Color
        {
            get => GlobalManager.MESSAGE_COLORS[(int)Priority];
        }

        /// <summary>
        /// Initializes a new instance of the Message class with specified content and priority.
        /// </summary>
        /// <param name="content">The textual content of the message.</param>
        /// <param name="priority">The priority level of the message.</param>
        public Message(string content, MessagePriority priority = MessagePriority.ALL)
        {
            Content = content;
            Priority = priority;
        }
    }

    /// <summary>
    /// Represents a box containing messages. Used to hold messages, which are shown in the UI.
    /// </summary>
    public class MessageBox : ReactiveObject
    {
        /// <summary>
        /// Gets the collection of messages displayed in the message box.
        /// </summary>
        public ObservableCollection<Message> Messages { get; private set; } = new ObservableCollection<Message>();

        private IList<Message> _messages = new List<Message>();

        private int _messagePriorityIndex;
        /// <summary>
        /// Gets or sets the index of the minimum message priority to display.
        /// </summary>
        public int MessagePriorityIndex
        {
            get => _messagePriorityIndex;
            set
            {
                this.RaiseAndSetIfChanged(ref _messagePriorityIndex, value);
                RecalculateMessages();
            }
        }

        /// <summary>
        /// Clears all messages from the message box.
        /// </summary>
        public void Clear()
        {
            Messages.Clear();
            _messages.Clear();
        }

        /// <summary>
        /// Inserts a message at the specified index in the message list and updates the displayed messages if it meets the priority criteria.
        /// </summary>
        /// <param name="index">The index at which to insert the message.</param>
        /// <param name="message">The message to insert.</param>
        public void Insert(int index, Message message)
        {
            _messages.Insert(index, message);
            if ((int)message.Priority >= MessagePriorityIndex)
            {
                Messages.Insert(index, message);
            }
        }

        /// <summary>
        /// Recalculates and updates the displayed messages based on the current message priority index.
        /// </summary>
        private void RecalculateMessages()
        {
            Messages.Clear();

            foreach (Message message in _messages)
            {
                if ((int)message.Priority >= MessagePriorityIndex)
                {
                    Messages.Add(message);
                }
            }
        }
    }
}
