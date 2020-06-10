using UnityEngine;

namespace FIMSpace
{
    public class FImp_ColliderData_Sphere : FImp_ColliderData_Base
    {
        public SphereCollider Sphere { get; private set; }
        private float SphereRadius;

        public FImp_ColliderData_Sphere(SphereCollider collider)
        {
            Collider = collider;
            Sphere = collider;
            ColliderType = EFColliderType.Sphere;
            RefreshColliderData();
        }

        public override void RefreshColliderData()
        {
            if (IsStatic) return; // No need to refresh collider data if it is static

            SphereRadius = CalculateTrueRadiusOfSphereCollider(Sphere.transform, Sphere.radius);
            base.RefreshColliderData();
        }

        public override bool PushIfInside(ref Vector3 point, float pointRadius, Vector3 pointOffset)
        {
            return PushOutFromSphereCollider(Sphere, pointRadius, ref point, SphereRadius, pointOffset);
        }


        public static bool PushOutFromSphereCollider(SphereCollider sphere, float segmentColliderRadius, ref Vector3 segmentPos, Vector3 segmentOffset)
        {
            return PushOutFromSphereCollider(sphere, segmentColliderRadius, ref segmentPos, CalculateTrueRadiusOfSphereCollider(sphere), segmentOffset);
        }


        public static bool PushOutFromSphereCollider(SphereCollider sphere, float segmentColliderRadius, ref Vector3 segmentPos, float collidingSphereRadius, Vector3 segmentOffset)
        {
            Vector3 sphereCenter = sphere.transform.position + sphere.transform.TransformVector(sphere.center);
            float radius = collidingSphereRadius + segmentColliderRadius;

            Vector3 pushNormal = (segmentPos + segmentOffset) - sphereCenter;
            float squaredPushMagn = pushNormal.sqrMagnitude;

            if (squaredPushMagn > 0 && squaredPushMagn < radius * radius)
            {
                segmentPos = sphereCenter - segmentOffset + pushNormal * (radius / Mathf.Sqrt(squaredPushMagn));
                return true;
            }

            return false;
        }

        #region Sphere Calculation Helpers

        /// <summary>
        /// Calculating radius of sphere collider including sphere collider's transform scalling
        /// </summary>
        public static float CalculateTrueRadiusOfSphereCollider(SphereCollider sphere)
        {
            return CalculateTrueRadiusOfSphereCollider(sphere.transform, sphere.radius);
        }

        /// <summary>
        /// Calculating radius of sphere collider including sphere collider's transform scalling
        /// </summary>
        public static float CalculateTrueRadiusOfSphereCollider(Transform transform, float componentRadius)
        {
            float radius = componentRadius;

            if (transform.lossyScale.x > transform.lossyScale.y)
            {
                if (transform.lossyScale.x > transform.lossyScale.z) radius *= transform.lossyScale.x;
                else
                    radius *= transform.lossyScale.z;
            }
            else
            {
                if (transform.lossyScale.y > transform.lossyScale.z) radius *= transform.lossyScale.y;
                else
                    radius *= transform.lossyScale.z;
            }

            return radius;
        }

        #endregion
    }
}
