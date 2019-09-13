

namespace GoogleARCoreInternal
{
    using UnityEditor;
    using UnityEngine;

    
    internal class ARCoreProjectSettingsGUI
    {
        
        public static readonly GUIContent ARCoreRequired = new GUIContent("ARCore Required");
        public static readonly GUIContent InstantPreviewEnabled =
            new GUIContent("Instant Preview Enabled");

        public static readonly GUIContent IOSSupportEnabled =
            new GUIContent("iOS Support Enabled");

        public static readonly GUIContent CloudAnchorAPIKeys =
            new GUIContent("Cloud Anchor API Keys");

        public static readonly GUIContent Android = new GUIContent("Android");
        public static readonly GUIContent IOS = new GUIContent("iOS");

        private static float s_GroupLabelIndent = 15;
        private static float s_GroupFieldIndent = EditorGUIUtility.labelWidth - s_GroupLabelIndent;
        private static bool s_FoldoutCloudAnchorAPIKeys = true;

       
        internal static void OnGUI(bool renderForStandaloneWindow)
        {
            ARCoreProjectSettings.Instance.IsARCoreRequired =
                EditorGUILayout.Toggle(ARCoreRequired,
                    ARCoreProjectSettings.Instance.IsARCoreRequired);
            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);

            ARCoreProjectSettings.Instance.IsInstantPreviewEnabled =
                EditorGUILayout.Toggle(InstantPreviewEnabled,
                    ARCoreProjectSettings.Instance.IsInstantPreviewEnabled);
            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);

            bool newARCoreIOSEnabled =
                EditorGUILayout.Toggle(IOSSupportEnabled,
                    ARCoreProjectSettings.Instance.IsIOSSupportEnabled);
            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);

            s_FoldoutCloudAnchorAPIKeys =
                EditorGUILayout.Foldout(s_FoldoutCloudAnchorAPIKeys, CloudAnchorAPIKeys);
            if (s_FoldoutCloudAnchorAPIKeys)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(s_GroupLabelIndent);
                EditorGUILayout.LabelField(Android, GUILayout.Width(s_GroupFieldIndent));
                ARCoreProjectSettings.Instance.CloudServicesApiKey =
                    EditorGUILayout.TextField(ARCoreProjectSettings.Instance.CloudServicesApiKey);
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(s_GroupLabelIndent);
                EditorGUILayout.LabelField(IOS, GUILayout.Width(s_GroupFieldIndent));
                ARCoreProjectSettings.Instance.IosCloudServicesApiKey =
                    EditorGUILayout.TextField(
                        ARCoreProjectSettings.Instance.IosCloudServicesApiKey);
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(10);
            }

            if (GUI.changed)
            {
                if (newARCoreIOSEnabled != ARCoreProjectSettings.Instance.IsIOSSupportEnabled)
                {
                    ARCoreProjectSettings.Instance.IsIOSSupportEnabled = newARCoreIOSEnabled;
                    ARCoreIOSSupportHelper.SetARCoreIOSSupportEnabled(newARCoreIOSEnabled);
                }
            }
        }
    }
}
