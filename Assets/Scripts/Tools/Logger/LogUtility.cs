using System;
using UnityEngine;

namespace Marsion
{
    public class LogUtility
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
            if (AreLogsEnabled)
            {
                var context = GetTypeName(typeof(T));

                // OpenColor에 colorName을 적용하여 색상 값 삽입
                string openColorFormatted = string.Format(OpenColor, colorName);

                // 로그 메시지를 포맷에 맞춰 작성
                log = string.Format("[{0}] {1}{2}{3}", context, openColorFormatted, log, CloseColor);

                // param이 null이 아닌 경우 타입 이름 추가
                if (param != null)
                {
                    log += GetTypeName(param);
                }

                // 로그 출력
                Debug.Log(log);
            }
        }

        public void LogWarning<T>(object log, string colorName = "black", Type param = null)
        {
            if (AreLogsEnabled)
            {
                var context = GetTypeName(typeof(T));

                // OpenColor에 colorName을 적용하여 색상 값 삽입
                string openColorFormatted = string.Format(OpenColor, colorName);

                // 로그 메시지를 포맷에 맞춰 작성
                log = string.Format("[{0}] {1}{2}{3}", context, openColorFormatted, log, CloseColor);

                // param이 null이 아닌 경우 타입 이름 추가
                if (param != null)
                {
                    log += GetTypeName(param);
                }

                Debug.LogWarning(log);
            }

        }

        public void LogError<T>(object log, string colorName = "black", Type param = null)
        {
            if (AreLogsEnabled)
            {
                var context = GetTypeName(typeof(T));

                // OpenColor에 colorName을 적용하여 색상 값 삽입
                string openColorFormatted = string.Format(OpenColor, colorName);

                // 로그 메시지를 포맷에 맞춰 작성
                log = string.Format("[{0}] {1}{2}{3}", context, openColorFormatted, log, CloseColor);

                // param이 null이 아닌 경우 타입 이름 추가
                if (param != null)
                {
                    log += GetTypeName(param);
                }

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