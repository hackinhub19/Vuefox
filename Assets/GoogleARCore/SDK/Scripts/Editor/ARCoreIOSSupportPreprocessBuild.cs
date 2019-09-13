

namespace GoogleARCoreInternal
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using UnityEditor;
    using UnityEditor.Build;
    using UnityEngine;

    internal class ARCoreIOSSupportPreprocessBuild : PreprocessBuildBase
    {
        public override void OnPreprocessBuild(BuildTarget target, string path)
        {
            if (target == BuildTarget.iOS)
            {
                bool arcoreiOSEnabled = ARCoreProjectSettings.Instance.IsIOSSupportEnabled;
                if (arcoreiOSEnabled)
                {
                    Debug.Log("Building application with ARCore iOS support ENABLED.");
                }
                else
                {
                    Debug.Log("Building application with ARCore iOS support DISABLED.");
                }

                ARCoreIOSSupportHelper.SetARCoreIOSSupportEnabled(arcoreiOSEnabled);
            }
        }
    }
}
