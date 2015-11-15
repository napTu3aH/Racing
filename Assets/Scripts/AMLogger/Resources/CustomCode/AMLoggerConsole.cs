using System.Collections.Generic;
using UnityEngine;
using AMLogging;

namespace AMLoggingConsole
{
	/// <summary>
	/// Консоль для отображения Unity логов в игре.
	/// </summary>
	public class AMLoggerConsole : MonoBehaviour
	{
		public static AMLoggerConsole Instance = null;

		public struct Log
		{
			public LogType type;
			public string message;
			public string stackTrace;
			public int logCount;
		}

		#region Inspector Settings
		/// <summary>
		/// Чекбокс для включения/отключения консоли.
		/// </summary>
		public bool debugMode = false;

		/// <summary>
		/// Горячая клавиша для отображения/скрытия консоли.
		/// </summary>
		public KeyCode toggleKey = KeyCode.BackQuote;
		
		/// <summary>
		/// Сила, с которой следует встряхнуть девайс для отображения/скрытия консоли.
		/// </summary>
		public float shakeAcceleration = 5f;
		#endregion

		public static bool ccDebugMode;
		public static KeyCode ccToggleKey;
		public static float ccShakeAcceleration;
		public static bool customCodeEnable = false;
		
		public static readonly List<Log> logs = new List<Log>();
		public static List<Log> tempLogs = new List<Log>();
		public static List<Log> collapsedLogs = new List<Log>();
		
		TextAsset infoAsset;
		TextAsset errorAsset;
		TextAsset warningAsset;
		TextAsset checkboxTrueAsset;
		TextAsset checkboxFalseAsset;
		
		static Texture2D infoIcon;
		static Texture2D errorIcon;
		static Texture2D warningIcon;
		static Texture2D checkboxTrueImage;
		static Texture2D checkboxFalseImage;
		
		Vector2 scrollPosition;
		bool visible;
		bool isCollapsed;
		bool showStackTrace;
		string isCollapsedLebel;
				
		// Визуальные элементы:
				
		static Dictionary<LogType, Texture2D> logTypeIcons;
		
		const string windowTitle = "AMLoggerConsole";
		const int margin = 20;
		static readonly GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
		static readonly GUIContent stackTraceLabel = new GUIContent("StackTrace", "Show log stack traces.");
		static readonly GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");
		
		readonly Rect titleBarRect = new Rect(0, 0, 10000, Screen.width/20);
		Rect windowRect = new Rect(margin, margin, Screen.width - (margin * 2), Screen.height - (margin * 2));
		/// <summary>
		/// Init this instance.
		/// </summary>
		void Init()
		{
			if (Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad(this);

				if (customCodeEnable) {
					debugMode = ccDebugMode;
					toggleKey = ccToggleKey;
					shakeAcceleration = ccShakeAcceleration;
				}

				if (debugMode) {
#if UNITY_5_0_2
					Application.logMessageReceived += AMLogger.Logger.HandleLog;
#elif UNITY_5_0
					Application.logMessageReceived += AMLogger.Logger.HandleLog;
					Application.logMessageReceivedThreaded += AMLogger.Logger.HandleLog;
#else 
					Application.RegisterLogCallback(new Application.LogCallback (AMLogger.Logger.HandleLog));
					Application.RegisterLogCallbackThreaded (new Application.LogCallback (AMLogger.Logger.HandleLog));
#endif
					visible = true;
				} else {
#if UNITY_5_0_2
					Application.logMessageReceived -= AMLogger.Logger.HandleLog;
#elif UNITY_5_0
					Application.logMessageReceived -= AMLogger.Logger.HandleLog;
					Application.logMessageReceivedThreaded -= AMLogger.Logger.HandleLog;
#else 
					Application.RegisterLogCallback(null);
					Application.RegisterLogCallbackThreaded (null);
#endif
				}
			}
			else
			{
				Destroy(gameObject);
			}	
		}

		/// <summary>
		/// Start this instance.
		/// </summary>
		void Start()
		{
			Init ();

			infoIcon = new Texture2D (200, 200);
			errorIcon = new Texture2D (200, 200);
			warningIcon = new Texture2D (200, 200);
			checkboxFalseImage = new Texture2D (200, 200);
			checkboxTrueImage = new Texture2D (200, 200);

			string imagePath = "Image/";

			infoAsset = Resources.Load (imagePath+"info") as TextAsset;
			errorAsset = Resources.Load (imagePath+"error") as TextAsset;
			warningAsset = Resources.Load (imagePath+"warning") as TextAsset;
			checkboxFalseAsset = Resources.Load (imagePath+"checkbox_false") as TextAsset;
			checkboxTrueAsset = Resources.Load (imagePath+"checkbox_true") as TextAsset;
			
			errorIcon.LoadImage(errorAsset.bytes);
			GetComponent<Renderer>().material.mainTexture = errorIcon;

			warningIcon.LoadImage(warningAsset.bytes);
			GetComponent<Renderer>().material.mainTexture = warningIcon;
			
			infoIcon.LoadImage(infoAsset.bytes);
			GetComponent<Renderer>().material.mainTexture = infoIcon;

			checkboxFalseImage.LoadImage(checkboxFalseAsset.bytes);
			GetComponent<Renderer>().material.mainTexture = checkboxFalseImage;

			checkboxTrueImage.LoadImage(checkboxTrueAsset.bytes);
			GetComponent<Renderer>().material.mainTexture = checkboxTrueImage;
			
			logTypeIcons = new Dictionary<LogType, Texture2D>
			{
				{ LogType.Assert, infoIcon },
				{ LogType.Error, errorIcon },
				{ LogType.Exception, errorIcon },
				{ LogType.Log, infoIcon },
				{ LogType.Warning, warningIcon },
			};
		}
		/// <summary>
		/// Update this instance.
		/// </summary>
		void Update ()
		{
			if (customCodeEnable) {
				debugMode = ccDebugMode;
				toggleKey = ccToggleKey;
				shakeAcceleration = ccShakeAcceleration;
			}
			if (debugMode) {
				if (Input.GetKeyDown (toggleKey))
					visible = !visible;
				else if (Input.acceleration.sqrMagnitude > shakeAcceleration)
					visible = !visible;
			} else {
				visible = false;
				logs.Clear();
				tempLogs.Clear();
				collapsedLogs.Clear();
			}
		}
		/// <summary>
		/// Raises the GUI event.
		/// </summary>
		void OnGUI ()
		{
			if (!visible) {
				return;
			}

			windowRect = GUILayout.Window(123456, windowRect, ConsoleWindow, windowTitle);
		}		
		
