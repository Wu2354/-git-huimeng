using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadAvatar : MonoBehaviour
{
    [SerializeField] Transform geometry;
    [SerializeField] Avatar wumenAvatar;
    [SerializeField] Animator TpersonAnimator;
    void Start()
    {
        AvatarSys._instance.GirlAvatar();
        GameObject target = AvatarSys._instance.GetGirlTarget();
        
        target.transform.SetParent(geometry);
        TpersonAnimator.avatar = wumenAvatar;
        target.transform.localPosition = Vector3.zero;
        target.transform.localRotation = Quaternion.identity;
    }
        
}
