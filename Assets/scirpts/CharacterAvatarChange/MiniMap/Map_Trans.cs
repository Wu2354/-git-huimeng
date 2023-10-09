using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
            ToggleDropdownAndButton(true);
        }
    }

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
        ToggleDropdownAndButton(false);

        int index = dropdown.value;
        if (index < targetObjects.Count)
        {
            StartCoroutine(DelayedTeleport(targetObjects[index].transform));
        }
    }


    void ToggleDropdownAndButton(bool view)
    {       
        dropdown.gameObject.SetActive(view);
        SureButton.gameObject.SetActive(view); // ��ʾ/���ش��Ͱ�ť
    }

    IEnumerator DelayedTeleport(Transform targetTrans)
    {
      
        //����
        yield return new WaitForSeconds(0.5f); // �����ӳ�0.5�룬����Ը�����Ҫ����
        thirdPersonCharacter.transform.position = targetTrans.position;
        thirdPersonCharacter.transform.position = targetTrans.position;

        // �ı䳯�����´����Ǵ���ģ�
        Vector3 directionToTarget = (targetTrans.position - thirdPersonCharacter.transform.position).normalized;
        thirdPersonCharacter.transform.forward = new Vector3(directionToTarget.x, 0, directionToTarget.z);


        // �ڴ���ǰ����Post-ProcessingЧ��
        postProcessVolume.enabled = true;

        infoText.text = targetTrans.name;
        yield return new WaitForSeconds(1f);

        // �ڴ��ͺ����Post-ProcessingЧ��
        postProcessVolume.enabled = false;
    }

}
