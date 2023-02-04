using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationWatcher : MonoBehaviour
{
    public UnityEvent onAnimationTrigger;

    public void triggerEvents()
    {
        onAnimationTrigger.Invoke();
    }
}
