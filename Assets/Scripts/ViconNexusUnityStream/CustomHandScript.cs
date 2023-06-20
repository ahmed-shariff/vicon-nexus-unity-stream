using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ubc.ok.ovilab.ViconUnityStream
{
    public class CustomHandScript : CustomSubjectScript
    {
        public float normalOffset = 0.001f;
        public bool setPosition = true;
        public bool setScale = true;
        private Vector3 normal;
        private Vector3 palm;
        private bool noHand = false;
    

        Dictionary<string, string> segmentChild = new Dictionary<string, string>()
        {
            //{"Arm", null},
            {"Arm", "Hand"},
            {"Hand", "R3D1"},

            {"R1D1", "R1D2"},
            {"R1D2", "R1D3"},
            {"R1D3", "R1D4"},
        
            {"R2D1", "R2D2"},
            {"R2D2", "R2D3"},
            {"R2D3", "R2D4"},
        
            {"R3D1", "R3D2"},
            {"R3D2", "R3D3"},
            {"R3D3", "R3D4"},
        
            {"R4D1", "R4D2"},
            {"R4D2", "R4D3"},
            {"R4D3", "R4D4"},

            {"R5D1", "R5D2"},
            {"R5D2", "R5D3"},
            {"R5D3", "R5D4"},
        };

        Dictionary<string, string> segmentParents = new Dictionary<string, string>()
        {
            //{"Arm", null},
            {"Hand", "Arm"},
            //{"R1D1", "Hand"},
            //{"R2D1", "Hand"},
            {"R3D1", "Hand"},
            //{"R4D1", "Hand"},
            //{"R5D1", "Hand"},

            {"R1D2", "R1D1"},
            {"R2D2", "R2D1"},
            {"R3D2", "R3D1"},
            {"R4D2", "R4D1"},
            {"R5D2", "R5D1"},
        
            {"R1D3", "R1D2"},
            {"R2D3", "R2D2"},
            {"R3D3", "R3D2"},
            {"R4D3", "R4D2"},
            {"R5D3", "R5D2"},
        
            {"R1D4", "R1D3"},
            {"R2D4", "R2D3"},
            {"R3D4", "R3D3"},
            {"R4D4", "R4D3"},
            {"R5D4", "R5D3"},
        };

        void Start()
        {
            segmentMarkers = new Dictionary<string, List<string>>() {
                { "Arm", new List<string>() {"RFA2", "RFA1"}} ,//{ "RWRB", "RFA2", "RFA1", "RWRA" } } ,
                {    "Hand", new List<string>() { "RWRB", "RWRA" }} ,//{"RH1", "RH3", "RH6"}}, 
                {    "R1D1",  new List<string>(){"RTH1"}},
                {    "R1D2", new List<string>(){"RTH2"}},
                {    "R1D3", new List<string>(){"RTH3", "RTH3P"}},
                {    "R1D4", new List<string>{"RTH4"}},

                {    "R2D1", new List<string>{"RH2"}},
                {    "R2D2", new List<string>{"RIF1"}}, 
                {    "R2D3", new List<string>{"RIF2"}},
                {    "R2D4", new List<string>{"RIF3"}}, 
        
                {    "R3D1", new List<string>{"RH3"}},
                {    "R3D2", new List<string>{"RTF1"}},
                {    "R3D3", new List<string>{"RTF2"}},
                {    "R3D4", new List<string>{"RTF3"}},
        
                {    "R4D1", new List<string>{"RH4"}},
                {    "R4D2", new List<string>{"RRF2"}},
                {    "R4D3", new List<string>{"RRF3"}},
                {    "R4D4", new List<string>{"RRF4"}},
        
                {   "R5D1", new List<string>(){"RH5"}}, 
                {    "R5D2",  new List<string>(){"RPF1"}}, 
                {    "R5D3", new List<string>(){"RPF2"}},
                {    "R5D4", new List<string>(){"RPF3"}}
            };

            SetupMessagePack();
            SetupWriter();
            SetupFilter();
        }
        private Dictionary<string, Vector3> baseVectors = new Dictionary<string, Vector3>();
        private Dictionary<string, Vector3> previousSegments = new Dictionary<string, Vector3>();

        private readonly Dictionary<string, List<string>> fingerSegments = new Dictionary<string, List<string>>()
        {
            {"R1", new List<string>{"R1D1", "R1D2", "R1D3", "R1D4"}},
            {"R2", new List<string>{"R2D1", "R2D2", "R2D3", "R2D4"}},
            {"R3", new List<string>{"R3D1", "R3D2", "R3D3", "R3D4"}},
            {"R4", new List<string>{"R4D1", "R4D2", "R4D3", "R4D4"}},
            {"R5", new List<string>{"R5D1", "R5D2", "R5D3", "R5D4"}},
        };


        protected override Dictionary<string, Vector3> ProcessSegments(Dictionary<string, Vector3> segments, Data data)
        {
            /// Filling any gaps that can be filled
            if (gapFillingStrategy == GapFillingStrategy.FillRelative)
            {
                foreach(string segment in segments.Keys.ToList())
                {
                    if (segments[segment] == Vector3.zero)
                    {
                        segments[segment] = FillWithRelativeAdjacent(segment, segments);
                    }
                }
            }

            // What sorcery is this?
            palm = segments["Hand"] - (segments["R3D1"] + 0.5f * (segments["R4D1"] - segments["R4D1"]));
            normal = Vector3.Cross(palm, segments["R4D1"] - segments["R3D1"]);

            if (segments[segmentChild["R1D1"]] != Vector3.zero && segments["R1D1"] != Vector3.zero)
            {
                baseVectors["R1"] = segments[segmentChild["R1D1"]] - segments["R1D1"];
            }

            if (segments[segmentChild["R2D1"]] != Vector3.zero && segments["R2D1"] != Vector3.zero)
            {
                baseVectors["R2"] = segments[segmentChild["R2D1"]] - segments["R2D1"];
            }

            if (segments[segmentChild["R3D1"]] != Vector3.zero && segments["R3D1"] != Vector3.zero)
            {
                baseVectors["R3"] = segments[segmentChild["R3D1"]] - segments["R3D1"];
            }

            if (segments[segmentChild["R4D1"]] != Vector3.zero && segments["R4D1"] != Vector3.zero)
            {
                baseVectors["R4"] = segments[segmentChild["R4D1"]] - segments["R4D1"];
            }

            if (segments[segmentChild["R5D1"]] != Vector3.zero && segments["R5D1"] != Vector3.zero)
            {
                baseVectors["R5"] = segments[segmentChild["R5D1"]] - segments["R5D1"];
            }

            // Debug.Log(data.data["RTH3P"] + "  -  "+ data.data["RTH3"]);
            var p1 = data.data["RTH3P"];
            var p2 = data.data["RTH3"];
            Vector3 p1Position = new Vector3(p1[0], p1[2], p1[1]);
            Vector3 p2Position = new Vector3(p2[0], p2[2], p2[1]);

            /// If one of the datapoints is missing, use the previous values, do this by not modifying the baseVectors
            if (p1[0] != 0 || p2[0] != 0)
            {
                /// Ensure p1 and p2 are not switched, as it can happen when accuracy is low
                /// PalmBase's Transform.up should be pointing into the palm.
                /// This is based on the assumption that the RTH3P->RTH3 vector, relative to
                /// PlamBase.up will be negative (pointing up from palm)

                Vector3 p1p2Vector = p2Position - p1Position;
                if (Vector3.Dot(p1p2Vector, normal) < 0)
                {
                    p1p2Vector = -p1p2Vector;
                }

                baseVectors["R1_right"] = p1p2Vector;
            }
            // Vector3.Cross(segments["R1D4"] - segments["R1D3"], segments["R1D3"] - segments["R1D2"]);
            // Debug.DrawRay(segments["R3D1"], normal);
            //Debug.Log(normal.magnitude);
            //Debug.Log((normal * 0.01f).magnitude);

            /// Using segments to store the normal vector instead of position?
            segments["PalmBase"] = normal;// *0.01f;
            if (palm == Vector3.zero)
                noHand = true;
            else
            {
                noHand = false;
                if (normal != Vector3.zero)
                    transform.root.rotation = Quaternion.LookRotation(-normal, -palm);
            }
            return segments;
        }

        private Matrix4x4 handWorldToLocalMatrix;
    
        protected override string ConstructFinalWriterString()
        {
            return "[" + base.ConstructFinalWriterString() + ", [" +
                handWorldToLocalMatrix[0, 0] + ", " + handWorldToLocalMatrix[0, 1] + ", " + handWorldToLocalMatrix[0, 2] + ", " + handWorldToLocalMatrix[0, 3] + ", " +
                handWorldToLocalMatrix[1, 0] + ", " + handWorldToLocalMatrix[1, 1] + ", " + handWorldToLocalMatrix[1, 2] + ", " + handWorldToLocalMatrix[1, 3] + ", " +
                handWorldToLocalMatrix[2, 0] + ", " + handWorldToLocalMatrix[2, 1] + ", " + handWorldToLocalMatrix[2, 2] + ", " + handWorldToLocalMatrix[2, 3] + ", " +
                handWorldToLocalMatrix[3, 0] + ", " + handWorldToLocalMatrix[3, 1] + ", " + handWorldToLocalMatrix[3, 2] + ", " + handWorldToLocalMatrix[3, 3] + ", " +
                "]]";
        }

        private Vector3 FillWithRelativeAdjacent(string boneName, Dictionary<string, Vector3> _segments)
        {
            if (gapFillingStrategy != GapFillingStrategy.FillRelative)
            {
                return Vector3.zero;
            }

            string fingerId = boneName.Substring(0, 2);
            // List<string> thisFingerSegments;
            // if (fingerSegments.ContainsKey(fingerId))
            // {
            //     thisFingerSegments = fingerSegments[fingerId];
            // }
            // else
            // {
            //     return Vector3.zero;
            // }
            // string thirdWheelName;

            string childName, parentName; // Thisrd wheel being the other segment of the 4 segments in the finger
            Vector3 childPos, parentPos, childPosPrevious, parentPosPrevious, segmentPosPrevious;
            segmentChild.TryGetValue(boneName, out childName);
            segmentParents.TryGetValue(boneName, out parentName);

            if (previousSegments.ContainsKey(boneName))
            {
                segmentPosPrevious = previousSegments[boneName];
                if (segmentPosPrevious == Vector3.zero)
                {
                    return Vector3.zero;
                }
            }
            else
            {
                return Vector3.zero;
            }

            if (!string.IsNullOrEmpty(childName) && _segments.ContainsKey(childName) && previousSegments.ContainsKey(childName))
            {
                childPos = _segments[childName];
                childPosPrevious = previousSegments[childName];
                if (childPos == Vector3.zero || childPosPrevious == Vector3.zero)
                {
                    return Vector3.zero;
                }
            }
            else
            {
                return Vector3.zero;
            }

            if (!string.IsNullOrEmpty(parentName) && _segments.ContainsKey(parentName) && previousSegments.ContainsKey(parentName))
            {
                parentPos = _segments[parentName];
                parentPosPrevious = previousSegments[parentName];
                if (parentPos == Vector3.zero || parentPosPrevious == Vector3.zero)
                {
                    return Vector3.zero;
                }
            }
            else
            {
                return Vector3.zero;
            }

            // /// get thirdWheelName
            // thirdWheelName = thisFingerSegments.Where(x => x != boneName && x != parentName && x != childName).First();

            /// Do the actual math
            Vector3 segmentToChildVector = (childPos - segmentPosPrevious);
            Vector3 segmentToParentVector = (parentPos - segmentPosPrevious);
            float segmentToChildDistance = segmentToChildVector.magnitude;
            float segmentToParentDistance = segmentToParentVector.magnitude;

            Vector3 planePerpendicularVector;
            // if (!string.IsNullOrEmpty(thirdWheelName) && _segments.ContainsKey(thirdWheelName) && _segments[thirdWheelName] != Vector3.zero)
            // {
            //     List<Vector3> vectors = thisFingerSegments.Select(x => _segments[x]).Where(x => x != Vector3.zero).ToList();
            //     if (vectors.Count != 3) // this should never happen at this point. if it does, something is going wrong.
            //     {
            //         Debug.LogWarning($"{thirdWheelName}  ummm... ??" + string.Join(", ", thisFingerSegments.Where(x => _segments[x] != Vector3.zero)));
            //         return Vector3.zero;
            //     }
            //     planePerpendicularVector = Vector3.Cross(vectors[0] - vectors[1], vectors[2] - vectors[1]);
            // }
            // /// Assuming the plane formed by parent, sgement, child are parallel now and in previous
            // else
            // {
            //     planePerpendicularVector = Vector3.Cross(segmentToChildVector, segmentToParentVector);
            // }

            planePerpendicularVector = Vector3.Cross(segmentToChildVector, segmentToParentVector);

            Vector3 parentToChildVector = childPos - parentPos;
            Vector3 projectionOfSegmentFromParent = parentToChildVector * segmentToParentDistance / (segmentToParentDistance + segmentToChildDistance);
            float projectionToSegmentDistance = (float) System.Math.Sqrt((System.Math.Pow(segmentToParentDistance, 2) - System.Math.Pow(projectionOfSegmentFromParent.magnitude, 2)));
            Vector3 projectionVector = Vector3.Cross(parentToChildVector, planePerpendicularVector).normalized * projectionToSegmentDistance;

            return parentPos + projectionOfSegmentFromParent + projectionVector;
        }

        protected override void ApplyBoneTransform(Transform Bone)
        {
            string BoneName = Bone.gameObject.name;

            //if (segmentParents.ContainsKey(BoneName) && segments.ContainsKey(BoneName))
            if (segments.ContainsKey(BoneName))
            //if (segmentChild.ContainsKey(BoneName) && segments.ContainsKey(BoneName))
            {
                Vector3 BonePosition = segments[BoneName];

                /// Ignore setting pos/rot/scale if GapFillingStrategy.Ignore and BonePosition is zero
                /// and cannot resolve with adjacent segments (in FillRelative mode)
                if (!(gapFillingStrategy == GapFillingStrategy.Ignore || gapFillingStrategy == GapFillingStrategy.FillRelative) || BonePosition != Vector3.zero)
                {
                    // bool usePreviousSegments = false;
                    // if (BonePosition == Vector3.zero && !noHand)
                    // {
                    //     if (previousSegments.ContainsKey(BoneName))
                    //         BonePosition = previousSegments[BoneName];
                    //     usePreviousSegments = true;
                    //     Debug.Log(BoneName +"   "+BonePosition + " " + usePreviousSegments + " "+ previousSegments[BoneName]);
                    // }
                    // else
                    // {
                    //     previousSegments[BoneName] = BonePosition;
                    // }

                    //string ParentName = segmentParents[BoneName];
                    // string childName = segmentChild[BoneName];
                    if (BoneName == "PalmBase")
                    {
                        if (!noHand && BonePosition != Vector3.zero)
                            Bone.rotation = Quaternion.LookRotation(-BonePosition.normalized, -palm);
                        Bone.position = Bone.parent.position - Bone.forward * scale_2 - Bone.up * scale_2;
                        // Debug.Log("===========================  " + Bone.position);
                    }
                    else
                    {
                        string fingerId = BoneName.Substring(0, 2);
                        if (setPosition)
                        {
                            Bone.position = BonePosition * scale_1; // Bone.parent.InverseTransformPoint();
                        }
                        // if (fingerId)
                        // + normal.normalized * normalOffset
                        if (setScale)
                        {
                            Transform p = Bone.parent;
                            Bone.parent = null;
                            Bone.localScale = Vector3.one * scale_2;
                            Bone.parent = p;
                        }
                        // Debug.Log("===========================+++++++++  " + Bone.position);
                        /// This was usePrevious; now handled in the ProcessData in CustomSubjectScript
                        /// with GapFillingStrategy.UsePrevious
                        if (true)
                        {
                            if (segmentChild.ContainsKey(BoneName))
                            {
                                Vector3 childSegment = segments[segmentChild[BoneName]];

                                /// Avoid setting rotation if childsegment is zero
                                if (childSegment != Vector3.zero && baseVectors.ContainsKey(fingerId))
                                {
                                    Vector3 upDirection = childSegment - BonePosition;
                                    if (upDirection != Vector3.zero)
                                    {
                                        Vector3 right;
                                        Vector3 forward;
                                        if (fingerId == "R1")
                                        {
                                            right = baseVectors["R1_right"];
                                            //right = Vector3.Cross(normal, baseVectors[fingerId]);
                                            forward = Vector3.Cross(upDirection, right);
                                        }
                                        else
                                        {
                                            right = Vector3.Cross(normal, baseVectors[fingerId]);
                                            forward = Vector3.Cross(upDirection, right);
                                        }
                                        if (forward != Vector3.zero)
                                            Bone.rotation = Quaternion.LookRotation(forward, upDirection);
                                    }
                                }
                            }
                            else
                            {
                                // Bone.rotation = Quaternion.identity;
                            }
                            if (setPosition)
                            {
                                if (fingerId == "R1")
                                    Bone.position += Bone.forward * normalOffset * 0.9f;
                                else if (fingerId == "R3")
                                    Bone.position += Bone.forward * normalOffset * 1.08f;
                                else if (fingerId == "R4")
                                    Bone.position += Bone.forward * normalOffset * 1.13f;
                                else if (fingerId == "R5")
                                    Bone.position += Bone.forward * normalOffset * 1.2f;
                                else
                                    Bone.position += Bone.forward * normalOffset;
                            }
                        }
                    }
                }
                previousSegments[BoneName] = BonePosition;
            }
            AddBoneDataToWriter(Bone);
            if (Bone.name == "Hand")
                handWorldToLocalMatrix = Bone.worldToLocalMatrix;
        }

        protected override bool TestSegmentsQulity(Dictionary<string, Vector3> segments)
        {
            float d3_d1_dot = Vector3.Dot(segments["R5D3"] - segments["R2D3"], segments["R5D1"] - segments["R2D1"]);
            if (d3_d1_dot > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
