using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Persistent GameObjects")]
    public GameObject[] persistentObjects;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            CleanUpAndDestroy();
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        MarkPersistentObjects();
        Initialize();
    }

    private void MarkPersistentObjects()
    {
        foreach (GameObject obj in persistentObjects)
        {
            if (obj != null)
            {
                DontDestroyOnLoad(obj);
            }
        }
    }

    private void CleanUpAndDestroy()
    {
        foreach (GameObject obj in persistentObjects)
        {
            Destroy(obj);
        }

        Destroy(gameObject);
    }

    private void Initialize()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioManager.Instance.LoadBGM(scene);
        PlayerDataHolder.Instance.SavePartyInfo();
    }
}
