using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;
    private List<CharacterInfo> characters = new List<CharacterInfo>();

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterCharacter(CharacterInfo character)
    {
        characters.Add(character);
    }

    public List<CharacterInfo> GetCharacters()
    {
        return characters;
    }
}
