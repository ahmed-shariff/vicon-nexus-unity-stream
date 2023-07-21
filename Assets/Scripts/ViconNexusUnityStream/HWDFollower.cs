using UnityEngine;

namespace ubc.ok.ovilab.ViconUnityStream
{
    public class HWDFollower: MonoBehaviour
    {
        public Transform base1;
        public Transform base2;
        public Transform base3;
        public Transform base4;

        public bool applyFilter = false;
        public float filterMinCutoff = 0.1f, filterBeta = 50;

        private OneEuroFilter<Quaternion> filter;

        void Start()
        {
            filter = new OneEuroFilter<Quaternion>(90, filterMinCutoff, filterBeta);
        }

        void Update()
        {
            transform.position = base1.position;
            Vector3 forward = base1.position - base2.position;
            if (forward != Vector3.zero)
            {
                Vector3 right = base3.position - base4.position;
                if (right != Vector3.zero)
                {
                    Quaternion rotation = Quaternion.LookRotation(forward, Vector3.Cross(right, forward));
                    if (applyFilter)
                    {
                        transform.rotation = filter.Filter(rotation, Time.realtimeSinceStartup);
                    }
                    else
                    {
                        transform.rotation = rotation;
                    }
                }
            }
        }
    }
}
