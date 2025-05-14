using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject prompt;
    [SerializeField] private string path;
    [SerializeField] private Button loadBtn;

    private void Start()
    {
        path = Path.Combine(Application.persistentDataPath, "PartyData.json");

        if (File.Exists(path))
        {
            loadBtn.interactable = true;
        }
        else
        {
            loadBtn.interactable = false;
        }
    }

    public void PlayGame()
    {
        if (File.Exists(path))
        {
            Debug.Log("Save file found");
            DeleteSavePrompt();
        }
        else
        {
            Debug.Log("No save file");
            SceneManager.LoadScene("Character Selection");
        }
    }

    public void LoadGame()
    {

        if (GameManager.Instance == null)
        {
            SceneManager.LoadScene("Persistent Data");
            SceneManager.LoadScene("Town 1");
        }
        else
        {
            SceneManager.LoadScene("Town 1");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void DeleteSavePrompt()
    {
        prompt.SetActive(true);
    }

    public void promptNo()
    {
        prompt.SetActive(false);
    }

    public void promptYes()
    {
        File.Delete(path);

        SceneManager.LoadScene("Character Selection");
    }
}
