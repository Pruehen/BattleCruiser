using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxChanger : GlobalSingleton<SkyboxChanger>    
{
    public Material[] newSkyboxMaterial;
    // Start is called before the first frame update
    void Awake()
    {
        SkyboxChanger.Instance.ChangeSkybox();
    }

    public void ChangeSkybox()
    {        
        RenderSettings.skybox = newSkyboxMaterial[Random.Range(0, newSkyboxMaterial.Length)];
    }
}
