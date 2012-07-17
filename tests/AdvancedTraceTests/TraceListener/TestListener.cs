using System;

namespace AdvancedTraceTests.TraceListener
{
    public class TestListener : System.Diagnostics.TraceListener
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
            LastMessage = "NL " + message;
        }
    }
}
