using UnityEngine;

public class Bird : MonoBehaviour
{

    public float speed = 5f;
    public Transform center;

    void Update()
    {

        // 计算鸟到中心的向量
        Vector3 direction = transform.position - center.position;

        // 获取旋转轴
        Vector3 rotateAxis = Vector3.Cross(direction, transform.right);
        if (rotateAxis.y < 0)
        {
            rotateAxis = -rotateAxis;
        }

        // 围绕中心旋转
        transform.RotateAround(center.position, rotateAxis, speed * Time.deltaTime);

        // 面向飞行方向
        transform.LookAt(center);
    }

}