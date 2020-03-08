using UnityEngine;

namespace FIMSpace
{
    public class FImp_ColliderData_Box : FImp_ColliderData_Base
    {
        public BoxCollider Box { get; private set; }

        private Vector3 boxCenter;

        private Vector3 right;
        private Vector3 up;
        private Vector3 forward;

        private Vector3 rightN;
        private Vector3 upN;
        private Vector3 forwardN;

        private Vector3 scales;

        public FImp_ColliderData_Box(BoxCollider collider)
        {
            Collider = collider;
            Box = collider;
            ColliderType = EFColliderType.Box;
            RefreshColliderData();
            previousPosition = collider.transform.position + Vector3.forward * 0.01f;
        }

        public override void RefreshColliderData()
        {
            if (IsStatic) return; // No need to refresh collider data if it is static

            bool diff = false;

            if ((previousPosition - Collider.transform.position).magnitude > 0f) diff = true;
            else
                if ((Collider.transform.rotation * Quaternion.Inverse(previousRotation)) != Quaternion.identity) diff = true;

            if (diff)
            {
                right = Box.transform.TransformVector((Vector3.right / 2f) * Box.size.x);
                up = Box.transform.TransformVector((Vector3.up / 2f) * Box.size.y);
                forward = Box.transform.TransformVector((Vector3.forward / 2f) * Box.size.z);

                rightN = right.normalized;
                upN = up.normalized;
                forwardN = forward.normalized;

                boxCenter = GetBoxCenter(Box);

                scales = Vector3.Scale(Box.size, Box.transform.lossyScale);
                scales.Normalize();
            }

            base.RefreshColliderData();

            previousPosition = Collider.transform.position;
            previousRotation = Collider.transform.rotation;
        }


        public override bool PushIfInside(ref Vector3 segmentPosition, float segmentRadius, Vector3 segmentOffset)
        {
            int inOrInt = 0;
            Vector3 interPlane = Vector3.zero;
            Vector3 segmentOffsetted = segmentPosition + segmentOffset;
            float planeDistance = PlaneDistance(boxCenter + up, upN, segmentOffsetted);
            if (SphereInsidePlane(planeDistance, segmentRadius)) inOrInt++; else if (SphereIntersectsPlane(planeDistance, segmentRadius)) { inOrInt++; interPlane = up; }

            planeDistance = PlaneDistance(boxCenter - up, -upN, segmentOffsetted);
            if (SphereInsidePlane(planeDistance, segmentRadius)) inOrInt++; else if (SphereIntersectsPlane(planeDistance, segmentRadius)) { inOrInt++; interPlane = -up; }

            planeDistance = PlaneDistance(boxCenter - right, -rightN, segmentOffsetted);
            if (SphereInsidePlane(planeDistance, segmentRadius)) inOrInt++; else if (SphereIntersectsPlane(planeDistance, segmentRadius)) { inOrInt++; interPlane = -right; }

            planeDistance = PlaneDistance(boxCenter + right, rightN, segmentOffsetted);
            if (SphereInsidePlane(planeDistance, segmentRadius)) inOrInt++; else if (SphereIntersectsPlane(planeDistance, segmentRadius)) { inOrInt++; interPlane = right; }

            planeDistance = PlaneDistance(boxCenter + forward, forwardN, segmentOffsetted);
            if (SphereInsidePlane(planeDistance, segmentRadius)) inOrInt++; else if (SphereIntersectsPlane(planeDistance, segmentRadius)) { inOrInt++; interPlane = forward; }

            planeDistance = PlaneDistance(boxCenter - forward, -forwardN, segmentOffsetted);
            if (SphereInsidePlane(planeDistance, segmentRadius)) inOrInt++; else if (SphereIntersectsPlane(planeDistance, segmentRadius)) { inOrInt++; interPlane = -forward; }

            // Collision occured - sphere intersecting box shape volume or is inside of it
            if (inOrInt == 6)
            {
                bool inside = false;
                Vector3 rayDirection;

                if (interPlane == Vector3.zero) // sphere is inside the box
                {
                    interPlane = -GetTargetPlaneNormal(Box, segmentOffsetted, right, up, forward, scales);
                    inside = true;

                    rayDirection = (interPlane).normalized; // poprawić przy przeskalowanych boxach
                }
                else // sphere is intersecting box
                {
                    rayDirection = (segmentOffsetted - boxCenter).normalized;

                    if (IsInsideBoxCollider(Box, segmentOffsetted))
                    {
                        inside = true;
                    }
                }

                Vector3 pointOnPlane = GetNearestPoint(segmentOffsetted);

                Vector3 toNormal = pointOnPlane - segmentOffsetted;
                if (inside) toNormal += toNormal.normalized * segmentRadius; else toNormal -= toNormal.normalized * segmentRadius;

                if (inside)
                {
                    segmentPosition = segmentPosition + toNormal;
                }
                else
                    if (toNormal.magnitude > 0) segmentPosition = segmentPosition + toNormal;

                return true;
            }

            return false;
        }


