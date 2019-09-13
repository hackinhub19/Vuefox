

namespace GoogleARCoreInternal
{
    using System.Diagnostics.CodeAnalysis;
    using GoogleARCore;
    using UnityEditor;
    using UnityEditor.Build;
#if UNITY_2018_1_OR_NEWER
    using UnityEditor.Build.Reporting;
#endif

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
     Justification = "Internal")]
#if UNITY_2018_1_OR_NEWER
    internal class PreprocessBuildBase : IPreprocessBuildWithReport
#else
    internal class PreprocessBuildBase : IPreprocessBuild
#endif
    {
        [SuppressMessage("UnityRules.UnityStyleRules", "US1000:FieldsMustBeUpperCamelCase",
         Justification = "Overriden property.")]
        public int callbackOrder
        {
            get
            {
                return 0;
            }
        }

#if UNITY_2018_1_OR_NEWER
        public void OnPreprocessBuild(BuildReport report)
        {
            OnPreprocessBuild(report.summary.platform, report.summary.outputPath);
        }
#endif

       public virtual void OnPreprocessBuild(BuildTarget target, string path)
       {
       }
    }
}
