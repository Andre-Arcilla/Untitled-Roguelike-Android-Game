using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    public string Name;
    public string Race;
    public int Level;
    public List<string> Class { get; private set; }

    //key = subclass, value = rank
    public Dictionary<string, int> Subclasses { get; private set; }

    //stats
    public int HP;
    public int EN;
    public int SPD;
    public int PWR;


    //cards the character has
    public List<string> CharacterDeck { get; private set; }

    public CharacterInfo(string name, string race)
    {
        Name = name;
        Race = race;
        Level = 1;
        Class = new List<string>();
        Subclasses = new Dictionary<string, int>();

        HP = 20;
        EN = 20;
        SPD = 20;
        PWR = 20;

        CharacterDeck = new List<string>();
    }

    public void AddClass(string className)
    {
        if (!Class.Contains(className))
            Class.Add(className);
    }

    public void AddSubclass(string subclassName, int rank = 1)
    {
        if (!Subclasses.ContainsKey(subclassName))
            Subclasses[subclassName] = rank;
    }
    public void LvlUp()
    {
        Level++;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

