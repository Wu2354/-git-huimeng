using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarSys : MonoBehaviour
{
    public static AvatarSys _instance;

    // [1] 女孩换装
    private Transform girlSourceTrans;//资源model，用来在使预制体禁用的时候提前存储预制体的信息
    private GameObject girlTarget; //骨架物体，换装的人

    /*字典用来存放model里的各部位的组件信息，但单一字典不够用（不同部位也有不同的编号），用套嵌字典的方法
    //外嵌是查找部位，内嵌是部位的某一种
    此处字典存储所有mesh上的组件信息*/  

    //小女孩所有的资源信息   //部位的名字，部位编号，部位对应的skm
    private Dictionary<string, Dictionary<string, SkinnedMeshRenderer>> girlData = new Dictionary<string, Dictionary<string, SkinnedMeshRenderer>>();

    Transform[] girlHips; //小女孩骨骼信息

    //部位的名字，部位对应的skm
    private Dictionary<string, SkinnedMeshRenderer> girlSmr = new Dictionary<string, SkinnedMeshRenderer>();// 换装骨骼身上的skm信息
    //初始化信息
    private string[,] girlStr = new string[,] { { "Hair", "1" }, { "Bottom", "1" }, { "Footwear", "1" }, { "Top", "1" }, { "EyeLeft", "1" }, { "EyeRight", "1" }, { "Body", "1" }, { "Head", "1" } };


    // [2] 男孩换装，据女孩注释理解
    private Transform boySourceTrans;//资源model
    private GameObject boyTarget; //骨架物体，换装的人  
    private Dictionary<string, Dictionary<string, SkinnedMeshRenderer>> boyData = new Dictionary<string, Dictionary<string, SkinnedMeshRenderer>>();

    Transform[] boyHips;
    //部位的名字，部位对应的skm
    private Dictionary<string, SkinnedMeshRenderer> boySmr = new Dictionary<string, SkinnedMeshRenderer>();// 换装骨骼身上的skm信息
    //初始化信息
    private string[,] boyStr = new string[,] { { "Bottom", "1" }, { "Footwear", "1" }, { "Hair", "1" }, { "Top", "1" }, { "Eye", "1" }, { "Head", "1" }, { "Body", "1" } };

    public int nowCount = 0; // 0代表小女孩，1 男孩
    public GameObject girlPanel;
    public GameObject boyPanel;

    void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(this); //不删除游戏物体
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
        GameObject go = Instantiate(Resources.Load("wumen")) as GameObject; //加载资源物体
        girlSourceTrans = go.transform;
        go.SetActive(false);
        girlTarget = Instantiate(Resources.Load("wumen_target")) as GameObject;
        girlHips = girlTarget.GetComponentsInChildren<Transform>();
    }

    void InstantiateBoy()
    {
        GameObject go = Instantiate(Resources.Load("man")) as GameObject; //加载资源物体
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

        SkinnedMeshRenderer[] parts = souceTrans.GetComponentsInChildren<SkinnedMeshRenderer>();// 遍历所有子物体有SkinnedMeshRenderer，进行存储
        foreach (var part in parts)
        {
            /*
              对于名字 "eyes-1"，names[0] 会是 "eyes"，而 names[1] 会是 "1"。
              对于名字 "head-2"，names[0] 会是 "head"，而 names[1] 会是 "2"。
              每一次循环part 例 part==eyes-1，而下一次part==head-1，names[0]就会改变成head，而head-2，names[0]也是head。
            */
            string[] names = part.name.Split('_');//将part的名字根据其中名字里的‘-’符号进行拆分成两个string，（name会自动指向Obj）

            if (!data.ContainsKey(names[0]))
            { //每次遍历到一个新的部位，names[0]为每一个部位
                //骨骼下边生成对应的skm
                GameObject partGo = new GameObject();
                partGo.name = names[0];
                partGo.transform.parent = target.transform;

                smr.Add(names[0], partGo.AddComponent<SkinnedMeshRenderer>()); //把骨骼target身上的skm信息存储，部位只记录一次
                data.Add(names[0], new Dictionary<string, SkinnedMeshRenderer>());
            }
            data[names[0]].Add(names[1], part); //将所有部位的种类物体都放入，例如eyes-2.3....等
        }

    }


    //注意!!  每一个部位可能有多个骨骼关联，也有多个材质，故用bones和materials两个自带变量包含所有信息替换
    void ChangeMesh(string part, string num, Dictionary<string, Dictionary<string, SkinnedMeshRenderer>> data,
        Transform[] hips, Dictionary<string, SkinnedMeshRenderer> smr, string[,] str)
    { //传入部位，编号，从data里边拿取对应的skm 

        SkinnedMeshRenderer skm = data[part][num];//要更换的部位

        //对比此部位骨骼transform和girlTarget的所有骨骼信息
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
        //换装某一部位实现，替换蒙皮mesh，所有带的材质,骨骼
        smr[part].bones = bones.ToArray();//绑定骨骼
        smr[part].materials = skm.materials;//替换材质
        smr[part].sharedMesh = skm.sharedMesh;//更换mesh

        SaveData(part, num, str); //保存数据（注意有两个SaveData）
    }

    void InitAvatarGirl()
    { //初始化骨架让他有mesh 材质 骨骼信息
        int length = girlStr.GetLength(0);//获得行数
        for (int i = 0; i < length; i++)
        {
            ChangeMesh(girlStr[i, 0], girlStr[i, 1], girlData, girlHips, girlSmr, girlStr); //穿上衣服
        }

    }

    void InitAvatarBoy()
    { //初始化骨架让他有mesh 材质 骨骼信息
        int length = boyStr.GetLength(0);//获得行数
        for (int i = 0; i < length; i++)
        {
            ChangeMesh(boyStr[i, 0], boyStr[i, 1], boyData, boyHips, boySmr, boyStr); //穿上衣服
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

    //用来保存使用的模型数据，通过单例实现多场景应用
    void SaveData(string part, string num, string[,] str)
    { //更改数据
        int length = str.GetLength(0);//获得行数
        for (int i = 0; i < length; i++)
        {
            if (str[i, 0] == part)
            {
                str[i, 1] = num;
            }
        }
    }
}
