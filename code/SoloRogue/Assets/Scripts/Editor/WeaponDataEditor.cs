using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

[CustomEditor(typeof(WeaponData))]
public class WeaponDataEditor : Editor
{
    WeaponData weaponData;
    string[] weaponSubtypes;
    int selectedWeaponSubtype;

    void OnEnable()
    {
        //Cache weapon data value
        weaponData = (WeaponData)target;

        //Retrieve all the weapon subtypes and cache it
        System.Type baseType = typeof(Weapon);
        List<System.Type> subTypes = System.AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(s => s.GetTypes())
        .Where(p => baseType.IsAssignableFrom(p) && p != baseType)
        .ToList();

        //Add a None Option in front
        List<string> subTypesString = subTypes.Select(t => t.Name).ToList();
        subTypesString.Insert(0, "None");
        weaponSubtypes = subTypesString.ToArray();

        //Ensure that we are using the correct weapon subtype
        selectedWeaponSubtype = Math.Max(0, Array.IndexOf(weaponSubtypes, weaponData.behavior));
    }

    public override void OnInspectorGUI()
    {
        //Draw a dropdown in the Inspector
        selectedWeaponSubtype = EditorGUILayout.Popup("Behavior", Math.Max(0, selectedWeaponSubtype), weaponSubtypes);

        if (selectedWeaponSubtype > 0)
        {
            //Updates the bahavior field
            weaponData.behavior = weaponSubtypes[selectedWeaponSubtype].ToString();
            EditorUtility.SetDirty(weaponData); //Marks the object to save
            DrawDefaultInspector(); //Draw the default inspector elements
        }




    }
}
