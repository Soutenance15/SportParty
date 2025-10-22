using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class KartDriveSystem : MonoBehaviour
{
    // --- Mouvements ---
    private float acceleration = 12f;
    private float maxSpeed = 40f;
    private float turnSpeed = 90f;
    private float deceleration = 16f;
    private float currentSpeed = 0f;

    // --- Boost ---
    private bool isBoosting = false;
    private float boostMultiplier = 8f; // facteur de vitesse pendant le boost
    private float boostDuration = 1f; // durée du boost (secondes)

    // --- Composant ---
    private Rigidbody2D rb;

    public void InitRb(Rigidbody2D rigidbody)
    {
        rb = rigidbody;
        rb.gravityScale = 0f;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
    }

    void OnEnable()
    {
        Booster.OnBoosterEnter += Boost;
    }

    void OnDisable()
    {
        Booster.OnBoosterEnter -= Boost;
    }

    void Boost(KartDriveSystem kartDriveSystem)
    {
        if (kartDriveSystem == this && !isBoosting)
        {
            Debug.Log("🚀 Boost activé !");
            StartCoroutine(BoostCoroutine());
        }
    }

    // private IEnumerator BoostCoroutine()
    // {
    //     isBoosting = true;

    //     float originalMaxSpeed = maxSpeed;
    //     float originalAcceleration = acceleration;

    //     // Appliquer le boost
    //     maxSpeed *= boostMultiplier;
    //     acceleration *= boostMultiplier;

    //     yield return new WaitForSeconds(boostDuration);

    //     // Revenir à la normale
    //     maxSpeed = originalMaxSpeed;
    //     acceleration = originalAcceleration;
    //     isBoosting = false;

    //     Debug.Log("Boost terminé !");
    // }

    // Chat GPT en dessous (BoostCoroutine)
    private IEnumerator BoostCoroutine()
    {
        isBoosting = true;

        float originalMaxSpeed = maxSpeed;
        float originalAcceleration = acceleration;

        // Étape 1 : activer le boost
        maxSpeed *= boostMultiplier;
        acceleration *= boostMultiplier;
        Debug.Log("🚀 Boost activé !");

        yield return new WaitForSeconds(boostDuration);

        // Étape 2 : retour progressif
        float fadeTime = 1f; // durée du retour à la normale
        float elapsed = 0f;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeTime;

            maxSpeed = Mathf.Lerp(originalMaxSpeed * boostMultiplier, originalMaxSpeed, t);
            acceleration = Mathf.Lerp(
                originalAcceleration * boostMultiplier,
                originalAcceleration,
                t
            );

            yield return null;
        }

        // Étape 3 : retour complet
        maxSpeed = originalMaxSpeed;
        acceleration = originalAcceleration;
        isBoosting = false;

        Debug.Log("Boost terminé !");
    }

    // ChatGPT au dessus (BoostCoroutine)

    public void Move(float moveInput, float turnInput)
    {
        // --- Accélération avant/arrière ---
        if (moveInput != 0f)
        {
            currentSpeed += moveInput * acceleration * Time.fixedDeltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed * 0.5f, maxSpeed);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.fixedDeltaTime);
        }

        // --- Rotation ---
        rb.rotation -= turnInput * turnSpeed * Time.fixedDeltaTime;

        // --- Appliquer la vitesse ---
        rb.linearVelocity = transform.up * currentSpeed;
    }

    public void ResetVelocity()
    {
        rb.linearVelocity = Vector2.zero;
    }

    public void ResetAll()
    {
        ResetVelocity();
        rb.rotation = 0f;
        rb.transform.rotation = Quaternion.identity;
    }
}
