using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public class Tweener : MonoBehaviour
{
    // private Tween activeTween;
    private Tween activeTween = null;
    private Tween[] tweenCycleArray = Array.Empty<Tween>();
    
    private float lerpStartTime;

    // Update is called once per frame
    void Update()
    {
        if (activeTween != null)
        {
            float fractionalLerp = (Time.time - activeTween.StartTime) / activeTween.Duration;
            DoLerp(fractionalLerp, activeTween);
        }
    }

    private void DoLerp(float fractionalLerp, Tween tween)
    {
        float distance = Vector3.Distance(tween.Target.position, tween.EndPos);
        
        if (distance > 0.1f)
        {
            tween.Target.position = Vector3.Lerp(tween.StartPos, tween.EndPos, fractionalLerp);
        }
        else
        {
            tween.Target.position = tween.EndPos;
            activeTween = null;
        }
    }

    public bool IsTweenActive()
    {
        return activeTween != null;
    }
    
    public void SetActiveTween(Tween tween)
    {
        activeTween = tween;
    }
}
