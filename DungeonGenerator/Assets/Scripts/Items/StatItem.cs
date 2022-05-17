using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatItem : Item
{
    protected override void Start()
    {
        base.Start();

        type = ItemTypes.Stat;
    }
}
