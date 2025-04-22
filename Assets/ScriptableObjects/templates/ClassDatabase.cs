using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ClassDatabase", menuName = "Database/Class Database")]
public class ClassDatabase : ScriptableObject
{
    public List<ClassDataSO> allClasses;
}
