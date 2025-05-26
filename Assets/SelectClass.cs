using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectClass : MonoBehaviour
{
    public static SelectClass Instance { get; private set; }

    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Only one allowed
            return;
        }

        Instance = this;
    }
    //a select class method that detects what is chosen between the classchoicedisplays
    [SerializeField] private Button class1;
    [SerializeField] private Button class2;
    [SerializeField] private Button class3;
    private ClassDataSO classData;

    public void SetClassChoice(ClassDataSO classData)
    {
        this.classData = classData;
    }

    public void ButtonAction()
    {
        PartyMenuManager.Instance.AddSelectedClass(classData);
    }
}
