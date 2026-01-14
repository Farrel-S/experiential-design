using System;
using UnityEngine;

public class SceneSwitcher : MonoBehaviour
{
    public void SwitchToLanding()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        Debug.Log("Switched to Landing Scene");
    }
    public void SwitchToHome()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        Debug.Log("Switched to Home Scene");
    }
    public void SwitchToScanner()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
        Debug.Log("Switched to Scanner Scene");
    }

    public void SwitchToCredits()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(3);
        Debug.Log("Switched to Credits Scene");
    }
}

