using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    PlayerMovement pm;
    PlayerStats stats;
    public static GameManager instance;
    //Defines the different game states
    public enum GameState
    {
        Gameplay,
        Paused,
        GameOver,
        LevelUp
    }
    //Stores current state of game
    public GameState currentState;
    public GameState previousState;

    [Header("UI Elements")]
    public GameObject pauseScreen;
    public GameObject resultsScreen;
    public GameObject levelUpScreen;

    [Header("Current Stat Displays")]
    //Current stat Displays
    public TextMeshProUGUI currentHealthDisplay;
    public TextMeshProUGUI currentRecoveryDisplay;
    public TextMeshProUGUI currentMoveSpeedDisplay;
    public TextMeshProUGUI currentMightDisplay;
    public TextMeshProUGUI currentProjectileSpeedDisplay;
    public TextMeshProUGUI currentMagnetDisplay;

    [Header("Results Screen Displays")]
    public Image chosenCharacterImage;
    public TextMeshProUGUI chosenCharacterName;
    public TextMeshProUGUI levelReachedDisplay;
    public List<Image> chosenWeaponsUI = new List<Image>(3);
    public List<Image> chosenPassiveItemsUI = new List<Image>(4);

    [Header("Timer")]
    public float timeLimit;
    public float stopwatchTime;
    public TextMeshProUGUI stopwatchDisplay;
    public TextMeshProUGUI timeSurvivedDisplay;

    //Check if player is choosing upgrades
    public bool choosingUpgrade;

    //Reference to player´s GO
    public GameObject playerObject;

    [Header("Sounds")]

    [SerializeField] private AudioSource clips;
    [SerializeField] private AudioClip levelUpClip;
    [SerializeField] private AudioClip gameOverClip;
    [SerializeField] private AudioClip victoryClip;




    //Flag to check if game has ended
    public bool isGameOver = false;
    void Awake()
    { //Warning check to check for another singleton of this type
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("EXTRA" + this + "DELETED");
            Destroy(gameObject);

        }
        DisableScreens();


    }
    void Start()
    {
        pm = FindObjectOfType<PlayerMovement>();
        stats = pm.GetComponent<PlayerStats>();
    }

    void Update()
    {   //Define behavior for each state

        // Check the active scene regularly
        UpdateSceneMusic();

        switch (currentState)
        {
            case GameState.Gameplay:
                //Code for gameplay state
                CheckForPauseAndResume();
                UpdateStopwatch();
                break;

            case GameState.Paused:
                //Code for paused state
                CheckForPauseAndResume();
                break;

            case GameState.GameOver:
                //Code for game over state
                if (!isGameOver)
                {
                    isGameOver = true; //prevents display function from being executed mutliple times
                    Time.timeScale = 0f; //Stop the game entirely
                    Debug.Log("GAME OVER");
                    DisplayResults();
                    timeSurvivedDisplay.text = stopwatchDisplay.text;

                }
                break;

            case GameState.LevelUp:
                //code for Level up state
                if (!choosingUpgrade)
                {
                    choosingUpgrade = true;
                    Time.timeScale = 0f; //Pause game 
                    Debug.Log("In Level Up State");
                    levelUpScreen.SetActive(true);
                    //clips.PlayOneShot(levelUpClip, 0.8f);
                }
                break;


            default:
                Debug.LogWarning("STATE DOES NOT EXIST");
                break;


        }


    }
    public void ChangeState(GameState newState)
    {
        currentState = newState;

        // Handle music transition based on state
        if (newState == GameState.Gameplay)
        {
            MenuMusic.instance.PlayGameMusic();
        }
        else if (newState == GameState.GameOver)
        {
            if (timeLimit == Mathf.Floor(stopwatchTime))
            {
                clips.PlayOneShot(victoryClip, 0.8f);
            }
            else
            {
                clips.PlayOneShot(gameOverClip, 0.8f);
            }
        }
        else if (newState == GameState.LevelUp)
        {
            clips.PlayOneShot(levelUpClip); // Play level-up sound
        }

    }
    public void PauseGame()
    {
        if (currentState != GameState.Paused)
        {
            previousState = currentState;
            ChangeState(GameState.Paused);
            Time.timeScale = 0f;
            pauseScreen.SetActive(true);
            GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + stats.CurrentRecovery;
            GameManager.instance.currentMoveSpeedDisplay.text = "Move Speed: " + stats.CurrentMoveSpeed;
            GameManager.instance.currentMightDisplay.text = "Might: " + stats.CurrentMight;
            GameManager.instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + stats.CurrentProjectileSpeed;
            GameManager.instance.currentMagnetDisplay.text = "Magnet: " + stats.CurrentMagnet;

            Debug.Log("Game Paused");
            pm.enabled = false; //test
        }

    }

    public void ResumeGame()
    {
        if (currentState != GameState.Gameplay)
        {
            currentState = previousState; //ensures that game continues from the exact point where it was stopped
            ChangeState(previousState);
            Time.timeScale = 1;
            pauseScreen.SetActive(false);
            pm.enabled = true; //test
            Debug.Log("Game Resumed");
        }


    }

    public void CheckForPauseAndResume() //Checks for inputs that pause and resume game
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }

        }
    }

    void DisableScreens() //ensure that all currently irrelevant screens are disabled
    {
        pauseScreen.SetActive(false);
        resultsScreen.SetActive(false);
        levelUpScreen.SetActive(false);
    }

    public void GameOver()
    {
        ChangeState(GameState.GameOver);





    }

    void DisplayResults()
    {
        resultsScreen.SetActive(true);

    }

    public void AssignChosenCharacterUI(CharacterData chosenCharacterData) //to show the chosen Character on the results Screen
    {
        chosenCharacterImage.sprite = chosenCharacterData.Icon;
        chosenCharacterImage.preserveAspect = true;
        chosenCharacterName.text = chosenCharacterData.name;
    }

    public void AssignLevelReachedUI(int levelReachedData) //to display level reached on results Screen
    {
        levelReachedDisplay.text = levelReachedData.ToString();
    }

    public void AssignChosenWeaponsAndPassiveItemsUI(List<Image> chosenWeaponsData, List<Image> chosenPassiveItemsData)
    {
        if (chosenWeaponsData.Count != chosenPassiveItemsData.Count || chosenPassiveItemsData.Count != chosenWeaponsData.Count)
        {
            Debug.Log("Chosen Weapons and passive items data lists have different lenghts");
            return;
        }
        //Assign chosen weapons data to chosenWeaponsUI
        for (int i = 0; i < chosenWeaponsUI.Count; i++)
        {
            //Check that the sprite of the corresponding element in chosenWeaponsData is not null
            if (chosenWeaponsData[i].sprite)
            {
                //Enable the corresponding element in chosen WeaponsUI and set its sprite to the corresponding sprite in chosenWeaponsData
                chosenWeaponsUI[i].enabled = true;
                chosenWeaponsUI[i].preserveAspect = true;
                chosenWeaponsUI[i].sprite = chosenWeaponsData[i].sprite;
            }
            else
            {
                //If the sprite is not null, disable the corresponding element in chosen WEapons UI
                chosenWeaponsUI[i].enabled = false;
            }
        }

        for (int i = 0; i < chosenPassiveItemsUI.Count; i++)
        {
            //Check that the sprite of the corresponding element in chosenPassiveItemsData is not null
            if (chosenPassiveItemsData[i].sprite)
            {
                //Enable the corresponding element in chosen PassiveItemsUI and set its sprite to the corresponding sprite in chosenPassiveItemsData
                chosenPassiveItemsUI[i].enabled = true;
                chosenWeaponsUI[i].preserveAspect = true;
                chosenPassiveItemsUI[i].sprite = chosenPassiveItemsData[i].sprite;
            }
            else
            {
                //If the sprite is not null, disable the corresponding element in chosenPassiveItemsUI
                chosenPassiveItemsUI[i].enabled = false;
            }
        }
    }

    void UpdateStopwatch()
    {
        stopwatchTime += Time.deltaTime;
        UpdateStopwatchDisplay();

        if (stopwatchTime >= timeLimit) //if timeLimit is reached
        {
            playerObject.SendMessage("Kill");


        }
    }

    void UpdateStopwatchDisplay()
    {
        int minutes = Mathf.FloorToInt(stopwatchTime / 60);
        int seconds = Mathf.FloorToInt(stopwatchTime % 60);
        stopwatchDisplay.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }

    public void StartLevelUp()
    {
        ChangeState(GameState.LevelUp); //changes state to LevelUp
        playerObject.GetComponent<PlayerInventory>().RemoveAndApplyUpgrades(); // Call RemoveAndApplyUpgrades on PlayerInventory
        clips.PlayOneShot(levelUpClip);                                                                    //playSoundFX TEST 


        //playerObject.SendMessage("RemoveAndApplyUpgradeOptions");
    }

    public void EndLevelUp()
    {
        choosingUpgrade = false; //indicates that we are no longer in upgrade mode
        Time.timeScale = 1; //starts time back up
        levelUpScreen.SetActive(false);
        ChangeState(GameState.Gameplay); //switch back to gameplay mode
    }

    void UpdateSceneMusic()
    {
        string activeScene = SceneManager.GetActiveScene().name;
        MenuMusic.instance.UpdateMusicBasedOnScene(activeScene);
    }


}



