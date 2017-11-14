using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace AdvancedTraceLib
{
    public class AdvancedTrace
    {
        private static readonly ConcurrentDictionary<string, List<TraceListener>> Tracers;
        private static readonly ConcurrentDictionary<string, ReaderWriterLockSlim> TraceLockers;
        private static readonly List<TraceListener> TraceAll;
        private static readonly ReaderWriterLockSlim LockTraceAll = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public class ListenerType
        {
            public const string All = "__ADVANCED_TRACE_ALL__";
            public const string Information = "__ADVANCED_TRACE_INFORMATION__";
            public const string Warning = "__ADVANCED_TRACE_WARNING__";
            public const string Error = "__ADVANCED_TRACE_ERROR__";
            public const string Problem = "__ADVANCED_TRACE_PROBLEM__";
            public const string Fatal = "__ADVANCED_TRACE_FATAL__";
            public const string Debug = "__ADVANCED_TRACE_DEBUG__";
            public const string Database = "__ADVANCED_TRACE_DATABASE__";
            public const string Sql = "__ADVANCED_TRACE_SQL__";
        }

        #region Default static constructor

        static AdvancedTrace()
        {
            Tracers = new ConcurrentDictionary<string, List<TraceListener>>();
            TraceLockers = new ConcurrentDictionary<string, ReaderWriterLockSlim>();
            TraceAll = new List<TraceListener>();

            AddTraceType(ListenerType.All);
            AddTraceType(ListenerType.Information);
            AddTraceType(ListenerType.Warning);
            AddTraceType(ListenerType.Error);
            AddTraceType(ListenerType.Problem);
            AddTraceType(ListenerType.Fatal);
            AddTraceType(ListenerType.Debug);
            AddTraceType(ListenerType.Database);
            AddTraceType(ListenerType.Sql);
        }

        #endregion

        #region TraceListener management

        /// <summary>
        /// Add a trace type
        /// </summary>
        /// <param name="type">String that represents the type</param>
        public static void AddTraceType(string type)
        {
            if (type == ListenerType.All)
                return;

            var listeners = new List<TraceListener>();
            var locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            Tracers.TryAdd(type, listeners);
            TraceLockers.TryAdd(type, locker);

            LockTraceAll.EnterReadLock();
            foreach (var listener in TraceAll)
            {
                InternalAddListener(type, listener);
            }
            LockTraceAll.ExitReadLock();
        }

        // Add a TraceListener to a type
        public static void AddTraceListener(string type, TraceListener traceListener)
        {
            if (type == ListenerType.All)
            {
                foreach (var tracerKey in Tracers.Keys)
                {
                    InternalAddListener(tracerKey, traceListener);
                }

                LockTraceAll.EnterWriteLock();
                TraceAll.Add(traceListener);
                LockTraceAll.ExitWriteLock();
            }
            else
            {
                InternalAddListener(type, traceListener);
            }
        }

        // Remove a TraceListener from a type
        public static void RemoveTraceListener(string type, TraceListener traceListener)
        {
            if (type == ListenerType.All)
            {
                LockTraceAll.EnterWriteLock();
                TraceAll.Remove(traceListener);
                LockTraceAll.ExitWriteLock();

                foreach (var tracerKey in Tracers.Keys)
                {
                    Tracers[tracerKey].Remove(traceListener);
                }
            }
            else
            {
                ReaderWriterLockSlim locker;

                if (TraceLockers.TryGetValue(type, out locker))
                {
                    locker.EnterWriteLock();
                    Tracers[type].Remove(traceListener);
                    locker.ExitWriteLock();
                }
            }
        }

        // Remove a TraceListener from a type
        public static void RemoveAllTraceListener()
        {
            foreach (var locker in TraceLockers)
            {
                locker.Value.EnterWriteLock();
                Tracers[locker.Key].Clear();
                locker.Value.ExitWriteLock();
            }
            TraceAll.Clear();
        }

        #endregion

        #region Our trace methods

        public static void Trace(string value, bool writeLine = true)
        {
            Trace(ListenerType.Information, value, writeLine);
        }

        public static void Trace(string traceType, string value, bool writeLine = true)
        {
            Action<TraceListener> traceAction = listener =>
            {
                if (listener is IAdvancedTraceListener)
                    if (writeLine)
                        ((IAdvancedTraceListener)listener).WriteLineEx(traceType, value);
                    else
                        ((IAdvancedTraceListener)listener).WriteEx(traceType, value);
                else
                    listener.WriteLine(value);
            };

            CommonWrite(traceType, traceAction);
        }

        public static void Trace(string traceType, string value, string userCategory, bool writeLine = true)
        {
            Action<TraceListener> traceAction = listener =>
            {
                if (listener is IAdvancedTraceListener)
                    if (writeLine)
                        ((IAdvancedTraceListener)listener).WriteLineEx(traceType, value, userCategory);
                    else
                        ((IAdvancedTraceListener)listener).WriteEx(traceType, value, userCategory);
                else
                    listener.WriteLine(value);
            };

            CommonWrite(traceType, traceAction);
        }

        public static void Trace(string traceType, object value, bool writeLine = true)
        {
            Action<TraceListener> traceAction = listener =>
            {
                if (listener is IAdvancedTraceListener)
                    if (writeLine)
                        ((IAdvancedTraceListener)listener).WriteLineEx(traceType, value);
                    else
                        ((IAdvancedTraceListener)listener).WriteEx(traceType, value);
                else
                    listener.WriteLine(value);
            };

            CommonWrite(traceType, traceAction);
        }

        public static void Trace(string traceType, object value, string userCategory, bool writeLine = true)
        {
            Action<TraceListener> traceAction = listener =>
            {
                if (listener is IAdvancedTraceListener)
                    if (writeLine)
                        ((IAdvancedTraceListener)listener).WriteLineEx(traceType, value, userCategory);
                    else
                        ((IAdvancedTraceListener)listener).WriteEx(traceType, value, userCategory);
                else
                    listener.WriteLine(value);
            };

            CommonWrite(traceType, traceAction);
        }

        public static void Trace(string traceType, string value, Exception exception, bool writeLine = true)
        {
            Action<TraceListener> traceAction = listener =>
            {
                if (listener is IAdvancedTraceListener)
                    if (writeLine)
                        ((IAdvancedTraceListener)listener).WriteLineEx(traceType, value, null, exception);
                    else
                        ((IAdvancedTraceListener)listener).WriteEx(traceType, value, null, exception);
                else
                    listener.WriteLine(value + " " + exception.ToString());
            };

            CommonWrite(traceType, traceAction);
        }

        public static void Trace(string traceType, string value, Exception exception, string userCategory, bool writeLine = true)
        {
            Action<TraceListener> traceAction = listener =>
            {
                if (listener is IAdvancedTraceListener)
                    if (writeLine)
                        ((IAdvancedTraceListener)listener).WriteLineEx(traceType, value, userCategory, exception);
                    else
                        ((IAdvancedTraceListener)listener).WriteEx(traceType, value, userCategory, exception);
                else
                    listener.WriteLine(value + " " + exception.ToString(), userCategory);
            };

            CommonWrite(traceType, traceAction);
        }

        public static void Trace(string[] traceTypeArray, string value)
        {
            for (int i = 0; traceTypeArray != null && i < traceTypeArray.Length; i++)
                Trace(traceTypeArray[i], value);
        }

        public static void Trace(string[] traceTypeArray, string value, string userCategory)
        {
            for (int i = 0; traceTypeArray != null && i < traceTypeArray.Length; i++)
                Trace(traceTypeArray[i], value, userCategory);
        }

        public static void Trace(string[] traceTypeArray, object value)
        {
            for (int i = 0; traceTypeArray != null && i < traceTypeArray.Length; i++)
                Trace(traceTypeArray[i], value);
        }

        public static void Trace(string[] traceTypeArray, object value, string userCategory)
        {
            for (int i = 0; traceTypeArray != null && i < traceTypeArray.Length; i++)
                Trace(traceTypeArray[i], value, userCategory);
        }

        public static void Trace(string[] traceTypeArray, string value, Exception exception)
        {
            if (exception == null)
            {
                Trace(traceTypeArray, value);
                return;
            }

            for (int i = 0; traceTypeArray != null && i < traceTypeArray.Length; i++)
                Trace(traceTypeArray[i], value, exception);
        }

        public static void Trace(string[] traceTypeArray, string value, Exception exception, string userCategory)
        {
            if (exception == null)
            {
                Trace(traceTypeArray, value, userCategory);
                return;
            }

            for (int i = 0; traceTypeArray != null && i < traceTypeArray.Length; i++)
                Trace(traceTypeArray[i], value, exception, userCategory);
        }

        public static void Flush()
        {
            Tracers.SelectMany(type => type.Value).Distinct().AsParallel().ForAll(listener => listener.Flush());
        }

        #endregion

        // Legacy Debug and Trace methods

        #region Debug.XXX / Trace.XXX

        public static void Write(string value)
        {
            Trace(value, false);
        }

        public static void Write(string value, string userCategory)
        {
            Trace(value, userCategory, false);
        }

        public static void Write(object value)
        {
            Trace(ListenerType.Information, value, false);
        }

        public static void Write(object value, string userCategory)
        {
            Trace(ListenerType.Information, value, userCategory, false);
        }

        public static void WriteLine(string value)
        {
            Trace(ListenerType.Information, value);
        }

        public static void WriteLine(string value, string userCategory)
        {
            Trace(ListenerType.Information, value, userCategory);
        }

        public static void WriteLine(object value)
        {
            Trace(ListenerType.Information, value);
        }

        public static void WriteLine(object value, string userCategory)
        {
            Trace(ListenerType.Information, value, userCategory);
        }

        #endregion

        #region Trace.TraceXXX

        public static void TraceInformation(string value)
        {
            Trace(ListenerType.Information, value);
        }

        public static void TraceWarning(string value)
        {
            Trace(ListenerType.Warning, value);
        }

        public static void TraceError(string value)
        {
            Trace(ListenerType.Error, value);
        }

        #endregion

        // Extended Trace methods

        #region TraceInformation

        public static void TraceInformation(string value, string userCategory)
        {
            Trace(ListenerType.Information, value, userCategory);
        }

        public static void TraceInformation(string value, Exception exception)
        {
            Trace(ListenerType.Information, value, exception);
        }

        public static void TraceInformation(string value, Exception exception, string userCategory)
        {
            Trace(ListenerType.Information, value, exception, userCategory);
        }

        #endregion

        #region TraceWarning

        public static void TraceWarning(string value, string userCategory)
        {
            Trace(ListenerType.Warning, value, userCategory);
        }

        public static void TraceWarning(string value, Exception exception)
        {
            Trace(ListenerType.Warning, value, exception);
        }

        public static void TraceWarning(string value, Exception exception, string userCategory)
        {
            Trace(ListenerType.Warning, value, exception, userCategory);
        }

        #endregion

        #region TraceError

        public static void TraceError(string value, string userCategory)
        {
            Trace(ListenerType.Error, value, userCategory);
        }

        public static void TraceError(string value, Exception exception)
        {
            Trace(ListenerType.Error, value, exception);
        }

        public static void TraceError(string value, Exception exception, string userCategory)
        {
            Trace(ListenerType.Error, value, exception, userCategory);
        }

        #endregion

        #region TraceProblem

        public static void TraceProblem(string value)
        {
            Trace(ListenerType.Problem, value);
        }

        public static void TraceProblem(string value, string userCategory)
        {
            Trace(ListenerType.Problem, value, userCategory);
        }

        public static void TraceProblem(string value, Exception exception)
        {
            Trace(ListenerType.Problem, value, exception);
        }

        public static void TraceProblem(string value, Exception exception, string userCategory)
        {
            Trace(ListenerType.Problem, value, exception, userCategory);
        }

        #endregion

        #region TraceFatal

        public static void TraceFatal(string value)
        {
            Trace(ListenerType.Fatal, value, string.Empty);
        }

        public static void TraceFatal(string value, string userCategory)
        {
            Trace(ListenerType.Fatal, value, userCategory);
        }

        public static void TraceFatal(string value, Exception exception)
        {
            Trace(ListenerType.Fatal, value, exception);
        }

        public static void TraceFatal(string value, Exception exception, string userCategory)
        {
            Trace(ListenerType.Fatal, value, exception, userCategory);
        }

        #endregion

        #region TraceDebug

        public static void TraceDebug(string value)
        {
            Trace(ListenerType.Debug, value);
        }

        public static void TraceDebug(string value, string userCategory)
        {
            Trace(ListenerType.Debug, value, userCategory);
        }

        public static void TraceDebug(string value, Exception exception)
        {
            Trace(ListenerType.Debug, value, exception);
        }

        public static void TraceDebug(string value, Exception exception, string userCategory)
        {
            Trace(ListenerType.Debug, value, exception, userCategory);
        }

        #endregion

        #region TraceDatabase

        public static void TraceDatabase(string value)
        {
            Trace(ListenerType.Database, value);
        }

        public static void TraceDatabase(string value, string userCategory)
        {
            Trace(ListenerType.Database, value, userCategory);
        }

        #endregion

        #region TraceSQL

        public static void TraceSql(string value)
        {
            Trace(ListenerType.Sql, value);
        }

        public static void TraceSql(string value, string userCategory)
        {
            Trace(ListenerType.Sql, value, userCategory);
        }

        #endregion

        private static void CommonWrite(string traceType, Action<TraceListener> traceAction)
        {
            ReaderWriterLockSlim locker;
            if (TraceLockers.TryGetValue(traceType, out locker))
            {
                locker.EnterReadLock();
                // Trace listeners added to Information type
                foreach (var tracer in Tracers[traceType])
                {
                    traceAction(tracer);
                }
                locker.ExitReadLock();
            }
        }

        private static void InternalAddListener(string type, TraceListener traceListener)
        {
            ReaderWriterLockSlim locker;
            if (TraceLockers.TryGetValue(type, out locker))
            {
                locker.EnterWriteLock();
                Tracers[type].Add(traceListener);
                locker.ExitWriteLock();
            }
        }

    }

    // Our base implementation of the TraceListener -> Used to build custom listener
    public abstract class AdvancedTraceListener : TraceListener, IAdvancedTraceListener
    {
        // The standard write methods will never be called on the AdvancedTraceListener
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void Write(string value) { }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void Write(string value, string userCategory) { }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void Write(object value) { }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void Write(object value, string userCategory) { }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void WriteLine(string value) { }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void WriteLine(string value, string userCategory) { }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void WriteLine(object value) { }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void WriteLine(object value, string userCategory) { }

        // Advanced trace method
        public virtual void WriteEx(string traceType, object value, string pstrUserCategory = null, Exception exception = null) { }
        public virtual void WriteLineEx(string traceType, object value, string pstrUserCategory = null, Exception exception = null) { }
    }

    public interface IAdvancedTraceListener
    {
        void WriteEx(string traceType, object value, string pstrUserCategory = null, Exception exception = null);
        void WriteLineEx(string traceType, object value, string pstrUserCategory = null, Exception exception = null);
    }

}
