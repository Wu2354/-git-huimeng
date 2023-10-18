using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [Tooltip("�˴��͵��ΨһID�����ڱ�ʶ���ڴ��͵㡣")]
    public string portalID;
    public LoadingPanel loadingPanel;

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ���ô��͵�ID
            PortalManager.Instance.LastUsedPortalID = portalID;
            // ���Խ����������ת��Ϊ������Ϊ��������
            if (int.TryParse(gameObject.name, out int sceneIndex))
            {
                if(sceneIndex == 2)
                {
                    LoadingPanel.SetAsyncLoading(true);                    
                    LoadingPanel.SetSceneToLoad(sceneIndex);
                }
                else
                {
                    LoadingPanel.SetAsyncLoading(false);
                    LoadingPanel.SetSceneToLoad(sceneIndex);
                }
                SceneManager.LoadScene("Loading");

            }
            else
            {
                // ���ת��ʧ�ܣ���ӡһ��������Ϣ
                Debug.LogError("�������Ʋ���һ����Ч������: " + gameObject.name);
            }

            
            
        }
    }
}
