using FIMSpace.Basics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.FTail
{
    /// <summary>
    /// FM: Derived class with feature to add sinusoidal waving to tail movement
    /// </summary>
    [AddComponentMenu("FImpossible Creations/Tail Animator/FTail Animator")]
    public class FTail_Animator : FTail_AnimatorBase, UnityEngine.EventSystems.IDropHandler, IFHierarchyIcon
    {
        public string EditorIconPath { get { return "Tail Animator/FTailAnimator Icon"; } }
        public void OnDrop(UnityEngine.EventSystems.PointerEventData data) { }

        public bool UseWaving = true;

        [Tooltip("Adding some variation to waving animation")]
        public bool CosinusAdd = false;

        public float WavingSpeed = 3f;
        public float WavingRange = 0.8f;
        public Vector3 WavingAxis = new Vector3(1f, 0.0f, 0f);

        public Vector3 TailRotationOffset = Vector3.zero;

        public enum FEWavingType { Simple, Advanced }

        [Tooltip("Type of waving animation algorithm, it can be simple trigonometric wave or animation based on noises (advanced)")]
        public FEWavingType WavingType = FEWavingType.Simple;
        public float AlternateWave = 1f;

        [Tooltip("Disconnecting whole bones chain inside your model (excluding first bone) - this helps motion be more free and independent from some factors related to hierarchy, it is not recommended to do - only when needed")]
        public bool DisconnectTransforms = false;

        /// <summary> Container for all other disconnected bones from other objects, to keep everything nice and clear</summary>
        protected static Transform disconnectedContainer;

        /// <summary> Inside this transforms will be bones or bone mimics </summary>
        protected Transform localDisconnectedContainer;

        /// <summary> Trigonometric function time variable </summary>
        protected float waveTime;
        protected float cosTime;

        private int RefreshCounter = 0;

        protected override void Init()
        {
            base.Init();

            waveTime = Random.Range(-Mathf.PI, Mathf.PI) * 100f;
            cosTime = Random.Range(-Mathf.PI, Mathf.PI);
        }

        protected virtual void WavingCalculations()
        {
            if (!RootToParent)
            {
                if (UseWaving)
                {
                    Vector3 rot = TailRotationOffset;

                    if (!AnimateCorrections || (AnimateCorrections && !AnimateRoot))
                    {
                        rot += proceduralPoints[0].InitialLocalRotation.eulerAngles;
                    }
                    else
                    {
                        rot += proceduralPoints[0].Transform.localRotation.eulerAngles;
                    }

                    // Defining base variables
                    waveTime += Time.deltaTime * (2 * WavingSpeed);

                    // Simple trigonometrical waving
                    if (WavingType == FEWavingType.Simple)
                    {
                        float sinVal = Mathf.Sin(waveTime) * (30f * WavingRange);

                        if (CosinusAdd)
                        {
                            cosTime += Time.deltaTime * (2.535f * WavingSpeed);
                            sinVal += Mathf.Cos(cosTime) * (27f * WavingRange);
                        }

                        rot += sinVal * WavingAxis;
                    }
                    else
                    // Advanced waving based on perlin noise
                    {
                        float perTime = waveTime * 0.23f;

                        float altX = AlternateWave * -5f;
                        float altY = AlternateWave * 100f;
                        float altZ = AlternateWave * 20f;

                        float x = Mathf.PerlinNoise(perTime, altX) * 2.0f - 1.0f;
                        float y = Mathf.PerlinNoise(altY + perTime, perTime + altY) * 2.0f - 1.0f;
                        float z = Mathf.PerlinNoise(altZ, perTime) * 2.0f - 1.0f;

                        rot += Vector3.Scale(WavingAxis * WavingRange * 35f, new Vector3(x, y, z));
                    }

                    //if (AnimateCorrections)
                    //{
                    //    // Applying rotation to root tail segment
                    //    if (rootTransform)
                    //        proceduralPoints[0].SetRotation(rootTransform.rotation * Quaternion.Euler(rot));
                    //    else
                    //        proceduralPoints[0].SetRotation(TailTransforms[0].transform.rotation * Quaternion.Euler(rot));
                    //}
                    //else
                    {
                        if (rootTransform)
                            proceduralPoints[0].SetRotation(rootTransform.rotation * Quaternion.Euler(rot));
                        else
                            proceduralPoints[0].SetRotation(TailTransforms[0].transform.rotation * Quaternion.Euler(rot));
                    }
                }
                else
                {
                    if (!AnimateCorrections || (AnimateCorrections && !AnimateRoot))
                    {
                        if (rootTransform)
                            proceduralPoints[0].SetRotation(rootTransform.rotation * proceduralPoints[0].InitialLocalRotation);
                        else
                            proceduralPoints[0].SetRotation(TailTransforms[0].transform.rotation * proceduralPoints[0].InitialLocalRotation);

                        //rot += proceduralPoints[0].InitialLocalRotation.eulerAngles;
                    }
                    else
                    {
                        if (rootTransform)
                            proceduralPoints[0].SetRotation(rootTransform.rotation * proceduralPoints[0].Transform.localRotation);
                        else
                            proceduralPoints[0].SetRotation(TailTransforms[0].transform.rotation * proceduralPoints[0].Transform.localRotation);

                        //rot += proceduralPoints[0].Transform.localRotation.eulerAngles;
                    }
                }
            }
        }


        /// <summary>
        /// Adding sinus wave rotation for first bone before other calculations
        /// </summary>
        public override void CalculateOffsets()
        {
            // It must be done here, not in courutine, before motion calculations
            if (RefreshHelpers)
            {
                RefreshCounter--;
                if (RefreshCounter < -5)
                {
                    RefreshHelpers = false;
                    CoputeHelperVariables();
                    RefreshCounter = 0;
                }
            }

            WavingCalculations();

            base.CalculateOffsets();

            SetTailTransformsFromPoints();


        }

        /// <summary>
        /// Supporting disconnecting feature
        /// </summary>
        protected override void ConfigureBonesTransforms()
        {
            base.ConfigureBonesTransforms();

            if (DisconnectTransforms)
                for (int i = 1; i < TailTransforms.Count; i++)
                {
                    TailTransforms[i].SetParent(GetDisconnectedContainer(), true);
                }
        }

        /// <summary>
        /// Returns local container transform for disconnected bones
        /// </summary>
        protected Transform GetDisconnectedContainer()
        {
            if (disconnectedContainer == null)
                disconnectedContainer = new GameObject("[Tail Animator Container]").transform;

            if (localDisconnectedContainer == null)
            {
                localDisconnectedContainer = new GameObject("Tail Container [" + name + "]").transform;
                localDisconnectedContainer.SetParent(disconnectedContainer, true);
            }

            return localDisconnectedContainer;
        }


        internal virtual void Update()
        {
            if (UpdateClock != EFUpdateClock.Update) return;
            if (!initialized) return;
            CalculateOffsets();
        }

        internal virtual void LateUpdate()
        {
            if (UpdateClock != EFUpdateClock.LateUpdate) return;
            if (!initialized) return;
            if (Time.deltaTime <= 0) return;
            CalculateOffsets();
        }

        internal virtual void FixedUpdate()
        {
            if (UpdateClock != EFUpdateClock.FixedUpdate) return;
            if (!initialized) return;
            CalculateOffsets();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (localDisconnectedContainer) Destroy(localDisconnectedContainer.gameObject);
        }


        // V1.2.0
#if UNITY_EDITOR


        /// <summary>
        /// Updating tail list chain for gizmos without need to define all tail transforms (auto collecting bones if any isn't added to tailBones list)
        /// </summary>
        public override void OnValidate()
        {
            base.OnValidate();
            GetEditorGizmoTailList();
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            if (!drawGizmos) return;

            if (editorGizmoTailList != null)
            {
                if (TailTransforms == null || TailTransforms.Count == 0)
                {
                    // Do not draw segment icon (would overlap icon gizmo icon)
                }
                else if (editorGizmoTailList.Count > 0) Gizmos.DrawIcon(editorGizmoTailList[0].position, "FIMSpace/FTail/SPR_TailAnimatorGizmoSegment.png");

                for (int i = 1; i < editorGizmoTailList.Count; i++)
                {
                    if (editorGizmoTailList[i] == null) continue;
                    Gizmos.DrawIcon(editorGizmoTailList[i].position, "FIMSpace/FTail/SPR_TailAnimatorGizmoSegment.png");
                }
            }
        }

#endif
    }
}
