

namespace GoogleARCoreInternal
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Security.Cryptography;
    using System.Text;
    using Google.Protobuf;
    using GoogleARCoreInternal.Proto;
    using UnityEditor;
    using UnityEngine;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
     Justification = "Internal")]
    public class LogRequestUtils
    {
        private const string k_GoogleAnalyticsId = "GoogleAnalyticsId";

        
        public static LogRequest BuildLogRequest()
        {
           
            ArCoreSdkLog.Types.UnityEngine.Types.EditionType editionType
                = ArCoreSdkLog.Types.UnityEngine.Types.EditionType.Personal;
            if (Application.HasProLicense() == true)
            {
                editionType = ArCoreSdkLog.Types.UnityEngine.Types.EditionType.Professional;
            }

            ArCoreSdkLog.Types.UnityEngine engine = new ArCoreSdkLog.Types.UnityEngine()
            {
                Version = Application.unityVersion,
                EditionType = editionType,
            };

           
            ArCoreSdkLog logSDK = new ArCoreSdkLog()
            {
                SdkInstanceId = _UniqueId(),
                OsVersion = SystemInfo.operatingSystem,
                ArcoreSdkVersion = GoogleARCore.VersionInfo.Version,
                Unity = engine,     
            };

       
            LogEvent logEvent = new LogEvent()
            {
                EventTimeMs = _GetCurrentUnixEpochTimeMs(),
                EventUptimeMs = _GetSystemUptimeMs(),
                SourceExtension = logSDK.ToByteString(),
            };

           
            LogRequest logRequest = new LogRequest()
            {
                RequestTimeMs = _GetCurrentUnixEpochTimeMs(),
                RequestUptimeMs = _GetSystemUptimeMs(),
                LogSourceVal = LogRequest.Types.LogSource.ArcoreSdk,
                LogEvent = { logEvent },
            };

            return logRequest;
        }

       
        private static string _UniqueId()
        {
            
            string id = EditorPrefs.GetString(k_GoogleAnalyticsId, string.Empty);
            if (id != string.Empty)
            {
                return id;
            }

          
            string salt = System.DateTime.Now.Ticks.ToString();
            HMACSHA512 hasher = new HMACSHA512(Encoding.UTF8.GetBytes(salt));
            byte[] hash =
                hasher.ComputeHash(Encoding.UTF8.GetBytes(SystemInfo.deviceUniqueIdentifier));

           
            StringBuilder str = new StringBuilder();
            foreach (byte b in hash)
            {
                str.Append(b.ToString("x2"));
            }

            id = str.ToString();

            
            EditorPrefs.SetString(k_GoogleAnalyticsId, id);

            return id;
        }

        
        private static long _GetCurrentUnixEpochTimeMs()
        {
           
            DateTimeOffset epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, TimeSpan.Zero);
            TimeSpan offset = DateTimeOffset.UtcNow - epoch;

           
            return offset.Ticks / TimeSpan.TicksPerMillisecond;
        }

       
        private static long _GetSystemUptimeMs()
        {
            return Environment.TickCount;
        }
    }
}
