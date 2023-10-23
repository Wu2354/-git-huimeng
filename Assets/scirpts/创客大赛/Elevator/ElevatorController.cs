using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Animator))]
public class ElevatorController : MonoBehaviour
{
    [SerializeField] float elevatorSpeed = 3f;
    [SerializeField] TMP_Dropdown floorDropdown;
    [SerializeField] GameObject dropDownObj;
    [SerializeField] float floorHeight = 3.65f;

    private Vector3 initialPosition;
    private Animator doorAnimator;
    private bool isPlayerInside = false;
    private int currentFloor = 1;  // 默认电梯在第一层

    // 动画名
    private string openDoorAnimation = "OpenElevator";
    private string closeDoorAnimation = "CloseElevator";

    private void Start()
    {
        initialPosition = transform.position;
        doorAnimator = transform.GetChild(0).GetComponent<Animator>();
        dropDownObj.SetActive(false);
    }
   
    private void MoveElevator(int floor)
    {
        Vector3 targetPosition = initialPosition + new Vector3(0, floorHeight * (floor - 1), 0);
        StartCoroutine(MoveToPosition(targetPosition)); 
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        doorAnimator.Play(closeDoorAnimation);

        // 等待一段时间确保门关闭
        yield return new WaitForSeconds(2f);

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, elevatorSpeed * Time.deltaTime);
            yield return null;
        }
        doorAnimator.Play(openDoorAnimation);

        // 更新电梯的当前楼层
        currentFloor = Mathf.RoundToInt((transform.position.y - initialPosition.y) / floorHeight) + 1;
    }

    public void MoveElevator()
    {
        int selectedFloor = floorDropdown.value + 1;
        if (selectedFloor != currentFloor)  // 只有当选择的楼层与当前楼层不同时，才移动电梯
        {
            Vector3 targetPosition = initialPosition + new Vector3(0, floorHeight * (selectedFloor - 1), 0);
            StartCoroutine(MoveToPosition(targetPosition));
        }
    }

    private void OnTriggerEnter(Collider other)
    {        
        if (other.CompareTag("Player"))
        {
            doorAnimator.Play(openDoorAnimation);
            isPlayerInside = true;
            ToggleDropdownDisplay();
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

    //切换dropdown的状态
    private void ToggleDropdownDisplay()
    {
        dropDownObj.SetActive(!dropDownObj.activeSelf);
    }
}
