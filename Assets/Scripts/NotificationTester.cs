using System;
using UnityEngine;
public class NotificationTester : MonoBehaviour
{
    [SerializeField] Notification prefab;
    [SerializeField] GameObject parent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Notification notification0 = Instantiate(prefab, parent.transform);
        notification0.Init("press [F] to pay respect", KeyCode.F, 5d, onResult);

        Notification notification1 = Instantiate(prefab, parent.transform);
        notification1.Init("press [G] to say GOIDA", KeyCode.G, 10d, onResult);

        Notification notification2 = Instantiate(prefab, parent.transform);
        notification2.Init("press [P] to say PELMENI", KeyCode.P, 7d, onResult);
    }

    void onResult(bool isButtonPressed)
    {
        if (isButtonPressed)
        {
            Notification notification1 = Instantiate(prefab, parent.transform);
            notification1.Init("press [G] to say GOIDA", KeyCode.G, 10d, onResult);
        }
    }
}
