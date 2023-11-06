using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcDialogTrriger : MonoBehaviour
{
    // 可以在Inspector中指定对话数据库
    [SerializeField] DialogueDatabase dialogueDatabase;
    [SerializeField] DialogManager dialogManager;

    // 对话触发区域标识
    private void OnTriggerEnter(Collider other)
    {
        // 检查触发对话的是玩家
        if (other.CompareTag("Player"))
        {
            // 从对话数据库中获取当前NPC的对话内容
            TextAsset dialogue = dialogueDatabase.GetDialogueForNPC(gameObject.name);

            // 检查是否成功获取到对话
            if (dialogue != null)
            {
                // 调用对话管理器来显示对话
               dialogManager.StartDialogue(dialogue);
            }
        }
    }

    // 当玩家离开触发区域时触发
    private void OnTriggerExit(Collider other)
    {
        // 确认是玩家离开触发区
        if (other.CompareTag("Player"))
        {
            // 调用对话管理器结束对话
            dialogManager.EndDialogue();
        }
    }
}
