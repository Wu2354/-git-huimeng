using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcDialogTrriger : MonoBehaviour
{
    // ������Inspector��ָ���Ի����ݿ�
    [SerializeField] DialogueDatabase dialogueDatabase;
    [SerializeField] DialogManager dialogManager;

    // �Ի����������ʶ
    private void OnTriggerEnter(Collider other)
    {
        // ��鴥���Ի��������
        if (other.CompareTag("Player"))
        {
            // �ӶԻ����ݿ��л�ȡ��ǰNPC�ĶԻ�����
            TextAsset dialogue = dialogueDatabase.GetDialogueForNPC(gameObject.name);

            // ����Ƿ�ɹ���ȡ���Ի�
            if (dialogue != null)
            {
                // ���öԻ�����������ʾ�Ի�
               dialogManager.StartDialogue(dialogue);
            }
        }
    }

    // ������뿪��������ʱ����
    private void OnTriggerExit(Collider other)
    {
        // ȷ��������뿪������
        if (other.CompareTag("Player"))
        {
            // ���öԻ������������Ի�
            dialogManager.EndDialogue();
        }
    }
}
