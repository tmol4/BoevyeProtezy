using UnityEngine;

public class UIFollowHalfSpeed : MonoBehaviour
{
    [SerializeField] private float speedMultiplier = 0.5f; // 0.5 = в 2 раза медленнее
    private RectTransform rectTransform;
    private Vector2 initialPosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        // Запоминаем начальную позицию объекта на Canvas
        initialPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        // Находим позицию мыши относительно центра экрана
        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 mouseOffset = (Vector2)Input.mousePosition - screenCenter;

        // Применяем смещение, умноженное на 0.5
        rectTransform.anchoredPosition = initialPosition + (mouseOffset * speedMultiplier);
    }
}