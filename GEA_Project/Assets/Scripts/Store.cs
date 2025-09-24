using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.Rendering;

public class Store : MonoBehaviour
{
    [Header("UI References (TMP)")]
    public TMP_InputField findItem;

    public GameObject itemPrefabs;
    public GameObject content;

    private List<ItemComponent> items = new List<ItemComponent>();

    private void Start()
    {
        for (int i = 0; i < 100; i++)
        {
            itemPrefabs.transform.GetChild(0).GetComponent<TMP_Text>().text = $"Item_{i:D2}";

            GameObject go = Instantiate(itemPrefabs, content.transform);
            ItemComponent item = go.GetComponent<ItemComponent>();
            item.SetName($"Item_{i:D2}");

            items.Add(item);
        }
    }

    public void OnLinearButton()
    {
        string target = findItem.text.Trim();
        int steps = FindItemLinearSteps(target);

        ShowOnlyItem(target);
    }

    public void OnBinaryButton()
    {
        string target = findItem.text.Trim();

        items.Sort((a, b) => a.itemName.CompareTo(b.itemName));

        int steps = FindItemBinarySteps(target);

        ShowOnlyItem(target);
    }

    private void ShowOnlyItem(string targetName)
    {
        foreach (ItemComponent item in items)
        {
            bool match = item.itemName.Equals(targetName, System.StringComparison.OrdinalIgnoreCase);
            item.gameObject.SetActive(match);
        }
    }

    private void QuickSort(List<Item> list, int left, int right)
    {
        if (left >= right) return;

        int pivotIndex = Partition(list, left, right);
        QuickSort(list, left, pivotIndex - 1);
        QuickSort(list, pivotIndex + 1, right);
    }

    private int Partition(List<Item> list, int left, int right)
    {
        Item pivot = list[right];
        int i = left - 1;

        for (int j = left; j < right; j++)
        {
            if (list[j].itemName.CompareTo(pivot.itemName) <= 0)
            {
                i++;
                Swap(list, i, j);
            }
        }
        Swap(list, i + 1, right);
        return i + 1;
    }

    private void Swap(List<Item> list, int a, int b)
    {
        Item temp = list[a];
        list[a] = list[b];
        list[b] = temp;
    }
    private int FindItemLinearSteps(string target)
    {
        int steps = 0;
        foreach (ItemComponent item in items)
        {
            steps++;
            if (item.itemName == target)
                return steps;
        }
        return steps;
    }

    private int FindItemBinarySteps(string target)
    {
        int steps = 0;
        int left = 0, right = items.Count - 1;

        while (left <= right)
        {
            steps++;
            int mid = (left + right) / 2;
            int cmp = items[mid].itemName.CompareTo(target);

            if (cmp == 0) return steps;
            else if (cmp < 0) left = mid + 1;
            else right = mid - 1;
        }
        return steps;
    }
}
