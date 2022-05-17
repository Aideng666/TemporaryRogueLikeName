using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] protected List<ItemInfo> possibleItems = new List<ItemInfo>();

    ItemInfo selectedItem;

    private void Start()
    {
        ChooseRandomItem();
    }

    void ChooseRandomItem()
    {
        int randomIndex = Random.Range(0, possibleItems.Count);

        selectedItem = possibleItems[randomIndex];

        selectedItem.SetItem();

        Instantiate(selectedItem.itemModel, new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), Quaternion.identity, transform);
    }

    public void GiveItem()
    {
        PlayerController.Instance.AddItem(selectedItem.item.GetComponent<Item>());

        Destroy(gameObject);
    }
}
