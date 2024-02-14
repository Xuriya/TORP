using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum State
    {  
        // スタート画面
        Start = 0,
        // プレイ画面
        Play
    }
    public static State CurrentState { get; private set; }
    [SerializeField] private float remainTime = 0.0f;
    static float currentTime;
    public static float CurrentTime { get => currentTime; }

    UIManager uiManager;
    AudioManager audioManager;
    GridController gridController;
    Player player;

    // Start is called before the first frame update

    void Awake()
    {
        uiManager = GetComponent<UIManager>();
        audioManager = GetComponent<AudioManager>();
        gridController = GameObject.Find("Grid").GetComponent<GridController>();
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    void Start()
    {
        CurrentState = State.Start;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            LoadNextStage();
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            LoadPreviousStage();
        }

        switch (CurrentState)
        {
            case State.Start:
                // スタート画面の読み込み
                TransitToPlay();
                break;
            case State.Play:
                // Rが押されたらステージをリセット
                if (Input.GetKeyDown(KeyCode.R))
                {
                    ReloadStage();
                    break;
                }

                // 時間の更新
                switch (Player.CurrentState)
                {
                    case Player.State.Idol:
                        uiManager.ShowRemainObserve();
                        break;
                    case Player.State.Active:
                        currentTime -= Time.deltaTime / Player.Gamma;
                        // UIの更新
                        // uiManager.UpdateTime();
                        // uiManager.UpdateGamma();
                        break;
                    case Player.State.Goal:
                        uiManager.ShowFinishMessage();
                        audioManager.StopNormalAudio();
                        break;
                    case Player.State.GameOver:
                        uiManager.ShowFinishMessage();
                        audioManager.StopNormalAudio();
                        break;
                }
                break;
        }
    }

    // PlayerがObserverModeに入ったときに呼ばれる処理
    // Player側で諸々の処理をした後に呼ばれる
    public void EnterObserverMode()
    {
        if (Player.CurrentState != Player.State.Idol) return;
        uiManager.ShowRemainObserve();
        audioManager.StopNormalAudio();
        audioManager.PlayShutterAudio();
        audioManager.PlayObserverAudio();
        gridController.FadeIn();
    }

    // PlayerがExitModeに入ったときに呼ばれる処理
    // Player側で諸々の処理をした後に呼ばれる（特に、Player.CurrentStateをIdolに更新した後で）
    public void ExitObserverMode()
    {
        if (Player.CurrentState != Player.State.Idol) return;
        audioManager.StopObserverAudio();
        audioManager.ResumeNormalAudio();
        gridController.FadeOut();
    }

    public void EnterStateActive()
    {
        audioManager.PlayFireAudio();
    }

    public void EnterStateIdol()
    {
        audioManager.StopFireAudio();
    }

    void ResetStage()
    {
        uiManager.Initialize();
        audioManager.Initialize();
        TransitToStart();
    }

    void TransitToPlay()
    {
        // 残り時間の設定
        currentTime = remainTime;

        // UIの更新
        uiManager.HidePressSpaceToStart();
        
        // Stateの移行
        CurrentState = State.Play;
    }

    void TransitToStart()
    {
        CurrentState = State.Start;
        audioManager.PlayNormalAudio();
    }

    void ReloadStage()
    {
        Scene sceneLoaded = SceneManager.GetActiveScene();
        SceneManager.LoadScene(sceneLoaded.buildIndex);
    }

    void LoadNextStage()
    {
        // ビルド番号で次のステージをロードする
        Scene sceneLoaded = SceneManager.GetActiveScene();
        SceneManager.LoadScene(sceneLoaded.buildIndex +1);
    }   
    void LoadPreviousStage()
    {
        // ビルド番号で前のステージをロードする
        Scene sceneLoaded = SceneManager.GetActiveScene();
        if (sceneLoaded.buildIndex == 0) return;
        SceneManager.LoadScene(sceneLoaded.buildIndex -1);
    }
}
