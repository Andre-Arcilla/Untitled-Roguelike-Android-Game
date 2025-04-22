using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TownSelection : MonoBehaviour
{
    public void BackBtn()
    {
        SceneManager.LoadScene("Character Selection");
    }
}
