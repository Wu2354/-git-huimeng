using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarButton : MonoBehaviour
{
    private Toggle toggle;
    public void OnEnable()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnValueChanged);
    }
    public void OnValueChanged(bool isOn)
    {
        if (isOn)
        {
            if (gameObject.name == "boy" || gameObject.name == "girl")
            {
                AvatarSys._instance.SexChange();
                return;
            }
            string[] names = gameObject.name.Split('_');
            AvatarSys._instance.OnChangePeople(names[0], names[1]);
        }
    }
   
}
