using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AdvancedTraceLib
{
    public static class AdvancedTrace
    {
        private static readonly Dictionary<string, Dictionary<Type, List<TraceListener>>> Tracers;
        private static readonly List<TraceListener> TraceAll;

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
            public const string SQL = "__ADVANCED_TRACE_SQL__";
        }

        #region Default static constructor

        static AdvancedTrace()
        {
            Tracers = new Dictionary<string, Dictionary<Type, List<TraceListener>>>();
            TraceAll = new List<TraceListener>();

            AddTraceType(ListenerType.Information);
            AddTraceType(ListenerType.Warning);
            AddTraceType(ListenerType.Error);
            AddTraceType(ListenerType.Problem);
            AddTraceType(ListenerType.Fatal);
            AddTraceType(ListenerType.Debug);
            AddTraceType(ListenerType.Database);
            AddTraceType(ListenerType.SQL);
        }

        #endregion

        #region TraceListener management

        // Add a trace type
        public static void AddTraceType(string type)
        {
            if (type == ListenerType.All)
                return;

            lock (Tracers)
            {
                if (Tracers.ContainsKey(type))
                    throw new ArgumentException("The trace type is already registred.");

                Tracers[type] = new Dictionary<Type, List<TraceListener>>();
            }

            lock (TraceAll)
            {
                foreach (var listener in TraceAll)
                    InternalAddListener(type, listener);
            }
        }

        // Add a TraceListener to a type
        public static void AddTraceListener(string type, TraceListener traceListener)
        {
            if (type == ListenerType.All)
            {
                lock (Tracers)
                {
                    foreach (var tracerKey in Tracers.Keys)
                        InternalAddListener(tracerKey, traceListener);
                }

                lock (TraceAll)
                {
                    TraceAll.Add(traceListener);
                }
            }
            else
            {
                lock (Tracers)
                {
                    if (!Tracers.ContainsKey(type))
                        throw new ArgumentException(string.Format("The trace type '{0}' must be registred before adding a trace listener.", type));

                    InternalAddListener(type, traceListener);
                }
            }
        }

        // Remove a TraceListener from a type
        public static void RemoveTraceListener(string type, TraceListener traceListener)
        {
            var traceListenerType = traceListener.GetType();

            if (type == ListenerType.All)
            {
                lock (TraceAll)
                {
                    TraceAll.Remove(traceListener);
                }

                lock (Tracers)
                {
                    foreach (var tracerKey in Tracers.Keys)
                    {
                        lock (Tracers[tracerKey])
                        {
                            Tracers[tracerKey][traceListenerType].Remove(traceListener);
                        }
                    }
                }
            }
            else
            {
                lock (Tracers[type])
                {
                    if (!Tracers.ContainsKey(type))
                        throw new ArgumentException("Cannot remove the trace listener as the trace type is unknown.");

                    Tracers[type][traceListenerType].Remove(traceListener);
                }
            }
        }

        // Remove all trace listeners
        public static void RemoveAllTraceListener()
        {
            lock (TraceAll)
            {
                TraceAll.Clear();
            }

            lock (Tracers)
            {
                Tracers.Clear();
            }
        }

        #endregion

        #region Our trace methods

        public static void Trace(string type, string trace, bool writeLine = true)
        {
            Action<TraceListener> traceAction = listener =>
            {
                if (listener is AdvancedTraceListener)
                {
                    if (writeLine)
                        ((AdvancedTraceListener)listener).WriteLineEx(type, trace);
                    else
                        ((AdvancedTraceListener)listener).WriteEx(type, trace);
                }
                else
                {
                    if (writeLine)
                        listener.WriteLine(trace);
                    else
                        listener.Write(trace);
                }
            };

            CommonWrite(type, traceAction);
        }

        public static void Trace(string type, string trace, string category, bool writeLine = true)
        {
            Action<TraceListener> traceAction = listener =>
            {
                if (listener is AdvancedTraceListener)
                {
                    if (writeLine)
                        ((AdvancedTraceListener)listener).WriteLineEx(type, trace, category);
                    else
                        ((AdvancedTraceListener)listener).WriteEx(type, trace, category);
                }
                else
                {
                    if (writeLine)
                        listener.WriteLine(trace, category);
                    else
                        listener.Write(trace, category);
                }
            };

            CommonWrite(type, traceAction);
        }

        public static void Trace(string type, object trace, bool writeLine = true)
        {
            Action<TraceListener> traceAction = listener =>
            {
                if (listener is AdvancedTraceListener)
                {
                    if (writeLine)
                        ((AdvancedTraceListener)listener).WriteLineEx(type, trace);
                    else
                        ((AdvancedTraceListener)listener).WriteEx(type, trace);
                }
                else
                {
                    if (writeLine)
                        listener.WriteLine(trace);
                    else
                        listener.Write(trace);
                }
            };

            CommonWrite(type, traceAction);
        }

        public static void Trace(string type, object trace, string category, bool writeLine = true)
        {
            Action<TraceListener> traceAction = listener =>
            {
                if (listener is AdvancedTraceListener)
                {
                    if (writeLine)
                        ((AdvancedTraceListener)listener).WriteLineEx(type, trace, category);
                    else
                        ((AdvancedTraceListener)listener).WriteEx(type, trace, category);
                }
                else
                {
                    if (writeLine)
                        listener.WriteLine(trace, category);
                    else
                        listener.Write(trace, category);
                }
            };

            CommonWrite(type, traceAction);
        }

        public static void Trace(string type, string trace, Exception exception, bool writeLine = true)
        {
            Action<TraceListener> traceAction = listener =>
            {
                if (listener is AdvancedTraceListener)
                {
                    if (writeLine)
                        ((AdvancedTraceListener)listener).WriteLineEx(type, trace, exception);
                    else
                        ((AdvancedTraceListener)listener).WriteEx(type, trace, exception);
                }
                else
                {
                    if (writeLine)
                        listener.WriteLine(trace + " " + exception);
                    else
                        listener.Write(trace + " " + exception);
                }
            };

            CommonWrite(type, traceAction);
        }

        public static void Trace(string type, string trace, Exception exception, string category, bool writeLine = true)
        {
            Action<TraceListener> traceAction = listener =>
            {
                if (listener is AdvancedTraceListener)
                {
                    if (writeLine)
                        ((AdvancedTraceListener)listener).WriteLineEx(type, trace, exception, category);
                    else
                        ((AdvancedTraceListener)listener).WriteEx(type, trace, exception, category);
                }
                else
                {
                    if (writeLine)
                        listener.WriteLine(trace + " " + exception, category);
                    else
                        listener.Write(trace + " " + exception, category);
                }
            };

            CommonWrite(type, traceAction);
        }

        public static void Trace(string[] types, string trace, bool writeLine = true)
        {
            for (int i = 0; types != null && i < types.Length; i++)
                Trace(types[i], trace, writeLine);
        }

        public static void Trace(string[] types, string trace, string category, bool writeLine = true)
        {
            for (int i = 0; types != null && i < types.Length; i++)
                Trace(types[i], trace, category, writeLine);
        }

        public static void Trace(string[] types, object trace, bool writeLine = true)
        {
            for (int i = 0; types != null && i < types.Length; i++)
                Trace(types[i], trace, writeLine);
        }

        public static void Trace(string[] types, object trace, string category, bool writeLine = true)
        {
            for (int i = 0; types != null && i < types.Length; i++)
                Trace(types[i], trace, category, writeLine);
        }

        public static void Trace(string[] types, string trace, Exception exception, bool writeLine = true)
        {
            if (exception == null)
            {
                Trace(types, trace, writeLine);

                return;
            }

            for (int i = 0; types != null && i < types.Length; i++)
                Trace(types[i], trace, exception, writeLine);
        }

        public static void Trace(string[] types, string trace, Exception exception, string category, bool writeLine = true)
        {
            if (exception == null)
            {
                Trace(types, trace, category, writeLine);

                return;
            }

            for (int i = 0; types != null && i < types.Length; i++)
                Trace(types[i], trace, exception, category, writeLine);
        }

        public static void Flush()
        {
            lock (Tracers)
            {
                Tracers.SelectMany(type => type.Value).SelectMany(listenerType => listenerType.Value).AsParallel().ForAll(listener => listener.Flush());
            }
        }

        #endregion

        #region Tools

        private static void CommonWrite(string type, Action<TraceListener> traceAction)
        {
            Dictionary<Type, List<TraceListener>> listeners;

            lock (Tracers)
            {
                if (!Tracers.TryGetValue(type, out listeners))
                    return;
            }

            // Trace listeners added to Information type
            lock (listeners)
                listeners.AsParallel().ForAll(dict => dict.Value.ForEach(traceAction));
        }

        private static void InternalAddListener(string type, TraceListener traceListener)
        {
            var listenerType = traceListener.GetType();

            lock (Tracers[type])
            {
                if (!Tracers[type].ContainsKey(listenerType))
                    Tracers[type].Add(listenerType, new List<TraceListener>());

                Tracers[type][listenerType].Add(traceListener);
            }
        }

        #endregion

        // Legacy Debug and Trace methods

        #region Debug.XXX / Trace.XXX

        public static void Write(string trace)
        {
            Trace(ListenerType.Information, trace, false);
        }

        public static void Write(string trace, string category)
        {
            Trace(ListenerType.Information, trace, category, false);
        }

        public static void Write(object trace)
        {
            Trace(ListenerType.Information, trace, false);
        }

        public static void Write(object trace, string category)
        {
            Trace(ListenerType.Information, trace, category, false);
        }

        public static void WriteLine(string trace)
        {
            Trace(ListenerType.Information, trace);
        }

        public static void WriteLine(string trace, string category)
        {
            Trace(ListenerType.Information, trace, category);
        }

        public static void WriteLine(object trace)
        {
            Trace(ListenerType.Information, trace);
        }

        public static void WriteLine(object trace, string category)
        {
            Trace(ListenerType.Information, trace, category);
        }

        #endregion

        // Extended Trace methods

        #region TraceInformation

        public static void TraceInformation(string trace)
        {
            Trace(ListenerType.Information, trace);
        }

        public static void TraceInformation(string trace, string category)
        {
            Trace(ListenerType.Information, trace, category);
        }

        public static void TraceInformation(string trace, Exception exception)
        {
            Trace(ListenerType.Information, trace, exception);
        }

        public static void TraceInformation(string trace, Exception exception, string category)
        {
            Trace(ListenerType.Information, trace, exception, category);
        }

        #endregion

        #region TraceWarning

        public static void TraceWarning(string trace)
        {
            Trace(ListenerType.Warning, trace);
        }

        public static void TraceWarning(string trace, string category)
        {
            Trace(ListenerType.Warning, trace, category);
        }

        public static void TraceWarning(string trace, Exception exception)
        {
            Trace(ListenerType.Warning, trace, exception);
        }

        public static void TraceWarning(string trace, Exception exception, string category)
        {
            Trace(ListenerType.Warning, trace, exception, category);
        }

        #endregion

        #region TraceError

        public static void TraceError(string trace)
        {
            Trace(ListenerType.Error, trace);
        }

        public static void TraceError(string trace, string category)
        {
            Trace(ListenerType.Error, trace, category);
        }

        public static void TraceError(string trace, Exception exception)
        {
            Trace(ListenerType.Error, trace, exception);
        }

        public static void TraceError(string trace, Exception exception, string category)
        {
            Trace(ListenerType.Error, trace, exception, category);
        }

        #endregion

        #region TraceProblem

        public static void TraceProblem(string trace)
        {
            Trace(ListenerType.Problem, trace);
        }

        public static void TraceProblem(string trace, string category)
        {
            Trace(ListenerType.Problem, trace, category);
        }

        public static void TraceProblem(string trace, Exception exception)
        {
            Trace(ListenerType.Problem, trace, exception);
        }

        public static void TraceProblem(string trace, Exception exception, string category)
        {
            Trace(ListenerType.Problem, trace, exception, category);
        }

        #endregion

        #region TraceFatal

        public static void TraceFatal(string trace)
        {
            Trace(ListenerType.Fatal, trace, string.Empty);
        }

        public static void TraceFatal(string trace, string category)
        {
            Trace(ListenerType.Fatal, trace, category);
        }

        public static void TraceFatal(string trace, Exception exception)
        {
            Trace(ListenerType.Fatal, trace, exception);
        }

        public static void TraceFatal(string trace, Exception exception, string category)
        {
            Trace(ListenerType.Fatal, trace, exception, category);
        }

        #endregion

        #region TraceDebug

        public static void TraceDebug(string trace)
        {
            Trace(ListenerType.Debug, trace);
        }

        public static void TraceDebug(string trace, string category)
        {
            Trace(ListenerType.Debug, trace, category);
        }

        public static void TraceDebug(string trace, Exception exception)
        {
            Trace(ListenerType.Debug, trace, exception);
        }

        public static void TraceDebug(string trace, Exception exception, string category)
        {
            Trace(ListenerType.Debug, trace, exception, category);
        }

        #endregion

        #region TraceDatabase

        public static void TraceDatabase(string trace)
        {
            Trace(ListenerType.Database, trace);
        }

        public static void TraceDatabase(string trace, string category)
        {
            Trace(ListenerType.Database, trace, category);
        }

        #endregion

        #region TraceSQL

        public static void TraceSQL(string trace)
        {
            Trace(ListenerType.SQL, trace);
        }

        public static void TraceSQL(string trace, string category)
        {
            Trace(ListenerType.SQL, trace, category);
        }

        #endregion
    }
}