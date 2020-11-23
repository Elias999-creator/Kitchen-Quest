using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource AudioSource;

    private float musicVolume = 1f;
    private static GameObject instance;

    void Awake()
    {
        AudioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(this);
        if (instance == null)
            instance = gameObject;
        else
            Destroy(gameObject);
        AudioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        AudioSource.volume = musicVolume;
    }

    public void updateVolume( float volume)
    {
        musicVolume = volume;
    }
}
