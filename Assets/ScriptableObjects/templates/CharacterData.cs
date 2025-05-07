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
        //public Target faction;
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
        [Header("Equipment")]
        [Tooltip("Helmet or headwear")]
        public string headGear;

        [Tooltip("Chest gear")]
        public string chestArmor;

        [Tooltip("Pants or leg protection")]
        public string legwear;

        [Tooltip("Gauntlets")]
        public string gloves;

        [Tooltip("Boots or footwear")]
        public string boots;

        [Tooltip("Primary weapon")]
        public string mainHand;

        [Tooltip("Secondary weapon or shield")]
        public string offHand;

        [Tooltip("First accessory (e.g., ring, amulet)")]
        public string accessory1;

        [Tooltip("Second accessory (e.g., ring, necklace)")]
        public string accessory2;

        [Tooltip("Third accessory (e.g., bracelet, charm)")]
        public string accessory3;
    }

    public bool isAlive;
    public BasicInfo basicInfo = new BasicInfo();
    public AllocatedStats allocatedStats = new AllocatedStats();
    public List<string> classes = new List<string>();
    public Equipment equipment = new Equipment();
}