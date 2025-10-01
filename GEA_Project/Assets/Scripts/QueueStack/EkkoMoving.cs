using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EkkoMoving : MonoBehaviour
{
    public float speed = 10f;

    private List<Vector3> inputRecord = new List<Vector3>();
    private Queue<(Vector3 move, bool isRewind)> commandQueue = new Queue<(Vector3, bool)>();

    bool isRecording = true;
    bool isPlaying = false;
    private int rewindStartIndex = 0;

    private Renderer rend;
    public Material normalMaterial;
    public Material rewindMaterial;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        if (normalMaterial == null)
            normalMaterial = rend.material;

        rend.material = normalMaterial;
    }

    void Update()
    {
        if (isRecording && !isPlaying)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");

            if (x != 0 || z != 0)
            {
                Vector3 move = new Vector3(x, 0, z).normalized * speed * Time.deltaTime;
                inputRecord.Add(move);
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && !isPlaying)
        {
            RewindInputRecord();
            rend.material = normalMaterial;
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isPlaying)
        {
            PreparePlayback();
            StartCoroutine(PlayerMoving());
        }
    }

    void RewindInputRecord()
    {
        float rewindTime = 2f;
        int rewindFrames = Mathf.RoundToInt(rewindTime / Time.deltaTime);

        int count = inputRecord.Count;
        int startIndex = Mathf.Max(0, count - rewindFrames);

        List<Vector3> rewindMoves = new List<Vector3>();

        for (int i = count - 1; i >= startIndex; i--)
        {
            rewindMoves.Add(-inputRecord[i]);
        }

        rewindStartIndex = inputRecord.Count;

        inputRecord.AddRange(rewindMoves);
    }

    void PreparePlayback()
    {
        rend.material = normalMaterial;

        isPlaying = true;
        isRecording = false;

        commandQueue.Clear();

        for (int i = 0; i < inputRecord.Count; i++)
        {
            bool isRewind = (i >= rewindStartIndex);
            commandQueue.Enqueue((inputRecord[i], isRewind));
        }
    }

    IEnumerator PlayerMoving()
    {
        bool currentRewindState = false;
        rend.material = normalMaterial;

        while (commandQueue.Count > 0)
        {
            var (move, isRewind) = commandQueue.Dequeue();

            if (isRewind != currentRewindState)
            {
                rend.material = isRewind ? rewindMaterial : normalMaterial;
                currentRewindState = isRewind;
            }

            transform.position += move;

            if (commandQueue.Count == 0 && currentRewindState == true)
            {
                rend.material = normalMaterial;
                yield return null;
                break; 
            }

            yield return null;
        }

        rend.material = normalMaterial;

        isPlaying = false;
        isRecording = true;
        inputRecord.Clear();
        rewindStartIndex = 0;
    }
}