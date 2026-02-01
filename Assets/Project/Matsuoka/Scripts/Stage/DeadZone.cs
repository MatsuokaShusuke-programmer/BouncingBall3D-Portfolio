using UnityEngine;

/// <summary>
/// 場外判定の制御クラス。
/// 場外へ侵入したオブジェクトがPlyayerのとき
/// 再生成する
/// </summary>
public class DeadZone:MonoBehaviour {
    GameManager _manager;

    string _playerTagName = "Player";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        _manager=GameManager.Ins;
        _playerTagName=_manager.PlayerTagName;
    }

    void OnTriggerEnter(Collider collider) {
        Debugger.Log($"名前:{collider.name},タグ:{collider.tag}");
        if(collider.CompareTag(_playerTagName)) {
            _manager.SpawnPlayer();
        }
    }
}
