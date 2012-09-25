using System;
using System.IO;
using AdvancedTraceLib;
using AdvancedTraceTests.AdvancedTraceListener;
using AdvancedTraceTests.TraceListener;
using NUnit.Framework;

namespace AdvancedTraceTests
{
    [TestFixture]
    public class AdvancedTraceTests
    {
        private TestListener _listener;
        private TestAdvancedTraceListener _advListener;
        
        [SetUp]
        public void InitTest()
        {
            _listener = new TestListener();
            _advListener = new TestAdvancedTraceListener();
        }

        [TearDown]
        public void ClearAll()
        {
            _listener.Dispose();
            _advListener.Dispose();

            AdvancedTrace.RemoveAllTraceListener();
        }

        [Test]
        public void SimpleTrace()
        {
            AdvancedTrace.AddTraceListener(AdvancedTrace.ListenerType.Information, _listener);
            AdvancedTrace.AddTraceListener(AdvancedTrace.ListenerType.Information, _advListener);

            AdvancedTrace.TraceInformation("Default information");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceInformation("Information with exception", new FileNotFoundException("file.ext"));
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceInformation("Information on a category", "Category2");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceInformation("Information with exception on a category", new Exception("Base exception"), "Category1");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());

            AdvancedTrace.TraceDatabase("Database");
            Assert.False(_listener.IsNewMessage());
            Assert.False(_advListener.IsNewMessage());
            AdvancedTrace.TraceError("Error");
            Assert.False(_listener.IsNewMessage());
            Assert.False(_advListener.IsNewMessage());
            AdvancedTrace.TraceFatal("Fatal");
            Assert.False(_listener.IsNewMessage());
            Assert.False(_advListener.IsNewMessage());
            AdvancedTrace.TraceDebug("Debug");
            Assert.False(_listener.IsNewMessage());
            Assert.False(_advListener.IsNewMessage());
            AdvancedTrace.TraceSql("SQL");
            Assert.False(_listener.IsNewMessage());
            Assert.False(_advListener.IsNewMessage());
            AdvancedTrace.TraceProblem("Problem");
            Assert.False(_listener.IsNewMessage());
            Assert.False(_advListener.IsNewMessage());
            AdvancedTrace.TraceWarning("Warning");
            Assert.False(_listener.IsNewMessage());
            Assert.False(_advListener.IsNewMessage());

            AdvancedTrace.TraceInformation("Finish");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());

            AdvancedTrace.RemoveTraceListener(AdvancedTrace.ListenerType.Information, _listener);
        }

        [Test]
        public void MultipleTrace()
        {
            AdvancedTrace.AddTraceListener(AdvancedTrace.ListenerType.Information, _listener);
            AdvancedTrace.AddTraceListener(AdvancedTrace.ListenerType.Warning, _listener);
            AdvancedTrace.AddTraceListener(AdvancedTrace.ListenerType.Information, _advListener);
            AdvancedTrace.AddTraceListener(AdvancedTrace.ListenerType.Problem, _advListener);

            AdvancedTrace.TraceInformation("Default information");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceInformation("Information with exception", new FileNotFoundException("file.ext"));
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceInformation("Information on a category", "Category2");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceInformation("Information with exception on a category", new Exception("Base exception"), "Category1");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());

            AdvancedTrace.TraceDatabase("Database");
            Assert.False(_listener.IsNewMessage());
            Assert.False(_advListener.IsNewMessage());
            AdvancedTrace.TraceError("Error");
            Assert.False(_listener.IsNewMessage());
            Assert.False(_advListener.IsNewMessage());
            AdvancedTrace.TraceFatal("Fatal");
            Assert.False(_listener.IsNewMessage());
            Assert.False(_advListener.IsNewMessage());
            AdvancedTrace.TraceDebug("Debug");
            Assert.False(_listener.IsNewMessage());
            Assert.False(_advListener.IsNewMessage());
            AdvancedTrace.TraceSql("SQL");
            Assert.False(_listener.IsNewMessage());
            Assert.False(_advListener.IsNewMessage());
            AdvancedTrace.TraceProblem("Problem");
            Assert.False(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceWarning("Warning");
            Assert.True(_listener.IsNewMessage());
            Assert.False(_advListener.IsNewMessage());

            AdvancedTrace.TraceInformation("Finish");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
        }

        [Test]
        public void TraceAll()
        {
            AdvancedTrace.AddTraceListener(AdvancedTrace.ListenerType.All, _listener);
            AdvancedTrace.AddTraceListener(AdvancedTrace.ListenerType.All, _advListener);

            AdvancedTrace.TraceInformation("Default information");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceInformation("Information with exception", new FileNotFoundException("file.ext"));
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceInformation("Information on a category", "Category2");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceInformation("Information with exception on a category", new Exception("Base exception"), "Category1");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());

            AdvancedTrace.TraceDatabase("Database");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceError("Error");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceFatal("Fatal");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceDebug("Debug");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceSql("SQL");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceProblem("Problem");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceWarning("Warning");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());

            AdvancedTrace.TraceInformation("Finish");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
        }

        [Test]
        public void TraceAllWithRemoveType()
        {
            AdvancedTrace.AddTraceListener(AdvancedTrace.ListenerType.All, _listener);
            AdvancedTrace.AddTraceListener(AdvancedTrace.ListenerType.All, _advListener);
            AdvancedTrace.RemoveTraceListener(AdvancedTrace.ListenerType.Warning, _advListener);

            AdvancedTrace.TraceInformation("Default information");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceInformation("Information with exception", new FileNotFoundException("file.ext"));
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceInformation("Information on a category", "Category2");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceInformation("Information with exception on a category", new Exception("Base exception"), "Category1");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());

            AdvancedTrace.TraceDatabase("Database");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceError("Error");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceFatal("Fatal");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceDebug("Debug");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceSql("SQL");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceProblem("Problem");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceWarning("Warning");
            Assert.True(_listener.IsNewMessage());
            Assert.False(_advListener.IsNewMessage());

            AdvancedTrace.TraceInformation("Finish");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
        }

        [Test]
        public void TraceAllWithNewType()
        {
            const string newType = "__NEW_TYPE__";

            AdvancedTrace.AddTraceListener(AdvancedTrace.ListenerType.All, _listener);
            AdvancedTrace.AddTraceListener(AdvancedTrace.ListenerType.All, _advListener);
            AdvancedTrace.AddTraceType(newType);

            AdvancedTrace.TraceInformation("Default information");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceInformation("Information with exception", new FileNotFoundException("file.ext"));
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceInformation("Information on a category", "Category2");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceInformation("Information with exception on a category", new Exception("Base exception"), "Category1");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());

            AdvancedTrace.TraceDatabase("Database");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceError("Error");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceFatal("Fatal");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceDebug("Debug");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceSql("SQL");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceProblem("Problem");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
            AdvancedTrace.TraceWarning("Warning");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());


            AdvancedTrace.Trace(newType, "New type");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());

            AdvancedTrace.TraceInformation("Finish");
            Assert.True(_listener.IsNewMessage());
            Assert.True(_advListener.IsNewMessage());
        }
    }
}
