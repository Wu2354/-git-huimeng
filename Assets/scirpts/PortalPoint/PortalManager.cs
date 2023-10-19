using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalManager : MonoBehaviour
{
    public static PortalManager Instance { get; private set; }

    [Tooltip("您的主场景的名称，例如 'MainScene'")]
    public string mainSceneName = "hm";

    // 用于跟踪上次使用的传送点
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
        if (scene.name == mainSceneName) // 如果
        {
            StartCoroutine(DelayedPortalPlacement());
        }
    }

    private System.Collections.IEnumerator DelayedPortalPlacement()
    {
        yield return new WaitForSeconds(0.1f); // 等待0.1秒或更长时间，可以根据需要调整

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

    public Portal FindPortalWithID(string id)
    {
        foreach (Portal portal in FindObjectsOfType<Portal>())
        {
            if (portal.portalID == id)
                return portal;
        }
        return null;
    }
}