        public static void PushOutFromBoxCollider(BoxCollider box, Collision collision, float segmentColliderRadius, ref Vector3 segmentPosition)
        {
            Vector3 right = box.transform.TransformVector((Vector3.right / 2f) * box.size.x + box.center.x * Vector3.right);
            Vector3 up = box.transform.TransformVector((Vector3.up / 2f) * box.size.y + box.center.y * Vector3.up);
            Vector3 forward = box.transform.TransformVector((Vector3.forward / 2f) * box.size.z + box.center.z * Vector3.forward);

            Vector3 scales = Vector3.Scale(box.size, box.transform.lossyScale);
            scales.Normalize();

            PushOutFromBoxCollider(box, collision, segmentColliderRadius, ref segmentPosition, right, up, forward, scales);
        }

        public static void PushOutFromBoxCollider(BoxCollider box, float segmentColliderRadius, ref Vector3 segmentPosition)
        {
            Vector3 right = box.transform.TransformVector((Vector3.right / 2f) * box.size.x + box.center.x * Vector3.right);
            Vector3 up = box.transform.TransformVector((Vector3.up / 2f) * box.size.y + box.center.y * Vector3.up);
            Vector3 forward = box.transform.TransformVector((Vector3.forward / 2f) * box.size.z + box.center.z * Vector3.forward);

            Vector3 scales = Vector3.Scale(box.size, box.transform.lossyScale);
            scales.Normalize();

            Vector3 boxCenter = GetBoxCenter(box);

            float pointRadius = segmentColliderRadius;
            Vector3 upN = up.normalized; Vector3 rightN = right.normalized; Vector3 forwardN = forward.normalized;

            int inOrInt = 0;
            Vector3 interPlane = Vector3.zero;
            float planeDistance = PlaneDistance(boxCenter + up, upN, segmentPosition);
            if (SphereInsidePlane(planeDistance, pointRadius)) inOrInt++; else if (SphereIntersectsPlane(planeDistance, pointRadius)) { inOrInt++; interPlane = up; }

            planeDistance = PlaneDistance(boxCenter - up, -upN, segmentPosition);
            if (SphereInsidePlane(planeDistance, pointRadius)) inOrInt++; else if (SphereIntersectsPlane(planeDistance, pointRadius)) { inOrInt++; interPlane = -up; }

            planeDistance = PlaneDistance(boxCenter - right, -rightN, segmentPosition);
            if (SphereInsidePlane(planeDistance, pointRadius)) inOrInt++; else if (SphereIntersectsPlane(planeDistance, pointRadius)) { inOrInt++; interPlane = -right; }

            planeDistance = PlaneDistance(boxCenter + right, rightN, segmentPosition);
            if (SphereInsidePlane(planeDistance, pointRadius)) inOrInt++; else if (SphereIntersectsPlane(planeDistance, pointRadius)) { inOrInt++; interPlane = right; }

            planeDistance = PlaneDistance(boxCenter + forward, forwardN, segmentPosition);
            if (SphereInsidePlane(planeDistance, pointRadius)) inOrInt++; else if (SphereIntersectsPlane(planeDistance, pointRadius)) { inOrInt++; interPlane = forward; }

            planeDistance = PlaneDistance(boxCenter - forward, -forwardN, segmentPosition);
            if (SphereInsidePlane(planeDistance, pointRadius)) inOrInt++; else if (SphereIntersectsPlane(planeDistance, pointRadius)) { inOrInt++; interPlane = -forward; }

            // Collision occured - sphere intersecting box shape volume or is inside of it
            if (inOrInt == 6)
            {
                bool inside = false;
                Vector3 rayDirection;

                if (interPlane == Vector3.zero) // sphere is inside the box
                {
                    interPlane = -GetTargetPlaneNormal(box, segmentPosition, right, up, forward, scales);
                    inside = true;

                    rayDirection = (interPlane).normalized; // poprawić przy przeskalowanych boxach
                }
                else // sphere is intersecting box
                {
                    rayDirection = (segmentPosition - boxCenter).normalized;
                    if (IsInsideBoxCollider(box, segmentPosition)) inside = true;
                }

                Vector3 pointOnPlane = GetNearestPoint(segmentPosition, boxCenter, right, up, forward);

                Vector3 toNormal = pointOnPlane - segmentPosition;
                if (inside) toNormal += toNormal.normalized * pointRadius * 1.01f; else toNormal -= toNormal.normalized * pointRadius * 1.01f;

                if (inside)
                {
                    segmentPosition = segmentPosition + toNormal;
                    //Debug.DrawRay(segmentPosition, Vector3.up, Color.blue);
                }
                else
                    if (toNormal.magnitude > 0)
                {
                    segmentPosition = segmentPosition + toNormal;
                    //Debug.DrawRay(segmentPosition, Vector3.up, Color.yellow);
                }
                //else
                //{
                //    Debug.DrawRay(segmentPosition, Vector3.up, Color.red);
                //}
            }
        }


