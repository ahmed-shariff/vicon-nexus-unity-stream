using System.Collections.Generic;
using MessagePack;

namespace ubc.ok.ovilab.ViconUnityStream
{
    [MessagePackObject]
    public class Data
    {
        [Key("data")]
        public Dictionary<string, List<float>> data;
        [Key("hierachy")]
        public Dictionary<string, List<string>> hierachy;
        [Key("sensorTriggered")]
        public bool sensorTriggered;
    }
}
