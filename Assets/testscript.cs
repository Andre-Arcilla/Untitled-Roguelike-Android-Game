using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static CharacterSelection;

public class testscript : MonoBehaviour
{
    private PlayerDataHolder playerData => PlayerDataHolder.Instance;

    public void OnMouseDown()
    {
        Debug.Log("clicked");
    }

    public void aaaaaaaaa()
    {
        Debug.Log("clicked");
    }
    
    public void ChangeScene1()
    {
        SceneManager.LoadScene("Town 1");
    }
    
    public void ChangeScene2()
    {
        SceneManager.LoadScene("Town Selection");
    }

    public void ShowDataTest()
    {
        Debug.Log(playerData.playerData.basicInfo.playerName);
    }

    public void SaveData()
    {
        playerData.SavePlayerInfo();
    }
}
