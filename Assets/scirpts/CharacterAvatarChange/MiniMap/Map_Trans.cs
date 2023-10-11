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
    public Button SureButton; // �����Ĵ��Ͱ�ť
    //public List<TargetObject> targetObjects;
    public List<GameObject> targetObjects;
    public GameObject thirdPersonCharacter;
    
    private TextMeshProUGUI infoText; // ������ʾ�������Ƶ�TextMeshProUGUI
    //public PostProcessVolume postProcessVolume; // ����Post-Processing Volume
   
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
        SureButton.gameObject.SetActive(false); // ��ʼ���ش��Ͱ�ť
        PopulateDropdown();

        SureButton.onClick.AddListener(TeleportCharacter); // ��Ӵ��͹��ܵ���ť����¼�                
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
        byte bVk, //�����ֵ ��Ӧ������ascll��ʮ����ֵ  
        byte bScan, //0
        int dwFlags, //0 Ϊ���£�1��ס��2Ϊ�ͷ� 
        int dwExtraInfo //0
    );

    //���������������    
    void PopulateDropdown()
    {
        dropdown.options.Clear();

        foreach (var obj in targetObjects)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(obj.name));
        }
    }

    //��ť�����߼�
    public void TeleportCharacter()
    {
        
        index = dropdown.value;
        if (index < targetObjects.Count)
        {
           
            // ���������������LookAtĿ��Ϊָ�����壨δʵ�֣�
            virtualCamera.LookAt = targetObjects[index].transform;
            //��ת��Э���߼�
            StartCoroutine(DelayedTeleport(targetObjects[index].transform));
            //��ת�ͻ��Զ���ΪС��ͼ
            miniMap.SetToMiniMapSize();
            ToggleDropdownAndButton(false);
        }
    }

    //�������������ʾ
    void ToggleDropdownAndButton(bool view)
    {       
        dropdown.gameObject.SetActive(view);
        SureButton.gameObject.SetActive(view); // ��ʾ/���ش��Ͱ�ť
    }
    

    //���͵�Я��
    IEnumerator DelayedTeleport(Transform targetTrans)
    {        
        //����
        yield return new WaitForSeconds(0.5f); // �����ӳ�0.5�룬����Ը�����Ҫ����        
        thirdPersonCharacter.transform.position = targetTrans.position; 
        thirdPersonCharacter.transform.rotation = targetTrans.rotation;

        keybd_event(77, 0, 1, 0);//M���İ��²���

        // �������������Ŀ�������Z�᷽��
        //Vector3 cameraLookDirection = targetTrans.forward;

        // ��������������ĳ���
        //virtualCamera.transform.rotation = Quaternion.LookRotation(cameraLookDirection);

        //����
        /*yield return new WaitForSeconds(0.5f);
        index = dropdown.value;
        Vector3 directionToTarget = targetObjects[index].transform.position - thirdPersonCharacter.transform.position;
        directionToTarget.Normalize();
        mainCamera.transform.LookAt(directionToTarget);*/


        // �ڴ��ͺ�����Post-ProcessingЧ��
        /*postProcessVolume.enabled = true;

        infoText.text = targetTrans.name;
        yield return new WaitForSeconds(1f);

        // ��һ�����Post-ProcessingЧ��
        postProcessVolume.enabled = false;*/
    }

}
