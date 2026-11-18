
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject titleScreenPanel;
    // [System.Serializable] lets Unity display this plain C# class in the Inspector
    // Reference: https://docs.unity3d.com/2022.1/Documentation/ScriptReference/Serializable.html


    void Start()
    {
        ShowTitleScreen();
    }

    void ShowTitleScreen()
    {
        titleScreenPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OnStartGame()
    {
        titleScreenPanel.SetActive(false);
        Time.timeScale = 1f;

        if (InstructionsManager.Instance != null)
            InstructionsManager.Instance.Show();
        else
            Debug.LogError("InstructionsManager.Instance is null!");
    }

    public void OnExitGame()
    {
        Application.Quit();
    }
}