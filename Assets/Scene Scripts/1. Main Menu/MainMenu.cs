using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject prompt;
    [SerializeField] private string path;
    [SerializeField] private Button loadBtn;
    [SerializeField] private string filName;

    private void Start()
    {
        path = Path.Combine(Application.persistentDataPath, "PartyData.json");
        StartCoroutine(InitAfterPersistentDataLoaded());
    }

    private IEnumerator InitAfterPersistentDataLoaded()
    {
        // Load the Persistent Data scene if it's not already loaded
        if (SceneManager.GetSceneByName("Persistent Data").isLoaded == false)
        {
            SceneManager.LoadScene("Persistent Data", LoadSceneMode.Additive);
        }

        // Wait 1–2 frames for Awake/Start to run
        yield return null;
        yield return null;

        // Wait until PlayerDataHolder is ready
        while (PlayerDataHolder.Instance == null)
        {
            yield return null;
        }

        // First check: does the file exist?
        if (File.Exists(path))
        {
            loadBtn.interactable = true;
            loadBtn.GetComponentInChildren<TMP_Text>().color = Color.white;
        }
        else
        {
            loadBtn.interactable = false;
            loadBtn.GetComponentInChildren<TMP_Text>().color = Color.gray;
        }

        // Second check: is PlayerDataHolder usable?
        if (PlayerDataHolder.Instance.partyMembers.Count > 0)
        {
            loadBtn.interactable = true;
            loadBtn.GetComponentInChildren<TMP_Text>().color = Color.white;
        }
        else
        {
            loadBtn.interactable = false;
            loadBtn.GetComponentInChildren<TMP_Text>().color = Color.gray;
        }

        // Now safe to trigger BGM loading
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.LoadBGM(SceneManager.GetActiveScene());
        }
    }

    public void PlayGame()
    {
        bool hasFile = File.Exists(path);
        bool hasMemory = PlayerDataHolder.Instance != null && PlayerDataHolder.Instance.partyMembers.Count > 0;

        if (hasFile && hasMemory)
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
        StartCoroutine(LoadGameRoutine());
    }

    private IEnumerator LoadGameRoutine()
    {
        if (GameManager.Instance == null)
        {
            SceneManager.LoadScene("Persistent Data", LoadSceneMode.Additive);
            yield return null;

            while (PlayerDataHolder.Instance == null || string.IsNullOrEmpty(PlayerDataHolder.Instance.partyCurrentTown))
            {
                yield return null;
            }

            SceneManager.LoadScene(PlayerDataHolder.Instance.partyCurrentTown);
        }
        else
        {
            SceneManager.LoadScene(PlayerDataHolder.Instance.partyCurrentTown);
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
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Save file deleted.");
        }

        if (PlayerDataHolder.Instance != null)
        {
            PlayerDataHolder.Instance.ClearData();
        }

        SceneManager.LoadScene("Character Selection");
    }
}
