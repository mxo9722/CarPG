using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    public Animator anim;
    private Text damageText;


    // Start is called before the first frame update
    void OnEnable()
    {
        AnimatorClipInfo[] clipInfo = anim.GetCurrentAnimatorClipInfo(0);
        Destroy(gameObject, clipInfo[0].clip.length);

        damageText = anim.GetComponent<Text>();
    }

    public void SetText(string text)
    {
        damageText.text = text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
