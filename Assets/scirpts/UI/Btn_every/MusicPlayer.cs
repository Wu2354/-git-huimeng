using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource audioSource; // ���ڲ������ֵ� AudioSource ���
    public AudioClip[] musicClips; // ���ּ�������
    public Slider volumeSlider; // ���ڵ��������Ļ���
    public TextMeshProUGUI volumeText; // ��ʾ��ǰ�������ı�
    public Button playPauseButton; // ����/��ͣ��ť
    public Button nextButton; // ��һ�װ�ť
    public Button previousButton; // ��һ�װ�ť

    private int currentTrack = 0; // ��ǰ���ŵ���������
    private bool isPlaying = true; // �Ƿ����ڲ�������

    public GameObject btn_item;//�м�Ԫ�ؿ���
    private bool isOpen = false;
    private Button button;

    private void Start()
    {
        // Ϊ��ť��Ӽ�����
        playPauseButton.onClick.AddListener(TogglePlayPause);
        nextButton.onClick.AddListener(NextTrack);
        previousButton.onClick.AddListener(PreviousTrack);

        // Ϊ����������Ӽ�����
        volumeSlider.onValueChanged.AddListener(UpdateVolume);

        // ���ų�ʼ����
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
        audioSource.volume = value; // ��������
        volumeText.text = (value * 100).ToString("0"); // ��ʾ��ǰ�����ٷֱ�
    }
    //����ֹͣ/����
    private void TogglePlayPause()
    {
        isPlaying = !isPlaying; // �л�����״̬

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
        // ������һ�����֣���������һ�ף���ӵ�һ�׿�ʼ
        currentTrack = (currentTrack + 1) % musicClips.Length;
        PlayTrack(currentTrack);
    }
    
    private void PreviousTrack()
    {
        // ������һ�����֣�����ǵ�һ�ף��򲥷����һ��
        currentTrack--;
        if (currentTrack < 0)
        {
            currentTrack = musicClips.Length - 1;
        }
        PlayTrack(currentTrack);
    }


    //�����м�Ԫ����ʾ
    private void ControlItem()
    {
        isOpen = !isOpen;
        btn_item.SetActive(isOpen);
    }    
}
