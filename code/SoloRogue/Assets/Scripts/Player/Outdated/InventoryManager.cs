using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
[System.Obsolete]
public class InventoryManager : MonoBehaviour
{
    public List<WeaponController> weaponSlots = new List<WeaponController>(3); //In vampire survivors its usually 6 weaponslots
    public int[] weaponLevels = new int[8];
    public List<Image> weaponUISlots = new List<Image>(3);
    public List<PassiveItem> passiveItemSlots = new List<PassiveItem>(4); //..and 6 passive items
    public int[] passiveItemLevels = new int[5];
    public List<Image> passiveItemUISlots = new List<Image>(5);

    [System.Serializable]
    public class WeaponUpgrade
    {
        public int weaponUpgradeIndex; //stores Index of the upgrade data, similar to weaponIndex and PIIndex in PlayerStats
        public GameObject initialWeapon; //will store initial weapon or first level prefab
        public WeaponScriptableObject weaponData; //will hold all relevant data for the weapon
    }

    [System.Serializable]
    public class PassiveItemUpgrade
    {
        public int passiveItemUpgradeIndex;
        public GameObject initialPassiveItem; //will store initial PI or first level prefab
        public PassiveItemScriptableObject passiveItemData;
    }

    [System.Serializable]
    public class UpgradeUI
    {
        public TextMeshProUGUI upgradeNameDisplay; //name of the upgrade
        public TextMeshProUGUI upgradeDescriptionDisplay; //description of upgrade
        public Image upgradeIcon; //corresponding icon 
        public Button upgradeButton; //used to handle interactions with the upgrade button
    }

    PlayerStats player;

    void Start()
    {
        player = GetComponent<PlayerStats>();
    }

    public List<WeaponUpgrade> weaponUpgradeOptions = new List<WeaponUpgrade>(); //List of upgrade options for weapons
    public List<PassiveItemUpgrade> passiveItemUpgradeOptions = new List<PassiveItemUpgrade>(); //List of upgrade options for passive Items
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>(); //List of iu for upgrade options present in scene

