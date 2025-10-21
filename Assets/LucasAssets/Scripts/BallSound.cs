using UnityEngine;

public class BallSound : MonoBehaviour
{
    public FootSoundManager footSound;

    public void OnCollisionEnter2D(Collision2D collision)
    {
        FootSoundManager.Play("Hit");
    }

}
