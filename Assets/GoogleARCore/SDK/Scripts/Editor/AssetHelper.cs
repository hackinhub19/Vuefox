

namespace GoogleARCoreInternal
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using UnityEditor;
    using UnityEditor.Build;
    using UnityEngine;

    internal class AssetHelper
    {
      
        public static PluginImporter GetPluginImporterByName(string name)
        {
            string[] guids = AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension(name));

            PluginImporter pluginImporter = null;
            int foundCount = 0;
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileName(path) == name)
                {
                    pluginImporter = AssetImporter.GetAtPath(path) as PluginImporter;
                    ++foundCount;
                }
            }

            if (foundCount == 0)
            {
                throw new BuildFailedException(
                    string.Format(
                        "ARCore could not find plugin {0}. Was it removed from the ARCore SDK?",
                        name));
            }
            else if (foundCount != 1)
            {
                throw new BuildFailedException(
                    string.Format(
                        "ARCore found multiple plugins named {0}. This project should only " +
                        "contain one such plugin and it should be inside the ARCore SDK", name));
            }
            else if (pluginImporter == null)
            {
                throw new BuildFailedException(
                    string.Format("Found {0} file, but it is not a plugin.", name));
            }

            return pluginImporter;
        }
    }
}
