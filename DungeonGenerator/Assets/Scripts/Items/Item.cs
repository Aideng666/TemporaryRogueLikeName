using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item : MonoBehaviour
{
    protected ItemTypes type;
    protected PlayerController player;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        player = PlayerController.Instance;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    public virtual void ItemEffect()
    {

    }

    public ItemTypes GetItemType()
    {
        return type;
    }
}
