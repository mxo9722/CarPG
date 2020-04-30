using UnityEngine;

namespace FIMSpace.FTail
{
    /// <summary>
    /// FM: Class which is animating Tail Animator behaviour in UI 2D space
    /// </summary>
    [AddComponentMenu("FImpossible Creations/Tail Animator/FTail Animator UI")]
    public class FTail_AnimatorUI : FTail_Animator
    {
        public bool Lock2D = true;

        protected override void Init()
        {
            UseAutoCorrectLookAxis = false;
            base.Init();
        }

        /// <summary>
        /// Setting extra correction variables for UI
        /// </summary>
        protected override void Reset()
        {
            UseAutoCorrectLookAxis = false;
            AxisCorrection = Vector3.right;
            AxisLookBack = Vector3.up;

            ExtraCorrectionOptions = false;
            ExtraFromDirection = Vector3.forward;
            ExtraToDirection = Vector3.right;

            WavingAxis = Vector3.forward;
            FullCorrection = false;
        }


        /// <summary>
        /// For UI we must calculate it differently
        /// </summary>
        protected override Quaternion CalculateTargetRotation(FTail_Point tailPoint)
        {
            if (Lock2D)
                return CalculateFor2D(tailPoint);
            else
                return base.CalculateTargetRotation(tailPoint);
        }


        protected Quaternion CalculateFor2D(FTail_Point tailPoint)
        {
            Quaternion targetRotation;

            if (FullCorrection)
            {
                Vector3 startLookPos = tailPoint.BackPoint.Position;
                Vector3 lookingAt = tailPoint.RotationTargetPos;

                targetRotation = Quaternion.identity;

                if (LookUpMethod != FELookUpMethod.Parental)
                {
                    startLookPos += tailPoint.BackPoint.TransformDirection(tailPoint.BackPoint.LookDirection) * tailPoint.InitBoneLength * tailPoint.ScaleFactor * sensitivityPower;
                    startLookPos -= (tailPoint.BackPoint.PreCollisionPosition - tailPoint.BackPoint.Position) * CollisionSwapping;

                    if (startLookPos - lookingAt != Vector3.zero)
                    {
                        targetRotation = FLogicMethods.TopDownAnglePosition2D(lookingAt, startLookPos);
                    }

                    targetRotation *= Quaternion.FromToRotation(tailPoint.BackPoint.LookDirection, ExtraToDirection);
                }
                else // Parental method
                {
                    startLookPos += tailPoint.BackPoint.TransformDirection(tailPoint.BackPoint.LookDirection) * tailPoint.InitBoneLength * tailPoint.ScaleFactor * sensitivityPower;

                    Vector3 targetPos = lookingAt - startLookPos;

                    Quaternion targetingRot = Quaternion.FromToRotation(tailPoint.BackPoint.TransformDirection(tailPoint.Transform.localPosition), targetPos);

                    targetRotation = targetingRot * tailPoint.BackPoint.Rotation;
                }

                if (Curving != Vector3.zero)
                {
                    float mul = 10 / ((float)tailPoint.index * 4.5f + 1f);
                    targetRotation *= Quaternion.Euler(Curving.y * mul, Curving.x * mul, 0f);
                }

                targetRotation *= tailPoint.Correction;
            }
            else
            {
                targetRotation = Quaternion.identity;
                Vector3 startLookPos = tailPoint.BackPoint.Position;
                Vector3 lookingAt = tailPoint.RotationTargetPos;

                startLookPos += tailPoint.BackPoint.TransformDirection(tailPoint.BackPoint.LookDirection) * tailPoint.InitBoneLength * sensitivityPower;
                startLookPos -= (tailPoint.BackPoint.PreCollisionPosition - tailPoint.BackPoint.Position) * CollisionSwapping;

                if (startLookPos - lookingAt != Vector3.zero) targetRotation = FLogicMethods.TopDownAnglePosition2D( startLookPos, lookingAt);
                //targetRotation = Quaternion.LookRotation(startLookPos - lookingAt, tailPoint.BackPoint.TransformDirection(AxisLookBack * Mathf.Sign(FVectorMethods.VectorSum(AxisCorrection))));

                if (Curving != Vector3.zero)
                {
                    float mul = 10f / ((float)tailPoint.index * 4.5f + 1f);
                    targetRotation *= Quaternion.Euler(Curving.y * mul, Curving.x * mul, 0f);
                }

                if (ExtraCorrectionOptions) targetRotation *= Quaternion.FromToRotation(ExtraFromDirection, ExtraToDirection);
            }

            return targetRotation;
        }

    }
}