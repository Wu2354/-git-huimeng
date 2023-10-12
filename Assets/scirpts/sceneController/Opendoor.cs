
using UnityEngine;
using DG.Tweening;


public class Opendoor : MonoBehaviour
{
    [Header("���ţ�����")]
    public Transform[] doorTransform;

    private Vector3 door0OriginalPos;
    private Vector3 door1OriginalPos;


    private void Start()
    {
        // ����Ϸ��ʼʱ��¼�ŵ�ԭʼλ��
        door0OriginalPos = doorTransform[0].position;
        door1OriginalPos = doorTransform[1].position;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            doorTransform[0].DOMove(doorTransform[0].position + new Vector3(0, 0, 7f), 5f);
            doorTransform[1].DOMove(doorTransform[1].position - new Vector3(0, 0, 7f), 5f);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            doorTransform[0].DOMove(door0OriginalPos, 5f);
            doorTransform[1].DOMove(door1OriginalPos, 5f);
        }
    }
}