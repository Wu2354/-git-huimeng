using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarSys : MonoBehaviour
{
    public static AvatarSys _instance;

    // [1] Ů����װ
    private Transform girlSourceTrans;//��Դmodel��������ʹԤ������õ�ʱ����ǰ�洢Ԥ�������Ϣ
    private GameObject girlTarget; //�Ǽ����壬��װ����

    /*�ֵ��������model��ĸ���λ�������Ϣ������һ�ֵ䲻���ã���ͬ��λҲ�в�ͬ�ı�ţ�������Ƕ�ֵ�ķ���
    //��Ƕ�ǲ��Ҳ�λ����Ƕ�ǲ�λ��ĳһ��
    �˴��ֵ�洢����mesh�ϵ������Ϣ*/  

    //СŮ�����е���Դ��Ϣ   //��λ�����֣���λ��ţ���λ��Ӧ��skm
    private Dictionary<string, Dictionary<string, SkinnedMeshRenderer>> girlData = new Dictionary<string, Dictionary<string, SkinnedMeshRenderer>>();

    Transform[] girlHips; //СŮ��������Ϣ

    //��λ�����֣���λ��Ӧ��skm
    private Dictionary<string, SkinnedMeshRenderer> girlSmr = new Dictionary<string, SkinnedMeshRenderer>();// ��װ�������ϵ�skm��Ϣ
    //��ʼ����Ϣ
    private string[,] girlStr = new string[,] { { "Hair", "1" }, { "Bottom", "1" }, { "Footwear", "1" }, { "Top", "1" }, { "EyeLeft", "1" }, { "EyeRight", "1" }, { "Body", "1" }, { "Head", "1" } };


    // [2] �к���װ����Ů��ע�����
    private Transform boySourceTrans;//��Դmodel
    private GameObject boyTarget; //�Ǽ����壬��װ����  
    private Dictionary<string, Dictionary<string, SkinnedMeshRenderer>> boyData = new Dictionary<string, Dictionary<string, SkinnedMeshRenderer>>();

    Transform[] boyHips;
    //��λ�����֣���λ��Ӧ��skm
    private Dictionary<string, SkinnedMeshRenderer> boySmr = new Dictionary<string, SkinnedMeshRenderer>();// ��װ�������ϵ�skm��Ϣ
    //��ʼ����Ϣ
    private string[,] boyStr = new string[,] { { "Bottom", "1" }, { "Footwear", "1" }, { "Hair", "1" }, { "Top", "1" }, { "Eye", "1" }, { "Head", "1" }, { "Body", "1" } };

    public int nowCount = 0; // 0����СŮ����1 �к�
    public GameObject girlPanel;
    public GameObject boyPanel;

    void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(this); //��ɾ����Ϸ����
    }

    void Start()
    {
        BoyAvatar();
        GirlAvatar();
        boyTarget.AddComponent<SpinWithMouse>();                        
        girlTarget.AddComponent<SpinWithMouse>(); 
        boyTarget.SetActive(false);
    }
    public void GirlAvatar()
    {
        InstantiateGirl();
        SaveData(girlSourceTrans, girlData, girlTarget, girlSmr);
        InitAvatarGirl();
    }
    
    public void BoyAvatar()
    {
        InstantiateBoy();
        SaveData(boySourceTrans, boyData, boyTarget, boySmr);
        InitAvatarBoy();

    }

    void InstantiateGirl()
    {
        GameObject go = Instantiate(Resources.Load("wumen")) as GameObject; //������Դ����
        girlSourceTrans = go.transform;
        go.SetActive(false);
        girlTarget = Instantiate(Resources.Load("wumen_target")) as GameObject;
        girlHips = girlTarget.GetComponentsInChildren<Transform>();
    }

    void InstantiateBoy()
    {
        GameObject go = Instantiate(Resources.Load("man")) as GameObject; //������Դ����
        boySourceTrans = go.transform;
        go.SetActive(false);
        boyTarget = Instantiate(Resources.Load("man_target")) as GameObject;
        boyHips = boyTarget.GetComponentsInChildren<Transform>();
    }

    public GameObject GetGirlTarget()
    {
        return girlTarget;
    }

    public GameObject GetBoyTarget()
    {
        return boyTarget;
    }

    void SaveData(Transform souceTrans, Dictionary<string, Dictionary<string, SkinnedMeshRenderer>> data, GameObject target,
        Dictionary<string, SkinnedMeshRenderer> smr)
    {

        data.Clear();
        smr.Clear();

        if (souceTrans == null)
            return;

        SkinnedMeshRenderer[] parts = souceTrans.GetComponentsInChildren<SkinnedMeshRenderer>();// ����������������SkinnedMeshRenderer�����д洢
        foreach (var part in parts)
        {
            /*
              �������� "eyes-1"��names[0] ���� "eyes"���� names[1] ���� "1"��
              �������� "head-2"��names[0] ���� "head"���� names[1] ���� "2"��
              ÿһ��ѭ��part �� part==eyes-1������һ��part==head-1��names[0]�ͻ�ı��head����head-2��names[0]Ҳ��head��
            */
            string[] names = part.name.Split('_');//��part�����ָ�������������ġ�-�����Ž��в�ֳ�����string����name���Զ�ָ��Obj��

            if (!data.ContainsKey(names[0]))
            { //ÿ�α�����һ���µĲ�λ��names[0]Ϊÿһ����λ
                //�����±����ɶ�Ӧ��skm
                GameObject partGo = new GameObject();
                partGo.name = names[0];
                partGo.transform.parent = target.transform;

                smr.Add(names[0], partGo.AddComponent<SkinnedMeshRenderer>()); //�ѹ���target���ϵ�skm��Ϣ�洢����λֻ��¼һ��
                data.Add(names[0], new Dictionary<string, SkinnedMeshRenderer>());
            }
            data[names[0]].Add(names[1], part); //�����в�λ���������嶼���룬����eyes-2.3....��
        }

    }


    //ע��!!  ÿһ����λ�����ж������������Ҳ�ж�����ʣ�����bones��materials�����Դ���������������Ϣ�滻
    void ChangeMesh(string part, string num, Dictionary<string, Dictionary<string, SkinnedMeshRenderer>> data,
        Transform[] hips, Dictionary<string, SkinnedMeshRenderer> smr, string[,] str)
    { //���벿λ����ţ���data�����ȡ��Ӧ��skm 

        SkinnedMeshRenderer skm = data[part][num];//Ҫ�����Ĳ�λ

        //�Աȴ˲�λ����transform��girlTarget�����й�����Ϣ
        List<Transform> bones = new List<Transform>();
        foreach (var trans in skm.bones)
        {
            foreach (var bone in hips)
            {
                if (bone.name == trans.name)
                {
                    bones.Add(bone);
                    break;
                }
            }
        }
        //��װĳһ��λʵ�֣��滻��Ƥmesh�����д��Ĳ���,����
        smr[part].bones = bones.ToArray();//�󶨹���
        smr[part].materials = skm.materials;//�滻����
        smr[part].sharedMesh = skm.sharedMesh;//����mesh

        SaveData(part, num, str); //�������ݣ�ע��������SaveData��
    }

    void InitAvatarGirl()
    { //��ʼ���Ǽ�������mesh ���� ������Ϣ
        int length = girlStr.GetLength(0);//�������
        for (int i = 0; i < length; i++)
        {
            ChangeMesh(girlStr[i, 0], girlStr[i, 1], girlData, girlHips, girlSmr, girlStr); //�����·�
        }

    }

    void InitAvatarBoy()
    { //��ʼ���Ǽ�������mesh ���� ������Ϣ
        int length = boyStr.GetLength(0);//�������
        for (int i = 0; i < length; i++)
        {
            ChangeMesh(boyStr[i, 0], boyStr[i, 1], boyData, boyHips, boySmr, boyStr); //�����·�
        }

    }


    public void OnChangePeople(string part, string num)
    {
        if (nowCount == 0)
        { //girl
            ChangeMesh(part, num, girlData, girlHips, girlSmr, girlStr);
        }
        else
        {
            ChangeMesh(part, num, boyData, boyHips, boySmr, boyStr);
        }
    }

    public void SexChange()
    {
        if (nowCount == 0)
        {
            nowCount = 1;
            boyTarget.SetActive(true);
            girlTarget.SetActive(false);
            boyPanel.SetActive(true);
            girlPanel.SetActive(false);
        }
        else
        {
            nowCount = 0;
            boyTarget.SetActive(false);
            girlTarget.SetActive(true);
            boyPanel.SetActive(false);
            girlPanel.SetActive(true);
        }
    }

    //��������ʹ�õ�ģ�����ݣ�ͨ������ʵ�ֶೡ��Ӧ��
    void SaveData(string part, string num, string[,] str)
    { //��������
        int length = str.GetLength(0);//�������
        for (int i = 0; i < length; i++)
        {
            if (str[i, 0] == part)
            {
                str[i, 1] = num;
            }
        }
    }
}
