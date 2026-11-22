using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
public class NotificationTester : MonoBehaviour
{
     [SerializeField] GameObject canvas;
    [SerializeField] GameObject notificationList;

    [SerializeField] ClickMark markPrefab;
    [SerializeField] Notification notificationPrefab;

    [SerializeField] AudioSource audioSource;

    [SerializeField] string notificationTagsPath;
    [SerializeField] string markTagsPath;


    [SerializeField] Transform a;
    [SerializeField] Transform b;
    [SerializeField] Transform c;
    [SerializeField] Transform d;
    [SerializeField] Transform e;
    [SerializeField] Transform f;

    double musicStartTime;

    Queue<NotificationContent> notificationsQueue = new Queue<NotificationContent>();
    Queue<MarkContent> marksQueue = new Queue<MarkContent>();
    Queue<ClickMark> activeMarksQueue = new Queue<ClickMark>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Notification notification = Instantiate(notificationPrefab, notificationList.transform);
        notification.Init(30, KeyCode.Space, "Начать [ПРОБЕЛ]", StartMusic);
        
        ParseNotificationTags();
        ParseMarkTags();
        
    }
    void ParseNotificationTags()
    {
        foreach (string line in File.ReadLines(notificationTagsPath))
        {
            if (line[0] == '#')
                continue;
            notificationsQueue.Enqueue(NotificationContent.ParseTag(line));
        }
    }
    void ParseMarkTags()
    {
       int meter = 0;
       int beat = 0;
        foreach (string line in File.ReadLines(markTagsPath))
        {
            if (line[0] == '#')
                continue;
            if (line[0] == '>')
            {
                string[] split = line.Split(' ');
                beat = int.Parse(split[1]);
                meter = int.Parse(split[2]);
                continue;
            }
            double clickTime = double.Parse(line);
            marksQueue.Enqueue(new MarkContent(clickTime, beat % meter, meter));
            beat++;
            //Debug.Log(clickTime + " " + beat % meter + " " + meter);
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

        while (notificationsQueue.Count != 0 && notificationsQueue.Peek().startTime <= AudioSettings.dspTime - musicStartTime)
        {
            //Debug.Log((AudioSettings.dspTime - musicStartTime).ToString());
            Notification notification = Instantiate(notificationPrefab, notificationList.transform);
            NotificationContent content = notificationsQueue.Dequeue();
            notification.Init(content.endTime - content.startTime, content.key, content.message, Void);
        }

        while (marksQueue.Count != 0 && marksQueue.Peek().clickTime <= AudioSettings.dspTime - musicStartTime + 3)
        {
            MarkContent content = marksQueue.Dequeue();

            Transform parentTransform = null;
            switch (content.meter)
            {
                case 4:
                    switch (content.beat)
                    {
                        case 0:
                            parentTransform = b;
                            break;
                        case 1:
                            parentTransform = d;
                            break;
                        case 2:
                            parentTransform = e;
                            break;
                        case 3:
                            parentTransform = a;
                            break;
                    }
                    break;
                case 6:
                    switch (content.beat)
                    {
                        case 0:
                            parentTransform = b;
                            break;
                        case 1:
                            parentTransform = c;
                            break;
                        case 2:
                            parentTransform = d;
                            break;
                        case 3:
                            parentTransform = e;
                            break;
                        case 4:
                            parentTransform = f;
                            break;
                        case 5:
                            parentTransform = a;
                            break;
                    }
                    break;
                }
                ClickMark mark = Instantiate(markPrefab, parentTransform.transform);
                mark.Init(musicStartTime + content.clickTime);
                activeMarksQueue.Enqueue(mark);
            }

            if (activeMarksQueue.Count > 0 && Input.GetMouseButtonDown(0))
            {

            while (activeMarksQueue.Count > 0 && activeMarksQueue.Peek() == null)
            {
                activeMarksQueue.Dequeue();
            }

            if (activeMarksQueue.Count == 0) return;

            ClickMark targetMark = activeMarksQueue.Peek();

            // 2. Создаем данные о текущем положении курсора для UI-системы
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            // 3. Пускаем UI-луч по всем элементам Canvas под курсором
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            bool isHitted = false;

            foreach (RaycastResult result in results)
            {
               // Проверяем, принадлежит ли UI-элемент нашей целевой метке (учитывая дочерние картинки/текст)
               if (result.gameObject.GetComponentInParent<ClickMark>() == targetMark)
               {
                    isHitted = true;
                    Debug.Log("hitted UI mark!");

                    activeMarksQueue.Dequeue();
                    Destroy(targetMark.gameObject);
                    break;
                }
            }

            if (!isHitted)
            {
                Debug.Log("missed UI mark!");
            }
        }
    }
    void Void(bool isButtonPressed)
    {
        
    }
}
