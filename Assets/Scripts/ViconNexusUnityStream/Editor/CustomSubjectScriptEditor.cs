using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ubc.ok.ovilab.ViconUnityStream.Editor
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
            CustomSubjectConfig.instance.Enabled = customSubjectScript.enabled;
            CustomSubjectConfig.instance.UseDefaultData = customSubjectScript.useDefaultData;
            CustomSubjectConfig.instance.UseJson = customSubjectScript.useJson;
            CustomSubjectConfig.instance.EnableWriteData = customSubjectScript.enableWriteData;
            CustomSubjectConfig.instance.BaseURI = customSubjectScript.baseURI;
            if (CustomSubjectConfig.instance.Changed)
            {
                UpdateContent();
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
            CustomSubjectConfig.instance.BaseURI = EditorGUILayout.TextField("base URI", CustomSubjectConfig.instance.BaseURI);
            CustomSubjectConfig.instance.Enabled = EditorGUILayout.Toggle("Enabled", CustomSubjectConfig.instance.Enabled);
            CustomSubjectConfig.instance.UseDefaultData = EditorGUILayout.Toggle("Use default data", CustomSubjectConfig.instance.UseDefaultData);
            CustomSubjectConfig.instance.UseJson = EditorGUILayout.Toggle("Use json for network", CustomSubjectConfig.instance.UseJson);
            CustomSubjectConfig.instance.EnableWriteData = EditorGUILayout.Toggle("Write data to file", CustomSubjectConfig.instance.EnableWriteData);
            if (EditorGUI.EndChangeCheck())
            {
                UpdateContent();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("<color=#eeeeee>----- Component Spcific Config -----</color>", new GUIStyle() { fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter, richText = true});
            base.DrawDefaultInspector();
        }

        private void UpdateContent()
        {
            CustomSubjectConfig.instance.Save();
            foreach (CustomSubjectScript script in subjectScripts)
            {
                script.enabled = CustomSubjectConfig.instance.Enabled;
                script.useDefaultData = CustomSubjectConfig.instance.UseDefaultData;
                script.useJson = CustomSubjectConfig.instance.UseJson;
                script.enableWriteData = CustomSubjectConfig.instance.EnableWriteData;
                script.baseURI = CustomSubjectConfig.instance.BaseURI;
                script.UpdateURI();
                EditorUtility.SetDirty(script);
            }
            Debug.Log($"Updated {subjectScripts.Count} subject script(s):"+
                      $"\n    Setting URI:        {customSubjectScript.URI};"+
                      $"\n    Enabled:            {CustomSubjectConfig.instance.Enabled};"+
                      $"\n    Using default data: {customSubjectScript.useDefaultData};"+
                      $"\n    Using json:         {customSubjectScript.useJson};"+
                      $"\n    Writing data:       {customSubjectScript.enableWriteData};"+
                      $"\n    Scripts updated: \n         " +
                      string.Join(",\n         ", subjectScripts));
        }
    }
}
