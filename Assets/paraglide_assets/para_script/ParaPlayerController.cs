using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class ParaPlayerController : MonoBehaviour
{
    [Header("CONFIGURATION")]
    [Tooltip("Réglez manuellement : 1 pour le Joueur 1, 2 pour le Joueur 2.")]
    public int playerID = 1;

    [Header("MOUVEMENT")]
    public float moveSpeed = 8f;
    public Vector2 horizontalBounds = new Vector2(-8f, 8f);
    public Vector2 verticalBounds = new Vector2(-4f, 4f);

    [Header("EFFET VISUEL")]
    public Transform visualsTransform;
    public float tiltAngle = 15f;
    public float tiltSpeed = 5f;
    public Color player1Color = Color.white;
    public Color player2Color = Color.cyan;

    public int Score { get; private set; } = 0; // La propriété qui remplace "score"
    
    private Vector2 moveInput;
    private SpriteRenderer spriteToColor;

    void OnEnable()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();
        var user = playerInput.user;
        if (!user.valid) return;

        user.UnpairDevices();

        if (playerID == 1) {
            if (Keyboard.current != null) InputUser.PerformPairingWithDevice(Keyboard.current, user: user);
        } else if (playerID == 2) {
            if (Gamepad.current != null) InputUser.PerformPairingWithDevice(Gamepad.current, user: user);
        }
        
        if (visualsTransform != null) {
            spriteToColor = visualsTransform.GetComponent<SpriteRenderer>();
        }
        if (spriteToColor != null) {
            spriteToColor.color = (playerID == 1) ? player1Color : player2Color;
        }
        
        if (ParaScoreManager.Instance != null) {
            // LA CORRECTION EST ICI
            ParaScoreManager.Instance.UpdateScoreUI(playerID, Score); 
        }
    }

    void Update()
    {
        Vector3 movement = new Vector3(moveInput.x, moveInput.y, 0) * moveSpeed * Time.deltaTime;
        transform.Translate(movement);

        Vector3 currentPosition = transform.position;
        float clampedX = Mathf.Clamp(currentPosition.x, horizontalBounds.x, horizontalBounds.y);
        float clampedY = Mathf.Clamp(currentPosition.y, verticalBounds.x, verticalBounds.y);
        transform.position = new Vector3(clampedX, clampedY, 0);

        if (visualsTransform != null) {
            float targetAngle = -moveInput.x * tiltAngle;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            visualsTransform.localRotation = Quaternion.Lerp(visualsTransform.localRotation, targetRotation, Time.deltaTime * tiltSpeed);
        }
    }

    public void AddScore(int pointsToAdd) {
        Score += pointsToAdd;
        if (ParaScoreManager.Instance != null) {
            ParaScoreManager.Instance.UpdateScoreUI(playerID, Score);
        }
    }

    public void OnMove(InputAction.CallbackContext context) {
        moveInput = context.ReadValue<Vector2>();
    }
}