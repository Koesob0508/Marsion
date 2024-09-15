using System;
using UnityEngine;

namespace Marsion
{
    public class Logger
    {
        #region Fields and Properties

        [SerializeField] bool AreLogsEnabled = true;
        const char Period = '.';
        const string OpenColor = "]: <color={0}><b>";
        const string CloseColor = "</b></color>";

        #endregion

        #region Log

        public void Log<T>(object log, string colorName = "black", Type param = null)
        {
            if(AreLogsEnabled)
            {
                var context = GetTypeName(typeof(T));
                log = string.Format("[" + context + OpenColor + log + CloseColor + GetTypeName(param), colorName);
                Debug.Log(log);
            }
        }

        public void LogWarning<T>(object log, string colorName = "black", Type param = null)
        {
            if (AreLogsEnabled)
            {
                var context = GetTypeName(typeof(T));
                log = string.Format("[" + context + OpenColor + log + CloseColor + GetTypeName(param), colorName);
                Debug.LogWarning(log);
            }

        }

        public void LogError<T>(object log, string colorName = "black", Type param = null)
        {
            if (AreLogsEnabled)
            {
                var context = GetTypeName(typeof(T));
                log = string.Format("[" + context + OpenColor + log + CloseColor + GetTypeName(param), colorName);
                Debug.LogError(log);
            }

        }

        public void LogPointer<T>(object log, string colorName = "yellow", Type param = null)
        {
            if (AreLogsEnabled)
            {
                var context = GetTypeName(typeof(T));
                log = string.Format("[" + context + OpenColor + log + CloseColor + GetTypeName(param), colorName);
                // Debug.Log(log);
            }
        }

        public void LogState<T>(object log, string colorName = "yellow", Type param = null) where T : BaseStateMachine
        {
            if (AreLogsEnabled)
            {
                var context = GetTypeName(typeof(T));
                log = string.Format("[" + context + OpenColor + log + CloseColor + GetTypeName(param), colorName);
                // Debug.Log(log);
            }
        }

        #endregion

        #region Util

        static string GetTypeName(Type type)
        {
            if (type == null)
                return string.Empty;

            var split = type.ToString().Split(Period);
            var last = split.Length - 1;
            return last > 0 ? split[last] : string.Empty;
        }

        #endregion
    }
}