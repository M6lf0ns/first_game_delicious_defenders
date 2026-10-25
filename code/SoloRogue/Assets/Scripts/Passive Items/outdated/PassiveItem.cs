using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveItem : MonoBehaviour
{
    protected PlayerStats player;
    public PassiveItemScriptableObject passiveItemData;

    protected virtual void ApplyModifier()
    {
        //Apply boost value to appropriate stat in child classes
    }

    void Start()
    {
        player = FindObjectOfType<PlayerStats>();
        ApplyModifier();
    }


}
