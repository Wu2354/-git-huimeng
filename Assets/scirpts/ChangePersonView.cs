using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

public class ChangePersonView : MonoBehaviour
{
    [SerializeField] GameObject firstPersonViewGO;

    [SerializeField] GameObject thirdPersonViewGO;
    private bool IsFirstView = true;    
    private void Start()
    {
       
    }

    private void Update()
    {
        if (firstPersonViewGO != null && thirdPersonViewGO != null)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Onclick();
            }
        }
        
    }
    public void Onclick()
    {       
        IsFirstView = !IsFirstView;
        if (IsFirstView)
        {           
            firstPersonViewGO.SetActive(true);
            thirdPersonViewGO.SetActive(false);
        }
        else
        {
            firstPersonViewGO.SetActive(false);
            thirdPersonViewGO.SetActive(true);            
        }

    }
}
