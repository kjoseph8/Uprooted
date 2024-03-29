using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Services.Core;
using Unity.Services.Authentication;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject stageSelectScreen;
    [SerializeField] private GameObject[] stagePreviews;
    [SerializeField] private GameObject characterSelectScreen;
    [SerializeField] private Image[] cpuToggles;
    [SerializeField] private Sprite[] cpuToggleSprites;
    [SerializeField] private Button[] p1ColorButtons;
    [SerializeField] private Button[] p2ColorButtons;
    [SerializeField] private GameObject[] p1CharacterPreviews;
    [SerializeField] private GameObject[] p2CharacterPreviews;
    [SerializeField] private Image[] p1Sprites;
    [SerializeField] private Image[] p2Sprites;
    [SerializeField] private GameObject howToPlayScreen;
    [SerializeField] private GameObject[] howToPlayPages;
    [SerializeField] private GameObject howToPlayBack;
    [SerializeField] private GameObject howToPlayNext;
    [SerializeField] private GameObject exitButton;
    [HideInInspector] public int stage;
    [HideInInspector] public bool[] ai;
    [HideInInspector] public int[] plant;
    [HideInInspector] public int[] color;
    private int pageIndex = 0;

    async void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (Exception e)
        {
            
        }
    }

    public void Start()
    {
        stage = 1;
        ai = new bool[] { false, true };
        plant = new int[] { 0, 1 };
        color = new int[] { 0, 0 };
#if !UNITY_WEBGL
        exitButton.SetActive(true);
#endif
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    public void ReturnToTitle()
    {
        stageSelectScreen.SetActive(false);
        characterSelectScreen.SetActive(false);
        howToPlayScreen.SetActive(false);
    }

    public void LoadStageSelect()
    {
        characterSelectScreen.SetActive(false);
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

    public void LoadCharacterSelect()
    {
        stageSelectScreen.SetActive(false);
        characterSelectScreen.SetActive(true);
    }

    public void ToggleCPU(int player)
    {
        ai[player] = !ai[player];
        if (ai[player])
        {
            cpuToggles[player].sprite = cpuToggleSprites[1];
        }
        else
        {
            cpuToggles[player].sprite = cpuToggleSprites[0];
        }
    }

    public void SelectP1Character(int plantIndex)
    {
        plant[0] = plantIndex;
        for (int i = 0; i < PlantConfigs.buttonColors[plantIndex].Length; i++)
        {
            p1ColorButtons[i].GetComponent<Image>().color = PlantConfigs.buttonColors[plantIndex][i];
        }
        foreach (GameObject preview in p1CharacterPreviews)
        {
            preview.SetActive(false);
        }
        p1CharacterPreviews[plantIndex].SetActive(true);

        if (plant[0] == plant[1])
        {
            if (color[1] == 0)
            {
                SetP1Color(1);
                p1ColorButtons[0].interactable = false;
                p2ColorButtons[1].interactable = false;
            }
            else
            {
                SetP1Color(0);
                p1ColorButtons[color[1]].interactable = false;
                p2ColorButtons[0].interactable = false;
            }
        }
        else
        {
            SetP1Color(0);
            for (int i = 0; i < p1ColorButtons.Length; i++)
            {
                p1ColorButtons[i].interactable = true;
                p2ColorButtons[i].interactable = true;
            }
        }
    }

    public void SelectP2Character(int plantIndex)
    {
        plant[1] = plantIndex;
        for (int i = 0; i < PlantConfigs.buttonColors[plantIndex].Length; i++)
        {
            p2ColorButtons[i].GetComponent<Image>().color = PlantConfigs.buttonColors[plantIndex][i];
        }
        foreach (GameObject preview in p2CharacterPreviews)
        {
            preview.SetActive(false);
        }
        p2CharacterPreviews[plantIndex].SetActive(true);

        if (plant[1] == plant[0])
        {
            if (color[0] == 0)
            {
                SetP2Color(1);
                p1ColorButtons[1].interactable = false;
                p2ColorButtons[0].interactable = false;
            }
            else
            {
                SetP2Color(0);
                p1ColorButtons[0].interactable = false;
                p2ColorButtons[color[0]].interactable = false;
            }
        }
        else
        {
            SetP2Color(0);
            for (int i = 0; i < p1ColorButtons.Length; i++)
            {
                p1ColorButtons[i].interactable = true;
                p2ColorButtons[i].interactable = true;
            }
        }
    }

    public void SetP1Color(int colorIndex)
    {
        color[0] = colorIndex;
        p1Sprites[plant[0]].color = PlantConfigs.plantColors[plant[0]][colorIndex];
        if (plant[1] == plant[0])
        {
            foreach (Button button in p2ColorButtons)
            {
                button.interactable = true;
            }
            p2ColorButtons[colorIndex].interactable = false;
        }
    }

    public void SetP2Color(int colorIndex)
    {
        color[1] = colorIndex;
        p2Sprites[plant[1]].color = PlantConfigs.plantColors[plant[1]][colorIndex];
        if (plant[0] == plant[1])
        {
            foreach (Button button in p1ColorButtons)
            {
                button.interactable = true;
            }
            p1ColorButtons[colorIndex].interactable = false;
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(stage);
    }

    public void LoadHowToPlay()
    {
        howToPlayScreen.SetActive(true);
        if (pageIndex == 0)
        {
            howToPlayBack.SetActive(false);
        }
        else
        {
            howToPlayBack.SetActive(true);
        }
        if (pageIndex == howToPlayPages.Length - 1)
        {
            howToPlayNext.SetActive(false);
        }
        else
        {
            howToPlayNext.SetActive(true);
        }
    }

    public void PreviousPage()
    {
        howToPlayPages[pageIndex].SetActive(false);
        pageIndex--;
        howToPlayPages[pageIndex].SetActive(true);

        if (pageIndex == 0)
        {
            howToPlayBack.SetActive(false);
        }
        else
        {
            howToPlayBack.SetActive(true);
        }
        if (pageIndex == howToPlayPages.Length - 1)
        {
            howToPlayNext.SetActive(false);
        }
        else
        {
            howToPlayNext.SetActive(true);
        }
    }

    public void NextPage()
    {
        howToPlayPages[pageIndex].SetActive(false);
        pageIndex++;
        howToPlayPages[pageIndex].SetActive(true);

        if (pageIndex == 0)
        {
            howToPlayBack.SetActive(false);
        }
        else
        {
            howToPlayBack.SetActive(true);
        }
        if (pageIndex == howToPlayPages.Length - 1)
        {
            howToPlayNext.SetActive(false);
        }
        else
        {
            howToPlayNext.SetActive(true);
        }
    }
}
