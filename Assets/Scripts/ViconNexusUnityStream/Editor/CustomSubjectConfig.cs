using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ubc.ok.ovilab.ViconUnityStream
{
    [FilePath("UserSettings/Vicon.config", FilePathAttribute.Location.ProjectFolder)]
    public class CustomSubjectConfig : ScriptableSingleton<CustomSubjectConfig>
    {
        public bool enableWriteData = true;
        public bool useDefaultData = false;
        public bool useJson = true;
        public string baseURI = "http://127.0.0.1:5000/marker/test";

        public void Save()
        {
            Save(true);
        }
    }
}
