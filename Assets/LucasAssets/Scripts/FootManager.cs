using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FootManager : MonoBehaviour
{
    public GameObject ballPrefab;
    

    public List<Transform> spawnpoints;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnBall();
    }

  
    
    public void SpawnBall()
    {
        int randomIndex = Random.Range(0, spawnpoints.Count);
        Transform randomSpawnPoint = spawnpoints[randomIndex];
        GameObject laBalle = Instantiate(ballPrefab, randomSpawnPoint.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
       if (GameObject.FindGameObjectWithTag("Ball") == null)
        {
            SpawnBall();
        }
    }
}