    public void AddWeapon(int slotIndex, WeaponController weapon)
    {
        weaponSlots[slotIndex] = weapon; //Adds new weapon at indicated index
        weaponLevels[slotIndex] = weapon.weaponData.Level;
        weaponUISlots[slotIndex].enabled = true;
        weaponUISlots[slotIndex].preserveAspect = true; //so image size isnt adjusted when fit into inventory slots
        weaponUISlots[slotIndex].sprite = weapon.weaponData.Icon;

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade) //if instance exists and we are currently choosing an upgrade
        {
            GameManager.instance.EndLevelUp(); // once a weapon is added, by selecting it, the LevelUp State is exited
        }

    }
    public void AddPassiveItem(int slotIndex, PassiveItem passiveItem)
    {
        passiveItemSlots[slotIndex] = passiveItem; //same but with PassiveItem
        passiveItemLevels[slotIndex] = passiveItem.passiveItemData.Level;
        passiveItemUISlots[slotIndex].enabled = true; //Enable image cpmponent
        passiveItemUISlots[slotIndex].preserveAspect = true;
        passiveItemUISlots[slotIndex].sprite = passiveItem.passiveItemData.Icon;

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade) //if instance exists and we are currently choosing an upgrade
        {
            GameManager.instance.EndLevelUp(); // once a weapon is added, by selecting it, the LevelUp State is exited
        }
    }

    public void LevelUpWeapon(int slotIndex, int upgradeIndex)
    {   //If a weapon exists
        if (weaponSlots.Count > slotIndex)
        {
            WeaponController weapon = weaponSlots[slotIndex];
            if (!weapon.weaponData.NextLevelPrefab)
            {
                Debug.LogError("No next Level for" + weapon.name);
                return;
            }
            GameObject upgradedWeapon = Instantiate(weapon.weaponData.NextLevelPrefab, transform.position, Quaternion.identity);
            upgradedWeapon.transform.SetParent(transform); //Set weapon as Child of Player
            AddWeapon(slotIndex, upgradedWeapon.GetComponent<WeaponController>()); //Add the leveled up weapon into the inventory
            Destroy(weapon.gameObject); //Destroy the old one
            weaponLevels[slotIndex] = upgradedWeapon.GetComponent<WeaponController>().weaponData.Level; //make sure the correct weapon level is fetched

            //Ensures that the chosen upgrade option receives the updated data after it´s selected
            weaponUpgradeOptions[upgradeIndex].weaponData = upgradedWeapon.GetComponent<WeaponController>().weaponData; //Assign the data of the upgraded weapon or passive item to the data of the corresponding upgrade option based on the upgrade index

            if (GameManager.instance != null && GameManager.instance.choosingUpgrade) //if instance exists and we are currently choosing an upgrade
            {
                GameManager.instance.EndLevelUp(); // once a weapon is added, by selecting it, the LevelUp State is exited
            }
        }
    }

    public void LevelUpPassiveItem(int slotIndex, int uprgadeIndex)
    {
        //If a passive item exists
        if (passiveItemSlots.Count > slotIndex)
        {
            PassiveItem passiveItem = passiveItemSlots[slotIndex];
            if (!passiveItem.passiveItemData.NextLevelPrefab)
            {
                Debug.LogError("No next Level for" + passiveItem.name);
                return;
            }
            GameObject upgradedPassiveItem = Instantiate(passiveItem.passiveItemData.NextLevelPrefab, transform.position, Quaternion.identity);
            upgradedPassiveItem.transform.SetParent(transform); //Set weapon as Child of Player
            AddPassiveItem(slotIndex, upgradedPassiveItem.GetComponent<PassiveItem>()); //Add the leveled up passive item into the inventory
            Destroy(passiveItem.gameObject); //Destroy the old one
            weaponLevels[slotIndex] = upgradedPassiveItem.GetComponent<PassiveItem>().passiveItemData.Level; //make sure the correct passive item level is fetched

            passiveItemUpgradeOptions[uprgadeIndex].passiveItemData = upgradedPassiveItem.GetComponent<PassiveItem>().passiveItemData;

            if (GameManager.instance != null && GameManager.instance.choosingUpgrade) //if instance exists and we are currently choosing an upgrade, here leveling up an item
            {
                GameManager.instance.EndLevelUp(); // once a weapon is added, by selecting it, the LevelUp State is exited
            }
        }

    }

    void ApplyUpgradeOptions()
    {
        //Create copies of the other lists so any modifications made will not be affected by original list and viceversa
        List<WeaponUpgrade> availableWeaponUpgrades = new List<WeaponUpgrade>(weaponUpgradeOptions);
        List<PassiveItemUpgrade> availablePassiveItemUpgrades = new List<PassiveItemUpgrade>(passiveItemUpgradeOptions);

        foreach (var upgradeOption in upgradeUIOptions)
        {

            if (availableWeaponUpgrades.Count == 0 && availablePassiveItemUpgrades.Count == 0) //If both lists are empty
            {
                return;
            }

            int upgradeType;

            if (availableWeaponUpgrades.Count == 0) //If there are no more available weapon upgrades
            {
                upgradeType = 2; //Set to passive item upgrades
            }
            else if (availablePassiveItemUpgrades.Count == 0) //vice versa
            {
                upgradeType = 1; //set to weapon upgrades
            }
            else
            {
                upgradeType = Random.Range(1, 3); // If both are available, Choose between weapon and passive item
            }
            if (upgradeType == 1) //If its a weapon upgrade
            {
                WeaponUpgrade chosenWeaponUpgrade = availableWeaponUpgrades[Random.Range(0, availableWeaponUpgrades.Count)]; //Chooses a random weaponUpgradeOption from the list

                availableWeaponUpgrades.Remove(chosenWeaponUpgrade); //Chosen upgrade is removed from copied list

                if (chosenWeaponUpgrade != null) //if upgrade exists
                {
                    EnableUpgradeUI(upgradeOption); //ensures that only the existing upgrades have buttons and UI to choose
                    bool newWeapon = false; //it is an existing weapon
                    for (int i = 0; i < weaponSlots.Count; i++) //searches for a slot that matches the weapon upgrade to determine whether weapon new or existing
                    {
                        if (weaponSlots[i] != null && weaponSlots[i].weaponData == chosenWeaponUpgrade.weaponData) //if the weapon at the index exists and its weaponData matches that of the chosenWeaponUpgrade
                        {
                            newWeapon = false;
                            if (!newWeapon) //if weapon already exists, upgrade it
                            {
                                //If the weapon is at max level
                                if (!chosenWeaponUpgrade.weaponData.NextLevelPrefab)
                                {
                                    DisableUpgradeUI(upgradeOption); //Disable the upgrade Option, since weapon can no longer be upgraded
                                    break;
                                }
                                //Set description and name to be that of the next level prefab
                                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWeapon(i, chosenWeaponUpgrade.weaponUpgradeIndex)); //Apply button fuctionality, when upgradeButton is pressed, the listener will call LevelUpWeapon on the weapon at the index
                                upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.weaponData.NextLevelPrefab.GetComponent<WeaponController>().weaponData.Description; //why does this even work??
                                upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.weaponData.NextLevelPrefab.GetComponent<WeaponController>().weaponData.Name;
                            }
                            break; //so that next upgrade option can be applied
                        }
                        else
                        {
                            newWeapon = true; //weapon is new
                        }
                    }
                    if (newWeapon) //Spawn a new weapon 
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => player.SpawnWeapon(chosenWeaponUpgrade.initialWeapon)); //Apply button functionality, when pressed, spawn the initial weapon
                                                                                                                                      //Apply initial description and name
                        upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.weaponData.Description;
                        upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.weaponData.Name;
                    }
                    upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.weaponData.Icon; //sprite should remain the same regardless of if weapon is "new" or not
                }
            }
            else if (upgradeType == 2) //If its a passive Item upgrade
            {
                PassiveItemUpgrade chosenPassiveItemUpgrade = availablePassiveItemUpgrades[Random.Range(0, availablePassiveItemUpgrades.Count)]; //Chooses a random upgrade FROM the COPIED list of upgrades

                availablePassiveItemUpgrades.Remove(chosenPassiveItemUpgrade); //Remove the chosen passive Item Upgrade from the copied list

                if (chosenPassiveItemUpgrade != null)
                {
                    EnableUpgradeUI(upgradeOption); //ensures that only the existing upgrades have buttons and UI to choose

                    bool newPassiveItem = false;
                    for (int i = 0; i < passiveItemSlots.Count; i++)
                    {
                        if (passiveItemSlots[i] != null && passiveItemSlots[i].passiveItemData == chosenPassiveItemUpgrade.passiveItemData)
                        {
                            newPassiveItem = false; //if PI at index exists and matches chosenPIUpgrade Data, its not new weapon

                            if (!newPassiveItem)
                            {
                                //If passive Item has reached max level, do not continue
                                if (!chosenPassiveItemUpgrade.passiveItemData.nextLevelPrefab)
                                {
                                    DisableUpgradeUI(upgradeOption); //Disable the upgrade Option, since Item can no longer be upgraded
                                    break;
                                }
                                //Set the description and name to be that of the next level
                                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpPassiveItem(i, chosenPassiveItemUpgrade.passiveItemUpgradeIndex)); //Apply button functionality
                                upgradeOption.upgradeDescriptionDisplay.text = chosenPassiveItemUpgrade.passiveItemData.nextLevelPrefab.GetComponent<PassiveItem>().passiveItemData.Description;
                                upgradeOption.upgradeNameDisplay.text = chosenPassiveItemUpgrade.passiveItemData.nextLevelPrefab.GetComponent<PassiveItem>().passiveItemData.Name;
                            }
                            break;

                        }
                        else
                        {
                            newPassiveItem = true;
                        }
                    }
                    if (newPassiveItem)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => player.SpawnPassiveItem(chosenPassiveItemUpgrade.initialPassiveItem)); //Apply button functionality
                                                                                                                                                     //Apply initial description and name
                        upgradeOption.upgradeDescriptionDisplay.text = chosenPassiveItemUpgrade.passiveItemData.Description;
                        upgradeOption.upgradeNameDisplay.text = chosenPassiveItemUpgrade.passiveItemData.Name;

                    }
                    upgradeOption.upgradeIcon.sprite = chosenPassiveItemUpgrade.passiveItemData.Icon;

                }

            }
        }
    }
    void RemoveUpgradeOptions()
    {
        foreach (var upgradeOption in upgradeUIOptions)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners(); //so the upgradebuttons are a clean slate
            DisableUpgradeUI(upgradeOption); //Disable all UI options before applying upgrades to them
        }
    }

    public void RemoveAndApplyUpgradeOptions() //because send message is very intensive
    {
        RemoveUpgradeOptions();
        ApplyUpgradeOptions();
    }

    void DisableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(false); //Get parent of the upgrade name display (the option button) and deactivate
    }

    void EnableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(true); //Get parent of the upgrade name display (the option button) and deactivate
    }
}
