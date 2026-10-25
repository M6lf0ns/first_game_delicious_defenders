using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMusic : MonoBehaviour
{
    public static MenuMusic instance;
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    private AudioSource audioSource;
    private float currentVolume = 0.8f; // Default volume



    void Awake()
    {
        Debug.Log("MenuMusic Awake called");

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);



            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            audioSource.loop = true;
            audioSource.volume = currentVolume; // Set initial volume here
            PlayMenuMusic();
        }
        else
        {
            Destroy(gameObject);

        }

    }


    void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneChanged; //Subscribe OnSceneChanged to the activeSceneChanged event
    }

    void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    void OnSceneChanged(Scene previousScene, Scene newScene) //Called when active Scene changes
    {
        UpdateMusicBasedOnScene(newScene.name);
    }

    public void UpdateMusicBasedOnScene(string activeScene)
    {
        if (GameManager.instance != null && GameManager.instance.resultsScreen.activeSelf) //Dont play music when player has died
        {
            audioSource.Stop();
            return;
        }

        if (activeScene == "Game")
        {
            PlayGameMusic();
        }
        else if (activeScene == "Title Screen" || activeScene == "Menu")
        {
            PlayMenuMusic();
        }
    }

    public void PlayMenuMusic()
    {
        if (audioSource.clip != menuMusic)
        {
            audioSource.clip = menuMusic;
            audioSource.volume = currentVolume; // Set volume here
            audioSource.Play();

        }
        if (!audioSource.isPlaying)
        {
            audioSource.volume = currentVolume; // Ensure the volume is set
            audioSource.Play();
        }

    }

    public void PlayGameMusic(float volume = 1f)
    {
        if (audioSource.clip != gameMusic)
        {
            audioSource.clip = gameMusic;
            audioSource.volume = currentVolume; // Set volume here
            audioSource.Play();
        }
    }

    // Method to update volume based on slider
    public void SetVolume(float volume)
    {
        // Ensure volume is clamped between 0 and 1
        volume = Mathf.Clamp01(volume);

        // Update current volume
        currentVolume = volume;

        // Update volume of currently playing audio source
        if (audioSource.isPlaying)
        {
            audioSource.volume = currentVolume;
        }
    }

    public float GetVolume()
    {
        return currentVolume;
    }
    void OnDestroy()
    {
        Debug.Log("MenuMusic OnDestroy called");
    }
}
