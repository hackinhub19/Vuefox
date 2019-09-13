

namespace GoogleARCoreInternal
{
    using System.Diagnostics.CodeAnalysis;
    using UnityEditor;
    using UnityEditor.Build;
    using UnityEngine;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Internal")]
    internal class ARCoreSupportedPreprocessBuild : PreprocessBuildBase
#if UNITY_2017_4_OR_NEWER
        , IActiveBuildTargetChanged
#endif
    {
        public override void OnPreprocessBuild(BuildTarget target, string path)
        {
            if (target == BuildTarget.Android)
            {
                CheckARCoreSupported();
            }
        }

        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            if (newTarget == BuildTarget.Android)
            {
                CheckARCoreSupported();
            }
        }

        private void CheckARCoreSupported()
        {
         
#if UNITY_2018_2_OR_NEWER && !UNITY_2018_2_0
            if (!PlayerSettings.Android.ARCoreEnabled)
            {
                Debug.LogWarning("ARCore support is disabled. To use ARCore on Android, 'XR Settings > ARCore Supported' must be enabled.");
            }
#endif  
        }
    }
}
