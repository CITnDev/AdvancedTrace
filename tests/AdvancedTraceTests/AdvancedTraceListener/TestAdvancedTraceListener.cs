using System;

namespace AdvancedTraceTests.AdvancedTraceListener
{
    public class TestAdvancedTraceListener : AdvancedTraceLib.AdvancedTraceListener
    {
        private string _previousMessage;

        private string _lastMessage;
        public string LastMessage
        {
            get { return _lastMessage; }
            private set { _lastMessage = value; Console.WriteLine(value); }
        }

        public bool IsNewMessage()
        {
            var result = _previousMessage != _lastMessage;
            _previousMessage = _lastMessage;

            return result;
        }


        public override void Write(string message)
        {
            LastMessage = message;
        }

        public override void WriteLine(string message)
        {
            LastMessage = message;
        }

        public override void Write(string message, string category)
        {
            if (string.IsNullOrEmpty(category))
                LastMessage = message;
            else
                LastMessage = category + " : " + message;
        }

        public override void Write(object o, string category)
        {
            if (string.IsNullOrEmpty(category))
                LastMessage = o.ToString();
            else
                LastMessage = category + " : " + o;
        }

        public override void WriteLine(string message, string category)
        {
            if (string.IsNullOrEmpty(category))
                LastMessage = message;
            else
                LastMessage = category + " : " + message;
        }

        public override void WriteLine(object o, string category)
        {
            if (string.IsNullOrEmpty(category))
                LastMessage = o.ToString();
            else
                LastMessage = category + " : " + o;
        }


        public override void WriteEx(string type, object message, string category, Exception exception)
        {
            LastMessage = "[Type : " + type + "] "
                          + (string.IsNullOrWhiteSpace(category) ? "" : " - [Category : " + category + "] ")
                          + message
                          + (exception == null ? "" : " (Exception : " + exception + ") ");
        }
        public override void WriteLineEx(string type, object message, string category, Exception exception)
        {
            LastMessage = "NL [Type : " + type + "] "
                          + (string.IsNullOrWhiteSpace(category) ? "" : " - [Category : " + category + "] ")
                          + message
                          + (exception == null ? "" : " (Exception : " + exception + ") ");
        }
    }
}