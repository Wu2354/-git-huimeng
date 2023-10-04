using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OkBtn : MonoBehaviour
{
    Button button;
    private void OnEnable()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Onclick);
    }

    private void Onclick()
    {
        SceneManager.LoadScene(1);
    }
}
