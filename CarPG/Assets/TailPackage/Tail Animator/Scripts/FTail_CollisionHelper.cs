using UnityEngine;

namespace FIMSpace.FTail
{
    /// <summary>
    /// FM: Simple class sending collision events to main script
    /// </summary>
    public class FTail_CollisionHelper : MonoBehaviour
    {
        public FTail_AnimatorBase ParentTail;
        public Collider TailCollider;
        public int Index;

        internal Rigidbody RigBody { get; private set; }

        private Transform previousCollision;

        internal FTail_CollisionHelper Init(bool addRigidbody = true, float mass = 1f)
        {
            if (addRigidbody)
            {
                Rigidbody rig = GetComponent<Rigidbody>();
                if (!rig) rig = gameObject.AddComponent<Rigidbody>();
                rig.interpolation = RigidbodyInterpolation.Interpolate;
                rig.useGravity = false;
                rig.isKinematic = false;
                rig.constraints = RigidbodyConstraints.FreezeAll;
                rig.mass = mass;
                RigBody = rig;
            }
            else
            {
                RigBody = GetComponent<Rigidbody>();
                if (RigBody) RigBody.mass = mass;
            }

            return this;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (ParentTail == null)
            {
                GameObject.Destroy(this);
                return;
            }

            FTail_CollisionHelper helper = collision.transform.GetComponent<FTail_CollisionHelper>();
            if ( helper )
            {
                if (ParentTail.CollideWithOtherTails == false) return;
                if (helper.ParentTail == ParentTail) return;
            }

            if (ParentTail.TailTransforms.Contains(collision.transform)) return;

            //if (previousCollision != null) return;

            if (ParentTail.IgnoredColliders.Contains(collision.collider)) return;

            ParentTail.CollisionDetection(Index, collision);
            previousCollision = collision.transform;
        }

        private void OnCollisionStay(Collision collision)
        {
            OnCollisionEnter(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            if ( collision.transform == previousCollision )
            {
                ParentTail.ExitCollision(Index);
                previousCollision = null;
            }
        }
    }
}