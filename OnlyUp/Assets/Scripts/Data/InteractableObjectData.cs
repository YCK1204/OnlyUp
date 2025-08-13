using UnityEngine;
using Player.Controller;


#if UNITY_EDITOR
using UnityEditor;
public abstract class InteractableObjectData : ScriptableObject
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
    public abstract void Use(PlayerController player);
}
#endif