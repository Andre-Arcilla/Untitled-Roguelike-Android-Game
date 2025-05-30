using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClassManager : MonoBehaviour
{
    public ClassDataSO selectedClass { get; private set; }

    [SerializeField] private CardDisplay cardDisplay;
    [SerializeField] private GameObject playerClass;
    [SerializeField] private TMP_Text classText;
    public string selectedGender { get; private set; }

    private int swordClassIndex = 0;
    private int mageClassIndex = 0;
    private int classIndex = 0;

    public List<ClassDataSO> swordClass = new List<ClassDataSO>();
    public List<ClassDataSO> mageClass = new List<ClassDataSO>();


    private void Start()
    {
        selectedGender = "Male";
        GenerateSprite();
    }

    public void NextBtn()
    {
        switch (classIndex)
        {
            case 0:
                swordClassIndex++;
                //if selected class goes under index, loop back to highest index
                if (swordClassIndex == swordClass.Count)
                {
                    swordClassIndex = 0;
                }
                break;

            case 1:
                mageClassIndex++;
                //if selected class goes under index, loop back to highest index
                if (mageClassIndex == mageClass.Count)
                {
                    mageClassIndex = 0;
                }
                break;

            default:
                swordClassIndex++;
                //if selected class goes under index, loop back to highest index
                if (swordClassIndex == swordClass.Count)
                {
                    swordClassIndex = 0;
                }
                break;
        }

        GenerateSprite();
    }

    public void PrevBtn()
    {
        switch (classIndex)
        {
            case 0:
                swordClassIndex--;
                //if selected class goes under index, loop back to highest index
                if (swordClassIndex < 0)
                {
                    swordClassIndex = swordClass.Count - 1;
                }
                break;

            case 1:
                mageClassIndex--;
                //if selected class goes under index, loop back to highest index
                if (mageClassIndex < 0)
                {
                    mageClassIndex = mageClass.Count - 1;
                }
                break;

            default:
                swordClassIndex--;
                //if selected class goes under index, loop back to highest index
                if (swordClassIndex < 0)
                {
                    swordClassIndex = swordClass.Count - 1;
                }
                break;
        }

        GenerateSprite();
    }

    public void UpBtn()
    {
        //only outputs 0 or 1 to switch between mage and swordsman class
        classIndex++;
        if (classIndex == 2)
        {
            classIndex = 0;
        }
        GenerateSprite();
    }

    public void MaleBtn()
    {
        selectedGender = "Male";
        GenerateSprite();
    }

    public void FemaleBtn()
    {
        selectedGender = "Female";
        GenerateSprite();
    }

    private void GenerateSprite()
    {
        //delete the old selected class
        Destroy(playerClass);

        if (selectedGender == "Male" && classIndex == 0)
        {
            playerClass = Instantiate(swordClass[swordClassIndex].spriteMale, transform.position, Quaternion.identity, transform);
            selectedClass = swordClass[swordClassIndex];
        }

        if (selectedGender == "Female" && classIndex == 0)
        {
            playerClass = Instantiate(swordClass[swordClassIndex].spriteFemale, transform.position, Quaternion.identity, transform);
            selectedClass = swordClass[swordClassIndex];
        }

        if (selectedGender == "Male" && classIndex == 1)
        {
            playerClass = Instantiate(mageClass[mageClassIndex].spriteMale, transform.position, Quaternion.identity, transform);
            selectedClass = mageClass[mageClassIndex];
        }

        if (selectedGender == "Female" && classIndex == 1)
        {
            playerClass = Instantiate(mageClass[mageClassIndex].spriteFemale, transform.position, Quaternion.identity, transform);
            selectedClass = mageClass[mageClassIndex];
        }

        classText.text = selectedClass.className;
        // set position and scale of playerclass
        playerClass.transform.localScale = new Vector3(20, 20, 0);
        playerClass.transform.localPosition = new Vector3(0, -1f, 0);

        cardDisplay.UpdateSelectedClass(selectedClass);
    }
}
