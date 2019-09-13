
namespace GoogleARCoreInternal
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using UnityEditor;
    using UnityEditor.Build;
    using UnityEngine;

    internal class RequiredOptionalPreprocessBuild : PreprocessBuildBase
    {
        public override void OnPreprocessBuild(BuildTarget target, string path)
        {
            var isARCoreRequired = ARCoreProjectSettings.Instance.IsARCoreRequired;

            Debug.LogFormat(
                "Building \"{0}\" app. Use 'Edit > Project Settings > ARCore' to adjust ARCore " +
                "settings.\n" +
                "See {1} for more information.",
                isARCoreRequired ? "AR Required" : "AR Optional",
                "https://developers.google.com/ar/develop/unity/enable-arcore");

            AssetHelper.GetPluginImporterByName("google_ar_required.aar")
                .SetCompatibleWithPlatform(BuildTarget.Android, isARCoreRequired);
            AssetHelper.GetPluginImporterByName("google_ar_optional.aar")
                .SetCompatibleWithPlatform(BuildTarget.Android, !isARCoreRequired);
        }
    }
}
