using UnityEngine;
using UnityEngine.SceneManagement;

public class GenerationChecker : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Optional: check if this is a town scene where you want to generate
        // if (scene.name == "TownSceneName") 

        var tavern = FindObjectOfType<TavernManager>();
        if (tavern != null)
        {
            //tavern.OnEnterTown();
        }
        else
        {
            Debug.LogWarning("TavernManager not found on scene load!");
        }
    }
}
