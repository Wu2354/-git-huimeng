using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.Layouts.InputControlLayout;

public class SkyBoxControl : MonoBehaviour
{
    public SkyboxBlender skyboxBlender;
    public GameObject btn_item;//中间元素开关
    private bool isOpen = false;
    private Button button;

    public Button cycleSkyBtn;//天空盒循环按钮
    public Button rotateSkyBtn;//天空盒旋转开关
    private bool isCycle = false;//判断循环
    private bool isRotate = false;//判断旋转


    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ControlItem);

        skyboxBlender.Blend(true,false);//设置天空盒一开始不循环,不旋转
        cycleSkyBtn.onClick.AddListener(cycleSky);
        rotateSkyBtn.onClick.AddListener(rotateSky);
       
    }
    
    //控制天空盒的旋转
    private void rotateSky()
    {
        isRotate = !isRotate;
        skyboxBlender.Blend(isCycle,isRotate);//true就会旋转了(第二个参数控制)
    }


    //循环天空盒
    private void cycleSky()
    {
        isCycle = !isCycle;
        skyboxBlender.Blend(isCycle,isRotate);  //false就会循环(第一个参数控制)     
    }


    //控制中间元素显示
    private void ControlItem()
    {
        isOpen = !isOpen;
        btn_item.SetActive(isOpen);
    }
}
