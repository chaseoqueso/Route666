using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cutscene : MonoBehaviour
{
    // Frames
    public List<Sprite> frames;
    public List<int> timings;
    public Image target;
    public bool isRunning;
    private bool isFinished;
    private int timer;
    private int index;

    // Start is called before the first frame update
    void Start()
    {
        isRunning = false;
        isFinished = false;
        timer = 0;
        index = 0;
    }

    void BeginCutscene()
    {
        isRunning = true;
        index = 0;
        target.sprite = frames[0];
        target.preserveAspect = true;
    }

    void EndCutscene()
    {
        isRunning = false;
        isFinished = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRunning && !isFinished)
        {
            BeginCutscene();
        }
        if (isRunning)
        {
            // are we at the end of the cutscene?
            if (index + 1 > frames.Count)
            {
                EndCutscene();
            }

            // are we done with this frame? 
            if (timer >= timings[index] / Time.deltaTime)
            {
                // advance frame and reset timer
                index++;
                target.sprite = frames[index];
                timer = 0;
            }
            // nah, keep displaying same frame and increment timer
            else
            {
                timer++;
            }
        }
    }
}
