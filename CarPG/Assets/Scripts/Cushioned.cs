using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cushioned : MonoBehaviour
{
    public float impulseDivider = 4;
    public bool takeDamage = true;

    public float GetImpulseMultiplier()
    {
        if (!takeDamage)
        {
            return 0;
        }
        else
        {
            return 1 / impulseDivider;
        }
    }
}
