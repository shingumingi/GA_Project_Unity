using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Stack : MonoBehaviour
{
    public float speed = 5f;

    private Stack<Vector3> moveHistory;
    private bool isbacking = false;

    // Start is called before the first frame update
    void Start()
    {
        moveHistory = new Stack<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isbacking)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");

            if (x != 0 || z != 0)
            {
                moveHistory.Push(transform.position);

                Vector3 move = new Vector3(x, 0, z).normalized * speed * Time.deltaTime;
                transform.position += move;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(GoBack());
        }
    }

    IEnumerator GoBack()
    {
        while (moveHistory.Count > 0)
        {
            transform.position = moveHistory.Pop();
            yield return new WaitForSeconds(0.01f);
        }
        isbacking = false;
    }
}
