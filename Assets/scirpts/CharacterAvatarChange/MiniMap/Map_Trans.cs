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
    public Button SureButton; // 新增的传送按钮
    //public List<TargetObject> targetObjects;
    public List<GameObject> targetObjects;
    public GameObject thirdPersonCharacter;
    
    private TextMeshProUGUI infoText; // 用于显示物体名称的TextMeshProUGUI
    public PostProcessVolume postProcessVolume; // 引用Post-Processing Volume

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
        SureButton.gameObject.SetActive(view); // 显示/隐藏传送按钮
    }

    IEnumerator DelayedTeleport(Transform targetTrans)
    {
      
        //传送
        yield return new WaitForSeconds(0.5f); // 例如延迟0.5秒，你可以根据需要调整
        thirdPersonCharacter.transform.position = targetTrans.position;
        thirdPersonCharacter.transform.position = targetTrans.position;

        // 改变朝向（以下代码是错误的）
        Vector3 directionToTarget = (targetTrans.position - thirdPersonCharacter.transform.position).normalized;
        thirdPersonCharacter.transform.forward = new Vector3(directionToTarget.x, 0, directionToTarget.z);


        // 在传送前启用Post-Processing效果
        postProcessVolume.enabled = true;

        infoText.text = targetTrans.name;
        yield return new WaitForSeconds(1f);

        // 在传送后禁用Post-Processing效果
        postProcessVolume.enabled = false;
    }

}
