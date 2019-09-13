

namespace GoogleARCoreInternal
{
    using UnityEditor;
    using UnityEngine;

#if UNITY_2018_3_OR_NEWER
#if UNITY_2019_1_OR_NEWER
    using UnityEngine.UIElements;
#else
    using UnityEngine.Experimental.UIElements;
#endif

    internal class ARCoreProjectSettingsProvider : SettingsProvider
    {
        public ARCoreProjectSettingsProvider(string path, SettingsScope scope): base(path, scope)
        {
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            ARCoreProjectSettings.Instance.Load();
        }

        [SettingsProvider]
        public static SettingsProvider CreateARCoreProjectSettingsProvider()
        {
            var provider =
                new ARCoreProjectSettingsProvider("Project/Google ARCore", SettingsScope.Project);

            
            provider.keywords =
                GetSearchKeywordsFromGUIContentProperties<ARCoreProjectSettingsGUI>();

            return provider;
        }

        public override void OnGUI(string searchContext)
        {
            ARCoreProjectSettingsGUI.OnGUI(false);

            if (GUI.changed)
            {
                ARCoreProjectSettings.Instance.Save();
            }
        }
    }
#endif
}
