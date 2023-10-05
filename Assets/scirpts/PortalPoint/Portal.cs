using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [Tooltip("�˴��͵��ΨһID�����ڱ�ʶ���ڴ��͵㡣")]
    public string portalID;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ���ô��͵�ID
            PortalManager.Instance.LastUsedPortalID = portalID;
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
