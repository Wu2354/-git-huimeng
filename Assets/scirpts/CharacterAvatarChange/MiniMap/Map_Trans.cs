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
    public PostProcessVolume postProcessVolume; // ����Post-Processing Volume
    private Keyboard keyboard;
    private bool OnView = false;
    private int index;


    private void Start()
    {        
        dropdown.gameObject.SetActive(false);
        SureButton.gameObject.SetActive(false); // ��ʼ���ش��Ͱ�ť
        PopulateDropdown();

        SureButton.onClick.AddListener(TeleportCharacter); // ��Ӵ��͹��ܵ���ť����¼�
        
        keyboard = Keyboard.current;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            OnView = !OnView;
            ToggleDropdownAndButton(OnView);
        }
    }


    [DllImport("User32.dll", EntryPoint = "keybd_event")]  
    static extern void keybd_event(
        byte bVk, //�����ֵ ��Ӧ������ascll��ʮ����ֵ  
        byte bScan, //0
        int dwFlags, //0 Ϊ���£�1��ס��2Ϊ�ͷ� 
        int dwExtraInfo //0
    );

    void PopulateDropdown()
    {
        dropdown.options.Clear();

        foreach (var obj in targetObjects)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(obj.name));
        }
    }

    public void TeleportCharacter()
    {        
        index = dropdown.value;
        if (index < targetObjects.Count)
        {
            keybd_event(77, 0, 1, 0);//M���İ��²���
            StartCoroutine(DelayedTeleport(targetObjects[index].transform));
        }
    }

    void ToggleDropdownAndButton(bool view)
    {       
        dropdown.gameObject.SetActive(view);
        SureButton.gameObject.SetActive(view); // ��ʾ/���ش��Ͱ�ť
    }


    // ģ�ⰴ������ĺ���
    

    IEnumerator DelayedTeleport(Transform targetTrans)
    {
      
        //����
        yield return new WaitForSeconds(0.5f); // �����ӳ�0.5�룬����Ը�����Ҫ����
        thirdPersonCharacter.transform.position = targetTrans.position; 
        thirdPersonCharacter.transform.rotation = targetTrans.rotation;

        //����
        //yield return new WaitForSeconds(0.5f);
        //targetObjects[index].transform.position = thirdPersonCharacter.transform.



        // �ڴ���ǰ����Post-ProcessingЧ��
        postProcessVolume.enabled = true;

        //infoText.text = targetTrans.name;
        yield return new WaitForSeconds(1f);

        // �ڴ��ͺ����Post-ProcessingЧ��
        postProcessVolume.enabled = false;
    }

}
