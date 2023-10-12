
using UnityEngine;

public class ChangePersonView : MonoBehaviour
{
    [SerializeField] LayerMask boardLayer;//չ���ڵĲ�
    [SerializeField] float viewRaduis = 3f;//���뾶
    [SerializeField] float angle = 45f;//���Ƕ�
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
                return; // �ҵ�һ��չ�������������ڣ��˳�����
            }            
        }

        // ���ѭ����ɲ�û�з��أ�˵��û��չ��������������
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
