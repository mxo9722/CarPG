using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimation : MonoBehaviour
{
    public GameObject bossLoc;

    public GameObject bossFightPrefab;

    void AnimationEnd()
    {
        GameObject.Destroy(gameObject);
        GameObject.Instantiate(bossFightPrefab,bossLoc.transform.position,bossLoc.transform.rotation);
    }
}
