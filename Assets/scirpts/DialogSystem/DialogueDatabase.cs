using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueDatabase", menuName = "Dialogue/New Dialogue Database")]
public class DialogueDatabase : ScriptableObject
{
    [SerializeField] private List<Person> persons = new List<Person>();
    // ��ӻ�ȡ�Ի��ļ��ķ���������NPC�����ֻ���������
    public TextAsset GetDialogueForNPC(string npcName)
    {
        Person person = persons.Find(de => de.npcName == npcName && de.isAvailable);
        return person?.dialogFile;
    }
}
