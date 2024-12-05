using System;
using UnityEngine;

namespace Marsion
{
    public static class Logger
    {
        #region Fields and Properties
        private static ILogger _logger = new DefaultLogger();

        const char Period = '.';
        const string OpenColor = ": <color={0}><b>";
        const string CloseColor = "</b></color>";

        #endregion

        public static void SetLogger(ILogger customLogger)
        {
            _logger = customLogger;
        }

        #region Log Methods

        public static void Log<T>(string log, string colorName = "black", Type param = null)
        {
            LogInternal<T>(log, colorName, param, _logger.Log);
        }

        public static void LogWarning<T>(string log, string colorName = "black", Type param = null)
        {
            LogInternal<T>(log, colorName, param, _logger.LogWarning);
        }

        public static void LogError<T>(string log, string colorName = "black", Type param = null)
        {
            LogInternal<T>(log, colorName, param, _logger.LogError);
        }

        public static void LogPointer<T>(string log, string colorName = "yellow", Type param = null)
        {
            LogInternal<T>(log, colorName, param, _logger.Log);
        }

        public static void LogState<T>(string log, string colorName = "yellow", Type param = null) where T : BaseStateMachine
        {
            LogInternal<T>(log, colorName, param, _logger.Log);
        }

        public static void LogSequence(string name, string log, bool isServer)
        {
            string color = isServer ? ColorCodes.ServerSequencer : ColorCodes.ClientSequencer;
            _logger.Log($"<color={color}><b>[{name}]</b></color> {log}");
        }

        private static void LogInternal<T>(string log, string colorName, Type param, Action<string> logAction)
        {
            var context = GetTypeName(typeof(T));
            string formattedLog = FormatLogMessage(context, log, colorName, param);
            logAction(formattedLog);
        }

        private static string FormatLogMessage(string context, string log, string colorName, Type param)
        {
            string openColorFormatted = string.Format(OpenColor, colorName);
            string formattedLog = $"[{context}] {openColorFormatted}{log}{CloseColor}";

            if(param != null)
            {
                formattedLog += GetTypeName(param);
            }

            return formattedLog;
        }

        static string GetTypeName(Type type)
        {
            if (type == null)
                return string.Empty;

            var split = type.ToString().Split(Period);
            return split.Length > 0 ? split[^1] : string.Empty;
        }

        #endregion
    }
}