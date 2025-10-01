using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queue : MonoBehaviour
{
    float speed = 5f;
    Queue<Vector3> moveQueue;
    bool isMoving = false;
    Vector3 targetPos;
    // Start is called before the first frame update
    void Start()
    {
        moveQueue = new Queue<Vector3>();
        targetPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        if (!isMoving)
        {
            if (x != 0 || z != 0)
            {
                Vector3 move = new Vector3(x, 0, z).normalized * speed * Time.deltaTime;
                transform.position += move;
                moveQueue.Enqueue(targetPos);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!isMoving && moveQueue.Count > 0)
                {
                    isMoving = true;
                }
            }
        }
        else
        {
            if(moveQueue.Count > 0)
            {
                transform.position = moveQueue.Dequeue();
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }
        }
    }
}
