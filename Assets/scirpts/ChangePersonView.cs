
using UnityEngine;

public class ChangePersonView : MonoBehaviour
{
    [SerializeField] LayerMask boardLayer;//展板在的层
    [SerializeField] float viewRaduis = 3f;//检测半径
    [SerializeField] float angle = 45f;//检测角度
    [SerializeField] GameObject firstPersonViewGO;
    [SerializeField] GameObject thirdPersonViewGO;          

    private void Update()
    {
        CheckForBoardView();
    }
    private void CheckForBoardView()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + new Vector3(0, 1.8f), viewRaduis, boardLayer);
        foreach (Collider collider in hitColliders)
        {
            Vector3 toHitCollider = collider.transform.position - transform.position;
            float angleToCollider = Vector3.Angle(transform.forward, toHitCollider);

            if(angleToCollider <= angle)
            {
                PersonViewChange(true);
                return; // 找到一个展板在扇形区域内，退出方法
            }            
        }

        // 如果循环完成并没有返回，说明没有展板在扇形区域内
        PersonViewChange(false);
    }
    public void PersonViewChange(bool isViewingBoard)
    {       
        //IsFirstView = !IsFirstView;
        if (isViewingBoard)
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
