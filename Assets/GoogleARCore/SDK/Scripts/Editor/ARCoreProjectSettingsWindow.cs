

namespace GoogleARCoreInternal
{
    using UnityEditor;
    using UnityEngine;


#if !UNITY_2018_3_OR_NEWER
    internal class ARCoreProjectSettingsWindow : EditorWindow
    {
        [MenuItem("Edit/Project Settings/Google ARCore")]
        private static void ShowARCoreProjectSettingsWindow()
        {
            ARCoreProjectSettings.Instance.Load();
            Rect rect = new Rect(500, 300, 400, 200);
            ARCoreProjectSettingsWindow window =
                GetWindowWithRect<ARCoreProjectSettingsWindow>(rect);
            window.titleContent = new GUIContent("ARCore Project Settings");
            window.Show();
        }

        private void OnGUI()
        {
            OnGUIHeader();
            ARCoreProjectSettingsGUI.OnGUI(true);
            OnGUIFooter();

            if (GUI.changed)
            {
                ARCoreProjectSettings.Instance.Save();
            }
        }

        private void OnGUIHeader()
        {
            GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.stretchWidth = true;
            titleStyle.fontSize = 14;
            titleStyle.fixedHeight = 20;

            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            EditorGUILayout.LabelField("ARCore Project Settings", titleStyle);
            GUILayout.Space(EditorGUIUtility.singleLineHeight);
        }

        private void OnGUIFooter()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Close", GUILayout.Width(50), GUILayout.Height(20)))
            {
                Close();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
#endif
}
