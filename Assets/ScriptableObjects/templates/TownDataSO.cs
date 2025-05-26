using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTown", menuName = "TownData")]
public class TownDataSO : ScriptableObject
{
    [Header("Town Information")]
    public string townName;

#if UNITY_EDITOR
    public SceneAsset sceneAsset;
#endif

    public string sceneName;

    public bool labyrinthCleared;

    [Header("Connected Towns")]
    public List<TownDataSO> connectedTowns;

#if UNITY_EDITOR
    // This automatically updates the sceneName field when edited
    private void OnValidate()
    {
        if (sceneAsset != null)
        {
            string path = AssetDatabase.GetAssetPath(sceneAsset);
            sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
        }
    }
#endif
}