using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalManager : MonoBehaviour
{
    public static PortalManager Instance { get; private set; }
    
    [Tooltip("���������������ƣ����� 'MainScene'")]
    public string mainSceneName = "hm";

    // ���ڸ����ϴ�ʹ�õĴ��͵�
    public string LastUsedPortalID { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == mainSceneName) // ���
        {
            Portal exitPortal = FindPortalWithID(LastUsedPortalID);
            if (exitPortal)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player)
                {
                    player.transform.position = exitPortal.transform.position - exitPortal.transform.right * 2f;
                    player.transform.rotation = exitPortal.transform.rotation;
                }
            }
        }
    }

    private Portal FindPortalWithID(string id)
    {
        foreach (Portal portal in FindObjectsOfType<Portal>())
        {
            if (portal.portalID == id)
                return portal;
        }
        return null;
    }
}
