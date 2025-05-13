using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;

public class TownPartySystem : MonoBehaviour
{
    public static TownPartySystem Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    [SerializeField] private List<CharacterData> party;
    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private ClassDatabase classDatabase;
    [SerializeField] private GameObject spriteHolder;

    private void Start()
    {
        party = PlayerDataHolder.Instance.partyMembers;
        GenerateCharacterSprite();
    }

    private void GenerateCharacterSprite()
    {
        int position = 0;

        foreach (CharacterData member in party)
        {
            string targetClassName = member.classes[0];
            string playerGender = member.basicInfo.gender.ToLower();

            ClassDataSO selectedClass = classDatabase.allClasses.Find(c => c.className == targetClassName);
            GameObject characterObj = null;

            if (playerGender == "male")
            {
                characterObj = Instantiate(selectedClass.spriteMale, Vector3.zero, Quaternion.identity, spriteHolder.transform);
            }
            else if (playerGender == "female")
            {
                characterObj = Instantiate(selectedClass.spriteFemale, Vector3.zero, Quaternion.identity, spriteHolder.transform);
            }

            characterObj.transform.localPosition = new Vector3(position, 0, 0);

            position += 1;
        }
    }
}
