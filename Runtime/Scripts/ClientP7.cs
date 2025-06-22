using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace P7
{
    [Serializable]
    public enum TimeType
    {
        System,
        Unity
    }

    [DefaultExecutionOrder(-999999)]
    [AddComponentMenu("P7/Client")]
    public class Client : MonoBehaviour
    {
        #region Inspector
        [Tooltip("Initialization parameters is a string. More info in docs...")]
        public string init_params = "/P7.Sink=Baical /P7.Addr=localhost";
        #endregion

        private API.Client __client = null;
        private API.Traces __trace = null;
        private List<API.Telemetry> _telemetries = new List<API.Telemetry>();

        private void Awake()
        {
            try
            {
                __client = new API.Client(init_params);
                __trace = new API.Traces(__client, "Debug");

                Application.logMessageReceivedThreaded += HandleLog;
                Debug.Log("Hello from P7 logger from Unity!");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                __client = null;
                enabled = false;
            }
        }

        private void OnDestroy()
        {
            if (__client != null)
            {
                lock (this)
                {
                    foreach (var t in _telemetries)
                        t?.Release();

                    _telemetries.Clear();
                }

                try
                {
                    __trace?.Release();
                }
                finally
                {
                    Application.logMessageReceivedThreaded -= HandleLog;
                    __trace = null;
                }

                try
                {
                    __client?.Release();
                }
                finally
                {
                    __client = null;
                }
            }
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (__trace is null)
                return;

            API.Traces.Level level;
            switch (type)
            {
                case LogType.Log: level = API.Traces.Level.INFO; break;
                case LogType.Warning: level = API.Traces.Level.WARNING; break;
                case LogType.Error: level = API.Traces.Level.ERROR; break;
                case LogType.Exception: level = API.Traces.Level.CRITICAL; break;
                default: level = API.Traces.Level.DEBUG; break;
            }

            __trace?.Add(0, level, IntPtr.Zero, 0, logString);
        }

        private ulong GetUnityTimeStamp(IntPtr i_pContext)
        {
            throw new NotImplementedException();
            //return (ulong)(Time.time * 1e9);
        }

        internal API.Telemetry TelemetryAdd(string name, TimeType tt = TimeType.System)
        {
            lock (this)
            {

                var t = new API.Telemetry(__client, name, tt == TimeType.Unity ? GetUnityTimeStamp : null);
                _telemetries.Add(t);
                return t;
            }
        }

        internal void TelemetryDel(API.Telemetry value)
        {
            if (value == null)
                return;

            lock (this)
            {
                _telemetries.Remove(value);
                value.Release();
            }
        }
    }
}