		/// <summary>
		/// Окно консоли, отображающее логи.
		/// </summary>
		/// <param name="windowID">ID окна консоли.</param>
		void ConsoleWindow (int windowID)
		{
			GUIStyle boxStyle = GUI.skin.GetStyle ("box");
			boxStyle.fontSize = (int)Screen.width/30;
			boxStyle.fixedWidth = 0;
			boxStyle.fixedHeight = 0;

			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			
			if (isCollapsed) {
				for (int i = 0; i < collapsedLogs.Count; i++) {
					var collapsedLog = collapsedLogs[i];
					
					if (i > 0 && collapsedLog.message == collapsedLogs[i - 1].message)
						continue;
					
					GUILayout.BeginHorizontal();
					
					GUIStyle labelStyle = GUI.skin.GetStyle ("label");
					labelStyle.fontSize = (int)Screen.width/25;
					
					GUILayout.Box (logTypeIcons[collapsedLog.type], new GUILayoutOption[]{
																		GUILayout.ExpandWidth(false),
																		GUILayout.MaxHeight(windowRect.width/20),
																		GUILayout.MaxWidth(windowRect.width/20)});
					if (showStackTrace)
						GUILayout.Label(collapsedLog.stackTrace);
					else
						GUILayout.Label(collapsedLog.message);
					GUILayout.Box(collapsedLog.logCount.ToString(), new GUILayoutOption[]{
																		GUILayout.ExpandWidth(false),
																		GUILayout.MinHeight(windowRect.width/20),
																		GUILayout.MinWidth(windowRect.width/20)});					
					GUILayout.EndHorizontal();
				}
			}
			else if (!isCollapsed){
				for (int i = 0; i < logs.Count; i++) {
					var log = logs[i];
					
					GUILayout.BeginHorizontal();
					
					GUIStyle labelStyle = GUI.skin.GetStyle ("label");
					labelStyle.fontSize = (int)Screen.width/25;
					
					GUILayout.Box (logTypeIcons[log.type], new GUILayoutOption[]{
																GUILayout.ExpandWidth(false),
																GUILayout.MaxHeight(windowRect.width/20),
																GUILayout.MaxWidth(windowRect.width/20)});
					if (showStackTrace)
						GUILayout.Label(log.stackTrace);
					else
						GUILayout.Label(log.message);
					
					GUILayout.EndHorizontal();
				}
			}
			
			GUILayout.EndScrollView();
			
			GUI.contentColor = Color.white;
			
			GUILayout.BeginHorizontal();
			
			GUIStyle boxStyle3 = GUI.skin.GetStyle ("box");
			boxStyle3.fontSize = (int)windowRect.width/20;
			
			if (!isCollapsed)
				GUILayout.Box (checkboxFalseImage, new GUILayoutOption[]{
														GUILayout.ExpandWidth(false),
														GUILayout.MinHeight(windowRect.width/15),
														GUILayout.MinWidth(windowRect.width/15),
														GUILayout.MaxHeight(windowRect.width/15),
														GUILayout.MaxWidth(windowRect.width/15)});
			else
				GUILayout.Box (checkboxTrueImage, new GUILayoutOption[]{
														GUILayout.ExpandWidth(false),
														GUILayout.MinHeight(windowRect.width/15),
														GUILayout.MinWidth(windowRect.width/15),
														GUILayout.MaxHeight(windowRect.width/15),
														GUILayout.MaxWidth(windowRect.width/15)});			
			if (GUILayout.Button(collapseLabel, GUILayout.ExpandWidth(false))) {
				isCollapsed = !isCollapsed;
			}
			if (GUILayout.Button(stackTraceLabel, GUILayout.ExpandWidth(false))) {
				showStackTrace = !showStackTrace;
			}
			if (GUILayout.Button(clearLabel)) {
				logs.Clear();
				tempLogs.Clear();
				collapsedLogs.Clear();
			}
			
			GUILayout.EndHorizontal();
			
			// Окно можно переместить за область заголовка.
			GUI.DragWindow(titleBarRect);
		}
	}
}