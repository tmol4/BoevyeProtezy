using System;
using UnityEngine;

public class ClickMark : MonoBehaviour
{
    [SerializeField] GameObject mainCircle;
    [SerializeField] Transform approachCircleTransform;

    double duration;
    double hitTime;
    double startScale;

    public void Init(double hitTime)
    {
        this.duration = hitTime - AudioSettings.dspTime;
        this.hitTime = hitTime;
        startScale = (double)approachCircleTransform.localScale.x;
    }
    public double DeltaTime()
    {
        return hitTime - AudioSettings.dspTime;  
    }

    void Update()
    {
        double remainingTime = hitTime - AudioSettings.dspTime;  
        float scale = Mathf.LerpUnclamped(1, (float)startScale, (float)(remainingTime / duration));
        if (scale <= 0)
            Destroy(gameObject);
        approachCircleTransform.transform.localScale = new Vector3(scale, scale, 1f);
    }
}
