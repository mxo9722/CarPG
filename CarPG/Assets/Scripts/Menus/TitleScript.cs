using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitleScript : MonoBehaviour
{
    public TextMeshProUGUI wText;
    public Light light1;
    public Light light2;
    private bool buttonPushed;
    private float timer;
    private bool carFlung;
    public Animator animator;
    public AudioSource song;
    public Rigidbody carRB;
    // Start is called before the first frame update
    void Start()
    {
        wText.color = new Color(255, 255, 255, 0);
        song.enabled = false;
        buttonPushed = false;
        carFlung = false;
        timer = 0.0f;
        light1.enabled = false;
        light2.enabled = false;
        animator.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer <= 2.5f)
            wText.color = new Color(255, 255, 255, timer / 2.5f);

        if (Input.GetKeyDown(KeyCode.W) && !buttonPushed)
        {
            buttonPushed = true;

            light1.enabled = true;
            light2.enabled = true;

            wText.enabled = false;
            timer = 0.0f;
        }
        if (buttonPushed && !carFlung && timer >= 2.0f)
        {
            carFlung = true;
            animator.enabled = true;
            song.enabled = true;
            carRB.AddForce(25000, 5000, 0);
            carRB.AddRelativeTorque(new Vector3(Random.Range(0, 100), Random.Range(0, 100), Random.Range(0, 100)));
        }
    }
}
