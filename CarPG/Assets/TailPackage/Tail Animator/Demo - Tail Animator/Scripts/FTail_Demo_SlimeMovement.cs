using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.FTail
{
    public class FTail_Demo_SlimeMovement : FTail_Demo_GroundMovement
    {
        private List< FTail_Animator> tails;

        protected override void Start()
        {
            base.Start();
            tails = FTransformMethods.FindComponentsInAllChildren<FTail_Animator>(transform);
        }

        protected override void Update()
        {
            base.Update();

            float waveSpeed = tails[0].WavingSpeed;
            float waveRange = tails[0].WavingRange;

            waveSpeed = Mathf.Lerp(waveSpeed, 3f + ActiveSpeed / 2f, Time.deltaTime * 2f);
            waveRange = Mathf.Lerp(waveRange, 0.1f + ActiveSpeed / 8f, Time.deltaTime * 1f);

            for (int i = 0; i < tails.Count; i++)
            {
                tails[i].WavingSpeed = waveSpeed;
                tails[i].WavingRange = waveRange;
            }
        }
    }

}