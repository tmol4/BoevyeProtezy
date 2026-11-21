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
    public void Init(string message, double timerTime)
    {
        endTime = AudioSettings.dspTime + timerTime;
        this.timerTime = timerTime;
        Text.text = message;
    }

    // Update is called once per frame
    void Update()
    {
        double time = AudioSettings.dspTime;
        double remainingTime = endTime - time;
        if (remainingTime <= 0)
            Destroy(gameObject);
      
        Bar.fillAmount = (float)(remainingTime / timerTime);
    }
}