        public static void PushOutFromBoxCollider(BoxCollider box, Collision collision, float segmentColliderRadius, ref Vector3 pos, Vector3 right, Vector3 up, Vector3 forward, Vector3 scales)
        {
            Vector3 collisionPoint = collision.contacts[0].point;
            Vector3 pushNormal = pos - collisionPoint;
            Vector3 boxCenter = GetBoxCenter(box);
            if (pushNormal == Vector3.zero) pushNormal = pos - boxCenter;

            float insideMul = 1f;
            if (IsInsideBoxCollider(box, pos))
            {
                // Finding intersection point on the box from the inside 
                float castFactor = GetBoxAverageScale(box);
                Vector3 fittingNormal = GetTargetPlaneNormal(box, pos, right, up, forward, scales);
                Vector3 fittingNormalNorm = fittingNormal.normalized;

                RaycastHit info;
                // Doing cheap boxCollider's raycast from outside to hit surface
                if (box.Raycast(new Ray(pos - fittingNormalNorm * castFactor * 3f, fittingNormalNorm), out info, castFactor * 4))
                {
                    collisionPoint = info.point;
                }
                else
                    collisionPoint = GetIntersectOnBoxFromInside(box, boxCenter, pos, fittingNormal);

                pushNormal = collisionPoint - pos;
                insideMul = 100f;
            }

            Vector3 toNormal = pos - ((pushNormal / insideMul + pushNormal.normalized * 1.15f) / 2f) * (segmentColliderRadius);
            toNormal = collisionPoint - toNormal;

            float pushMagn = toNormal.sqrMagnitude;
            if (pushMagn > 0 && pushMagn < segmentColliderRadius * segmentColliderRadius * insideMul) pos = pos + toNormal;
        }


        #region Box Calculations Helpers


        /// <summary>
        /// Getting nearest plane normal fitting to given point position
        /// </summary>
        private Vector3 GetNearestPoint(Vector3 point)
        {
            Vector3 pointOnBox = point;

            Vector3 distancesPositive;
            distancesPositive.x = PlaneDistance(boxCenter + right, rightN, point);
            distancesPositive.y = PlaneDistance(boxCenter + up, upN, point);
            distancesPositive.z = PlaneDistance(boxCenter + forward, forwardN, point);

            Vector3 distancesNegative;
            distancesNegative.x = PlaneDistance(boxCenter - right, -rightN, point);
            distancesNegative.y = PlaneDistance(boxCenter - up, -upN, point);
            distancesNegative.z = PlaneDistance(boxCenter - forward, -forwardN, point);

            float nearestX, nearestY, nearestZ;
            float negX = 1f, negY = 1f, negZ = 1f;

            if (distancesPositive.x > distancesNegative.x) { nearestX = distancesPositive.x; negX = -1f; } else { nearestX = distancesNegative.x; negX = 1f; }
            if (distancesPositive.y > distancesNegative.y) { nearestY = distancesPositive.y; negY = -1f; } else { nearestY = distancesNegative.y; negY = 1f; }
            if (distancesPositive.z > distancesNegative.z) { nearestZ = distancesPositive.z; negZ = -1f; } else { nearestZ = distancesNegative.z; negZ = 1f; }

            if (nearestX > nearestZ)
            {
                if (nearestX > nearestY) { pointOnBox = ProjectPointOnPlane(right * negX, point, nearestX); }
                else
                    pointOnBox = ProjectPointOnPlane(up * negY, point, nearestY);
            }
            else
            {
                if (nearestZ > nearestY) { pointOnBox = ProjectPointOnPlane(forward * negZ, point, nearestZ); }
                else
                    pointOnBox = ProjectPointOnPlane(up * negY, point, nearestY);
            }

            return pointOnBox;
        }

