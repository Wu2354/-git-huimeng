using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    public Button exitButton; // ��ť���ã������˳���Ϸ
    public Button reloadButton; // ��ť���ã��������¼�����Ϸ
    private Button button;
    public GameObject btn_item;//�м�Ԫ�ؿ���
    private bool isOpen = false;

    private void Start()
    {
        // ���˳������¼��صķ����ҹ�����ť�ĵ���¼���
        exitButton.onClick.AddListener(ExitGame);
        reloadButton.onClick.AddListener(ReloadGame);

        button = GetComponent<Button>();
        button.onClick.AddListener(ControlItem);
    }

    // �˳���Ϸ�ķ���
    private void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode(); // �ڱ༭����ֹͣ����
#else
            Application.Quit(); // ��ʵ����Ϸ���˳�
#endif
    }

    // ���¼�����Ϸ�ķ���
    private void ReloadGame()
    {
        // ���¼��ص�ǰ����
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ControlItem()
    {
        isOpen = !isOpen;
        btn_item.SetActive(isOpen);
    }
}
