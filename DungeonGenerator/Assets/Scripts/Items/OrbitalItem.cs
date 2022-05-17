using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalItem : Item
{
    [SerializeField] GameObject orbitingObject;

    protected override void Start()
    {
        base.Start();

        type = ItemTypes.Orbital;
    }
}
