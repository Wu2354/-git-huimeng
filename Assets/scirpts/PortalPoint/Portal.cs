using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{      
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ���ô��͵�ID
            PortalManager.Instance.LastUsedPortalID = gameObject.name;
            // ���Խ����������ת��Ϊ������Ϊ��������
            if (int.TryParse(gameObject.name, out int sceneIndex))
            {
                // ��������������������Ӧ�ĳ���
                SceneManager.LoadScene(sceneIndex);
            }
            else
            {
                // ���ת��ʧ�ܣ���ӡһ��������Ϣ
                Debug.LogError("�������Ʋ���һ����Ч������: " + gameObject.name);
            }
        }
    }
}
