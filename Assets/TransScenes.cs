using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransScenes : MonoBehaviour
{
    

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if(this.gameObject.name == "G1")
            {
                SceneManager.LoadScene(0);
            }else if(this.gameObject.name == "G2")
            {
                SceneManager.LoadScene(1);
            }else if (this.gameObject.name == "G3")
            {
                SceneManager.LoadScene(2);
            }
        }
    }
}
