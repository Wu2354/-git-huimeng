using UnityEngine;

public class Bird : MonoBehaviour
{

    public float speed = 5f;
    public Transform center;

    void Update()
    {

        // ���������ĵ�����
        Vector3 direction = transform.position - center.position;

        // ��ȡ��ת��
        Vector3 rotateAxis = Vector3.Cross(direction, transform.right);
        if (rotateAxis.y < 0)
        {
            rotateAxis = -rotateAxis;
        }

        // Χ��������ת
        transform.RotateAround(center.position, rotateAxis, speed * Time.deltaTime);

        // ������з���
        transform.LookAt(center);
    }

}