using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.FTail
{
    /// <summary>
    /// FM: Component used to apply tail animation to animated skeletons / objects with blending option
    /// </summary>
    [AddComponentMenu("FImpossible Creations/Tail Animator/FTail Animator Blending")]
    public class FTail_AnimatorBlending : FTail_Animator
    {
        [Header("[ Animator Support Parameters ]")]
        [Tooltip("Blend source animation and tail animator")]
        [Range(0f, 1f)]
        public float BlendToOriginal = 0f;
        internal float InitialBlendToOriginal;

        [Tooltip("Change it value when you want first bones to be not animated by tail animator, but by your keyframed animation")]
        [Range(0f, 1f)]
        public float BlendChainValue = 1f;

        [Tooltip("In most cases your animation could not have keyframes in position track, then you set tick here")]
        public bool PositionsNotAnimated = true;
        [Tooltip("In most cases your animation could have rotation keyframes, but sometimes some bones can not have, then you can tweak it using this variable")]
        public bool RotationsNotAnimated = false;
        public List<bool> SelectiveRotsNotAnimated;


        /// <summary> Preventing jump transition to original blend from partially blended tail </summary>
        private float smoothChainBlend = 0f;

        /// <summary> List for holding not animated positions feature </summary>
        private List<Vector3> staticPositions = new List<Vector3>();

        protected override void Init()
        {
            base.Init();

            if (SelectiveRotsNotAnimated == null || SelectiveRotsNotAnimated.Count == 0)
            {
                SelectiveRotsNotAnimated = new List<bool>();
                for (int i = 0; i < TailTransforms.Count; i++) SelectiveRotsNotAnimated.Add(true);
            }

            InitialBlendToOriginal = BlendToOriginal;
        }

        protected override void CoputeHelperVariables()
        {
            base.CoputeHelperVariables();

            staticPositions = new List<Vector3>();
            for (int i = 0; i < TailTransforms.Count; i++)
            {
                staticPositions.Add(TailTransforms[i].localPosition);
            }
        }

        protected override void Reset()
        {
            UpdateClock = Basics.EFUpdateClock.LateUpdate;
        }

        // When we want animate already animated object, we use just lateUpdate
        internal override void Update()
        {
            if (PositionsNotAnimated)
                for (int i = 0; i < staticPositions.Count; i++)
                {
                    TailTransforms[i].localPosition = staticPositions[i];
                }

            if (RotationsNotAnimated)
                if (proceduralPoints != null)
                    for (int i = 0; i < proceduralPoints.Count; i++)
                        if (SelectiveRotsNotAnimated[i]) proceduralPoints[i].Transform.localRotation = proceduralPoints[i].InitialLocalRotation;
        }

        internal override void FixedUpdate() { }

        /// <summary>
        /// Doing calculations for lateUpdate to be blendable with animator's positions for bones
        /// </summary>
        public override void CalculateOffsets()
        {
            if (Time.deltaTime <= 0) return;

            // When we don't use tail animator at all
            if (BlendToOriginal >= 1f)
            {
                // Smooth transition when we use partially blended tail chain
                if (BlendChainValue >= 1f) smoothChainBlend = 1f;

                if (smoothChainBlend < 1f)
                {
                    smoothChainBlend = Mathf.Lerp(smoothChainBlend, 1.05f, Time.deltaTime * 3f);

                    for (int i = 0; i < proceduralPoints.Count; i++)
                    {
                        proceduralPoints[i].SetPosition(Vector3.Lerp(proceduralPoints[i].Position, TailTransforms[i].position, smoothChainBlend));
                        proceduralPoints[i].SetRotation(Quaternion.Lerp(proceduralPoints[i].Rotation, TailTransforms[i].rotation, smoothChainBlend));

                        TailTransforms[i].position = proceduralPoints[i].Position;
                        TailTransforms[i].rotation = proceduralPoints[i].Rotation;
                    }
                }
                else
                    for (int i = 0; i < proceduralPoints.Count; i++)
                    {
                        proceduralPoints[i].SetPosition(TailTransforms[i].position);
                        proceduralPoints[i].SetRotation(TailTransforms[i].rotation);
                    }

                return;
            }

            smoothChainBlend = 0f;

            base.CalculateOffsets();

            // SetTailTransformsPositionsFromPoints() executed in other order and other way
            // Optimizing code when we want only procedural animation
            if (BlendToOriginal > 0 || BlendChainValue > 0f)
            {
                for (int i = 0; i < TailTransforms.Count; i++)
                {
                    if ((float)(i) / (float)TailTransforms.Count <= BlendChainValue)
                    {
                        float blendValue = 1f;
                        if (BlendChainValue < 1f) blendValue = Mathf.Clamp(BlendChainValue * (float)TailTransforms.Count - i, 0f, 1f);

                        proceduralPoints[i].SetPosition(Vector3.Lerp(proceduralPoints[i].Position, TailTransforms[i].position, BlendToOriginal * blendValue));
                        proceduralPoints[i].SetRotation(Quaternion.Lerp(proceduralPoints[i].Rotation, TailTransforms[i].rotation, BlendToOriginal * blendValue));
                    }

                    TailTransforms[i].position = proceduralPoints[i].Position;
                    TailTransforms[i].rotation = proceduralPoints[i].Rotation;
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
        }


        internal override void LateUpdate()
        {
            if (!initialized) return;
            CalculateOffsets();
        }

        protected override void SetTailTransformsFromPoints() { }
    }
}
