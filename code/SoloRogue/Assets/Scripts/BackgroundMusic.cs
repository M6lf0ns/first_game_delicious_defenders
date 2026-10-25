using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour

{
    [SerializeField] private AudioSource clip;

    // Start is called before the first frame update
    void Start()
    {
        clip.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.currentState == GameManager.GameState.GameOver)
        {
            clip.Stop();
        }
        else if (GameManager.instance.currentState == GameManager.GameState.Paused || GameManager.instance.currentState == GameManager.GameState.LevelUp)
        {
            clip.volume = 0.5f;
        }
    }
}
