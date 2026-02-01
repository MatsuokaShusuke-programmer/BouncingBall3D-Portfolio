using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// シーン遷移を管理するクラス
/// 通常ロード、加算ロード、非同期ロード、アクティブ切り替え
/// </summary>
public class SceneLoader:PersistentSingleton<SceneLoader> {
    //ロードが完了したか
    public bool IsLoaded { get; private set; } = false;

    AsyncOperation _bufLoader;

    int _currentIndex = 0;
    int _nextIndex = 0;

    /// <summary>
    /// シーン名でシーンをロード
    /// </summary>
    /// <param name="sceneName">ロードしたいシーンの名前</param>
    public void LoadScene(string sceneName) {
        Debugger.Log($"シーンロード開始: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// ビルドインデックスでシーンをロード
    /// </summary>
    /// <param name="index">BuildSettingsのインデックス番号</param>
    public void LoadScene(int index) {
        Debugger.Log($"シーンロード開始: Index {index}");
        //範囲外チェック
        if(index>=0&&index<SceneManager.sceneCountInBuildSettings) {
            SceneManager.LoadScene(index);
        }
        else {
            Debugger.LogError($"指定されたシーンIndex {index} は存在しません。Build Settingsを確認してください");
        }
    }

    /// <summary>
    /// 現在のシーンを再読み込み
    /// </summary>
    public void ReloadCurrentScene() {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        Debugger.Log($"現在のシーンをリロードします: Index {currentIndex}");
        SceneManager.LoadScene(currentIndex);
    }

    /// <summary>
    /// Build Settings上の次のシーンをロード
    /// </summary>
    public void LoadNextScene() {
        _currentIndex=SceneManager.GetActiveScene().buildIndex;
        _nextIndex=_currentIndex+1;

        if(_nextIndex<SceneManager.sceneCountInBuildSettings) {
            LoadScene(_nextIndex);
        }
        else {
            Debugger.Log("次のシーンがありません");
        }
    }

    /// <summary>
    /// 別のシーンをシーン名で重ねてロード
    /// </summary>
    public void LoadSceneAdditive(string sceneName) {
        IsLoaded=false;
        StartCoroutine(LoadAsyncProcess(sceneName));
    }

    /// <summary>
    /// シーン名で非同期加算ロード
    /// </summary>
    IEnumerator LoadAsyncProcess(string sceneName) {
        Debugger.Log($"加算ロード開始:{sceneName}");

        //非同期ロード開始
        _bufLoader
            = SceneManager.LoadSceneAsync(sceneName,LoadSceneMode.Additive);
        //非表示
        _bufLoader.allowSceneActivation=false;

        //ロードが終わるまで待機
        while(_bufLoader.progress<0.9f) {
            float progress = Mathf.Clamp01(_bufLoader.progress/0.9f);
            Debugger.Log($"ロード進捗: {progress*100}%");
            yield return null;
        }

        Debugger.Log($"非同期加算ロード完了:{sceneName}");
    }

    /// <summary>
    /// 別のシーンを要素数で重ねてロード
    /// </summary>
    public void LoadSceneAdditive(int index) {
        IsLoaded=false;
        StartCoroutine(LoadAsyncProcess(index));
    }

    /// <summary>
    /// シーン番号で非同期加算ロード
    /// </summary>
    IEnumerator LoadAsyncProcess(int index) {
        Debugger.Log($"加算ロード開始:{index}");

        //非同期ロード開始
        _bufLoader
            = SceneManager.LoadSceneAsync(index,LoadSceneMode.Additive);
        //非表示
        _bufLoader.allowSceneActivation=false;

        //ロードが終わるまで待機
        while(_bufLoader.progress<0.9f) {
            float progress = Mathf.Clamp01(_bufLoader.progress/0.9f);
            Debugger.Log($"ロード進捗: {progress*100}%");
            yield return null;
        }


        Debugger.Log($"非同期加算ロード完了:{index}");
    }

    /// <summary>
    /// 重ねて表示しているシーンをシーン名で削除
    /// </summary>
    public void UnloadScene(string sceneName) {
        Debugger.Log($"シーン破棄開始:{sceneName}");
        SceneManager.UnloadSceneAsync(sceneName);
    }

    /// <summary>
    /// 重ねて表示しているシーン番号で削除
    /// </summary>
    public void UnloadScene(int index) {
        Debugger.Log($"シーン破棄開始:{index}");
        SceneManager.UnloadSceneAsync(index);
    }

    /// <summary>
    /// 指定したシーンをアクティブに設定
    /// </summary>
    /// <param name="isUnloadTheCurrentScene">現在のシーンをアンロードするか</param>
    public void SetActiveScene(string sceneName,bool isUnloadTheCurrentScene) {
        StartCoroutine(ActivateRoutine(sceneName,isUnloadTheCurrentScene));
    }

    /// <summary>
    /// ロード待機中のシーンを可視化し、アクティブなシーンとして設置
    /// </summary>
    /// <param name="isUnloadTheCurrentScene">切り替え前のシーンを破棄するか</param>
    IEnumerator ActivateRoutine(string sceneName,bool isUnloadTheCurrentScene) {
        //アクティブする前に表示許可を出してロード
        if(_bufLoader!=null) {
            _bufLoader.allowSceneActivation=true;

            //完全に完了するまで待つ
            while(!_bufLoader.isDone) {
                yield return null;
            }
        }

        IsLoaded=true;
        //ロード完了後にシーンを取得
        Scene scene = SceneManager.GetSceneByName(sceneName);

        //シーンが有効のとき
        if(scene.IsValid()) {
            //古いシーンを取得
            Scene oldScene = SceneManager.GetActiveScene();

            SceneManager.SetActiveScene(scene);
            _bufLoader.allowSceneActivation=true;
            Debugger.Log($"アクティブシーンを切り替えました:{sceneName}");

            if(isUnloadTheCurrentScene)
                SceneManager.UnloadSceneAsync(oldScene);
        }
        else {
            Debugger.LogError($"シーン{sceneName}がロードされていないため設定できません");
        }

        _bufLoader=null;
    }



    /// <summary>
    /// 指定したシーンをアクティブに設定
    /// </summary>
    /// <param name="isUnloadTheCurrentScene">現在のシーンをアンロードするか</param>
    public void SetActiveScene(int index,bool isUnloadTheCurrentScene) {
        StartCoroutine(ActivateRoutine(index,isUnloadTheCurrentScene));
    }

    /// <summary>
    /// ロード待機中のシーンを可視化し、アクティブなシーンとして設置
    /// </summary>
    /// <param name="isUnloadTheCurrentScene">切り替え前のシーンを破棄するか</param>
    IEnumerator ActivateRoutine(int index,bool isUnloadTheCurrentScene) {
        //アクティブする前に表示許可を出してロード
        if(_bufLoader!=null) {
            _bufLoader.allowSceneActivation=true;

            //完全に完了するまで待つ
            while(!_bufLoader.isDone) {
                yield return null;
            }
        }

        IsLoaded=true;
        //ロード完了後にシーンを取得
        Scene scene = SceneManager.GetSceneByBuildIndex(index);

        //シーンが有効のとき
        if(scene.IsValid()) {
            //古いシーンを取得
            Scene oldScene = SceneManager.GetActiveScene();

            SceneManager.SetActiveScene(scene);
            _bufLoader.allowSceneActivation=true;
            Debugger.Log($"アクティブシーンを切り替えました:{index}");

            if(isUnloadTheCurrentScene)
                SceneManager.UnloadSceneAsync(oldScene);
        }
        else {
            Debugger.LogError($"シーン{index}がロードされていないため設定できません");
        }

        _bufLoader = null;
    }
}