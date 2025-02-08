using UnityEngine;

public class AudioScript : MonoBehaviour
{
    [SerializeField] private AudioSource Music;
    [SerializeField] private AudioSource Ambience;
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private AudioClip Ambienceclip;

    void Start()
    {
        Music.clip = musicClip;
        Ambience.clip = Ambienceclip;

        Ambience.Play();
        Music.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
