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
        [SerializeField]
        public Handedness handedness = Handedness.Right;

        // NOTE: Considered using an enum with a dictionary, but that
        // means using a dictionary lookup everytime a name is needed,
        // so doing it the ugly (or not?) way
        protected string segment_Arm = "Arm";
        protected string segment_Hand = "Hand";
        protected string segment_1D1 = "1D1";
        protected string segment_1D2 = "1D2";
        protected string segment_1D3 = "1D3";
        protected string segment_1D4 = "1D4";
        protected string segment_2D1 = "2D1";
        protected string segment_2D2 = "2D2";
        protected string segment_2D3 = "2D3";
        protected string segment_2D4 = "2D4";
        protected string segment_3D1 = "3D1";
        protected string segment_3D2 = "3D2";
        protected string segment_3D3 = "3D3";
        protected string segment_3D4 = "3D4";
        protected string segment_4D1 = "4D1";
        protected string segment_4D2 = "4D2";
        protected string segment_4D3 = "4D3";
        protected string segment_4D4 = "4D4";
        protected string segment_5D1 = "5D1";
        protected string segment_5D2 = "5D2";
        protected string segment_5D3 = "5D3";
        protected string segment_5D4 = "5D4";

        // Hand and arm markers
        protected string marker_FA2 = "FA2";
        protected string marker_FA1 = "FA1";
        protected string marker_WRA = "WRA";
        protected string marker_WRB = "WRB";
    
        // thumb markers
        protected string marker_TH1 = "TH1";
        protected string marker_TH2 = "TH2";
        protected string marker_TH3 = "TH3";
        protected string marker_TH3P = "TH3P";
        protected string marker_TH4 = "TH4";

        // index finger markers
        protected string marker_H2 = "H2";
        protected string marker_IF1 = "IF1";
        protected string marker_IF2 = "IF2";
        protected string marker_IF3 = "IF3";

        // middle finger markers
        protected string marker_H3 = "H3";
        protected string marker_TF1 = "TF1";
        protected string marker_TF2 = "TF2";
        protected string marker_TF3 = "TF3";

        // ring finger markers
        protected string marker_H4 = "H4";
        protected string marker_RF2 = "RF2";
        protected string marker_RF3 = "RF3";
        protected string marker_RF4 = "RF4";

        // pinky finger markers
        protected string marker_H5 = "H5";
        protected string marker_PF1 = "PF1";
        protected string marker_PF2 = "PF2";
        protected string marker_PF3 = "PF3";

        protected string finger_1 = "1";
        protected string finger_2 = "2";
        protected string finger_3 = "3";
        protected string finger_4 = "4";
        protected string finger_5 = "5";

        protected Dictionary<string, Vector3> baseVectors = new Dictionary<string, Vector3>();
        protected Dictionary<string, Vector3> previousSegments = new Dictionary<string, Vector3>();

        protected Dictionary<string, string> segmentChild;
        protected Dictionary<string, string> segmentParents;
        protected Dictionary<string, List<string>> fingerSegments;

        protected virtual void Start()
        {
            string prefix = handedness == Handedness.Right ? "R": "L";

            segment_1D1 = prefix + segment_1D1;
            segment_1D2 = prefix + segment_1D2;
            segment_1D3 = prefix + segment_1D3;
            segment_1D4 = prefix + segment_1D4;
            segment_2D1 = prefix + segment_2D1;
            segment_2D2 = prefix + segment_2D2;
            segment_2D3 = prefix + segment_2D3;
            segment_2D4 = prefix + segment_2D4;
            segment_3D1 = prefix + segment_3D1;
            segment_3D2 = prefix + segment_3D2;
            segment_3D3 = prefix + segment_3D3;
            segment_3D4 = prefix + segment_3D4;
            segment_4D1 = prefix + segment_4D1;
            segment_4D2 = prefix + segment_4D2;
            segment_4D3 = prefix + segment_4D3;
            segment_4D4 = prefix + segment_4D4;
            segment_5D1 = prefix + segment_5D1;
            segment_5D2 = prefix + segment_5D2;
            segment_5D3 = prefix + segment_5D3;
            segment_5D4 = prefix + segment_5D4;
            marker_FA2 = prefix + marker_FA2;
            marker_FA1 = prefix + marker_FA1;
            marker_WRA = prefix + marker_WRA;
            marker_WRB = prefix + marker_WRB;
            marker_TH1 = prefix + marker_TH1;
            marker_TH2 = prefix + marker_TH2;
            marker_TH3 = prefix + marker_TH3;
            marker_TH3P = prefix + marker_TH3P;
            marker_TH4 = prefix + marker_TH4;
            marker_H2 = prefix + marker_H2;
            marker_IF1 = prefix + marker_IF1;
            marker_IF2 = prefix + marker_IF2;
            marker_IF3 = prefix + marker_IF3;
            marker_H3 = prefix + marker_H3;
            marker_TF1 = prefix + marker_TF1;
            marker_TF2 = prefix + marker_TF2;
            marker_TF3 = prefix + marker_TF3;
            marker_H4 = prefix + marker_H4;
            marker_RF2 = prefix + marker_RF2;
            marker_RF3 = prefix + marker_RF3;
            marker_RF4 = prefix + marker_RF4;
            marker_H5 = prefix + marker_H5;
            marker_PF1 = prefix + marker_PF1;
            marker_PF2 = prefix + marker_PF2;
            marker_PF3 = prefix + marker_PF3;
            finger_1 = prefix + finger_1;
            finger_2 = prefix + finger_2;
            finger_3 = prefix + finger_3;
            finger_4 = prefix + finger_4;
            finger_5 = prefix + finger_5;

            segmentChild = new Dictionary<string, string>()
            {
                //{segment_Arm, null},
                {segment_Arm, segment_Hand},
                {segment_Hand, segment_3D1},

                {segment_1D1, segment_1D2},
                {segment_1D2, segment_1D3},
                {segment_1D3, segment_1D4},
        
                {segment_2D1, segment_2D2},
                {segment_2D2, segment_2D3},
                {segment_2D3, segment_2D4},
        
                {segment_3D1, segment_3D2},
                {segment_3D2, segment_3D3},
                {segment_3D3, segment_3D4},
        
                {segment_4D1, segment_4D2},
                {segment_4D2, segment_4D3},
                {segment_4D3, segment_4D4},

                {segment_5D1, segment_5D2},
                {segment_5D2, segment_5D3},
                {segment_5D3, segment_5D4},
            };

            segmentParents = new Dictionary<string, string>()
            {
                //{segment_Arm, null},
                {segment_Hand, segment_Arm},
                //{segment_1D1, segment_Hand},
                //{segment_2D1, segment_Hand},
                {segment_3D1, segment_Hand},
                //{segment_4D1, segment_Hand},
                //{segment_5D1, segment_Hand},

                {segment_1D2, segment_1D1},
                {segment_2D2, segment_2D1},
                {segment_3D2, segment_3D1},
                {segment_4D2, segment_4D1},
                {segment_5D2, segment_5D1},
        
                {segment_1D3, segment_1D2},
                {segment_2D3, segment_2D2},
                {segment_3D3, segment_3D2},
                {segment_4D3, segment_4D2},
                {segment_5D3, segment_5D2},
        
                {segment_1D4, segment_1D3},
                {segment_2D4, segment_2D3},
                {segment_3D4, segment_3D3},
                {segment_4D4, segment_4D3},
                {segment_5D4, segment_5D3},
            };

            segmentMarkers = new Dictionary<string, List<string>>() {
                { segment_Arm, new List<string>() {marker_FA2, marker_FA1}} ,//{ marker_WRB, marker_FA2, marker_FA1, marker_WRA } } ,
                {    segment_Hand, new List<string>() { marker_WRB, marker_WRA }} ,//{"RH1", marker_H3, "RH6"}}, 
                {    segment_1D1,  new List<string>(){marker_TH1}},
                {    segment_1D2, new List<string>(){marker_TH2}},
                {    segment_1D3, new List<string>(){marker_TH3, marker_TH3P}},
                {    segment_1D4, new List<string>{marker_TH4}},

                {    segment_2D1, new List<string>{marker_H2}},
                {    segment_2D2, new List<string>{marker_IF1}}, 
                {    segment_2D3, new List<string>{marker_IF2}},
                {    segment_2D4, new List<string>{marker_IF3}}, 
        
                {    segment_3D1, new List<string>{marker_H3}},
                {    segment_3D2, new List<string>{marker_TF1}},
                {    segment_3D3, new List<string>{marker_TF2}},
                {    segment_3D4, new List<string>{marker_TF3}},
        
                {    segment_4D1, new List<string>{marker_H4}},
                {    segment_4D2, new List<string>{marker_RF2}},
                {    segment_4D3, new List<string>{marker_RF3}},
                {    segment_4D4, new List<string>{marker_RF4}},
        
                {   segment_5D1, new List<string>(){marker_H5}}, 
                {    segment_5D2,  new List<string>(){marker_PF1}}, 
                {    segment_5D3, new List<string>(){marker_PF2}},
                {    segment_5D4, new List<string>(){marker_PF3}}
            };

            fingerSegments = new Dictionary<string, List<string>>()
            {
                {finger_1, new List<string>{segment_1D1, segment_1D2, segment_1D3, segment_1D4}},
                {finger_2, new List<string>{segment_2D1, segment_2D2, segment_2D3, segment_2D4}},
                {finger_3, new List<string>{segment_3D1, segment_3D2, segment_3D3, segment_3D4}},
                {finger_4, new List<string>{segment_4D1, segment_4D2, segment_4D3, segment_4D4}},
                {finger_5, new List<string>{segment_5D1, segment_5D2, segment_5D3, segment_5D4}},
            };


            SetupMessagePack();
            SetupWriter();
            SetupFilter();
        }

        protected bool isRightHand()
        {
            return handedness == Handedness.Right;
        }

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
            palm = segments[segment_Hand] - (segments[segment_3D1] + 0.5f * (segments[segment_4D1] - segments[segment_4D1]));
            normal = Vector3.Cross(palm, segments[segment_4D1] - segments[segment_3D1]);
            if (!isRightHand())
            {
                 normal = -normal;
            }

            if (segmentChild.ContainsKey(segment_1D1) && segments[segmentChild[segment_1D1]] != Vector3.zero && segments[segment_1D1] != Vector3.zero)
            {
                baseVectors[finger_1] = segments[segmentChild[segment_1D1]] - segments[segment_1D1];
            }

            if (segmentChild.ContainsKey(segment_2D1) && segments[segmentChild[segment_2D1]] != Vector3.zero && segments[segment_2D1] != Vector3.zero)
            {
                baseVectors[finger_2] = segments[segmentChild[segment_2D1]] - segments[segment_2D1];
            }

            if (segmentChild.ContainsKey(segment_3D1) && segments[segmentChild[segment_3D1]] != Vector3.zero && segments[segment_3D1] != Vector3.zero)
            {
                baseVectors[finger_3] = segments[segmentChild[segment_3D1]] - segments[segment_3D1];
            }

            if (segmentChild.ContainsKey(segment_4D1) && segments[segmentChild[segment_4D1]] != Vector3.zero && segments[segment_4D1] != Vector3.zero)
            {
                baseVectors[finger_4] = segments[segmentChild[segment_4D1]] - segments[segment_4D1];
            }

            if (segmentChild.ContainsKey(segment_5D1) && segments[segmentChild[segment_5D1]] != Vector3.zero && segments[segment_5D1] != Vector3.zero)
            {
                baseVectors[finger_5] = segments[segmentChild[segment_5D1]] - segments[segment_5D1];
            }

            // Debug.Log(data.data[marker_TH3P] + "  -  "+ data.data[marker_TH3]);
            if (data.data.ContainsKey(marker_TH3P) && data.data.ContainsKey(marker_TH3))
            {
                var p1 = data.data[marker_TH3P];
                var p2 = data.data[marker_TH3];
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
                    float dotPrduct = Vector3.Dot(p1p2Vector, isRightHand() ? normal : -normal);

                    if (dotPrduct < 0)
                    {
                        p1p2Vector = -p1p2Vector;
                    }

                    baseVectors["R1_right"] = p1p2Vector;
                }
            }
            // Vector3.Cross(segments[segment_1D4] - segments[segment_1D3], segments[segment_1D3] - segments[segment_1D2]);
            // Debug.DrawRay(segments[segment_3D1], normal);
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
                                        if (fingerId == finger_1)
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
                                if (fingerId == finger_1)
                                    Bone.position += Bone.forward * normalOffset * 0.9f;
                                else if (fingerId == finger_3)
                                    Bone.position += Bone.forward * normalOffset * 1.08f;
                                else if (fingerId == finger_4)
                                    Bone.position += Bone.forward * normalOffset * 1.13f;
                                else if (fingerId == finger_5)
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
            if (Bone.name == segment_Hand)
                handWorldToLocalMatrix = Bone.worldToLocalMatrix;
        }

        protected override bool TestSegmentsQulity(Dictionary<string, Vector3> segments)
        {
            if (segments.ContainsKey(segment_5D3) &&
                segments.ContainsKey(segment_2D3) &&
                segments.ContainsKey(segment_5D1) &&
                segments.ContainsKey(segment_2D1))
            {
                float d3_d1_dot = Vector3.Dot(segments[segment_5D3] - segments[segment_2D3], segments[segment_5D1] - segments[segment_2D1]);
                if (d3_d1_dot > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                // TODO quality check when specific markers are missing
                return true;
            }
        }
    }

    public enum Handedness {
        Left, Right
    }
}
