using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_whellButtonController : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{    
    public string name;
    public TextMeshProUGUI showText;
    private bool select = false;
    private Vector3 initiateScale;//��ʼ��ť�ߴ�
    [Range(1.0f, 1.5f)]
    public float btnChangeRate = 1.2f;//�仯��С
    [Range(0.1f, 1.0f)]
    public float btn_ChangeTime = 0.3f;//��ť�仯��ʱ��    

    void Start()
    {
        initiateScale = transform.localScale;                
    }

    // Update is called once per frame
    void Update()
    {

    }
    // �������밴ťʱ����
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(initiateScale * btnChangeRate, btn_ChangeTime);
        showText.text = name;
    }

    // ������˳���ťʱ����
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(initiateScale, btn_ChangeTime);
        showText.text = null;
    }
}
