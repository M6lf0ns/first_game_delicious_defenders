using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
  public static CharacterSelector instance; //stores an instance of class
  public CharacterData characterData; //stores the selected Character´s data

  void Awake()
  {
    if (instance == null)
    {
      instance = this; //
      DontDestroyOnLoad(gameObject);
    }
    else //makes sure there is only one instance of the class at all times
    {
      Debug.Log("Extra" + this + "Deleted");
      Destroy(gameObject);
    }
  }
  //returns character data to use for other scripts
  public static CharacterData GetData() //this way GetData() can be called immediately from CharacterSelector, without referencing instance
  {
    if (instance && instance.characterData)
      return instance.characterData;
    else
    {
      //If no character has been assigned, we randomly pick one
      CharacterData[] characters = Resources.FindObjectsOfTypeAll<CharacterData>();
      if (characters.Length > 0)
      {
        return characters[Random.Range(0, characters.Length)];
      }
    }
    return null;

  }

  public void SelectCharacter(CharacterData character)
  {
    characterData = character; //set to the chosen CharacterSO
    Debug.Log(character.name);

  }

  public void DestroySingleton()
  {
    //after accessing the characterData, it is no longer necessary, also ensures that there are no unnecessary elements in background
    instance = null;
    Destroy(gameObject);


  }
}
