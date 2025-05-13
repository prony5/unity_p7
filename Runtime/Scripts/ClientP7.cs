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
        private List<API.Telemetry> _telemetries = new List<API.Telemetry>();

        private void Awake()
        {
            try
            {
                __client = new API.Client(init_params);
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
                    {
                        if (t != null)
                            t.Release();
                    }

                    _telemetries.Clear();
                }

                __client.Release();
                __client = null;
            }
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

