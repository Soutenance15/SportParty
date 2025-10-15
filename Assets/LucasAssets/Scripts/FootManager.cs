using UnityEngine;
using UnityEngine.InputSystem;

public class FootManager : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform spawnPoint1;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnBall();
    }

  
    
    public void SpawnBall()
    {
        GameObject laBalle = Instantiate(ballPrefab, spawnPoint1.position, Quaternion.identity);
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
