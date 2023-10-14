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
    private bool isCoroutineFinished = false;//�ж�Э�̽���
    
    public TextMeshProUGUI StartButtonText;//���İ�ť�µ���ʾ�ı�

    public Button middleScreenButton;  // ��������Ļ�м�İ�ť
    public TextMeshProUGUI middleButtonText;  // ��ť�ϵ��ı�
    [SerializeField] GameObject girlHairPanel;


    private void Start()
    {
        PlaneBackground.SetActive(true);
        StartCoroutine(ShowOpeningTexts());

        middleScreenButton.gameObject.SetActive(false);  // ��ʼ��ʱ���ذ�ť
        middleScreenButton.onClick.AddListener(OnMiddleButtonClicked);  // Ϊ��ť��ӵ���¼�
    }    
    private IEnumerator ShowOpeningTexts()
    {
        foreach (TextMeshProUGUI text in textObjects)
        {
            text.gameObject.SetActive(true);  
            Sequence sequence = DOTween.Sequence();//����һ����������
            //Append�ǰ�˳�򲥷�
            sequence.Append(text.DOFade(1, duration));
            sequence.Append(text.DOFade(0, duration));

            // ���浱ǰ�ı��������С
            float originalSize = text.fontSize;

            // �ı��ڵ�������еķŴ�Ч��
            sequence.Insert(0, DOTween.To(() => text.fontSize, x => text.fontSize = x, 70f, duration));
            // �ı��ڵ��������лָ�ԭʼ��С
            sequence.Insert(duration, DOTween.To(() => text.fontSize, x => text.fontSize = x, originalSize, duration));

            yield return new WaitForSeconds(duration*2);  
            text.gameObject.SetActive(false);  
        }

        //�����ı�������ɺ���ʾ��Ļ���밴ť
        middleScreenButton.gameObject.SetActive(true);

        isCoroutineFinished = true;
        yield return null; // �������ֱ�ӷ��أ����ٴ�����ͼƬ��ʧ���߼�

    }

    // ����Ļ����İ�ť�����ʱ���ô˷���
    private void OnMiddleButtonClicked()
    {

        middleScreenButton.gameObject.SetActive(false); // �����������밴ť
        girlHairPanel.SetActive(true);
        // ��ʼ����ͼƬ��ʧ���߼�
        var planeBackgroundImage = PlaneBackground.GetComponent<Image>();
        Sequence sequence = DOTween.Sequence();
        sequence.Append(planeBackgroundImage.DOFade(0, duration));
        sequence.OnComplete(() => this.gameObject.SetActive(false)); // ����ͼƬ��ʧ��ر�����ű��󶨵�����
    }
}
