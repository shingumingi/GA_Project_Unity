using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecursiveTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CountDown(10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CountDown(int i)
    {
        if (i == 0) return;
        Debug.Log("Count : " + i);
        CountDown(i - 1);
    }
}
