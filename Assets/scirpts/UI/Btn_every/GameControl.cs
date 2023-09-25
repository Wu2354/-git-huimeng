using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    public Button exitButton; // 按钮引用，用于退出游戏
    public Button reloadButton; // 按钮引用，用于重新加载游戏
    private Button button;
    public GameObject btn_item;//中间元素开关
    private bool isOpen = false;

    private void Start()
    {
        // 将退出和重新加载的方法挂钩到按钮的点击事件上
        exitButton.onClick.AddListener(ExitGame);
        reloadButton.onClick.AddListener(ReloadGame);

        button = GetComponent<Button>();
        button.onClick.AddListener(ControlItem);
    }

    // 退出游戏的方法
    private void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode(); // 在编辑器中停止播放
#else
            Application.Quit(); // 在实际游戏中退出
#endif
    }

    // 重新加载游戏的方法
    private void ReloadGame()
    {
        // 重新加载当前场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ControlItem()
    {
        isOpen = !isOpen;
        btn_item.SetActive(isOpen);
    }
}
