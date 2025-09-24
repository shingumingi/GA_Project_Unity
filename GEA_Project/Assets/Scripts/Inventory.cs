using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Item> items = new List<Item>(); 

    public string item_Name = string.Empty;

    void Start()
    {
        items.Add(new Item("Sword"));
        items.Add(new Item("Shield"));
        items.Add(new Item("Potion"));
        items.Add(new Item("Gun"));

        Item found = FindItem(item_Name);

        if (found != null)
            Debug.Log("찾은 아이템 : " + found.itemName);
        else
            Debug.Log("아이템을 찾을 수 없습니다.");
    }

    public Item FindItem(string _itemName)
    {
        foreach (var item in items)
        {
            if (item.itemName == _itemName)
                return item;
        }
        return null;
    }
}
