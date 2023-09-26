
using UnityEngine;

public class ChangePersonView : MonoBehaviour
{
    [SerializeField] LayerMask boardLayer;//展板在的层
    [SerializeField] float viewRaduis;//检测半径
    [SerializeField] GameObject firstPersonViewGO;
    [SerializeField] GameObject thirdPersonViewGO;    
    private bool isViewingBoard = false; //追踪是否正在查看展板    

    private void Update()
    {
        CheckForBoardView();
    }
    private void CheckForBoardView()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + new Vector3(0, 1.8f), viewRaduis, boardLayer);
        foreach (Collider collider in colliders)
        {
            Vector3 toHitCollider = collider.transform.position - transform.position;
        }
    }
    public void PersonViewChange()
    {       
        //IsFirstView = !IsFirstView;
        if (!isViewingBoard)
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
