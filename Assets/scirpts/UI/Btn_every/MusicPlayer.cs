using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource audioSource; // 用于播放音乐的 AudioSource 组件
    public AudioClip[] musicClips; // 音乐剪辑数组
    public Slider volumeSlider; // 用于调整音量的滑块
    public TextMeshProUGUI volumeText; // 显示当前音量的文本
    public Button playPauseButton; // 播放/暂停按钮
    public Button nextButton; // 下一首按钮
    public Button previousButton; // 上一首按钮

    private int currentTrack = 0; // 当前播放的音轨索引
    private bool isPlaying = true; // 是否正在播放音乐

    public GameObject btn_item;//中间元素开关
    private bool isOpen = false;
    private Button button;

    private void Start()
    {
        // 为按钮添加监听器
        playPauseButton.onClick.AddListener(TogglePlayPause);
        nextButton.onClick.AddListener(NextTrack);
        previousButton.onClick.AddListener(PreviousTrack);

        // 为音量滑块添加监听器
        volumeSlider.onValueChanged.AddListener(UpdateVolume);

        // 播放初始音轨
        PlayTrack(currentTrack);

        button = GetComponent<Button>();
        button.onClick.AddListener(ControlItem);
    }

    private void PlayTrack(int trackIndex)
    {
        audioSource.clip = musicClips[trackIndex];
        audioSource.Play();
    }

    private void UpdateVolume(float value)
    {
        audioSource.volume = value; // 调整音量
        volumeText.text = (value * 100).ToString("0"); // 显示当前音量百分比
    }
    //播放停止/开启
    private void TogglePlayPause()
    {
        isPlaying = !isPlaying; // 切换播放状态

        if (isPlaying)
        {
            audioSource.UnPause();
        }
        else
        {
            audioSource.Pause();
        }
    }

    private void NextTrack()
    {
        // 播放下一首音乐，如果是最后一首，则从第一首开始
        currentTrack = (currentTrack + 1) % musicClips.Length;
        PlayTrack(currentTrack);
    }
    
    private void PreviousTrack()
    {
        // 播放上一首音乐，如果是第一首，则播放最后一首
        currentTrack--;
        if (currentTrack < 0)
        {
            currentTrack = musicClips.Length - 1;
        }
        PlayTrack(currentTrack);
    }


    //控制中间元素显示
    private void ControlItem()
    {
        isOpen = !isOpen;
        btn_item.SetActive(isOpen);
    }    
}
