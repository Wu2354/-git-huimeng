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

    public Button middleScreenButton;  // 新增的屏幕中间的按钮
    public TextMeshProUGUI middleButtonText;  // 按钮上的文本
    [SerializeField] GameObject girlHairPanel;


    private void Start()
    {
        PlaneBackground.SetActive(true);
        StartCoroutine(ShowOpeningTexts());

        middleScreenButton.gameObject.SetActive(false);  // 初始化时隐藏按钮
        middleScreenButton.onClick.AddListener(OnMiddleButtonClicked);  // 为按钮添加点击事件
    }    
    private IEnumerator ShowOpeningTexts()
    {
        foreach (TextMeshProUGUI text in textObjects)
        {
            text.gameObject.SetActive(true);  
            Sequence sequence = DOTween.Sequence();//创建一个动画序列
            //Append是按顺序播放
            sequence.Append(text.DOFade(1, duration));
            sequence.Append(text.DOFade(0, duration));

            // 保存当前文本的字体大小
            float originalSize = text.fontSize;

            // 文本在淡入过程中的放大效果
            sequence.Insert(0, DOTween.To(() => text.fontSize, x => text.fontSize = x, 70f, duration));
            // 文本在淡出过程中恢复原始大小
            sequence.Insert(duration, DOTween.To(() => text.fontSize, x => text.fontSize = x, originalSize, duration));

            yield return new WaitForSeconds(duration*2);  
            text.gameObject.SetActive(false);  
        }

        //所有文本动画完成后显示屏幕中央按钮
        middleScreenButton.gameObject.SetActive(true);

        isCoroutineFinished = true;
        yield return null; // 这里可以直接返回，不再处理背景图片消失的逻辑

    }

    // 当屏幕中央的按钮被点击时调用此方法
    private void OnMiddleButtonClicked()
    {

        middleScreenButton.gameObject.SetActive(false); // 首先隐藏中央按钮
        girlHairPanel.SetActive(true);
        // 开始背景图片消失的逻辑
        var planeBackgroundImage = PlaneBackground.GetComponent<Image>();
        Sequence sequence = DOTween.Sequence();
        sequence.Append(planeBackgroundImage.DOFade(0, duration));
        sequence.OnComplete(() => this.gameObject.SetActive(false)); // 背景图片消失后关闭这个脚本绑定的物体
    }
}
