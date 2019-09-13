

namespace GoogleARCoreInternal
{
    using System.Diagnostics.CodeAnalysis;
    using GoogleARCore;
    using UnityEditor;
    using UnityEditor.Build;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
     Justification = "Internal")]
    internal class AugmentedImageDatabasePreprocessBuild : PreprocessBuildBase
    {
        public override void OnPreprocessBuild(BuildTarget target, string path)
        {
            var augmentedImageDatabaseGuids = AssetDatabase.FindAssets("t:AugmentedImageDatabase");
            foreach (var databaseGuid in augmentedImageDatabaseGuids)
            {
                var database = AssetDatabase.LoadAssetAtPath<AugmentedImageDatabase>(
                    AssetDatabase.GUIDToAssetPath(databaseGuid));

                string error;
                database.BuildIfNeeded(out error);
                if (!string.IsNullOrEmpty(error))
                {
                    throw new BuildFailedException(error);
                }
            }
        }
    }
}
