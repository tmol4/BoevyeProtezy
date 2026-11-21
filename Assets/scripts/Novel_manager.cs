
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Novel_manager : MonoBehaviour
{
    public Image background;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;

    public Sprite[] backgrounds;
    public string[] dialogues;

    private int scene = 0;
    private bool showText = false;

    void Start()
    {
        UpdateScene();
    }

    public void Next()
    {
        if (!showText)
        {
            showText = true;
            dialoguePanel.SetActive(true);
            dialogueText.text = dialogues[scene];
        }
        else
        {
            scene++;

            if (scene >= backgrounds.Length)
                scene = 0;

            showText = false;
            UpdateScene();
        }
    }

    public void Previous()
    {
        if (showText)
        {
            showText = false;
            dialoguePanel.SetActive(false);
        }
        else if (scene > 0)
        {
            scene--;
            UpdateScene();
        }
    }

    void UpdateScene()
    {
        background.sprite = backgrounds[scene];
        dialoguePanel.SetActive(false);
    }
}
