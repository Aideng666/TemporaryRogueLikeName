using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Item", menuName = "Item Information")]
public class ItemInfo : ScriptableObject
{
    public GameObject itemModel;
    public int itemNum;

    public GameObject item;

    private void Awake()
    {
        item = ItemPool.Instance.GetItemFromPool(itemNum);
    }

    public void SetItem()
    {
        item = ItemPool.Instance.GetItemFromPool(itemNum);
    }
}
