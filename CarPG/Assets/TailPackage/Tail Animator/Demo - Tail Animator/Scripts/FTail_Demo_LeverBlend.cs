using UnityEngine;
using FIMSpace.Basics;

namespace FIMSpace.FTail
{
    public class FTail_Demo_LeverBlend : MonoBehaviour
    {
        public FBasic_PullableLever lever;
        public FTail_AnimatorBlending tailToBlend;

        private Transform TailSegmentsContainer;

        public bool blendChain = false;

        void Update()
        {
            if (blendChain)
                tailToBlend.BlendChainValue = lever.LeverValueY;
            else
                tailToBlend.BlendToOriginal = lever.LeverValueY;

        }
    }
}
