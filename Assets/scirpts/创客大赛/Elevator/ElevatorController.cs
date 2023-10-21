using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Animator))]
public class ElevatorController : MonoBehaviour
{
    [SerializeField] float elevatorSpeed = 3f;
    [SerializeField] TMP_Dropdown floorDropdown;
    [SerializeField] float floorHeight = 3.65f;

    private Vector3 initialPosition;
    private Animator doorAnimator;
    private bool isPlayerInside = false;

    // ������
    private string openDoorAnimation = "OpenElevator";
    private string closeDoorAnimation = "CloseElevator";

    private void Start()
    {
        initialPosition = transform.position;
        doorAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isPlayerInside && Input.GetKeyDown(KeyCode.E))  // E��Ϊȷ�ϼ������Ը�����Ҫ����
        {
            int selectedFloor = floorDropdown.value + 1;  // +1��Ϊ¥���1��ʼ
            MoveElevator(selectedFloor);
        }
    }

    private void MoveElevator(int floor)
    {
        Vector3 targetPosition = initialPosition + new Vector3(0, floorHeight * (floor - 1), 0);
        StartCoroutine(MoveToPosition(targetPosition));
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        doorAnimator.Play(closeDoorAnimation);

        // �ȴ�һ��ʱ��ȷ���Źر�
        yield return new WaitForSeconds(2f);

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, elevatorSpeed * Time.deltaTime);
            yield return null;
        }

        doorAnimator.Play(openDoorAnimation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorAnimator.Play(openDoorAnimation);
            isPlayerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorAnimator.Play(closeDoorAnimation);
            isPlayerInside = false;
        }
    }
}
