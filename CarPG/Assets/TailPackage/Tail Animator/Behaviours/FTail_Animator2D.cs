using UnityEngine;

namespace FIMSpace.FTail
{
    /// <summary>
    /// FM: Class which is animating Tail Animator behaviour in Sprites 2D space
    /// </summary>
    [AddComponentMenu("FImpossible Creations/Tail Animator/FTail Animator2D")]
    public class FTail_Animator2D : FTail_AnimatorUI
    { 
        /// <summary>
        /// Setting correction options to be setted for 2D sprites space behaviour
        /// </summary>
        protected override void Reset()
        {
            AxisCorrection = -Vector3.right;
            AxisLookBack = Vector3.up;

            ExtraCorrectionOptions = false;
            ExtraFromDirection = Vector3.forward;
            ExtraToDirection = Vector3.right;

            FullCorrection = false;
            WavingAxis = Vector3.forward;
        }
    }
}