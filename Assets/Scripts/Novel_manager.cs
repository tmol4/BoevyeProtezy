using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Novel_manager : MonoBehaviour
{
    public Image background;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public TMP_Text perexod;
    public Sprite[] backgrounds;
    public string[] dialogues;

    [Header("Выбор персонажа (Люда / Марта)")]
    public GameObject choicePanel; 
    private bool isChoiceActive = false;
    private string playerChoice = "";
    private int choicePhase = 1;

    [Header("Выбор реплик в диалоге")]
    public GameObject dialogueChoicePanel; 
    public TMP_Text option1Text;           
    public TMP_Text option2Text;           
    
    // Переменные для ведения комбинаций выборов Люды (П1)
    private int ludaFirstChoice = 0;  // 1 или 2
    private int ludaSecondChoice = 0; // 1 или 2
    private bool ludaAgreed = false;       

    [Header("Настройки затемнения")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1.5f;

    private int scene = 0;
    private bool showText = false;
    private bool isFading = false;
    public float textSpeed = 0.04f;
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    void Start()
    {
        if (backgrounds.Length == 0 || backgrounds.Length != dialogues.Length)
        {
            Debug.LogError("Ошибка в массивах Backgrounds или Dialogues!");
            return;
        }

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;
        }

        if (choicePanel != null) choicePanel.SetActive(false);
        if (dialogueChoicePanel != null) dialogueChoicePanel.SetActive(false);

        UpdateScene();
    }

    public void ChangeOnClick()
    {
        if (isFading) return;

        if (isChoiceActive)
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = dialogues[scene];
                isTyping = false;
            }
            return;
        }

        if (!showText)
        {
            showText = true;
            dialoguePanel.SetActive(true);
            StartDialogueTyping(dialogues[scene]);
        }
        else
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = dialogues[scene];
                isTyping = false;
            }
            else
            {
                // Переходы первого выбора (осмотр)
                if (playerChoice == "Marta" && scene == 10)
                {
                    scene = 6;
                }
                else if (playerChoice == "Marta" && scene == 7)
                {
                    scene = 11; 
                }
                // Переход после финала Люды (если отказала -> идём к Марте на индекс 18)
                else if (scene == 17)
                {
                    if (ludaAgreed)
                    {
                        StartCoroutine(FadeOutRoutine());
                        return;
                    }
                    else
                    {
                        scene = 18; // Переход к Марте (П2)
                    }
                }
                // Прыжок сразу на 23-й слайд после слов Марты (на сценах 19 и 22)
                else if (scene == 19 || scene == 22)
                {
                    scene = 23;
                }
                // Конец ветки Марты
                else if (scene >= 23)
                {
                    StartCoroutine(FadeOutRoutine());
                    return;
                }
                else if (scene < backgrounds.Length - 1)
                {
                    scene++;
                    if (scene == 12) choicePhase = 2;
                }
                else
                {
                    StartCoroutine(FadeOutRoutine());
                    return;
                }

                // Обязательное обновление экрана при переключении сцены
                showText = false;
                UpdateScene();
            }
        }
    }

    void UpdateScene()
    {
        if (backgrounds.Length == 0 || backgrounds[scene] == null) return;

        Sprite newSprite = backgrounds[scene];
        background.sprite = newSprite;

        if (dialogueText != null) dialogueText.text = "";

        AspectRatioFitter aspectFitter = background.GetComponent<AspectRatioFitter>();
        if (aspectFitter != null && newSprite != null)
        {
            float spriteWidth = newSprite.rect.width;
            float spriteHeight = newSprite.rect.height;
            if (spriteHeight > 0) aspectFitter.aspectRatio = spriteWidth / spriteHeight;
        }

        isChoiceActive = false;
        if (choicePanel != null) choicePanel.SetActive(false);
        if (dialogueChoicePanel != null) dialogueChoicePanel.SetActive(false);

        // 1. Первый выбор персонажа (Индекс 5)
        if (scene == 5 && choicePhase == 1)
        {
            isChoiceActive = true;
            showText = true;
            dialoguePanel.SetActive(true);
            if (choicePanel != null) choicePanel.SetActive(true);
            StartDialogueTyping(dialogues[scene]);
        }
        // 2. Второй выбор персонажа (Индекс 12)
        else if (scene == 12 && choicePhase == 2)
        {
            isChoiceActive = true;
            showText = true;
            dialoguePanel.SetActive(true);
            if (choicePanel != null) choicePanel.SetActive(true);
            StartDialogueTyping(dialogues[scene]);
        }
        // --- ВЕТКА ЛЮДЫ (П1) ---
        else if (scene == 13) // Первый выбор реплики игрока
        {
            ActivateDialogueChoices(
                "Татьяна последнее время себя плохо чувствует и до конца Пасхи не будет участвовать в хоре. Хорошо, что она отдохнёт. Теперь нам нужен человек, который временно заменит её. Я думаю, что у вас получится.",
                "Я хочу, чтобы богослужения на праздниках были проведены хорошо, и ищу человека, который сможет петь в церковном хоре до конца Пасхи. Думаю, что вы отлично справитесь в этой задачей."
            );
        }
        else if (scene == 15) // Второй вопрос: Хотите присоединиться к хору на время праздников?
        {
            StartDialogueTyping(dialogues[scene]);
        }
        else if (scene == 16) // Второй выбор реплики игрока
        {
            ActivateDialogueChoices(
                "Раз вы ноты с листа читаете, вам много времени и не понадобится. Будете приходить только на общие спевки перед самой службой, буквально на полчаса. Справитесь без лишней зубрёжки.",
                "Понимаю, но Пасха — дело серьезное. Будем собираться каждый вечер часа на два, чтобы всё отточить. Времени мало, нужно больше репетировать."
            );
        }
        else if (scene == 17) // Итог ответа Люды по комбинациям (1+1, 1+2, 2+1, 2+2)
        {
            if (ludaFirstChoice == 1 && ludaSecondChoice == 1)
            {
                ludaAgreed = false;
                dialogues[17] = "Люда: Нет, спасибо. Даже на полчаса не пойду. Ищите кого-нибудь другого.";
            }
            else if (ludaFirstChoice == 1 && ludaSecondChoice == 2)
            {
                ludaAgreed = false;
                dialogues[17] = "Люда: Ты меня вообще слушал? Я же сказала, времени у меня нет. И всё ради того, чтобы просто подменить вашу Таню? Нет уж, без меня.";
            }
            else if (ludaFirstChoice == 2 && ludaSecondChoice == 1)
            {
                ludaAgreed = true;
                dialogues[17] = "Люда: Лааадно, помогу вам. Умеешь ты договариваться. На полчаса перед службой, значит?";
            }
            else if (ludaFirstChoice == 2 && ludaSecondChoice == 2)
            {
                ludaAgreed = false;
                dialogues[17] = "Люда: Сделать красивый праздник — это здорово, но по два часа каждый вечер я не хочу. У меня своя жизнь есть. Прости, но я пас.";
            }
            dialoguePanel.SetActive(false);
        }
        // --- ВЕТКА МАРТЫ (П2) ---
        else if (scene == 18) // Первый выбор реплики игрока для Марты
        {
            ActivateDialogueChoices(
                "Татьяна последнее время себя плохо чувствует и до конца Пасхи не будет участвовать в хоре. Хорошо, что она отдохнёт. Теперь нам нужен человек, который временно заменит её. Я думаю, что у вас получится.",
                "Я хочу, чтобы богослужения на праздниках были проведены хорошо, и ищу человека, который сможет петь в церковном хоре до конца Пасхи. Думаю, что вы отлично справитесь в этой задачей."
            );
        }
        else if (scene == 20) // Вопрос: Хотите присоединиться к хору на время праздников?
        {
            StartDialogueTyping(dialogues[scene]);
        }
        else if (scene == 21) // Второй выбор реплики игрока для Марты
        {
            ActivateDialogueChoices(
                "А я и смотрю, бледная какая-то Таня последнее время ходила. Правильно, здоровье дороже. А я, конечно, помогу!",
                "Ой, да вы на меня сильно-то не надейтесь, я ж не профессионал. Но раз людей не хватает — с радостью приду!"
            );
        }
        else if (scene == 23) // Финальный ответ Марты (всегда согласна)
        {
            dialoguePanel.SetActive(false);
        }
        else
        {
            dialoguePanel.SetActive(false);
        }
    }

    void ActivateDialogueChoices(string opt1, string opt2)
    {
        isChoiceActive = true;
        showText = true;
        dialoguePanel.SetActive(true);
        if (dialogueChoicePanel != null) dialogueChoicePanel.SetActive(true);

        option1Text.text = opt1;
        option2Text.text = opt2;

        StartDialogueTyping(dialogues[scene]);
    }

    // --- КНОПКИ ВЫБОРА РЕПЛИК ---

    public void SelectOption1()
    {
        if (scene == 13) // Первый выбор для Люды -> вариант 1
        {
            ludaFirstChoice = 1;
            dialogues[14] = "Люда: Слушай, мне, конечно, жаль вашу Татьяну, пусть поправляется, но мне-то какое до этого дело? У меня своих забот хватает, чтобы чужие дыры закрывать. Да и не особо мне хочется в церковном хоре торчать.";
            JumpToScene(14);
        }
        else if (scene == 16) // Второй выбор для Люды -> вариант 1
        {
            ludaSecondChoice = 1;
            JumpToScene(17);
        }
        else if (scene == 18) // Первый выбор для Марты -> вариант 1
        {
            dialogues[19] = "Марта: Ой, бедняжка Таня... Да, отдохнуть ей точно нужно. А в хоре помочь? Ну, если надо — я попробую.";
            JumpToScene(19);
        }
        else if (scene == 21) // Второй выбор для Марты -> вариант 1
        {
            dialogues[22] = "Марта: А я и смотрю, бледная какая-то Таня последнее время ходила. Правильно, здоровье дороже. А я, конечно, помогу!";
            JumpToScene(22);
        }
    }

    public void SelectOption2()
    {
        if (scene == 13) // Первый выбор для Люды -> вариант 2
        {
            ludaFirstChoice = 2;
            dialogues[14] = "Люда: На Пасху, говоришь... Вообще, идея неплохая, праздник большой, хочется, чтобы красиво всё прошло. Только вот у меня времени в обрез — сидеть у вас тут на репетициях с утра до ночи я точно не буду.";
            JumpToScene(14);
        }
        else if (scene == 16) // Второй выбор для Люды -> вариант 2
        {
            ludaSecondChoice = 2;
            JumpToScene(17);
        }
        else if (scene == 18) // Первый выбор для Марты -> вариант 2
        {
            dialogues[19] = "Марта: Праздники — это прекрасно, музыка должна звучать красиво. Если вы уверены во мне, то я с радостью согласна.";
            JumpToScene(19);
        }
        else if (scene == 21) // Второй выбор для Марты -> вариант 2
        {
            dialogues[22] = "Марта: Ой, да вы на меня сильно-то не надейтесь, я ж не профессионал. Но раз людей не хватает — с радостью приду!";
            JumpToScene(22);
        }
    }

    // --- КНОПКИ ВЫБОРА ПЕРСОНАЖА (Люда / Марта) ---

    public void ChooseLuda()
    {
        if (choicePhase == 1)
        {
            playerChoice = "Luda";
            JumpToScene(6);
        }
        else if (choicePhase == 2)
        {
            ludaFirstChoice = 0;
            ludaSecondChoice = 0;
            ludaAgreed = false;
            JumpToScene(13);
        }
    }

    public void ChooseMarta()
    {
        if (choicePhase == 1)
        {
            playerChoice = "Marta";
            JumpToScene(8);
        }
        else if (choicePhase == 2)
        {
            JumpToScene(18); // Переход к ветке Марты
        }
    }

    private void JumpToScene(int targetSceneIndex)
    {
        isChoiceActive = false;
        if (choicePanel != null) choicePanel.SetActive(false);
        if (dialogueChoicePanel != null) dialogueChoicePanel.SetActive(false);

        scene = targetSceneIndex;
        showText = false;
        UpdateScene();
    }

    public void StartDialogueTyping(string textToPrint)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeTextRoutine(textToPrint));
    }

    IEnumerator TypeTextRoutine(string textToPrint)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in textToPrint.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
    }

    IEnumerator FadeOutRoutine()
    {
        isFading = true;

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.blocksRaycasts = true;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
                yield return null;
            }

            fadeCanvasGroup.alpha = 1f;
        }

        if (perexod != null)
        {
            perexod.text = "Приступим к исполнению";
        }
    }
}