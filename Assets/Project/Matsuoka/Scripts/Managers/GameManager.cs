using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲーム全体を管理
/// タグの名前を保管
/// Playerを生成
/// ゴール処理
/// </summary>
public class GameManager:PersistentSingleton<GameManager> {
    [Header("Player")]
    [SerializeField]
    GameObject _playerPrefab;
    [SerializeField]
    Vector3[] _spawnPlayerPoses = { new Vector3(0,0) };

    [Header("Tag")]
    [SerializeField]
    string _playerTagName = "Player";
    [SerializeField]
    string _groundTagName = "Ground";

    public string PlayerTagName => _playerTagName;
    public string GroundTagName => _groundTagName;

    SceneLoader _sceneLoader;
    InputManager _inputManager;
    CameraContoller _camContoller;

    GameObject _player;

    int _currentStageIndex = 0;//現在のシーン番号
    int _finalStageIndex;//最後のシーン番号

    bool _isGorl = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        _sceneLoader=SceneLoader.Ins;
        _inputManager=InputManager.Ins;

        _camContoller=FindAnyObjectByType<CameraContoller>();
        SpawnPlayer();

        _finalStageIndex=SceneManager.sceneCountInBuildSettings-1;
    }

    void Update() {
        StartCoroutine(NextScene());
        //Debugger.Log(_camContoller.gameObject.name);
    }

    /// <summary>
    /// Playerの生成
    /// </summary>
    public void SpawnPlayer() {
        StartCoroutine(SpawnRoutine());
    }

    /// <summary>
    /// デストロイのあと、1fまって生成
    /// </summary>
    IEnumerator SpawnRoutine() {
        if(_player) {
            Destroy(_player);
            _player=null;
            yield return null;
        }

        // 古いのが完全に消えた後に生成するので、Singletonのチェックに引っかからない
        _player=Instantiate(
            _playerPrefab,_spawnPlayerPoses[_currentStageIndex],Quaternion.identity
        );
        yield return null;//Playerが完全に生成し終わるまでまつ
        if(!_isGorl) {
            Debugger.Log(PlayerController.Ins);
            //_camContoller=FindAnyObjectByType<CameraContoller>();
            _camContoller.SetTargetToPlayer();
        }
    }

    /// <summary>
    /// ゴール処理
    /// </summary>
    public void Goal() {
        _isGorl=true;
        if(_currentStageIndex<_finalStageIndex) {
            _sceneLoader.LoadSceneAdditive(_currentStageIndex+1);
        }
        else {
            _sceneLoader.LoadSceneAdditive(0);
        }
    }


    /// <summary>
    /// ゴールしたとき、クリックで次のステージへ
    /// </summary>
    IEnumerator NextScene() {
        if(_isGorl&&_inputManager.IsLeftClickDown) {
            _isGorl=false;
            if(_currentStageIndex<_finalStageIndex) {
                _sceneLoader.SetActiveScene(_currentStageIndex+1,true);
                _currentStageIndex++;
            }
            else {
                _currentStageIndex=0;
                _sceneLoader.SetActiveScene(_currentStageIndex,true);
            }

            //ロードし終わるまで待機
            while(SceneLoader.Ins.IsLoaded) {
                yield return null;
                Debugger.Log("ロード中");
            }
            yield return null;

            _camContoller=FindAnyObjectByType<CameraContoller>();
            Debugger.Log($"Managaer\\NextScene()\\{_camContoller}");
            Debugger.Log($"現在のシーンは{SceneManager.GetActiveScene().name}");

            SpawnPlayer();

            //Debugger.Log(PlayerController.Ins);
            //Debugger.Log(_camContoller);
            //Debugger.Log(_isGorl);
        }
    }
}
