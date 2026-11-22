using UnityEngine;

[ExecuteInEditMode]
public class Pixelate : MonoBehaviour
{
    [Range(1, 20)]
    public int pixelSize = 4; // Размер пикселя (чем больше, тем крупнее пиксели)

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        // Вычисляем уменьшенное разрешение
        int width = src.width / pixelSize;
        int height = src.height / pixelSize;

        // Создаем временную текстуру и отключаем сглаживание (Point)
        RenderTexture temp = RenderTexture.GetTemporary(width, height, 0);
        temp.filterMode = FilterMode.Point;

        // Сжимаем картинку, а затем растягиваем обратно на весь экран
        Graphics.Blit(src, temp);
        Graphics.Blit(temp, dest);

        RenderTexture.ReleaseTemporary(temp);
    }
}
