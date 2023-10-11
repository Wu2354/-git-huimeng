using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Runtime.InteropServices;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;



public class Map_Trans : MonoBehaviour
{
    //[System.Serializable]
    //public struct TargetObject
    //{
    //    public Transform targetTransform;
    //    public string customName;
    //}    
    
    public TMP_Dropdown dropdown;
    public Button SureButton; // 新增的传送按钮
    //public List<TargetObject> targetObjects;
    public List<GameObject> targetObjects;
    public GameObject thirdPersonCharacter;
    
    private TextMeshProUGUI infoText; // 用于显示物体名称的TextMeshProUGUI
    //public PostProcessVolume postProcessVolume; // 引用Post-Processing Volume
   
    private bool OnView = false;
    private int index;
    public CinemachineVirtualCamera virtualCamera;
    private bl_MiniMap miniMap;


    private void Awake()
    {
        miniMap = GetComponent<bl_MiniMap>();
    }

    private void Start()
    {        
        dropdown.gameObject.SetActive(false);
        SureButton.gameObject.SetActive(false); // 初始隐藏传送按钮
        PopulateDropdown();

        SureButton.onClick.AddListener(TeleportCharacter); // 添加传送功能到按钮点击事件                
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            //OnView = !OnView;
            ToggleDropdownAndButton(true);
        }
    }


    [DllImport("User32.dll", EntryPoint = "keybd_event")]  
    static extern void keybd_event(
        byte bVk, //虚拟键值 对应按键的ascll码十进制值  
        byte bScan, //0
        int dwFlags, //0 为按下，1按住，2为释放 
        int dwExtraInfo //0
    );

    //给下拉框添加名称    
    void PopulateDropdown()
    {
        dropdown.options.Clear();

        foreach (var obj in targetObjects)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(obj.name));
        }
    }

    //按钮触发逻辑
    public void TeleportCharacter()
    {
        
        index = dropdown.value;
        if (index < targetObjects.Count)
        {
           
            // 设置虚拟摄像机的LookAt目标为指定物体（未实现）
            virtualCamera.LookAt = targetObjects[index].transform;
            //跳转的协程逻辑
            StartCoroutine(DelayedTeleport(targetObjects[index].transform));
            //跳转就会自动变为小地图
            miniMap.SetToMiniMapSize();
            ToggleDropdownAndButton(false);
        }
    }

    //控制下拉框的显示
    void ToggleDropdownAndButton(bool view)
    {       
        dropdown.gameObject.SetActive(view);
        SureButton.gameObject.SetActive(view); // 显示/隐藏传送按钮
    }
    

    //传送的携程
    IEnumerator DelayedTeleport(Transform targetTrans)
    {        
        //传送
        yield return new WaitForSeconds(0.5f); // 例如延迟0.5秒，你可以根据需要调整        
        thirdPersonCharacter.transform.position = targetTrans.position; 
        thirdPersonCharacter.transform.rotation = targetTrans.rotation;

        keybd_event(77, 0, 1, 0);//M键的按下操作

        // 计算摄像机朝向目标物体的Z轴方向
        //Vector3 cameraLookDirection = targetTrans.forward;

        // 设置虚拟摄像机的朝向
        //virtualCamera.transform.rotation = Quaternion.LookRotation(cameraLookDirection);

        //朝向
        /*yield return new WaitForSeconds(0.5f);
        index = dropdown.value;
        Vector3 directionToTarget = targetObjects[index].transform.position - thirdPersonCharacter.transform.position;
        directionToTarget.Normalize();
        mainCamera.transform.LookAt(directionToTarget);*/


        // 在传送后启用Post-Processing效果
        /*postProcessVolume.enabled = true;

        infoText.text = targetTrans.name;
        yield return new WaitForSeconds(1f);

        // 过一秒禁用Post-Processing效果
        postProcessVolume.enabled = false;*/
    }

}
