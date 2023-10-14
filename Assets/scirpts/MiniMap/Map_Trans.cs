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
    public Button SureButton; // �����Ĵ��Ͱ�ť    
    public List<GameObject> targetObjects;
    public GameObject thirdPersonCharacter;
    
    //������ת��ģ��������ʾ
    public TextMeshProUGUI infoText; // ������ʾ�������Ƶ�TextMeshProUGUI
    public PostProcessVolume postProcessVolume; // ����Post-Processing Volume
    private DepthOfField depthOfField;//����ľ���Ч��
     
    //����С��ͼ
    private int index;    
    private bl_MiniMap miniMap;

    //����
    public CinemachineVirtualCamera virtualCamera;
    public GameObject personRoot;


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

        // ��ȡDepth of FieldЧ��������
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
    /// ������������Ԫ����ʾָ������
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
    /// ��ť�����߼�
    /// </summary>
    public void TeleportCharacter()
    {
        
        index = dropdown.value;
        if (index < targetObjects.Count)
        {
                       
            //��ת��Э���߼�
            StartCoroutine(DelayedTeleport(targetObjects[index].transform));
            //��ת�ͻ��Զ���ΪС��ͼ
            miniMap.SetToMiniMapSize();
            ToggleDropdownAndButton(false);
        }
    }

    /// <summary>
    /// �������������ʾ
    /// </summary>
    /// <param name="view"></param>
    void ToggleDropdownAndButton(bool view)
    {       
        dropdown.gameObject.SetActive(view);
        SureButton.gameObject.SetActive(view); // ��ʾ/���ش��Ͱ�ť
    }
    

    /// <summary>
    /// ���ͣ�����ģ����ʾ���֣�����δ�����
    /// </summary>
    /// <param name="targetTrans">����Ŀ��ص������λ��</param>
    /// <returns></returns>
    IEnumerator DelayedTeleport(Transform targetTrans)
    {        
        //����
        yield return new WaitForSeconds(0.5f); // �����ӳ�0.5�룬����Ը�����Ҫ����        
        thirdPersonCharacter.transform.position = targetTrans.position;
        thirdPersonCharacter.transform.forward = targetTrans.forward;

        // ����Cinemachine��Follow��LookAt����
        virtualCamera.Follow = thirdPersonCharacter.transform;
        virtualCamera.LookAt = thirdPersonCharacter.transform;
        virtualCamera.Follow = personRoot.transform;


        /*
         * ����ģ��Ч����post�еľ���
         */
        depthOfField.focusDistance.value = 0.2f; // ������Ҫ����

        // ��ʾ��������
        infoText.text = "��ӭ���� " + targetTrans.name;
        infoText.gameObject.SetActive(true);
        //������ȫ��͸��
        infoText.color = new Color(infoText.color.r, infoText.color.g, infoText.color.b, 1f);

        yield return new WaitForSeconds(1f);

        // �ָ�����Ч����������ʽ��
        DOVirtual.Float(0.2f, 10f, 1f, value => {
            depthOfField.focusDistance.value = value;
        });

        //���ֻ�����ʧ 
        infoText.DOFade(0f, 1f).OnComplete(() => {
            infoText.gameObject.SetActive(false);
        });
    }
    
}
