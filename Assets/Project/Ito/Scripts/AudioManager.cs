using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("ボールが跳ねる音")]
    [SerializeField] AudioClip BounceClip;
    [SerializeField][Range(0f, 1f)] float BounceVolume = 1f;

    [Header("パワーがマックスの時に鳴る音")]
    [SerializeField] AudioClip FullPowerClip;
    [SerializeField][Range(0f, 1f)] float FullPowerVolume = 1f;

    [Header("ゴールしたときの音")]
    [SerializeField] AudioClip GoalClip;
    [SerializeField][Range(0f, 1f)] float GoalVolume = 1f;

    [SerializeField] AudioSource sfxSource;

    [Header("BGM")]
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioClip titleBGM;
    [SerializeField] AudioClip gameBGM;
    [SerializeField] AudioClip gameOverBGM;

    public static AudioManager Instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBGM();
    }

    // シーン名に応じてBGMが変わる
    private void UpdateBGM()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        AudioClip clipToPlay = null;

        switch (currentSceneName)
        {
            case "Title":
                clipToPlay = titleBGM;
                break;
            case "Stage1":
                clipToPlay = gameBGM;
                break;
            case "Stage2":
                clipToPlay = gameBGM;
                break;
            case "Stage3":
                clipToPlay = gameBGM;
                break;
            case "Stage4":
                clipToPlay = gameBGM;
                break;
            case "Stage5":
                clipToPlay = gameBGM;
                break;
            case "Stage6":
                clipToPlay = gameBGM;
                break;
            case "GameOver":
                clipToPlay = gameOverBGM;
                break;
        }

        // 選択されたBGMが現在のBGMと異なる場合、または再生されていない場合に新しいBGMを再生
        if (clipToPlay != null && (bgmSource.clip != clipToPlay || !bgmSource.isPlaying))
        {
            bgmSource.clip = clipToPlay;
            bgmSource.Play();
        }
    }

    //SEを再生する変数
    private void PlayClip(AudioClip clip, float volume)
    {

        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    public void PlayBounceClip()
    {
        PlayClip(BounceClip, BounceVolume);
    }

    public void PlayFullPowerClip()
    {
        PlayClip(FullPowerClip, FullPowerVolume);
    }

    public void PlayGoalClip()
    {
        PlayClip(GoalClip, GoalVolume);
    }
}
