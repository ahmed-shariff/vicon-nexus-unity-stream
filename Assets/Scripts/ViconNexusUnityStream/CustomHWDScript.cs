using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace umanitoba.hcilab.ViconUnityStream
{
    public class CustomHWDScript : CustomSubjectScript
    {
        protected override Dictionary<string, Vector3> ProcessSegments(Dictionary<string, Vector3> segments, Data data)
        {
            Vector3 forward = segments["base2"] - segments["base1"];
            Vector3 up = Vector3.Cross(forward, segments["base3"] - segments["base4"]);
            if (forward != Vector3.zero && up != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(forward, up);
                foreach (var key in segmentsRotation.Keys.ToArray())
                {
                    segmentsRotation[key] = rotation;
                }
            }
            return segments;
        }
    }
}
