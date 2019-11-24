using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransparencyScript : MonoBehaviour
{
    private Camera camera;
    private GameObject car;

    private List<MeshRenderer> hiddenObjects = new List<MeshRenderer>();

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        car = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < hiddenObjects.Count; i++)
        {
            hiddenObjects[i].enabled = true;
            hiddenObjects[i].gameObject.layer = 0;
        }
        hiddenObjects.Clear();

        RaycastHit hit;
        Ray ray = camera.ViewportPointToRay(new Vector3(0.4F, 0.1F, 0));

        while (Physics.Raycast(ray, out hit) && hit.transform.gameObject.tag == "Wall")
        {
                hit.transform.gameObject.GetComponent<MeshRenderer>().enabled = false;
                hit.transform.gameObject.layer = 2;
                hiddenObjects.Add(hit.transform.gameObject.GetComponent<MeshRenderer>());
        }

        ray = camera.ViewportPointToRay(new Vector3(0.6F, 0.1F, 0));
        while (Physics.Raycast(ray, out hit) && hit.transform.gameObject.tag == "Wall")
        {
            hit.transform.gameObject.GetComponent<MeshRenderer>().enabled = false;
            hit.transform.gameObject.layer = 2;
            hiddenObjects.Add(hit.transform.gameObject.GetComponent<MeshRenderer>());
        }
    }
}
