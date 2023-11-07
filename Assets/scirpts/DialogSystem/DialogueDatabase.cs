using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueDatabase", menuName = "Dialogue/New Dialogue Database")]
public class DialogueDatabase : ScriptableObject
{
    [SerializeField] private List<Person> persons = new List<Person>();
    // 添加获取对话文件的方法，根据NPC的名字或其他属性
    public TextAsset GetDialogueForNPC(string npcName)
    {
        Person person = persons.Find(de => de.npcName == npcName && de.isAvailable);
        return person?.dialogFile;
    }
}
