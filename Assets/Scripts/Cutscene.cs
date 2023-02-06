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
    private int cutsceneIndex;

    // Start is called before the first frame update
    void Start()
    {
        isRunning = false;
        isFinished = false;
        timer = 0;
        cutsceneIndex = 0;
        BeginCutscene();
    }

    void BeginCutscene()
    {
        isRunning = true;
        target.sprite = frames[0];
        target.preserveAspect = true;
    }

    void EndCutscene()
    {
        isRunning = false;
        isFinished = true;
        cutsceneIndex = 0;
        GameManager.instance.ChangeScene(GameManager.LEVEL_1_SCENE_NAME);
    }

    // Update is called once per frame
    void Update()
    {
        if (isRunning && !isFinished)
        {
            // are we at the end of the cutscene?
            if (cutsceneIndex + 1 > frames.Count - 1)
            {
                EndCutscene();
            }

            // are we done with this frame? 
            else if (timer >= timings[cutsceneIndex] / Time.deltaTime)
            {
                // advance frame and reset timer
                cutsceneIndex++;
                target.sprite = frames[cutsceneIndex];
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
