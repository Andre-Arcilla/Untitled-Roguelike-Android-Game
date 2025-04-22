using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject prompt;
    public string path;

    public void PlayGame()
    {
        path = Path.Combine(Application.persistentDataPath, "Character1Data.json");

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
