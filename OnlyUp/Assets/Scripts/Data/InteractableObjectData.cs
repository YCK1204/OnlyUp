using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
public class InteractableObjectData : ScriptableObject
{
    [SerializeField]
    string ObjectName;
    public string Name => ObjectName;
    [SerializeField]
    string Summary;
    public string Description => Summary;
    [SerializeField]
    float EffectValue;
    public float Value => EffectValue;
    [SerializeField]
    bool Usable;
    public bool IsUsable => Usable;

    static string targetFolderPath = "Assets/Resources/Data/InteractableObject";
    [MenuItem("Tools/Create/Data/Interactable")]
    public static void CreateData()
    {
        if (!AssetDatabase.IsValidFolder(targetFolderPath))
        {
            Debug.LogError($"Target folder does not exist: {targetFolderPath}");
            return;
        }

        InteractableObjectData data = CreateInstance<InteractableObjectData>();
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(targetFolderPath + "/New ItemData.asset");

        AssetDatabase.CreateAsset(data, assetPathAndName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = data;
    }
}
#endif