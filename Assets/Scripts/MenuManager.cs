using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject stageSelectScreen;
    [SerializeField] private GameObject[] stagePreviews;
    [HideInInspector] public bool[] ai = new bool[] { false, false };
    private int stage = 0;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void ReturnToTitle()
    {
        stageSelectScreen.SetActive(false);
    }

    public void LoadStageSelect()
    {
        SetStage(1);
        stageSelectScreen.SetActive(true);
    }

    public void SetStage(int index)
    {
        stage = index;
        foreach (GameObject preview in stagePreviews)
        {
            preview.SetActive(false);
        }
        stagePreviews[index - 1].SetActive(true);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(stage);
    }

    public void SinglePlayer()
    {
        ai[1] = true;
        LoadStageSelect();
    }

    public void Multiplayer()
    {
        ai[1] = false;
        LoadStageSelect();
    }
}
