using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Net.Http;
using System;
using System.IO;
using System.Linq;

//// using ViconDataStreamSDK.CSharp;

// using ViconDataStreamSDK.DotNET;
using Newtonsoft.Json;

namespace umanitoba.hcilab.ViconUnityStream
{
    public class CustomHWDScript : CustomSubjectScript
    {
        protected override Dictionary<string, Vector3> processSegments(Dictionary<string, Vector3> segments, Data data)
        {
            Vector3 forward = segments["base2"] - segments["base1"];
            Vector3 up = Vector3.Cross(forward, segments["base3"] - segments["base4"]);
            Quaternion rotation = Quaternion.LookRotation(forward, up);
            foreach(var key in segmentsRotation.Keys.ToArray())
            {
                segmentsRotation[key] = rotation;
            }
            return segments;
        }
    }
}
