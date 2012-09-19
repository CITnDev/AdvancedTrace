using System;
using System.ComponentModel;
using System.Diagnostics;

namespace AdvancedTraceLib
{
    // Our base implementation of the TraceListener -> Used to build custom listener
    public abstract class AdvancedTraceListener : TraceListener
    {
        // The standard write methods will never be called on the AdvancedTraceListener
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void Write(string pstrMessage)
        {
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void Write(string pstrMessage, string pstrCategory)
        {
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void Write(object poTrace)
        {
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void Write(object poTrace, string pstrCategory)
        {
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void WriteLine(string pstrMessage)
        {
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void WriteLine(string pstrMessage, string pstrCategory)
        {
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void WriteLine(object poTrace)
        {
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void WriteLine(object poTrace, string pstrCategory)
        {
        }

        // Advanced trace method
        public virtual void WriteEx(string pstrType, string pstrTrace)
        {
        }

        public virtual void WriteEx(string pstrType, string pstrTrace, string pstrCategory)
        {
        }

        public virtual void WriteEx(string pstrType, object poTrace)
        {
        }

        public virtual void WriteEx(string pstrType, object poTrace, string pstrCategory)
        {
        }

        public virtual void WriteEx(string pstrType, string pstrMessage, Exception poException)
        {
        }

        public virtual void WriteEx(string pstrType, string pstrMessage, Exception poException, string pstrUserCategory)
        {
        }

        public virtual void WriteLineEx(string pstrType, string pstrTrace)
        {
        }

        public virtual void WriteLineEx(string pstrType, string pstrTrace, string pstrCategory)
        {
        }

        public virtual void WriteLineEx(string pstrType, object poTrace)
        {
        }

        public virtual void WriteLineEx(string pstrType, object poTrace, string pstrCategory)
        {
        }

        public virtual void WriteLineEx(string pstrType, string pstrMessage, Exception poException)
        {
        }

        public virtual void WriteLineEx(string pstrType, string pstrMessage, Exception poException, string pstrUserCategory)
        {
        }
    }
}