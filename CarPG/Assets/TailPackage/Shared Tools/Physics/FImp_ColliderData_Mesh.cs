using UnityEngine;

namespace FIMSpace
{
    public class FImp_ColliderData_Mesh : FImp_ColliderData_Base
    {
        public MeshCollider Mesh { get; private set; }

        public FImp_ColliderData_Mesh(MeshCollider collider)
        {
            Collider = collider;
            Mesh = collider;
            ColliderType = EFColliderType.Mesh;
        }

        public override bool PushIfInside(ref Vector3 segmentPosition, float segmentRadius, Vector3 segmentOffset)
        {
            Vector3 closest;
            float plus = 0f;

            Vector3 positionOffsetted = segmentPosition + segmentOffset;

            closest = Mesh.ClosestPointOnBounds(positionOffsetted);
            plus = (closest - Mesh.transform.position).magnitude;

            bool inside = false;
            float insideMul = 1f;

            if (closest == positionOffsetted)
            {
                inside = true;
                insideMul = 7f;
                closest = Mesh.transform.position;
            }

            Vector3 targeting = closest - positionOffsetted;
            Vector3 rayDirection = targeting.normalized;
            Vector3 rayOrigin = positionOffsetted - rayDirection * (segmentRadius * 2f + Mesh.bounds.extents.magnitude);

            float rayDistance = targeting.magnitude + segmentRadius * 2f + plus + Mesh.bounds.extents.magnitude;
            //Debug.DrawLine(point, closest, Color.white);
            //Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.magenta);
            //Debug.DrawRay(rayOrigin, -rayDirection, Color.red);

            if ((positionOffsetted - closest).magnitude < segmentRadius * insideMul)
            {
                Ray ray = new Ray(rayOrigin, rayDirection);

                RaycastHit hit;
                if (Mesh.Raycast(ray, out hit, rayDistance))
                {
                    float hitToPointDist = (positionOffsetted - hit.point).magnitude;

                    if (hitToPointDist < segmentRadius * insideMul)
                    {

                        Vector3 toNormal = hit.point - positionOffsetted;
                        Vector3 pushNormal;
                        //Debug.Log("c = " + closest + " p " + point + " is == " + (closest == point) + " magn " + (point - hit.point).magnitude + " n " + toNormal + " pr " + pointRadius );

                        if (inside) pushNormal = toNormal + toNormal.normalized * segmentRadius; else pushNormal = toNormal - toNormal.normalized * segmentRadius;

                        //if (!inside) if (((point + pushNormal) - Mesh.transform.position).magnitude < hitToPointDist ) pushNormal = toNormal + toNormal.normalized * pointRadius;
                        //if (!inside) if (((point + toNormal) - hit.point).magnitude > hitToPointDist ) pushNormal = toNormal + toNormal.normalized * pointRadius;

                        float dot = Vector3.Dot((hit.point - positionOffsetted).normalized, rayDirection);
                        if (inside && dot > 0f) pushNormal = toNormal - toNormal.normalized * segmentRadius;
                        //Debug.Log(Vector3.Dot((hit.point - point).normalized, rayDirection) + " in " + inside);

                        segmentPosition = segmentPosition + pushNormal;

                        return true;
                    }
                }
            }

            return false;
        }



        public static void PushOutFromMeshCollider(MeshCollider mesh, Collision collision, float segmentColliderRadius, ref Vector3 pos)
        {
            Vector3 collisionPoint = collision.contacts[0].point;
            Vector3 pushNormal = collision.contacts[0].normal;

            RaycastHit info;
            // Doing cheap mesh raycast from outside to hit surface
            if (mesh.Raycast(new Ray(pos + pushNormal * segmentColliderRadius * 2f, -pushNormal), out info, segmentColliderRadius * 5))
            {
                pushNormal = info.point - pos;
                float pushMagn = pushNormal.sqrMagnitude;
                if (pushMagn > 0 && pushMagn < segmentColliderRadius * segmentColliderRadius) pos = info.point - pushNormal * (segmentColliderRadius / Mathf.Sqrt(pushMagn)) * 0.9f;
            }
            else
            {
                pushNormal = collisionPoint - pos;
                float pushMagn = pushNormal.sqrMagnitude;
                if (pushMagn > 0 && pushMagn < segmentColliderRadius * segmentColliderRadius) pos = collisionPoint - pushNormal * (segmentColliderRadius / Mathf.Sqrt(pushMagn)) * 0.9f;
            }
        }



        public static void PushOutFromMesh(MeshCollider mesh, Collision collision, float pointRadius, ref Vector3 point)
        {
            Vector3 closest;
            float plus = 0f;

            closest = mesh.ClosestPointOnBounds(point);
            plus = (closest - mesh.transform.position).magnitude;

            bool inside = false;
            float insideMul = 1f;

            if (closest == point)
            {
                inside = true;
                insideMul = 7f;
                closest = mesh.transform.position;
            }

            Vector3 targeting = closest - point;
            Vector3 rayDirection = targeting.normalized;
            Vector3 rayOrigin = point - rayDirection * (pointRadius * 2f + mesh.bounds.extents.magnitude);

            float rayDistance = targeting.magnitude + pointRadius * 2f + plus + mesh.bounds.extents.magnitude;

            if ((point - closest).magnitude < pointRadius * insideMul)
            {
                Vector3 collisionPoint;

                if (!inside)
                    collisionPoint = collision.contacts[0].point;
                else
                {
                    Ray ray = new Ray(rayOrigin, rayDirection);
                    RaycastHit hit;
                    if (mesh.Raycast(ray, out hit, rayDistance)) collisionPoint = hit.point; else collisionPoint = collision.contacts[0].point;
                }

                float hitToPointDist = (point - collisionPoint).magnitude;

                if (hitToPointDist < pointRadius * insideMul)
                {
                    Vector3 toNormal = collisionPoint - point;
                    Vector3 pushNormal;

                    if (inside) pushNormal = toNormal + toNormal.normalized * pointRadius; else pushNormal = toNormal - toNormal.normalized * pointRadius;

                    float dot = Vector3.Dot((collisionPoint - point).normalized, rayDirection);
                    if (inside && dot > 0f) pushNormal = toNormal - toNormal.normalized * pointRadius;

                    point = point + pushNormal;
                }
            }
        }

    }
}
