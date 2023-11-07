using UnityEngine;

[System.Serializable]
public class Person 
{
    public string name;
    public int likeValue;
    public int strengthValue;
    public TextAsset dialogFile;//对话文件
    public bool isAvailable;//对话是否可用
}
