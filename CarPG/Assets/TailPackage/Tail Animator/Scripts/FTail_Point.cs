using UnityEngine;

namespace FIMSpace.FTail
{
    /// <summary>
    /// FM: Helper class to animate tail bones freely
    /// </summary>
    [System.Serializable]
    public class FTail_Point
    {
        // References for tail chain operations
        public FTail_Point NextPoint;
        public FTail_Point BackPoint;

        // Main identification variables for tail point
        public int index = -1;
        public Transform Transform;
        public Vector3 Position = Vector3.zero;
        public Quaternion Rotation = Quaternion.identity;
        public Quaternion PreviousRotation = Quaternion.identity;

        // Variables to upport different features
        public Quaternion InitialRotation = Quaternion.identity;
        public Vector3 InitialLossyScale = Vector3.one;
        public Vector3 PreviousPosition = Vector3.zero;

        public Vector3 SpringOffset = Vector3.zero;
        public Vector3 SpringOffset2 = Vector3.zero;
        //public Vector3 Momentum = Vector3.zero;

        /// <summary> Vector of forward direction onto other bone</summary>
        public Vector3 LookDirection;
        public Vector3 InitLookDirection;
        public Vector3 CrossUp = Vector3.up;

        /// <summary> Length of the bone - distance to next bone transform </summary>
        public float BoneLength;
        public float InitBoneLength;
        public float ScaleFactor = 1f;

        /// <summary> Correction offsets for bones to support animator stuff etc. </summary>
        public Quaternion Correction = Quaternion.identity;
        public Vector3 InitialLocalPosition = Vector3.zero;
        public Quaternion InitialLocalRotation = Quaternion.identity;
        public Vector3 LookBackDirection;

        // Collision related variables
        public Vector3 RotationTargetPos;
        public Vector3 PreCollisionPosition;
        public Quaternion PreCollisionRotation;
        public float ColliderRadius = 1f;
        public Vector3 collisionOffsets;
        public float collisionFlags = 0f;
        public Collision collisionContacts = null;

        public FTail_CollisionHelper CollisionHelper { get; internal set; }
        public Vector3 PreCalcPos { get; internal set; }
        public Vector3 PreCalcOtherPos { get; internal set; }

        public FTail_Point() { }

        public FTail_Point(Transform transform)
        {
            if (transform == null) return;
            this.Transform = transform;
            Position = transform.position;
            PreviousPosition = Position;
            Rotation = transform.rotation;
            PreviousRotation = Rotation;
            InitialRotation = Rotation;
            InitialLossyScale = transform.lossyScale;
            RotationTargetPos = Position;
            InitialLocalRotation = transform.localRotation;
            InitialLocalPosition = transform.localPosition;
            LookBackDirection = transform.localRotation * Vector3.forward;
            PreCollisionPosition = Position;
        }

        public void SetPosition(Vector3 position)
        {
            //PreviousPosition = Position;
            Position = position;
        }

        public void SetRotation(Quaternion rotation)
        {
            //PreviousRotation = Rotation;
            Rotation = rotation;
        }

        public float GetRadiusScaled()
        {
            return ColliderRadius * Transform.lossyScale.x;
        }

        public Vector3 TransformDirection(Vector3 dir)
        {
            return Rotation * dir;
        }

        public Vector3 TransformCollisionDirection(Vector3 dir)
        {
            return PreCollisionRotation * dir;
        }

        public static Vector3 CalculateLocalForward(Transform parent, Transform child)
        {
            return -(parent.InverseTransformPoint(child.position) - parent.InverseTransformPoint(parent.position)).normalized;
        }

        public static Vector3 CalculateCrossUp(Transform parent, Transform child, Transform orientationReference)
        {
            Vector3 inversedNext = parent.InverseTransformPoint(child.position);
            Vector3 inversedRoot = parent.InverseTransformPoint(parent.position);

            Vector3 forward = -(inversedNext - inversedRoot).normalized;

            Vector3 cross = Vector3.Cross(forward, orientationReference.right).normalized;
            if (cross == Vector3.zero) cross = Vector3.up;

            return cross;
        }
    }
}