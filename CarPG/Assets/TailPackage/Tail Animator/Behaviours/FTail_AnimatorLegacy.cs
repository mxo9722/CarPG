using FIMSpace.Basics;
using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.FTail
{
    /// <summary>
    /// FM: Tail animator script with option for disconnecting whole tail chain for more free motion
    /// It may be helpful when using fixed update
    /// It also have blending feature, works very similar to legacy version of Tail Animator previous version below 1.2.0
    /// </summary>
    [AddComponentMenu("FImpossible Creations/Tail Animator/Utilities/FTail Animator Legacy")]
    public class FTail_AnimatorLegacy : FTail_Animator
    {
        /// <summary> We will re-assign bones transform for this renderer </summary>
        public SkinnedMeshRenderer BonesOwner;

        [Tooltip("We can blend bones to move like animator movement or just tail animator motion")]
        [Range(0f,1f)]
        public float BlendToOriginal = 0f;

        /// <summary> Duplication of tail bones on which will be applied weights from skinned mesh </summary>
        protected List<Transform> newSkinnedBones;
        protected List<Transform> previousSkinnedBones;

        protected override void Reset()
        {
            DisconnectTransforms = true;
        }

        protected override void Init()
        {
            if (initialized) return;

            AutoGetTailTransforms();

            base.Init();

            for (int i = 0; i < proceduralPoints.Count; i++)
            {
                proceduralPoints[i].SetPosition(previousSkinnedBones[i].position);
                proceduralPoints[i].SetRotation(previousSkinnedBones[i].rotation);
            }

            // Bones disconnecting have some troubles when object is rotated, I will try fix it in next updates
            // for this time this operation is solution 
            rootTransform = previousSkinnedBones[0].parent;
            if (rootTransform == null) rootTransform = previousSkinnedBones[0];
        }

        /// <summary>
        /// Original bones will be animated without changes but they will lost weights
        /// Weights will have new transfroms disconnected from hierarchy to provide more free-motion movement
        /// </summary>
        protected override void ConfigureBonesTransforms()
        {
            if (!DisconnectTransforms) return;

            // If we disconnecting transforms we do a bunch of things...

            if (BonesOwner == null)
            {
                TryFindBonesOwner();
                if ( !BonesOwner ) Debug.LogError("[Tail Animator] You didn't assigned BonesOwner! (skinnedMeshRednerer)");
            }

            List<int> bonesIndextesToReplace = new List<int>();

            // Finding bones in skinned renderer's bones list to replace
            Transform[] bonesList = BonesOwner.bones;
            for (int i = 0; i < TailTransforms.Count; i++)
            {
                for (int j = 0; j < bonesList.Length; j++)
                    if (bonesList[j] == TailTransforms[i])
                        bonesIndextesToReplace.Add(j);
            }

            // Duplicating and replacing bones so we animate the same bones but they're not skinned
            // And skinned ones we will have in separated, mimic transforms
            previousSkinnedBones = new List<Transform>();
            newSkinnedBones = new List<Transform>();
            for (int i = 0; i < bonesIndextesToReplace.Count; i++)
            {
                Transform originalBone = bonesList[bonesIndextesToReplace[i]];
                previousSkinnedBones.Add(originalBone);

                Transform newSkinBone = new GameObject(originalBone.name).transform;
                newSkinBone.SetParent(GetDisconnectedContainer());
                newSkinBone.transform.position = originalBone.transform.position;
                newSkinBone.transform.rotation = originalBone.transform.rotation;

                newSkinnedBones.Add(newSkinBone);
                bonesList[bonesIndextesToReplace[i]] = newSkinBone;
            }

            TailTransforms = newSkinnedBones;

            // Applying new bone weights transforms
            BonesOwner.bones = bonesList;
        }


        /// <summary>
        /// If you want to use it will Unity's animator, set update clock to LateUpdate()
        /// </summary>
        protected override void MotionCalculations()
        {
            #region Animator blending stuff

            if (BlendToOriginal > 0)
            {
                if (BlendToOriginal >= 1f)
                {
                    for (int i = 0; i < TailTransforms.Count; i++)
                    {
                        TailTransforms[i].position = previousSkinnedBones[i].position;
                        TailTransforms[i].rotation = previousSkinnedBones[i].rotation;
                    }

                    // We aligning tail world positions to hierarchy copy of tail and ending method here
                    return;
                }
            }

            #endregion

            base.MotionCalculations();

            proceduralPoints[0].SetPosition(previousSkinnedBones[0].position);

            if (BlendToOriginal > 0)
            {
                for (int i = 0; i < TailTransforms.Count; i++)
                {
                    TailTransforms[i].position = Vector3.Lerp(proceduralPoints[i].Position, previousSkinnedBones[i].position, BlendToOriginal);
                    TailTransforms[i].rotation = Quaternion.Lerp(proceduralPoints[i].Rotation, previousSkinnedBones[i].rotation, BlendToOriginal);
                }
            }
            else
            {
                for (int i = 0; i < TailTransforms.Count; i++)
                {
                    TailTransforms[i].position = proceduralPoints[i].Position;
                    TailTransforms[i].rotation = proceduralPoints[i].Rotation;
                }
            }

            // We set previous procedural bones to procedural positions and rotations, because if they have children, all other bones would stay in original place, making whole model look buggy
            // Every late update previousSkinnedBones are resetting positions
            for (int i = 0; i < previousSkinnedBones.Count; i++)
            {
                previousSkinnedBones[i].position = TailTransforms[i].position;
                previousSkinnedBones[i].rotation = TailTransforms[i].rotation;
            }
        }

        public override void CalculateOffsets()
        {
            WavingCalculations();
            MotionCalculations();
        }

        /// <summary>
        /// Looking for bones owner to create mimics of bones
        /// </summary>
        public void TryFindBonesOwner()
        {
            BonesOwner = GetComponentInChildren<SkinnedMeshRenderer>();

            if (!BonesOwner) if (transform.parent != null)
                {
                    BonesOwner = transform.parent.GetComponentInChildren<SkinnedMeshRenderer>();

                    if (!BonesOwner) if (transform.parent.parent != null) BonesOwner = transform.parent.parent.GetComponentInChildren<SkinnedMeshRenderer>();

                    if (BonesOwner == null)
                    {
                        SkinnedMeshRenderer foundSkinnedMesh = null;
                        Transform p = transform;
                        for (int i = 0; i < 100; i++)
                        {
                            if (p != null)
                            {
                                foundSkinnedMesh = p.GetComponent<SkinnedMeshRenderer>();

                                if (!foundSkinnedMesh)
                                {
                                    for (int j = 0; j < p.childCount; j++)
                                    {
                                        foundSkinnedMesh = p.GetChild(j).GetComponent<SkinnedMeshRenderer>();
                                        if (foundSkinnedMesh) break;
                                    }
                                }

                                if (foundSkinnedMesh) break;
                                p = p.transform.parent;
                            }
                        }

                        BonesOwner = foundSkinnedMesh;
                    }
                }

            if (BonesOwner) Debug.LogWarning("[Tail Animator] I found skinned mesh renderer in object '" + BonesOwner.transform.name + "' is it correct?");

            if (!BonesOwner) Debug.LogError("[Tail Animator] Skinned mesh renderer couldn't be found! ");
        }
    }
}
