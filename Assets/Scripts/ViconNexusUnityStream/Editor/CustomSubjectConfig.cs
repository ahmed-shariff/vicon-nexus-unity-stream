using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ubc.ok.ovilab.ViconUnityStream
{
    [FilePath("UserSettings/Vicon.config", FilePathAttribute.Location.ProjectFolder)]
    public class CustomSubjectConfig : ScriptableSingleton<CustomSubjectConfig>
    {
        private bool enabled = true;
        private bool enableWriteData = true;
        private bool useDefaultData = false;
        private bool useJson = true;
        private string baseURI = "http://127.0.0.1:5000/marker/test";

        public bool Changed { get; private set; } = false;

        public bool Enabled {
            get {
                return enabled;
            }
            set {
                if (enabled != value)
                {
                    Changed = true;
                }
                enabled = value;
            }
        }

        public bool EnableWriteData {
            get {
                return enableWriteData;
            }
            set {
                if (enableWriteData != value)
                {
                    Changed = true;
                }
                enableWriteData = value;
            }
        }

        public bool UseDefaultData {
            get {
                return useDefaultData;
            }
            set {
                if (useDefaultData != value)
                {
                    Changed = true;
                }
                useDefaultData = value;
            }
        }

        public bool UseJson {
            get {
                return useJson;
            }
            set {
                if (useJson != value)
                {
                    Changed = true;
                }
                useJson = value;
            }
        }

        public string BaseURI {
            get {
                return baseURI;
            }
            set {
                if (baseURI != value)
                {
                    Changed = true;
                }
                baseURI = value;
            }
        }

        public void Save()
        {
            Save(true);
        }
    }
}
