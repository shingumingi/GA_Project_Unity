using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public int range = 100;
    public Text text;

    Stopwatch sw = new Stopwatch();
    int[] GenerateRnadomArray(int size)
    {
        int[] arr = new int[size];
        System.Random rand = new System.Random();
        for (int i = 0; i < size; i++)
        {
            arr[i] = rand.Next(0, 10000);
        }
        return arr;
    }
    public void ClickSelection()
    {
        sw.Reset();
        sw.Start();
        int[] data = GenerateRnadomArray(range);
        StartSelectionSort(data);
        sw.Stop();
        long selectionTime = sw.ElapsedMilliseconds;
        
        text.text = $"Selection : \n{selectionTime:F2}ms";
        
    }

    public void ClickBubble()
    {
        sw.Reset();
        sw.Start();
        int[] data = GenerateRnadomArray(range);
        StartBubbleSort(data);
        sw.Stop();
        long selectionTime = sw.ElapsedMilliseconds;
        
        text.text = $"Bubble : \n{selectionTime:F2}ms";

    }

    public void ClickQuick()
    {
        sw.Reset();
        sw.Start();
        int[] data = GenerateRnadomArray(range);
        StartQuickSort(data, 0, data.Length - 1);
        sw.Stop();
        long selectionTime = sw.ElapsedMilliseconds;
        
        text.text = $"Quick : \n{selectionTime:F2}ms";

    }

    // Selection
    public static void StartSelectionSort(int[] arr)
    {
        int n = arr.Length;
        for (int i = 0; i < n - 1; i++)
        {
            int minIndex = i;
            for (int j = i + 1; j < n; j++)
            {
                if (arr[j] < arr[minIndex])
                {
                    minIndex = j;
                }
            }
            int temp = arr[minIndex];
            arr[minIndex] = arr[i];
            arr[i] = temp;
        }
    }

    // Bubble
    public static void StartBubbleSort(int[] arr)
    {
        int n = arr.Length;
        for (int i = 0; i < n - 1; i++)
        {
            bool swapped = false;
            for (int j = 0; j < n - i - 1; j++)
            {
                if (arr[j] > arr[j + 1])
                {
                    int temp = arr[j];
                    arr[j] = arr[j + 1];
                    arr[j + 1] = temp;
                    swapped = true;
                }
            }

            if (!swapped) break;
        }
    }

    // Quick
    public static void StartQuickSort(int[] arr, int low, int high)
    {
        if (low < high)
        {
            int pivotIndex = Partition(arr, low, high);

            StartQuickSort(arr, low, pivotIndex - 1);
            StartQuickSort(arr, pivotIndex + 1, high);
        }
    }
    private static int Partition(int[] arr, int low, int high)
    {
        int pivot = arr[high];
        int i = (low - 1);

        for (int j = low; j < high; j++)
        {
            if (arr[j] <= pivot)
            {
                i++;
                int temp = arr[i];
                arr[i] = arr[j];
                arr[j] = temp;
            }
        }
        int temp2 = arr[i + 1];
        arr[i + 1] = arr[high];
        arr[high] = temp2;

        return i + 1;
    }
}
