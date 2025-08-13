using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionObject : InteractableObject
{
    PotionData _data;
    public override void Use()
    {
        if (Data.IsUsable == false)
            return;
        var player = Manager.Game.Player;
        player.UsePotion(_data);
        Manager.UI.HideObjectSummary();
        Destroy(gameObject);
    }
    protected override void Init()
    {
        base.Init();
        _data = Data as PotionData;
    }
}
