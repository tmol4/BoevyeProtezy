using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour
{
    [SerializeField] TMP_Text Text;
    [SerializeField] Image Bar;

    private double endTime;
    private double timerTime;
    private KeyCode key;

    private event Action<bool> onResult;
    public void Init(string message, KeyCode key, double timerTime, Action<bool> action)
    {
        endTime = AudioSettings.dspTime + timerTime;
        this.timerTime = timerTime;
        this.key = key;
        Text.text = message;
        onResult += action;
    }

    // Update is called once per frame
    void Update()
    {
        double time = AudioSettings.dspTime;
        double remainingTime = endTime - time;
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
        Bar.fillAmount = (float)(remainingTime / timerTime);
    }
}
