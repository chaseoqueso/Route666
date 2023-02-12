using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cutscene : MonoBehaviour
{
    // Frames
    public List<Sprite> frames;
    public List<float> timings;
    public Image target;
    [SerializeField]
    private bool isRunning;
    [SerializeField]
    private bool isFinished;
    [SerializeField]
    private float timer;
    [SerializeField]
    private int cutsceneIndex;

    // Start is called before the first frame update
    void Start()
    {
        BeginCutscene();
    }

    void BeginCutscene()
    {
        Time.timeScale = 1f;
        isRunning = true;
        isFinished = false;
        timer = 0;
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
            else if (timer >= timings[cutsceneIndex])
            {
                // advance frame and reset timer
                cutsceneIndex++;
                target.sprite = frames[cutsceneIndex];
                timer = 0;
            }
            // nah, keep displaying same frame and increment timer
            else
            {
                timer += Time.deltaTime;
            }
        }
    }
}
