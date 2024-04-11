using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : SceneSingleton<SceneChanger>
{
    public enum SceneName
    {
        Intro,
        Main,
        Battle
    }

    private void Awake()
    {
        Debug.Log($"{Instance.name} 인스턴싱 완료");
    }
    public void SceneChange(SceneName sceneName)
    {
        string name;
        switch (sceneName)
        {
            case SceneName.Intro:
                name = "Intro";
                break;
            case SceneName.Main:
                name = "Main";
                break;
            case SceneName.Battle:
                name = "Battle";
                break;
            default:
                name = "Intro";
                break;
        }

        SceneManager.LoadScene(name);
    }
    public void SceneChange(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
