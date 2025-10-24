using UnityEngine;

public class FootAudioManager : MonoBehaviour
{
    [SerializeField] AudioSource music;

    public AudioClip background;

    private void Start()
    {
        music.clip = background;
        music.Play();
    }

}
