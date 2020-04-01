using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Song
{
    DrivingSong = 0,
    FightSong = 1,
    BossSong = 2
}

public class MusicManager : MonoBehaviour
{
    public AudioSource BGM;
    public GameObject enemyHolder;
    public List<EnemyBehaviorScript> enemyList;

    public AudioClip[] songList;

    public bool lockCurrentSong;

    private Song currentSong = 0;

    private float musicDelay = 3;

    // Start is called before the first frame update
    void Start()
    {
        EnemyBehaviorScript[] enemyArr = enemyHolder.GetComponentsInChildren<EnemyBehaviorScript>();
        enemyList = new List<EnemyBehaviorScript>(enemyArr);
        //songList = new AudioClip[3];
    }

    // Update is called once per frame
    void Update()
    {
        if (musicDelay > 0)
        {
            musicDelay -= Time.deltaTime;
        }
        else if (!lockCurrentSong)
        {
            Song newSong = Song.DrivingSong;

            for (int i = 0; i < enemyList.Count; i++)
            {
                if (enemyList[i].currentState != EnemyState.Idle)
                {
                    if (enemyList[i].currentState == EnemyState.Dead)
                    {
                        enemyList.RemoveAt(i);
                        i--;
                    }
                    else if (enemyList[i].currentState == EnemyState.StandingUp)
                    {
                        
                    }
                    else
                    {
                        Debug.Log(enemyList[i].currentState.ToString());
                        newSong = Song.FightSong;
                        break;
                    }
                }
            }

            if (currentSong != newSong)
            {
                BGM.Stop();
                BGM.clip = songList[(int)newSong];
                BGM.Play();

                currentSong = newSong;
                musicDelay = 3;
            }
        }
    }
}
