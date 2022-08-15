using UnityEngine;

namespace umanitoba.hcilab.ViconUnityStream
{
    public class HWDFollower: MonoBehaviour
    {
        public Transform base1;
        public Transform base2;
        public Transform base3;
        public Transform base4;

        void Update()
        {
            transform.position = base1.position;
            Vector3 forward = base1.position - base2.position;
            if (forward != Vector3.zero)
            {
                Vector3 right = base3.position - base4.position;
                if (right != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(forward, Vector3.Cross(right, forward));
                }
            }
        }
    }
}
