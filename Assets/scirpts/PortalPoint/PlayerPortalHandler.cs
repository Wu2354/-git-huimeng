using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPortalHandler : MonoBehaviour
{
    private void Start()
    {
        if (PortalManager.Instance.LastUsedPortalID != null)
        {
            // ���Ҿ���ƥ��ID�Ĵ��͵�
            Portal matchingPortal = FindPortalWithID(PortalManager.Instance.LastUsedPortalID);
            if (matchingPortal)
            {
                transform.position = matchingPortal.transform.position + matchingPortal.transform.forward * 2f;
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
