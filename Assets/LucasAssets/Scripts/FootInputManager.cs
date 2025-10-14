using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
public class FootInputManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private bool wasdJoined = false;  
    private bool gamepadJoined = false; 

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current == null) return;
        if (!wasdJoined && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            var player = PlayerInput.Instantiate(playerPrefab, controlScheme: "Keyboard", pairWithDevice: Keyboard.current);

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
                
                            if (spawnPoints.Length > 0)
            {
                player.transform.position = spawnPoints[1].position;
            }
                
                gamepadJoined = true;
            }

    }
}
