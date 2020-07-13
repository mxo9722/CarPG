using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public List<GameObject> spawnableEnemies = new List<GameObject>();
    private GameObject placedEnemy;
    
    // Start is called before the first frame update
    void Start()
    {
        //spawn the enemy and then destroy this object
        placedEnemy = Instantiate(spawnableEnemies[Random.Range(0, spawnableEnemies.Count)]);
        placedEnemy.GetComponent<Transform>().position = transform.position;
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
