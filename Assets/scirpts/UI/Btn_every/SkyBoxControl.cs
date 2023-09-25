using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.Layouts.InputControlLayout;

public class SkyBoxControl : MonoBehaviour
{
    public SkyboxBlender skyboxBlender;
    public GameObject btn_item;//�м�Ԫ�ؿ���
    private bool isOpen = false;
    private Button button;

    public Button cycleSkyBtn;//��պ�ѭ����ť
    public Button rotateSkyBtn;//��պ���ת����
    private bool isCycle = false;//�ж�ѭ��
    private bool isRotate = false;//�ж���ת


    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ControlItem);

        skyboxBlender.Blend(true,false);//������պ�һ��ʼ��ѭ��,����ת
        cycleSkyBtn.onClick.AddListener(cycleSky);
        rotateSkyBtn.onClick.AddListener(rotateSky);
       
    }
    
    //������պе���ת
    private void rotateSky()
    {
        isRotate = !isRotate;
        skyboxBlender.Blend(isCycle,isRotate);//true�ͻ���ת��(�ڶ�����������)
    }


    //ѭ����պ�
    private void cycleSky()
    {
        isCycle = !isCycle;
        skyboxBlender.Blend(isCycle,isRotate);  //false�ͻ�ѭ��(��һ����������)     
    }


    //�����м�Ԫ����ʾ
    private void ControlItem()
    {
        isOpen = !isOpen;
        btn_item.SetActive(isOpen);
    }
}
