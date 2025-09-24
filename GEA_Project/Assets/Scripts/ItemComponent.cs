using TMPro;
using UnityEngine;

public class ItemComponent : MonoBehaviour
{
    public string itemName;
    public TMP_Text itemText;

    public void SetName(string name)
    {
        itemName = name;
        if (itemText != null)
            itemText.text = name;
    }
}