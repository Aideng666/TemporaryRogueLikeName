using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPIncreaseItem : StatItem
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    public override void ItemEffect()
    {
        base.ItemEffect();

        player.ModifyMaxHealth(2);
        player.Heal(2);

        print("Effect Applied");
    }
}
