using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightObject : OnActionObject
{
    public GameObject lightObject;
    public Material startingMaterial;
    public Material swapMaterial;
    public int materialIndex;

    public bool activateOnlyOnce = false;

    [HideInInspector]
    public bool toggled;
    [HideInInspector]
    public bool lightEnabled;

    public void Start() {
        lightEnabled = true;
        toggled = false;
        Renderer rend = lightObject.GetComponent<Renderer>();
        if (rend == null || rend.materials.Length <= materialIndex){
            Debug.Log($"OnAction Light {lightObject.name} was not configured correctly");
            lightEnabled = false;
        }

        ChangeLight(startingMaterial);
    }

    public override void OnAction(){
        if (toggled){
            ChangeLight(startingMaterial);
            toggled = false;
        }
        else {
            ChangeLight(swapMaterial);
            toggled = true;
        }
        
        if (activateOnlyOnce) lightEnabled = false;
    }

    protected void ChangeLight(Material mat){
        if (!lightEnabled) return;
        Material[] matArray = lightObject.GetComponent<Renderer>().materials;
        matArray[materialIndex] = mat;
        lightObject.GetComponent<Renderer>().materials = matArray;
    }
}
