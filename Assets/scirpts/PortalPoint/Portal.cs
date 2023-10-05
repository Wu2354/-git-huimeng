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
            // 设置传送点ID
            PortalManager.Instance.LastUsedPortalID = gameObject.name;
            // 尝试将物体的名称转换为整数作为场景索引
            if (int.TryParse(gameObject.name, out int sceneIndex))
            {
                // 加载与这个物体名称相对应的场景
                SceneManager.LoadScene(sceneIndex);
            }
            else
            {
                // 如果转换失败，打印一条错误消息
                Debug.LogError("物体名称不是一个有效的整数: " + gameObject.name);
            }
        }
    }
}
