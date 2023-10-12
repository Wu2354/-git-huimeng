using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructBook : MonoBehaviour
{
    [SerializeField] Button rBtn;
    [SerializeField] Button lBtn;
    [SerializeField] Button instrcBtn;
    bool isShow = false;    
    [SerializeField] List<Sprite> images = new List<Sprite>();
    [SerializeField] GameObject backImgeObj;
    [SerializeField] RawImage showImage;
    int currentIndex = 0;
    void Start()
    {
        instrcBtn.onClick.AddListener(OnActive);
        rBtn.onClick.AddListener(() => OnShow(true));
        lBtn.onClick.AddListener(() => OnShow(false));
    }
        
    void OnActive() 
    {
        isShow = !isShow;
        backImgeObj.SetActive(isShow);
    }

    void OnShow(bool isNext)
    {
        if(isNext == true)
        {
            currentIndex = (currentIndex + 1) % images.Count;
        }
        else
        {
            currentIndex = (currentIndex -1 +images.Count) % images.Count;
        }

        showImage.texture = images[currentIndex].texture;
    }
}
