using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    /// <summary>
    /// 对话文本文件
    /// </summary>
    [SerializeField] TextAsset dialogDataFile;
    /// <summary>
    /// 左右侧角色图像
    /// </summary>
    [SerializeField] Image imageLeft;
    [SerializeField] Image imageRight;
    /// <summary>
    /// 角色名称文本
    /// </summary>
    [SerializeField] TMP_Text nameText;
    /// <summary>
    /// 对话内容文本
    /// </summary>
    [SerializeField] TMP_Text dialogText;
    /// <summary>
    /// 角色图片列表
    /// </summary>
    [SerializeField] List<Sprite> images = new List<Sprite>();
    /// <summary>
    /// 角色名字对应图片的字典
    /// </summary>
    Dictionary<string, Sprite> imageDic = new Dictionary<string, Sprite>();

    private void Awake()
    {
        imageDic["凌光"] = images[0];
        imageDic["甘雨"] = images[1];
    }
    void Start()
    {
        //UpdateText("凌光", "欢迎来到学堂");
        //UpdateImage("凌光", true);
        ReadText(dialogDataFile);
    }
        
    void Update()
    {
        
    }

    private void UpdateText(string _name, string _text)
    {
        nameText.text = _name;
        dialogText.text = _text;
    }

    private void UpdateImage(string _name, bool _atLeft)
    {
        if(_atLeft)
        {
            imageLeft.sprite = imageDic[_name];
        }
        else
        {
            imageRight.sprite = imageDic[_name];
        }
    }
    public void ReadText(TextAsset _textAsset)
    {
        string[] rows = _textAsset.text.Split('\n');
        foreach (var row in rows)
        {
            string[] cell = row.Split(',');
        }
        Debug.Log("读取成功");
    } 
}
