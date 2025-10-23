using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
public class FootInputManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;

    public Sprite player1Sprite;
    public Sprite player2Sprite;

    public RuntimeAnimatorController player1Animator;
    public RuntimeAnimatorController player2Animator;

    private bool wasdJoined = false;  
    private bool gamepadJoined = false; 

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current == null) return;
        if (!wasdJoined && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            var player = PlayerInput.Instantiate(playerPrefab, controlScheme: "Keyboard", pairWithDevice: Keyboard.current);
            player.GetComponent<SpriteRenderer>().sprite = player1Sprite;

            var anim = player.GetComponent<Animator>();
            anim.runtimeAnimatorController = player1Animator;


            if (spawnPoints.Length > 0)
            {
                player.transform.position = spawnPoints[0].position;
                

            }

            wasdJoined = true;
        }

        foreach (var gamepad in Gamepad.all)
        if (gamepad.startButton.wasPressedThisFrame && !gamepadJoined)
            {
                var player = PlayerInput.Instantiate(playerPrefab, controlScheme: "Controller", pairWithDevice: gamepad);
                player.GetComponent<SpriteRenderer>().sprite = player2Sprite;

                var anim = player.GetComponent<Animator>();
                anim.runtimeAnimatorController = player2Animator;

            

                
                            if (spawnPoints.Length > 0)
            {
                    player.transform.position = spawnPoints[1].position;
                    player.transform.localScale = new Vector3(-1, 1, 1);
            }
                
                gamepadJoined = true;
            }

    }
}
