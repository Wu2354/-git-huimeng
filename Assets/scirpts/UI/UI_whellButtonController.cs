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
    private Vector3 initiateScale;//初始按钮尺寸
    [Range(1.0f, 1.5f)]
    public float btnChangeRate = 1.2f;//变化大小
    [Range(0.1f, 1.0f)]
    public float btn_ChangeTime = 0.3f;//按钮变化的时间    

    void Start()
    {
        initiateScale = transform.localScale;                
    }

    // Update is called once per frame
    void Update()
    {

    }
    // 当鼠标进入按钮时调用
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(initiateScale * btnChangeRate, btn_ChangeTime);
        showText.text = name;
    }

    // 当鼠标退出按钮时调用
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(initiateScale, btn_ChangeTime);
        showText.text = null;
    }
}
