using UnityEngine;
using Player.Controller;


#if UNITY_EDITOR
using UnityEditor;
[CreateAssetMenu(fileName = "DefaultData", menuName = "ScriptableObjects/Interactables/Default", order = 1)]
public class InteractableObjectData : ScriptableObject
{
    [SerializeField]
    string ObjectName;
    public string Name => ObjectName;
    [SerializeField]
    string Summary;
    public string Description => Summary;
    [SerializeField]
    bool Usable;
    public bool IsUsable => Usable;
}
#endif