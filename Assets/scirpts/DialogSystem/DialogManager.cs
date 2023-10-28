using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    /// <summary>
    /// �Ի��ı��ļ�
    /// </summary>
    [SerializeField] TextAsset dialogDataFile;
    /// <summary>
    /// ���Ҳ��ɫͼ��
    /// </summary>
    [SerializeField] Image imageLeft;
    [SerializeField] Image imageRight;
    /// <summary>
    /// ��ɫ�����ı�
    /// </summary>
    [SerializeField] TMP_Text nameText;
    /// <summary>
    /// �Ի������ı�
    /// </summary>
    [SerializeField] TMP_Text dialogText;
    /// <summary>
    /// ��ɫͼƬ�б�
    /// </summary>
    [SerializeField] List<Sprite> images = new List<Sprite>();
    /// <summary>
    /// ��ɫ���ֶ�ӦͼƬ���ֵ�
    /// </summary>
    Dictionary<string, Sprite> imageDic = new Dictionary<string, Sprite>();

    private void Awake()
    {
        imageDic["���"] = images[0];
        imageDic["����"] = images[1];
    }
    void Start()
    {
        //UpdateText("���", "��ӭ����ѧ��");
        //UpdateImage("���", true);
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
        Debug.Log("��ȡ�ɹ�");
    } 
}
