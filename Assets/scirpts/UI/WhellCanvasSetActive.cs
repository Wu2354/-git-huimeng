using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhellCanvasSetActive : MonoBehaviour
{
    [SerializeField] private GameObject whellObject;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            whellObject.SetActive(!whellObject.activeSelf);
        }
    }
}
