//This is free to use and no attribution is required
//No warranty is implied or given
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]

public class LaserBeam : MonoBehaviour
{

    public float laserWidth = 1.0f;
    public float noise = 1.0f;
    public float maxLength = 100.0f;
    public Color color = Color.red;

    public float damage = 5;


    LineRenderer lineRenderer;
    int length;
    Vector3[] position;
    //Cache any transforms here
    Transform myTransform;
    Transform endEffectTransform;
    //The particle system, in this case sparks which will be created by the Laser
    public ParticleSystem endEffect;
    Vector3 offset;

    private SphereCollider col;


    // Use this for initialization
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = (laserWidth);
        lineRenderer.endWidth = laserWidth;
        myTransform = transform;
        offset = new Vector3(0, 0, 0);
        endEffect = GetComponentInChildren<ParticleSystem>();

        col = GetComponent<SphereCollider>();

        if (endEffect)
            endEffectTransform = endEffect.transform;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit raycastHit;
        Physics.Raycast(transform.position + transform.forward * 2, transform.forward, out raycastHit);
        //endEffectTransform = raycastHit.transform;
        RenderLaser();
    }

    void RenderLaser()
    {

        //Shoot our laserbeam forwards!
        UpdateLength();

        lineRenderer.startColor = (color);
        lineRenderer.endColor = (color);
        //Move through the Array
        for (int i = 0; i < length; i++)
        {
            //Set the position here to the current location and project it in the forward direction of the object it is attached to
            offset.x = myTransform.position.x + i * myTransform.forward.x + Random.Range(-noise, noise);
            offset.y = myTransform.position.y + i * myTransform.forward.y + Random.Range(-noise, noise);
            offset.z = i * myTransform.forward.z + Random.Range(-noise, noise) + myTransform.position.z;
            position[i] = offset;
            position[0] = myTransform.position;

            lineRenderer.SetPosition(i, position[i]);

        }
    }

    void UpdateLength()
    {
        //Raycast from the location of the cube forwards
        RaycastHit[] hit;

        hit=Physics.RaycastAll(transform.position+transform.forward, transform.forward, maxLength);

        int cIndex=0;

        for (int i = 0; i<hit.Length; i++)
        {
            if (hit[i].distance < hit[cIndex].distance && !hit[i].collider.isTrigger)
            {
                cIndex = i;
            }
        }

        

        //Check to make sure we aren't hitting triggers but colliders

        length = (int)Mathf.Round(hit[cIndex].distance);
        position = new Vector3[length];
        //Move our End Effect particle system to the hit point and start playing it
        if (endEffect)
        {
            endEffectTransform.position = hit[cIndex].point;
        }
        lineRenderer.positionCount = (length);

        var dam = hit[cIndex].collider.gameObject.GetComponent<Damagable>();

        if (dam != null)
        {
            if (dam.gameObject.name == "Pillar")
                dam.ApplyDamage(Time.deltaTime * damage * 20);
            else
                dam.ApplyDamage(Time.deltaTime * damage+dam.damageThreshhold);
        }

        var rb = hit[cIndex].collider.gameObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForceAtPosition(transform.forward*Time.deltaTime*10,hit[cIndex].point,ForceMode.VelocityChange);
        }
    }

    private void OnEnable()
    {
        endEffect.Play();
    }

    private void OnDisable()
    {
        endEffect.Stop();
    }
}