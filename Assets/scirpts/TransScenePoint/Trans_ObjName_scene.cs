using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Trans_ObjName_scene : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // �����ײ�����Ƿ��С�Player����ǩ
        if (other.CompareTag("Player"))
        {
            // ���Խ����������ת��Ϊ����
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
