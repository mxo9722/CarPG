using FIMSpace.Basics;
using UnityEngine;

namespace FIMSpace.FTail
{
    /// <summary>
    /// FM: Demo class to give levers functionality to manipulate objects a bit
    /// </summary>
    public class FTail_Demo_LeverUsage : MonoBehaviour
    {
        public Transform TargetToMove;
        public Vector3 TargetLocalOffset;

        private Vector3 initPosition;
        private Quaternion initRotation;

        private FBasic_PullableLever lever;

        public bool RotationLever = false;

        void Start()
        {
            lever = GetComponent<FBasic_PullableLever>();

            if (!RotationLever)
                initPosition = TargetToMove.position;

        }

        void Update()
        {
            if (RotationLever)
            {
                TargetToMove.rotation = Quaternion.Euler(360 * lever.LeverValueY, 720f * lever.LeverValueY, 0f) * Quaternion.Euler(0f,-90f,0f);
            }
            else
                TargetToMove.position = Vector3.Lerp(initPosition, initPosition + TargetLocalOffset, lever.LeverValueY);
        }
    }
}