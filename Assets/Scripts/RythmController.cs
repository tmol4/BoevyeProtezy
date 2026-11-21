using System;
using UnityEngine;
public class NotificationTester : MonoBehaviour
{
    [SerializeField] Notification prefab;
    [SerializeField] GameObject parent;
    [SerializeField] AudioSource audioSource;

    double musicStartTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Notification notification1 = Instantiate(prefab, parent.transform);
        notification1.Init("Вступление [S]", KeyCode.S, 1e9d, StartMusic);

        
    }

    void StartMusic(bool isButtonPressed)
    {
        audioSource.Play();
        musicStartTime = AudioSettings.dspTime;
    }
}
