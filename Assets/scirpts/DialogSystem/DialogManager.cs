using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Text.RegularExpressions;

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
    /// <summary>
    /// 当前的对话索引
    /// </summary>
    [SerializeField] int dialogIndex;
    /// <summary>
    /// 对话文本按行分割
    /// </summary>
    [SerializeField] string[] dialogRows;
    /// <summary>
    /// 继续对话按钮
    /// </summary>
    [SerializeField] Button nextButton;
    /// <summary>
    /// 选项按钮预制体
    /// </summary>
    [SerializeField] GameObject optionButton;
    /// <summary>
    /// 选项按钮父节点，用于自动排列。
    /// </summary>
    [SerializeField] Transform buttonGroup;
    /// <summary>
    /// 动态跟踪NPC在游戏中的状态,例如好感度和体力值
    /// </summary>
    public List<Person> people = new List<Person>();
    /// <summary>
    /// 对话面板UI根物体
    /// </summary>
    [SerializeField] GameObject dialogUI;
    /// <summary>
    /// 判断是否在对话状态
    /// </summary>
    private bool isInDialogue = false;

    private void Awake()
    {
        imageDic["凌光"] = images[0];
        imageDic["甘雨"] = images[1];
        
        Person person1 = new Person();
        person1.npcName = "凌光";
        people.Add(person1);

        Person person2 = new Person();
        person2.npcName = "甘雨";
        people.Add(person2);
    }
    void Start()
    {       
        //ReadText(dialogDataFile);
        ShowDialogRow();
    }
               
    private void UpdateText(string _name, string _text)
    {
        nameText.text = _name;
        dialogText.text = _text;
    }

    private void UpdateImage(string _name,string _position)
    {
        if(_position == "左")
        {
            imageLeft.sprite = imageDic[_name];
        }
        else if(_position == "右")
        {
            imageLeft.sprite = imageDic[_name];
        }
    }
    public void ReadText(TextAsset _textAsset)
    {
        dialogRows = _textAsset.text.Split('\n');
        //foreach (var row in rows)
        //{
        //    string[] cell = row.Split(',');            
        //}
        Debug.Log("读取成功");
    } 


    /// <summary>
    /// 文本判断，功能添加都在这里
    /// </summary>
    public void ShowDialogRow()
    {
        for(int i = 0; i < dialogRows.Length; i++)
        {
            string[] cells = dialogRows[i].Split(',');
            if (cells[0]== "#" && int.Parse(cells[1]) == dialogIndex)
            {
                UpdateText(cells[2], cells[4]);
                UpdateImage(cells[2], cells[3]);

                dialogIndex = int.Parse(cells[5]);
                break;
            }
            else if (cells[0] == "@" && int.Parse(cells[1]) == dialogIndex)
            {
                UpdateImage(cells[2], cells[3]);
                nextButton.gameObject.SetActive(false);
                GenerateOption(i);
            }
            else if (cells[0] == "END" && int.Parse(cells[1]) == dialogIndex)
            {
                EndDialogue();
            }
        }
    }
        
    public void OnclickNext()
    {
        ShowDialogRow();        
    }

    public void GenerateOption(int _index)
    {
        string[] cells = dialogRows[_index].Split(',');
        if (cells[0] == "@")
        {
            GameObject button = Instantiate(optionButton, buttonGroup);
            //绑定按钮事件
            button.GetComponentInChildren<TMP_Text>().text = cells[4];

            
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnOptionClick(int.Parse(cells[5]));
                //按钮分支选择效果的添加（搭配对话文本实现）
                /*if (cells.Length > 6 && cells[6] != null)
                {
                    string[] effect = cells[6].Split("@");
                    cells[7] = Regex.Replace(cells[7], @"[\r\n]", "");
                    OptionEffect(effect[0], int.Parse(effect[1]), cells[7]);
                }*/
            });
            GenerateOption(_index + 1);
        }        
    }   

    public void OnOptionClick(int _id)
    {
        dialogIndex = _id;
        ShowDialogRow();
        for(int i = 0; i< buttonGroup.childCount; i++)
        {
            Destroy(buttonGroup.GetChild(i).gameObject);
        }
        nextButton.gameObject.SetActive(true );
    }

    private void OptionEffect(string _effect,  int _param, string _target)
    {
        if(_effect == "好感度增加")
        {
            foreach(var person in people)
            {
                if(person.npcName == _target)
                {
                    person.likeValue += _param;
                }
            }
        }
        else if(_effect == "体力值增加")
        {
            foreach (var person in people)
            {
                if (person.npcName== _target)
                {
                    person.strengthValue += _param;
                }
            }
        }
    }
    public void StartDialogue(TextAsset dialogText)
    {
        //如果玩家不在对话中，初始化对话
        if(!isInDialogue)
        {
            ReadText(dialogText);
            dialogIndex = 0;
            isInDialogue = true;
        }
        
        dialogUI.SetActive(true); // 激活对话界面
        ShowDialogRow(); // 显示当前对话行
    }
    
    public void EndDialogue()
    {
        dialogUI.SetActive(false); // 关闭对话界面
        isInDialogue = false;
        // 清理或重置其他对话相关的状态
        Debug.Log("对话结束");       
    }
}
