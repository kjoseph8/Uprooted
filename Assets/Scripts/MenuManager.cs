using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [HideInInspector] public bool[] ai = new bool[] { false, false };

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void SinglePlayer()
    {
        ai[1] = true;
        PlayGame();
    }
}
