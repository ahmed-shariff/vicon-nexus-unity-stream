using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace umanitoba.hcilab.ViconUnityStream.Editor
{
    [CustomEditor(typeof(CustomSubjectScript), true)]
    [CanEditMultipleObjects]
    public class CustomSubjectScriptEditor: UnityEditor.Editor
    {
        private static List<CustomSubjectScript> subjectScripts;
        private CustomSubjectScript customSubjectScript;

        private void OnEnable()
        {
            if (subjectScripts == null)
            {
                subjectScripts = new List<CustomSubjectScript>();
                subjectScripts.AddRange(FindObjectsOfType<CustomSubjectScript>());
            }

            customSubjectScript = target as CustomSubjectScript;
            if (!subjectScripts.Contains(customSubjectScript))
            {
                subjectScripts.Add(customSubjectScript);
            }
        }

        private void OnDisable()
        {
            subjectScripts = null;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("<color=#eeeeee>----- Common Config -----</color>", new GUIStyle() { fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter, richText = true});
            EditorGUI.BeginChangeCheck();
            CustomSubjectConfig.instance.baseURI = EditorGUILayout.TextField("base URI", CustomSubjectConfig.instance.baseURI);
            CustomSubjectConfig.instance.useDefaultData = EditorGUILayout.Toggle("Use default data", CustomSubjectConfig.instance.useDefaultData);
            CustomSubjectConfig.instance.useJson = EditorGUILayout.Toggle("Use json for network", CustomSubjectConfig.instance.useJson);
            CustomSubjectConfig.instance.enableWriteData = EditorGUILayout.Toggle("Write data to file", CustomSubjectConfig.instance.enableWriteData);
            if (EditorGUI.EndChangeCheck())
            {
                CustomSubjectConfig.instance.Save();
                foreach (CustomSubjectScript script in subjectScripts)
                {
                    script.useDefaultData = CustomSubjectConfig.instance.useDefaultData;
                    script.useJson = CustomSubjectConfig.instance.useJson;
                    script.enableWriteData = CustomSubjectConfig.instance.enableWriteData;
                    script.URI = CustomSubjectConfig.instance.baseURI + script.subjectName;
                    EditorUtility.SetDirty(script);
                }
                Debug.Log($"Setting URI: {customSubjectScript.URI};    Using default data: {customSubjectScript.useDefaultData};     Using json: {customSubjectScript.useJson};    Writing data: {customSubjectScript.enableWriteData};    Scripts updated: \n" + string.Join(",\n", subjectScripts));
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("<color=#eeeeee>----- Component Spcific Config -----</color>", new GUIStyle() { fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter, richText = true});
            base.DrawDefaultInspector();
        }
    }
}
