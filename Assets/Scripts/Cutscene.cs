using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CutsceneFrame{
    [Tooltip("The frame image sprite")]
    [SerializeField] private Sprite frameSprite;

    [Tooltip("The time (in seconds) this frame is shown")]
    [SerializeField] private float timing;

    public Sprite FrameSprite(){return frameSprite;}
    public float Timing(){return timing;}
}

public class Cutscene : MonoBehaviour
{
    // Frames
    [SerializeField] private List<CutsceneFrame> cutsceneFrames = new List<CutsceneFrame>();
    // public List<Sprite> frames;
    // public List<float> timings;
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
        target.sprite = cutsceneFrames[0].FrameSprite();
        target.preserveAspect = true;
    }

    public void EndCutscene()
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
            if (cutsceneIndex + 1 > cutsceneFrames.Count - 1)
            {
                EndCutscene();
            }

            // are we done with this frame? 
            else if (timer >= cutsceneFrames[cutsceneIndex].Timing())
            {
                // advance frame and reset timer
                cutsceneIndex++;
                target.sprite = cutsceneFrames[cutsceneIndex].FrameSprite();
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
