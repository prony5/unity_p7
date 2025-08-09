using System;
using System.Collections.Generic;

using UnityEngine;

namespace P7
{
    [DefaultExecutionOrder(-999998)]
    [AddComponentMenu("P7/Telemetry")]
    public class Telemetry : MonoBehaviour
    {
        [Serializable]
        public class CounterSettings
        {
            [Tooltip("Name of counter, max length 64 characters, should be unique for current channel (case sensitive)")]
            public string name;
            [Tooltip("Minimal counter value, helping information for visualization, later you can override it in telemetry viewer")]
            public double min = 0;
            [Tooltip("Alarm counter value, values below will be highlighted")]
            public double a_min = double.NegativeInfinity;
            [Tooltip("Maximum counter value, helping information for visualization, later you can override it in telemetry viewer")]
            public double max = 1;
            [Tooltip("Alarm counter value, values above will be highlighted")]
            public double a_max = double.PositiveInfinity;
            [Tooltip("Parameter specifies is counter enabled or disabled by default, later you can enable/disable it in real time from Baical server")]
            public bool enabled = true;
        }

        #region Inspector
        [SerializeField]
        private Client client;
        [SerializeField]
        private string id = "Telemetry 1";
        [SerializeField]
        private TimeType timeType = TimeType.System;
        [SerializeField]
        private List<CounterSettings> values = new List<CounterSettings>();
        #endregion

        private API.Telemetry _telemetry = null;
        private List<Counter> _counters = new List<Counter>();
        private Dictionary<string, ushort> _countersKey = new Dictionary<string, ushort>();


        public void setClient(Client client)
        {
            if (client is null)
                return;

            this.client = client;

            _telemetry = this.client.TelemetryAdd(id, timeType);
            foreach (var c in values)
                CounterAdd(c);
        }

        private void Awake()
        {
            setClient(client);
        }

        private void OnApplicationQuit()
        {
            try
            {
                client?.TelemetryDel(_telemetry);
            }
            finally
            {
                Disposed = true;
                _telemetry = null;
            }
        }

        public void SetClient(Client value)
        {
            lock (this)
            {
                if (client)
                {
                    _counters.Clear();
                    _countersKey.Clear();
                    client.TelemetryDel(_telemetry);
                    _telemetry = null;
                }
                client = value;
            }

            if (client)
            {
                _telemetry = client?.TelemetryAdd(id, timeType);
                foreach (var c in values)
                    CounterAdd(c);
            }
        }

        public bool Disposed { get; private set; } = false;

        public Counter this[int index]
        {
            get
            {
                lock (_counters)
                {
                    if (index < 0 || index >= _counters.Count)
                        return null;
                    else
                        return _counters[index];
                }
            }
        }

        public Counter this[string index]
        {
            get
            {
                lock (_counters)
                {
                    if (_countersKey.ContainsKey(index))
                        return _counters[_countersKey[index]];
                    else
                        return null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get
            {
                lock (_counters)
                {
                    return _counters.Count;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public int CounterAdd(CounterSettings value)
        {
            if (_telemetry == null)
                return -1;

            ushort counter_id = 0;
            if (_telemetry.Create(value.name, value.min, value.a_min, value.max, value.a_max, value.enabled ? 1 : 0, ref counter_id))
            {
                lock (_counters)
                {
                    _counters.Add(new Counter(this, value, counter_id));
                    _countersKey.Add(value.name, counter_id);
                    return _counters.Count - 1;
                }
            }
            else
                return -1;
        }

        [Serializable]
        public class Counter
        {
            #region Inspector
            [SerializeField]
            private CounterSettings _settings = new CounterSettings();
            #endregion

            private System.UInt16 _id;
            private Telemetry _telemetry;

            public Counter(Telemetry telemetry, CounterSettings settings, ushort id)
            {
                _telemetry = telemetry;
                _settings = settings;
                _id = id;
            }

            public CounterSettings GetSettings()
            {
                return _settings;
            }

            public bool Add(double value)
            {
                if (_telemetry.Disposed)
                    return false;

                return _telemetry._telemetry.Add(_id, value);
            }
        }
    }
}

