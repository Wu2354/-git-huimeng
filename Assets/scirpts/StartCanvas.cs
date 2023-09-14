using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class StartCanvas : MonoBehaviour
{
    public List<TextMeshProUGUI> textObjects;  // List of all the Text objects for the opening screen.
    public float duration = 2.0f;  // Duration for the fade in/out effect.
    public GameObject PlaneBackground;
    private bool isCoroutineFinished = false;//判断协程结束
    
    public TextMeshProUGUI StartButtonText;//最后的按钮下的提示文本

    
    private void Start()
    {        
        PlaneBackground.SetActive(true);        
        StartButtonText.gameObject.SetActive(true);//显示出按钮下面的提示文本
        StartButtonText.DOFade(0, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);//给提示文本添加效果                       
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            StartGame();
        }
    }

    private IEnumerator ShowOpeningTexts()
    {
        foreach (TextMeshProUGUI text in textObjects)
        {
            text.gameObject.SetActive(true);  // Show the text object.
            Sequence sequence = DOTween.Sequence();
            sequence.Append(text.DOFade(1, duration));
            sequence.Append(text.DOFade(0, duration));
            // Save original font size
            float originalSize = text.fontSize;

            // Scale up during fade in
            sequence.Insert(0, DOTween.To(() => text.fontSize, x => text.fontSize = x, 70f, duration));

            // Scale down during fade out
            sequence.Insert(duration, DOTween.To(() => text.fontSize, x => text.fontSize = x, originalSize, duration));
            yield return new WaitForSeconds(duration*2);  // Wait for the fade in/out effect and delay.
            text.gameObject.SetActive(false);  // Hide the text object.

        }
        var planeBackgroundImage = PlaneBackground.GetComponent<Image>();
        Sequence sequence1 = DOTween.Sequence();
        sequence1.Append(planeBackgroundImage.DOFade(0, duration));
        yield return new WaitForSeconds(duration);

        isCoroutineFinished = true;
        this.gameObject.SetActive(false);
       
    }

    private void StartGame()
    {        
        StartCoroutine(ShowOpeningTexts());
        //隐藏提示文本        
        StartButtonText.gameObject.SetActive(false);
    }
}
