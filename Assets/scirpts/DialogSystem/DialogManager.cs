using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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
    /// <summary>
    /// ��ǰ�ĶԻ�����
    /// </summary>
    [SerializeField] int dialogIndex;
    /// <summary>
    /// �Ի��ı����зָ�
    /// </summary>
    [SerializeField] string[] dialogRows;
    /// <summary>
    /// �����Ի���ť
    /// </summary>
    [SerializeField] Button nextButton;
    /// <summary>
    /// ѡ�ťԤ����
    /// </summary>
    [SerializeField] GameObject optionButton;
    /// <summary>
    /// ѡ�ť���ڵ㣬�����Զ����С�
    /// </summary>
    [SerializeField] Transform buttonGroup;

    public List<Person> people = new List<Person>();

    private void Awake()
    {
        imageDic["���"] = images[0];
        imageDic["����"] = images[1];
        
        Person person1 = new Person();
<<<<<<< HEAD
        person1.npcName = "���";
        people.Add(person1);

        Person person2 = new Person();
        person2.npcName = "����";
=======
        person1.name = "���";
        people.Add(person1);

        Person person2 = new Person();
        person2.name = "����";
>>>>>>> bcd55077721ec647186aec230b4a9251804aea71
        people.Add(person2);
    }
    void Start()
    {       
        ReadText(dialogDataFile);
        ShowDialogRow();
    }
        
    void Update()
    {
        
    }

    private void UpdateText(string _name, string _text)
    {
        nameText.text = _name;
        dialogText.text = _text;
    }

    private void UpdateImage(string _name,string _position)
    {
        if(_position == "��")
        {
            imageLeft.sprite = imageDic[_name];
        }
        else if(_position == "��")
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
        Debug.Log("��ȡ�ɹ�");
    } 


    /// <summary>
    /// �ı��жϣ��������Ӷ�������
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
                Debug.Log("�Ի�����");
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
            //�󶨰�ť�¼�
            button.GetComponentInChildren<TMP_Text>().text = cells[4];
<<<<<<< HEAD

            
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnOptionClick(int.Parse(cells[5]));
                //��ť��֧ѡ��Ч�������ӣ�����Ի��ı�ʵ�֣�
                /*if (cells.Length > 6 && cells[6] != null)
                {
                    string[] effect = cells[6].Split("@");
                    cells[7] = Regex.Replace(cells[7], @"[\r\n]", "");
                    OptionEffect(effect[0], int.Parse(effect[1]), cells[7]);
                }*/
            });
=======
            button.GetComponent<Button>().onClick.AddListener( 
                ()=> OnOptionClick( int.Parse(cells[5])); 
            if(cells[6] != null)
            {

            }
            
            );
>>>>>>> bcd55077721ec647186aec230b4a9251804aea71
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

    public void OptionEffect(string _effect,  int _param, string _target)
    {
        if(_effect == "�øж�����")
        {
            foreach(var person in people)
            {
                if(person.npcName == _target)
                {
                    person.likeValue += _param;
                }
            }
        }
        else if(_effect == "����ֵ����")
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
}
