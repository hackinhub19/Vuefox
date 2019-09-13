

namespace GoogleARCoreInternal
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using Google.Protobuf;
    using GoogleARCoreInternal.Proto;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Networking;

    [InitializeOnLoad]
    [Serializable]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
     Justification = "Internal")]
    public class ARCoreAnalytics
    {
        public bool EnableAnalytics;
        private const string k_EnableAnalyticsKey = "EnableGoogleARCoreAnalytics";
        private const string k_GoogleAnalyticsHost = "https://play.googleapis.com/log";
        private const long k_AnalyticsResendDelayTicks = TimeSpan.TicksPerDay * 7;
        private long m_LastUpdateTicks;
        private bool m_Verbose;
#if UNITY_2017_1_OR_NEWER
        private UnityWebRequest m_WebRequest;
#endif

    
        static ARCoreAnalytics()
        {
          
            Instance = new ARCoreAnalytics();
            Instance.Load();

           
            EditorApplication.update +=
                new EditorApplication.CallbackFunction(Instance._OnAnalyticsUpdate);

            
            Instance.SendAnalytics(k_GoogleAnalyticsHost, LogRequestUtils.BuildLogRequest(), false);
        }

        public static ARCoreAnalytics Instance { get; private set; }

       
        [PreferenceItem("Google ARCore")]
        public static void PreferencesGUI()
        {
            
            Instance.EnableAnalytics = EditorGUILayout.Toggle(
                "Enable Google ARCore SDK Analytics.", Instance.EnableAnalytics);

           
            if (GUI.changed)
            {
                Instance.Save();
            }
        }

       >
        public void Load()
        {
            EnableAnalytics = EditorPrefs.GetBool(k_EnableAnalyticsKey, true);
        }

       
        public void Save()
        {
            EditorPrefs.SetBool(k_EnableAnalyticsKey, EnableAnalytics);
        }

       
        public void SendAnalytics(string analyticsHost, LogRequest logRequest, bool verbose)
        {
#if UNITY_2017_1_OR_NEWER
            
            if (EnableAnalytics == false)
            {
                if (verbose == true)
                {
                    Debug.Log("GoogleARCore analytics is disabled, not sending.");
                }

                return;
            }

           
            if (m_WebRequest != null)
            {
                if (verbose == true)
                {
                    Debug.Log("GoogleARCore analytics is already sending data.");
                }

                return;
            }

         
            UnityWebRequest webRequest = new UnityWebRequest(analyticsHost);
            webRequest.method = UnityWebRequest.kHttpVerbPOST;
            webRequest.uploadHandler = new UploadHandlerRaw(logRequest.ToByteArray());
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/x-protobuf");
            webRequest.SendWebRequest();

           
            m_Verbose = verbose;
            if (verbose == true)
            {
                Debug.Log("Sending GoogleARCore Analytics.");
            }

           
            m_WebRequest = webRequest;
#endif

           
            m_LastUpdateTicks = DateTime.Now.Ticks;
        }

        
        private void _OnAnalyticsUpdate()
        {
           
            if (EnableAnalytics == false)
            {
                return;
            }

            
#if UNITY_2017_1_OR_NEWER
            if (m_WebRequest != null)
            {
                if (m_WebRequest.isDone == true)
                {
                    if (m_Verbose == true)
                    {
                        if (m_WebRequest.isNetworkError == true)
                        {
                            Debug.Log("Error sending analytics: " + m_WebRequest.error);
                        }
                        else
                        {
                            Debug.Log("Analytics sent: " + m_WebRequest.downloadHandler.text);
                        }
                    }

                    m_WebRequest = null;
                }
            }
#endif

            
            if (DateTime.Now.Ticks - m_LastUpdateTicks >= k_AnalyticsResendDelayTicks)
            {
                Instance.SendAnalytics(
                    k_GoogleAnalyticsHost, LogRequestUtils.BuildLogRequest(), false);
            }
        }
    }
}
