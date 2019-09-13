using UnityEngine;
using System;
using UnityEditor;


namespace UnityStandardAssets.Water
{
    [CustomEditor(typeof(GerstnerDisplace))]
    public class GerstnerDisplaceEditor : Editor
    {
        private SerializedObject serObj;

        public void OnEnable()
        {
            serObj = new SerializedObject(target);
        }

        public override void OnInspectorGUI()
        {
            serObj.Update();

            GameObject go = ((GerstnerDisplace)serObj.targetObject).gameObject;
            WaterBase wb = (WaterBase)go.GetComponent(typeof(WaterBase));
            Material sharedWaterMaterial = wb.sharedMaterial;

            GUILayout.Label("Animates vertices using up 4 generated waves", EditorStyles.miniBoldLabel);

            if (sharedWaterMaterial)
            {
                Vector4 amplitude = WaterEditorUtility.GetMaterialVector("_GAmplitude", sharedWaterMaterial);
                Vector4 frequency = WaterEditorUtility.GetMaterialVector("_GFrequency", sharedWaterMaterial);
                Vector4 steepness = WaterEditorUtility.GetMaterialVector("_GSteepness", sharedWaterMaterial);
                Vector4 speed = WaterEditorUtility.GetMaterialVector("_GSpeed", sharedWaterMaterial);
                Vector4 directionAB = WaterEditorUtility.GetMaterialVector("_GDirectionAB", sharedWaterMaterial);
                Vector4 directionCD = WaterEditorUtility.GetMaterialVector("_GDirectionCD", sharedWaterMaterial);

                amplitude = EditorGUILayout.Vector4Field("Amplitude (Height offset)", amplitude);
                frequency = EditorGUILayout.Vector4Field("Frequency (Tiling)", frequency);
                steepness = EditorGUILayout.Vector4Field("Steepness", steepness);
                speed = EditorGUILayout.Vector4Field("Speed", speed);
                directionAB = EditorGUILayout.Vector4Field("Direction scale (Wave 1 (X,Y) and 2 (Z,W))", directionAB);
                directionCD = EditorGUILayout.Vector4Field("Direction scale (Wave 3 (X,Y) and 4 (Z,W))", directionCD);

                if (GUI.changed)
                {
                    WaterEditorUtility.SetMaterialVector("_GAmplitude", amplitude, sharedWaterMaterial);
                    WaterEditorUtility.SetMaterialVector("_GFrequency", frequency, sharedWaterMaterial);
                    WaterEditorUtility.SetMaterialVector("_GSteepness", steepness, sharedWaterMaterial);
                    WaterEditorUtility.SetMaterialVector("_GSpeed", speed, sharedWaterMaterial);
                    WaterEditorUtility.SetMaterialVector("_GDirectionAB", directionAB, sharedWaterMaterial);
                    WaterEditorUtility.SetMaterialVector("_GDirectionCD", directionCD, sharedWaterMaterial);
                }

            }

            serObj.ApplyModifiedProperties();
        }
    }
}