        /// <summary>
        /// Getting nearest plane normal fitting to given point position
        /// </summary>
        private static Vector3 GetNearestPoint(Vector3 point, Vector3 boxCenter, Vector3 right, Vector3 up, Vector3 forward)
        {
            Vector3 pointOnBox = point;

            Vector3 distancesPositive;
            distancesPositive.x = PlaneDistance(boxCenter + right, right.normalized, point);
            distancesPositive.y = PlaneDistance(boxCenter + up, up.normalized, point);
            distancesPositive.z = PlaneDistance(boxCenter + forward, forward.normalized, point);

            Vector3 distancesNegative;
            distancesNegative.x = PlaneDistance(boxCenter - right, -right.normalized, point);
            distancesNegative.y = PlaneDistance(boxCenter - up, -up.normalized, point);
            distancesNegative.z = PlaneDistance(boxCenter - forward, -forward.normalized, point);

            float nearestX, nearestY, nearestZ;
            float negX = 1f, negY = 1f, negZ = 1f;

            if (distancesPositive.x > distancesNegative.x) { nearestX = distancesPositive.x; negX = -1f; } else { nearestX = distancesNegative.x; negX = 1f; }
            if (distancesPositive.y > distancesNegative.y) { nearestY = distancesPositive.y; negY = -1f; } else { nearestY = distancesNegative.y; negY = 1f; }
            if (distancesPositive.z > distancesNegative.z) { nearestZ = distancesPositive.z; negZ = -1f; } else { nearestZ = distancesNegative.z; negZ = 1f; }

            if (nearestX > nearestZ)
            {
                if (nearestX > nearestY) { pointOnBox = ProjectPointOnPlane(right * negX, point, nearestX); }
                else
                    pointOnBox = ProjectPointOnPlane(up * negY, point, nearestY);
            }
            else
            {
                if (nearestZ > nearestY) { pointOnBox = ProjectPointOnPlane(forward * negZ, point, nearestZ); }
                else
                    pointOnBox = ProjectPointOnPlane(up * negY, point, nearestY);
            }

            return pointOnBox;
        }


        /// <summary>
        /// Getting nearest plane point on box collider
        /// </summary>
        public static Vector3 GetNearestPointOnBox(BoxCollider boxCollider, Vector3 point)
        {
            Vector3 right = boxCollider.transform.TransformVector(Vector3.right / 2f);
            Vector3 up = boxCollider.transform.TransformVector(Vector3.up / 2f);
            Vector3 forward = boxCollider.transform.TransformVector(Vector3.forward / 2f);

            Vector3 pointOnBox = point;
            Vector3 center = GetBoxCenter(boxCollider);

            Vector3 rightN = right.normalized;
            Vector3 upN = up.normalized;
            Vector3 forwardN = forward.normalized;

            Vector3 distancesPositive;
            distancesPositive.x = PlaneDistance(center + right, rightN, point);
            distancesPositive.y = PlaneDistance(center + up, upN, point);
            distancesPositive.z = PlaneDistance(center + forward, forwardN, point);

            Vector3 distancesNegative;
            distancesNegative.x = PlaneDistance(center - right, -rightN, point);
            distancesNegative.y = PlaneDistance(center - up, -upN, point);
            distancesNegative.z = PlaneDistance(center - forward, -forwardN, point);

            float nearestX, nearestY, nearestZ;
            float negX = 1f, negY = 1f, negZ = 1f;

            if (distancesPositive.x > distancesNegative.x) { nearestX = distancesPositive.x; negX = -1f; } else { nearestX = distancesNegative.x; negX = 1f; }
            if (distancesPositive.y > distancesNegative.y) { nearestY = distancesPositive.y; negY = -1f; } else { nearestY = distancesNegative.y; negY = 1f; }
            if (distancesPositive.z > distancesNegative.z) { nearestZ = distancesPositive.z; negZ = -1f; } else { nearestZ = distancesNegative.z; negZ = 1f; }

            if (nearestX > nearestZ)
            {
                if (nearestX > nearestY) { pointOnBox = ProjectPointOnPlane(right * negX, point, nearestX); }
                else
                    pointOnBox = ProjectPointOnPlane(up * negY, point, nearestY);
            }
            else
            {
                if (nearestZ > nearestY) { pointOnBox = ProjectPointOnPlane(forward * negZ, point, nearestZ); }
                else
                    pointOnBox = ProjectPointOnPlane(up * negY, point, nearestY);
            }

            return pointOnBox;
        }


        private static float PlaneDistance(Vector3 planeCenter, Vector3 planeNormal, Vector3 point)
        {
            return Vector3.Dot(point - planeCenter, planeNormal);
        }

