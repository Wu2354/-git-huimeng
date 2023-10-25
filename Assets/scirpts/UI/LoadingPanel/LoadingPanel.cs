using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{

    [SerializeField] Image panelImage;
    private float currtProgress; //���ؽ���
    private static float loadTime = 3f; //(��ٵ�)����ʱ��
    [SerializeField] TextMeshProUGUI text; //�ٷֱ���ʾ
    private static bool isReally = true; //���������Ƿ��첽����
    AsyncOperation operation;
    private static int sceneToLoad; //��̬��������ȫ�ָı�

    void Start()
    {
        currtProgress = 0;
        panelImage.fillAmount = 0;

        //�첽����
        if (isReally)
        {
            operation = SceneManager.LoadSceneAsync(sceneToLoad); //�첽���صĽ��ȸ�ֵ
            operation.allowSceneActivation = false; //���Ƴ����Ƿ��ڼ�����Ϻ���������
        }
    }

    void OnAmountLineChage(float amount)
    {
        panelImage.fillAmount = amount;
        text.text = (panelImage.fillAmount * 100).ToString("F2")+ "%";
        
        if(panelImage.fillAmount >= 1) 
        {
            if (isReally)
            {
                operation.allowSceneActivation = true; //�첽���ؿ��Ƽ����
            }
            else
            {
                SceneManager.LoadScene(sceneToLoad);
            }            
        }
    } 

    void Update()
    {             
        if(!isReally) 
        {
            currtProgress += Time.deltaTime / loadTime;
            if (currtProgress > 1.0)
            {
                currtProgress = 1f;
            }
            OnAmountLineChage(currtProgress);
        }
        else
        {
            currtProgress = Mathf.Clamp01(operation.progress / 0.9f); //operation.progressֻ��0��0.9
            OnAmountLineChage(currtProgress);
        }       
    }

    //���������ű����øı䣨�쳡�����ù�static��
    public static void SetSceneToLoad(int sceneIndex)
    {
        sceneToLoad = sceneIndex;
    }

    public static void SetAsyncLoading(bool asyncLoading)
    {
        isReally = asyncLoading;
    }

    public static void SetLoadTime(float loadtime)
    {
        loadTime = loadtime;
    }
}
