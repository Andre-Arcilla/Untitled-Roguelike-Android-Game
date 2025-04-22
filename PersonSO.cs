using UnityEngine;

[CreateAssetMenu(fileName = "NewPerson", menuName = "Person")]
public class PersonSO : ScriptableObject
{
    public string personName;
    public int age;
}