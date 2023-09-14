using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIPanel : MonoBehaviour
{
    public static MainUIPanel Instance;

    public Material skyDay;

    public Material skyNight;
    public Button mDay,mNight;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        mDay.onClick.AddListener(() => { ChangeSkybox(skyDay); });
        // mNoon.onClick.Addlistener(() => { ChangeSkybox(skyNoon); });
        mNight.onClick.AddListener(() => { ChangeSkybox(skyNight); });
    }
    private void ChangeSkybox(Material newSkybox)
    {
        RenderSettings.skybox = newSkybox;
    }
}
