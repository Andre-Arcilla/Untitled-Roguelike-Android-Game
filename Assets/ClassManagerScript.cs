using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClassManagerScript : MonoBehaviour
{
    public SpriteRenderer sprite;
    public List<GameObject> classes = new List<GameObject>();
    private int selectedClass = 0;
    public GameObject playerClass;

    public void NextBtn()
    {
        selectedClass++;

        if (selectedClass == classes.Count)
        {
            selectedClass = 0;
        }

        Destroy(playerClass);

        // Instantiate the new class prefab
        playerClass = Instantiate(classes[selectedClass], transform.position, Quaternion.identity);

        // Set the scale of the new prefab (X = 4, Y = 4, Z = 4)
        playerClass.transform.localScale = new Vector3(4, 4, 0);

        // Set the position of the new prefab (X = 0, Y = 0.6, Z = 0)
        playerClass.transform.position = new Vector3(0, 0.6f, 0);
    }

    public void PrevBtn()
    {
        selectedClass--;

        if (selectedClass < 0)
        {
            selectedClass = classes.Count-1;
        }

        Destroy(playerClass);

        // Instantiate the new class prefab
        playerClass = Instantiate(classes[selectedClass], transform.position, Quaternion.identity);

        // Set the scale of the new prefab (X = 4, Y = 4, Z = 4)
        playerClass.transform.localScale = new Vector3(4, 4, 0);

        // Set the position of the new prefab (X = 0, Y = 0.6, Z = 0)
        playerClass.transform.position = new Vector3(0, 0.6f, 0);
    }

    public void Create()
    {
        PrefabUtility.SaveAsPrefabAsset(playerClass, "Assets/SelectedUnit.prefab");
        //SceneManager.LoadScene("");
    }

    public void Cancel()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
