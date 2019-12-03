using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractureOnDeath : MonoBehaviour
{
    void Die()
    {
        GetComponent<DinoFracture.FractureGeometry>().Fracture();
    }
}
