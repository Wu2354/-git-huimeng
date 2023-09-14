using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aaaa : MonoBehaviour
{
    public GameObject Menu;
    public GameObject TalkPlane;
    public GameObject Talk2;


    private bool MenuStatus = false;

    //Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if (Menu == true)
        //{
        //    Debug.Log("dadas");
        //}
        //if(Menu.activeSelf== false)
        //{
        //    Debug.Log("error");

        //}

        if (Menu.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            TalkPlane.SetActive(true);
            //Debug.Log("dhas");
            Time.timeScale = 0f;
            Talk2.SetActive(false);
        }

        if (TalkPlane.activeSelf == false)
        {
            Time.timeScale = 1f;
        }


    }

    public void OnTriggerEnter(Collider other)
    {
        MenuStatus = !MenuStatus;
        Menu.SetActive(true);
        //Debug.Log("chufa");
        //TalkPlane.SetActive(true );

    }


    public void OnTriggerExit(Collider other)
    {
        MenuStatus = false;
        Menu.SetActive(false);
        //TalkPlane.SetActive(false);
        Debug.Log("exit");
    }
}
