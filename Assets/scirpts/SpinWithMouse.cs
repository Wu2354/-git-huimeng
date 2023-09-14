using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinWithMouse : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 lastMousePosition;

    [SerializeField] float rotationSpeed = 1.0f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 mouseOffset = currentMousePosition - lastMousePosition;

            float rotationY = mouseOffset.x * rotationSpeed;

            transform.Rotate(Vector3.up, -rotationY, Space.World);

            lastMousePosition = currentMousePosition;
        }
    }
}
