using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ElevatorController : MonoBehaviour
{
    public Dropdown floorDropdown;
    public float floorHeight = 3.65f;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private int currentFloor = 0;
    private Animator doorAnimator;

    // ¶¯»­´¥·¢Ãû×Ö
    private string isOpenBool = "isOpen";
    private string shouldPlayBool = "shouldPlay";

    private string doorProgressParam = "DoorProgress";


    private void Start()
    {
        doorAnimator = GetComponentInChildren<Animator>(); // Assuming Animator is attached to ElevatorDoors
        floorDropdown.onValueChanged.AddListener(MoveToFloor);
    }

    private void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, 2f * Time.deltaTime);

            if (transform.position == targetPosition)
            {
                isMoving = false;
                UpdateCurrentFloor();
                OpenDoors(); // Open doors when elevator stops
            }
        }
    }

    public void MoveToFloor(int floorNumber)
    {
        if (!isMoving)
        {
            CloseDoors(); // Close doors before moving
            targetPosition = new Vector3(transform.position.x, floorNumber * floorHeight, transform.position.z);
            isMoving = true;
        }
    }

    private void UpdateCurrentFloor()
    {
        currentFloor = Mathf.RoundToInt(transform.position.y / floorHeight);
    }

    public void ToggleDoor(bool open)
    {
        if (doorAnimator == null) return;

        doorAnimator.SetBool(shouldPlayBool, true);

        if (open)
        {
            doorAnimator.SetBool(isOpenBool, true);
        }
        else
        {
            doorAnimator.SetBool(isOpenBool, false);
        }
    }

    private IEnumerator AnimateDoors(float target)
    {
        float currentProgress = doorAnimator.GetFloat(doorProgressParam);
        float duration = 1f; // The time it takes for the door to fully open/close
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            float elapsed = Time.time - startTime;
            doorAnimator.SetFloat(doorProgressParam, Mathf.Lerp(currentProgress, target, elapsed / duration));
            yield return null;
        }

        doorAnimator.SetFloat(doorProgressParam, target);
    }

    private void OpenDoors()
    {
        StartCoroutine(AnimateDoors(1f)); // Open
    }

    private void CloseDoors()
    {
        StartCoroutine(AnimateDoors(0f)); // Close
    }
}
