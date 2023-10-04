using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Trans_ObjName_scene : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 检查碰撞物体是否有“Player”标签
        if (other.CompareTag("Player"))
        {
            // 尝试将物体的名称转换为整数
            if (int.TryParse(gameObject.name, out int sceneIndex))
            {
                // 加载与这个物体名称相对应的场景
                SceneManager.LoadScene(sceneIndex);
            }
            else
            {
                // 如果转换失败，打印一条错误消息
                Debug.LogError("场景名称不是一个有效的整数: " + gameObject.name);
            }
        }
    }
}
