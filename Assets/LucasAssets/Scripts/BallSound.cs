using UnityEngine;

public class BallSound : MonoBehaviour
{

    public void OnCollisionEnter2D(Collision2D collision)
    {
        FootSoundManager.Play("Hit");
    }

}
