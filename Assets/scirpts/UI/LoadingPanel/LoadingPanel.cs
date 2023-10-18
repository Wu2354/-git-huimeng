using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{

    [SerializeField] Image panelImage;
    private float currtProgress; //加载进度
    private float loadTime = 2f; //(虚假的)加载时间
    [SerializeField] TextMeshProUGUI text; //百分比显示
    public bool isReally = true; //用来调整是否异步加载
    AsyncOperation operation;
    private static int sceneToLoad; //静态变量可以全局改变

    void Start()
    {
        currtProgress = 0;
        panelImage.fillAmount = 0;

        //异步加载
        if (isReally)
        {
            operation = SceneManager.LoadSceneAsync(sceneToLoad); //异步加载的进度赋值
            operation.allowSceneActivation = false; //控制场景是否在加载完毕后立即激活
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
                operation.allowSceneActivation = true; //异步加载控制激活即可
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
            currtProgress = Mathf.Clamp01(operation.progress / 0.9f); //operation.progress只有0到0.9
            OnAmountLineChage(currtProgress);
        }

       
    }

    public static void SetSceneToLoad(int sceneIndex)
    {
        sceneToLoad = sceneIndex;
    }
}
