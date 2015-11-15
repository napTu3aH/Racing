using System;
using System.Runtime.InteropServices;
using UnityEngine;
using AMLoggingConsole;

namespace AMLogging
{
	/// <summary>
	/// Класс для логирования, а так же сбора и передачи Unity логов в консоль в игре и в натив для OSX.
	/// </summary>
	public class AMLogger
	{

#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
		[DllImport("AMUnityMacLogging")]
		private static extern void _sendAMLog(string message);
#endif
		private string logPrefix = "";
		public static readonly AMLogger Logger = new AMLogger();
		public string handleLogOutput = "before handle";

		private AMLogger() {}
		private AMLogger(string logPrefix) {
			if (logPrefix != null) this.logPrefix = logPrefix;
		}

		/// <summary>
		/// Метод для создания инстанса логгера с префиксом.
		/// </summary>
		/// <ret<returns>Инстанс логгера с префиксом.</returns>
		/// <param name="logPrefix">Префикс логгера.</param>
		public static AMLogger GetInstance(string logPrefix) {
			return new AMLogger(logPrefix);
		}

		/// <summary>
		/// Метод для сбора Unity логов.
		/// </summary>
		/// <param name="logString">Текст лога.</param>
		/// <param name="stackTrace">Стэк трэйс лога.</param>
		/// <param name="type">Тип лога.</param>
		public void HandleLog(string logString, string stackTrace, LogType type) {
			CorrectLogs ();

			AMLoggerConsole.logs.Add (new AMLoggerConsole.Log {
				type = type,
				message = logString, 
				stackTrace = stackTrace, 
				logCount = 1,
			});
			handleLogOutput = logString;//for handler test

			CountLogs ();
		}

		/// <summary>
		/// Метод для контроля количества логов.
		/// </summary>
		void CorrectLogs() {
			if (AMLoggerConsole.logs.Count == 50) {
				AMLoggerConsole.logs.RemoveAt(0);
			}
			if (AMLoggerConsole.collapsedLogs.Count == 50) {
				AMLoggerConsole.collapsedLogs.RemoveAt(0);
			}
		}

		/// <summary>
		/// Метод для подсчета повторяющихся логов.
		/// </summary>
		void CountLogs () {
			AMLoggerConsole.tempLogs = AMLoggerConsole.logs;

			var logs = AMLoggerConsole.logs;
			var templogs = AMLoggerConsole.tempLogs;

			for (int i = 0; i < templogs.Count; i++) {
				var templog = templogs[i];
				int count = 1;
				
				for (int j = 0; j < logs.Count; j++) {
					if (j != i && templog.message == logs[j].message)
						count += 1;
				}
				templog.logCount = count;
				templogs[i] = templog;
			}
			AMLoggerConsole.tempLogs = templogs;
			CollapseLogs ();
		}

		/// <summary>
		/// Метод для группировки повторяющихся логов.
		/// </summary>
		void CollapseLogs () {
			var templogs = AMLoggerConsole.tempLogs;

			var templog = templogs[templogs.Count - 1];

			if (AMLoggerConsole.collapsedLogs.Count == 0) {
				if (templog.logCount == 1)
					AMLoggerConsole.collapsedLogs.Add(new AMLoggerConsole.Log {
						type = templog.type,
						message = templog.message,
						stackTrace = templog.stackTrace,
						logCount = templog.logCount,
					});
			}
			else if (AMLoggerConsole.collapsedLogs.Count != 0) {
				for (int j = 0; j < AMLoggerConsole.collapsedLogs.Count; j++)
					if (templogs[templogs.Count-1].message == AMLoggerConsole.collapsedLogs[j].message)
						continue;
					if (templog.logCount == 1)
						AMLoggerConsole.collapsedLogs.Add(new AMLoggerConsole.Log {
							message = templog.message,
							stackTrace = templog.stackTrace,
							type = templog.type,
							logCount = templog.logCount,
						});
			}
			CountCollapsedLogs ();
		}

		/// <summary>
		/// Метод для подсчета сгруппированных логов.
		/// </summary>
		void CountCollapsedLogs () {
			var collapsedlogs = AMLoggerConsole.collapsedLogs;
			var templogs = AMLoggerConsole.tempLogs;

			for (int i = 0; i < collapsedlogs.Count; i++) {
				var collapsedLog = collapsedlogs[i];
				
				for (int j = 0; j < templogs.Count; j++) {
					if (collapsedLog.message == templogs[j].message)
						collapsedLog.logCount = templogs[j].logCount;
					collapsedlogs[i] = collapsedLog;
				}
			}
			AMLoggerConsole.collapsedLogs = collapsedlogs;
		}

		/// <summary>
		/// Метод для вывода обычного сообщения в лог Unity и отправки в натив для вывода на OSX.
		/// </summary>
		/// <param name="message">Текст лога.</param>
		public void Log (string message) {
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			if (message == null) return;

			try {
				_sendAMLog(logPrefix + message);
				Debug.Log(logPrefix + message);
			}
			catch (Exception e) {
				Debug.Log(e);
			}
#else
			if (message == null) return;
			
			try {
				Debug.Log (logPrefix + message);
			} 
			catch (Exception e) {
				Debug.LogException(e);
			}
#endif
		}

		/// <summary>
		/// Метод для вывода предупреждающего сообщения в лог Unity и отправки в натив для вывода на OSX.
		/// </summary>
		/// <param name="message">Текст лога.</param>
		public void LogWarning (string message) {
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			if (message == null) return;
			
			try {
				_sendAMLog(logPrefix + message);
				Debug.LogWarning(logPrefix + message);
			}
			catch (Exception e) {
				Debug.Log(e);
			}
#else
			if (message == null) return;
			
			try {
				Debug.LogWarning (logPrefix + message);
			} 
			catch (Exception e) {
				Debug.LogException(e);
			}
#endif
		}

		/// <summary>
		/// Метод для вывода сообщения об ошибке в лог Unity и отправки в натив для вывода на OSX.
		/// </summary>
		/// <param name="message">Текст лога.</param>
		public void LogError (string message) {
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			if (message == null) return;
			
			try {
				_sendAMLog(logPrefix + message);
				Debug.LogError(logPrefix + message);
			}
			catch (Exception e) {
				Debug.Log(e);
			}
#else
			if (message == null) return;
			
			try {
				Debug.LogError (logPrefix + message);
			} 
			catch (Exception e) {
				Debug.LogException(e);
			}
#endif
		}

		/// <summary>
		/// Метод для вывода исключения в лог Unity.
		/// </summary>
		/// <param name="message">Текст лога.</param>
		public void LogException (Exception exception) {
			if (exception == null) return;
			
			try {
				Debug.LogException (exception);
			}
			catch (Exception e) {
				Debug.LogException(e);
			}
		}
	}
}