using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadAvatar : MonoBehaviour
{
    [SerializeField] Transform geometry;
    [SerializeField] Avatar wumenAvatar;  
    [SerializeField] Avatar menAvatar;     
    [SerializeField] Animator TpersonAnimator;
    private GameObject target;

    void Start()
    {
        if (AvatarSys._instance.nowCount == 0)
        {
            AvatarSys._instance.GirlAvatar();
            target = AvatarSys._instance.GetGirlTarget();

            target.transform.SetParent(geometry);
            TpersonAnimator.avatar = wumenAvatar;  
        }
        else
        {
            AvatarSys._instance.BoyAvatar();
            target = AvatarSys._instance.GetBoyTarget();  

            target.transform.SetParent(geometry);
            TpersonAnimator.avatar = menAvatar;  
        }

        target.transform.localPosition = Vector3.zero;
        target.transform.localRotation = Quaternion.identity;
    }

}
