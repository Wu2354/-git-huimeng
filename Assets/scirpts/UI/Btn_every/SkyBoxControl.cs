using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.Layouts.InputControlLayout;

public class SkyBoxControl : MonoBehaviour
{
    public SkyboxBlender skyboxBlender;
    public GameObject btnItem;//�м�Ԫ�ؿ���
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
        // ���������ת����ֹͣ��ת������ʼ��ת
        if (isRotate)
        {
            isRotate = false;
            skyboxBlender.StopRotation();
        }
        else
        {
            isRotate = true;
            UpdateSkybox();
        }
    }


    //ѭ����պ�
    private void cycleSky()
    {        
        if (isOpen)
        {
            skyboxBlender.Stop();
        }
        else
        {
            UpdateSkybox(); //false�ͻ�ѭ��(��һ����������)  
        }
           
        isCycle = !isCycle;
    }
    
    private void UpdateSkybox()
    {
        skyboxBlender.Blend(isCycle, isRotate);
    }

    //�����м�Ԫ����ʾ
    private void ControlItem()
    {
        isOpen = !isOpen;
        btnItem.SetActive(isOpen);
    }
}
