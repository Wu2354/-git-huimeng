using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using DG.Tweening;




public class Map_Trans : MonoBehaviour
{
           
    public TMP_Dropdown dropdown;
    public Button SureButton; // 新增的传送按钮    
    public List<GameObject> targetObjects;
    public GameObject thirdPersonCharacter;
    
    //控制跳转后模糊文字显示
    public TextMeshProUGUI infoText; // 用于显示物体名称的TextMeshProUGUI
    public PostProcessVolume postProcessVolume; // 引用Post-Processing Volume
    private DepthOfField depthOfField;//后处理的景深效果
     
    //控制小地图
    private int index;    
    private bl_MiniMap miniMap;

    //朝向
    public CinemachineVirtualCamera virtualCamera;
    public GameObject personRoot;


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

        // 获取Depth of Field效果的引用
        postProcessVolume.profile.TryGetSettings(out depthOfField);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            //OnView = !OnView;
            ToggleDropdownAndButton(true);
        }
    }

    
    /// <summary>
    /// 给下拉框其中元素显示指定文字
    /// </summary>
    void PopulateDropdown()
    {
        dropdown.options.Clear();

        foreach (var obj in targetObjects)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(obj.name));
        }
    }

    /// <summary>
    /// 按钮触发逻辑
    /// </summary>
    public void TeleportCharacter()
    {
        
        index = dropdown.value;
        if (index < targetObjects.Count)
        {
                       
            //跳转的协程逻辑
            StartCoroutine(DelayedTeleport(targetObjects[index].transform));
            //跳转就会自动变为小地图
            miniMap.SetToMiniMapSize();
            ToggleDropdownAndButton(false);
        }
    }

    /// <summary>
    /// 控制下拉框的显示
    /// </summary>
    /// <param name="view"></param>
    void ToggleDropdownAndButton(bool view)
    {       
        dropdown.gameObject.SetActive(view);
        SureButton.gameObject.SetActive(view); // 显示/隐藏传送按钮
    }
    

    /// <summary>
    /// 传送，景深模糊显示文字，朝向（未解决）
    /// </summary>
    /// <param name="targetTrans">这是目标地点的物体位置</param>
    /// <returns></returns>
    IEnumerator DelayedTeleport(Transform targetTrans)
    {        
        //传送
        yield return new WaitForSeconds(0.5f); // 例如延迟0.5秒，你可以根据需要调整        
        thirdPersonCharacter.transform.position = targetTrans.position;
        thirdPersonCharacter.transform.forward = targetTrans.forward;

        // 设置Cinemachine的Follow和LookAt属性
        virtualCamera.Follow = thirdPersonCharacter.transform;
        virtualCamera.LookAt = thirdPersonCharacter.transform;
        virtualCamera.Follow = personRoot.transform;


        /*
         * 后处理模糊效果用post中的景深
         */
        depthOfField.focusDistance.value = 0.2f; // 根据需要调整

        // 显示传送文字
        infoText.text = "欢迎来到 " + targetTrans.name;
        infoText.gameObject.SetActive(true);
        //设置完全不透明
        infoText.color = new Color(infoText.color.r, infoText.color.g, infoText.color.b, 1f);

        yield return new WaitForSeconds(1f);

        // 恢复清晰效果（动画形式）
        DOVirtual.Float(0.2f, 10f, 1f, value => {
            depthOfField.focusDistance.value = value;
        });

        //文字缓动消失 
        infoText.DOFade(0f, 1f).OnComplete(() => {
            infoText.gameObject.SetActive(false);
        });
    }
    
}
