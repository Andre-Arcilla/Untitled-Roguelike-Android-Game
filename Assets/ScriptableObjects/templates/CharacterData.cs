using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    [System.Serializable]
    public class BasicInfo
    {
        [Header("Basic Info")]
        public string characterName;
        public int level;
        public string gender;
        public string raceName;
    }

    [System.Serializable]
    public class AllocatedStats
    {
        [Header("Allocated Stats")]
        public int allocatedHP;
        public int allocatedEN;
        public int allocatedPWR;
        public int allocatedSPD;
    }

    [System.Serializable]
    public class Equipment
    {
        [Tooltip("Equipped armor set or piece providing overall defense")]
        public string armor;

        [Tooltip("Equipped main weapon (e.g., sword, staff, bow)")]
        public string weapon;

        [Tooltip("First equipped accessory (e.g., ring or amulet)")]
        public string accessory1;

        [Tooltip("Second equipped accessory (e.g., necklace or charm)")]
        public string accessory2;

        [Tooltip("Third equipped accessory (e.g., bracelet or talisman)")]
        public string accessory3;
    }


    public bool isAlive;
    public BasicInfo basicInfo = new BasicInfo();
    public AllocatedStats allocatedStats = new AllocatedStats();
    public List<string> classes = new List<string>();
    public Equipment equipment = new Equipment();
}