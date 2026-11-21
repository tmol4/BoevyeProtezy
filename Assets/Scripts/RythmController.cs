using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
public class NotificationTester : MonoBehaviour
{
    [SerializeField] Notification prefab;
    [SerializeField] GameObject parent;
    [SerializeField] AudioSource audioSource;
    [SerializeField] string tagPath;

    double musicStartTime;

    Queue<NotificationContent> queue = new Queue<NotificationContent>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Notification notification = Instantiate(prefab, parent.transform);
        notification.Init(30, KeyCode.Space, "Начать [ПРОБЕЛ]", StartMusic);
        
        foreach (string line in File.ReadLines(tagPath))
        {
            if (line[0] == '#')
                continue;
            string[] split = line.Split('\t');
            queue.Enqueue(NotificationContent.ParseTag(line));
        }
    }

    void StartMusic(bool isButtonPressed)
    {
        audioSource.Play();
        musicStartTime = AudioSettings.dspTime;
    }

    void Update()
    {
        if (!audioSource.isPlaying)
            return;

        while (queue.Count != 0 && queue.Peek().startTime <= AudioSettings.dspTime - musicStartTime)
        {
            Debug.Log((AudioSettings.dspTime - musicStartTime).ToString());
            Notification notification = Instantiate(prefab, parent.transform);
            NotificationContent content = queue.Dequeue();
            notification.Init(content.endTime - content.startTime, content.key, content.message, Void);
        }
    }
    void Void(bool isButtonPressed)
    {
        
    }
}
