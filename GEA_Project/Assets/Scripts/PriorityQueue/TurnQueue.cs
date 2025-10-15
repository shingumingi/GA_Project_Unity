using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnQueue : MonoBehaviour
{
    private TurnPriorityQueue<string> queue = new TurnPriorityQueue<string>();
    private Dictionary<string, float> unitSpeeds = new Dictionary<string, float>();
    private Dictionary<string, float> nextActionTime = new Dictionary<string, float>();
    private float currentTime = 0f;
    private int turnCount = 1;

    void Start()
    {
        unitSpeeds["전사"] = 5;
        unitSpeeds["마법사"] = 7;
        unitSpeeds["궁수"] = 10;
        unitSpeeds["도적"] = 12;

        foreach (var unit in unitSpeeds.Keys)
        {
            nextActionTime[unit] = 0f;
            queue.Enqueue(unit, 0f);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (queue.Count == 0) return;

            string currentUnit = queue.Dequeue();
            Debug.Log($"{turnCount++}턴 / {currentUnit}의 턴입니다.");

            float speed = unitSpeeds[currentUnit];
            float cooldown = 100f / speed;

            nextActionTime[currentUnit] += cooldown;
            queue.Enqueue(currentUnit, nextActionTime[currentUnit]);
        }
    }
}
