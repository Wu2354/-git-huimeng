using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Map_Trans : MonoBehaviour
{
    [System.Serializable]
    public struct TargetObject
    {
        public Transform targetTransform;
        public string customName;
    }    
    public TMP_Dropdown dropdown;
    public Button SureButton; // 新增的传送按钮
    public List<TargetObject> targetObjects;
    public GameObject thirdPersonCharacter;
    private bool isDropdownVisible = false;    

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
            ToggleDropdownAndButton();
        }
    }

    void PopulateDropdown()
    {
        dropdown.options.Clear();

        foreach (TargetObject obj in targetObjects)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(obj.customName));
        }
    }

    public void TeleportCharacter()
    {
        int index = dropdown.value;
        if (index < targetObjects.Count)
        {
            StartCoroutine(DelayedTeleport(targetObjects[index].targetTransform));
        }
    }


    void ToggleDropdownAndButton()
    {
        isDropdownVisible = !isDropdownVisible;
        dropdown.gameObject.SetActive(isDropdownVisible);
        SureButton.gameObject.SetActive(isDropdownVisible); // 显示/隐藏传送按钮
    }

    IEnumerator DelayedTeleport(Transform targetTrans)
    {
        yield return new WaitForSeconds(0.5f); // 例如延迟0.5秒，你可以根据需要调整
        thirdPersonCharacter.transform.position = targetTrans.position;
        thirdPersonCharacter.transform.position = targetTrans.position;

        // Calculate direction to targetTrans and set as forward direction of thirdPersonCharacter
        Vector3 directionToTarget = (targetTrans.position - thirdPersonCharacter.transform.position).normalized;
        thirdPersonCharacter.transform.forward = new Vector3(directionToTarget.x, 0, directionToTarget.z);
    }

}
