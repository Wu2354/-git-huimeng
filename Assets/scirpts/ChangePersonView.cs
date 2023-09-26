
using UnityEngine;

public class ChangePersonView : MonoBehaviour
{
    [SerializeField] LayerMask boardLayer;//չ���ڵĲ�
    [SerializeField] float viewRaduis;//���뾶
    [SerializeField] GameObject firstPersonViewGO;
    [SerializeField] GameObject thirdPersonViewGO;    
    private bool isViewingBoard = false; //׷���Ƿ����ڲ鿴չ��    

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