        private static Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 point, float distance)
        {
            Vector3 translationVector = planeNormal.normalized * distance;
            return point + translationVector;
        }

        private static bool SphereInsidePlane(float planeDistance, float pointRadius) { return -planeDistance > pointRadius; }
        private static bool SphereOutsidePlane(float planeDistance, float pointRadius) { return planeDistance > pointRadius; }
        private static bool SphereIntersectsPlane(float planeDistance, float pointRadius) { return Mathf.Abs(planeDistance) <= pointRadius; }


        public static bool IsInsideBoxCollider(BoxCollider collider, Vector3 point)
        {
            point = collider.transform.InverseTransformPoint(point) - collider.center;

            float xExtend = (collider.size.x * 0.5f);
            float yExtend = (collider.size.y * 0.5f);
            float zExtend = (collider.size.z * 0.5f);
            return (point.x < xExtend && point.x > -xExtend && point.y < yExtend && point.y > -yExtend && point.z < zExtend && point.z > -zExtend);
        }


        /// <summary>
        /// Getting average scale of box's dimensions
        /// </summary>
        protected static float GetBoxAverageScale(BoxCollider box)
        {
            Vector3 scales = box.transform.lossyScale;
            scales = Vector3.Scale(scales, box.size);
            return (scales.x + scales.y + scales.z) / 3f;
        }

        protected static Vector3 GetBoxCenter(BoxCollider box)
        {
            return box.transform.position + box.transform.TransformVector(box.center);
        }

        protected static Vector3 GetTargetPlaneNormal(BoxCollider boxCollider, Vector3 point)
        {
            Vector3 right = boxCollider.transform.TransformVector((Vector3.right / 2f) * boxCollider.size.x);
            Vector3 up = boxCollider.transform.TransformVector((Vector3.up / 2f) * boxCollider.size.y);
            Vector3 forward = boxCollider.transform.TransformVector((Vector3.forward / 2f) * boxCollider.size.z);

            Vector3 scales = Vector3.Scale(boxCollider.size, boxCollider.transform.lossyScale);
            scales.Normalize();

            return GetTargetPlaneNormal(boxCollider, point, right, up, forward, scales);
        }

        /// <summary>
        /// Getting nearest plane normal fitting to given point position
        /// </summary>
        protected static Vector3 GetTargetPlaneNormal(BoxCollider boxCollider, Vector3 point, Vector3 right, Vector3 up, Vector3 forward, Vector3 scales)
        {
            Vector3 rayDirection = (GetBoxCenter(boxCollider) - point).normalized;

            // Finding proper box's plane
            Vector3 dots;
            dots.x = Vector3.Dot(rayDirection, right.normalized);
            dots.y = Vector3.Dot(rayDirection, up.normalized);
            dots.z = Vector3.Dot(rayDirection, forward.normalized);
            dots.x = dots.x * scales.y * scales.z;
            dots.y = dots.y * scales.x * scales.z;
            dots.z = dots.z * scales.y * scales.x;
            dots.Normalize();

            Vector3 dotsAbs = dots;
            if (dots.x < 0) dotsAbs.x = -dots.x;
            if (dots.y < 0) dotsAbs.y = -dots.y;
            if (dots.z < 0) dotsAbs.z = -dots.z;

            Vector3 planeNormal = forward;

            if (dotsAbs.x > dotsAbs.y)
            {
                if (dotsAbs.x > dotsAbs.z) planeNormal = right * Mathf.Sign(dots.x); else planeNormal = forward * Mathf.Sign(dots.z);
            }
            else
            {
                if (dotsAbs.y > dotsAbs.z) planeNormal = up * Mathf.Sign(dots.y); else planeNormal = forward * Mathf.Sign(dots.z);
            }

            return planeNormal;
        }

        /// <summary>
        /// Calculating cheap ray on box plane to detect position from inside
        /// </summary>
        protected static Vector3 GetIntersectOnBoxFromInside(BoxCollider boxCollider, Vector3 from, Vector3 to, Vector3 planeNormal)
        {
            Vector3 rayDirection = (to - from);

            // Creating box's plane and casting cheap ray on it to detect intersection position
            Plane plane = new Plane(-planeNormal, GetBoxCenter(boxCollider) + planeNormal);
            Vector3 intersectionPoint = to;

            float enter = 0f;
            Ray ray = new Ray(from, rayDirection);
            if (plane.Raycast(ray, out enter)) intersectionPoint = ray.GetPoint(enter);

            return intersectionPoint;
        }



        #endregion

    }
}
