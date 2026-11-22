using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour
{
    [SerializeField] TMP_Text Text;
    [SerializeField] Image Bar;
    double duration;
    double endTime;
    KeyCode key;

    private event Action<bool> onResult;
    public void Init(double duration, KeyCode key, string message, Action<bool> action)
    {
        this.duration = duration;
        this.endTime = AudioSettings.dspTime + duration;
        this.key = key;
        Text.text = message;
        onResult += action;
    }

    // Update is called once per frame
    void Update()
    {
        double remainingTime = endTime - AudioSettings.dspTime;
        if (remainingTime <= 0)
        {
            onResult(false);
            Destroy(gameObject);
        }
        if (Input.GetKeyDown(key))
        {
            onResult(true);
            Destroy(gameObject);
        }
        Bar.fillAmount = (float)(remainingTime / duration);
    }
}
