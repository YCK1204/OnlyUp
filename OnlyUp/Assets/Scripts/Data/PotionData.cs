using UnityEngine;
using Player.Controller;


#if UNITY_EDITOR
using UnityEditor;
public enum PotionType
{
    Health,
    Stamina
}
[CreateAssetMenu(fileName = "PotionData", menuName = "ScriptableObjects/Interactables/PotionData", order = 1)]
public class PotionData : InteractableObjectData
{
    [SerializeField]
    private float healAmount;
    public float HealAmount => healAmount;
    [SerializeField]
    private float duration;
    public float Duration => duration;
    [SerializeField]
    float interval;
    public float Interval => interval;
    [SerializeField]
    private PotionType potionType;
    public PotionType Type => potionType;
    public override void Use(PlayerController player)
    {
        player.UsePotion(this);
    }
}
#endif