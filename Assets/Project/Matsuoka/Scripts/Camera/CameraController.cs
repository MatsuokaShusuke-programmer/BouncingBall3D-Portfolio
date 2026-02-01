using UnityEngine;

/// <summary>
/// Playerを追尾し、マウス入力で回転するカメラ制御クラス
/// </summary>
public class CameraContoller:PersistentSingleton<CameraContoller> {
    [Header("カメラ関連")]
    [SerializeField]
    float _camRotMagnification = 0.5f;
    [SerializeField]
    float _cameraDistance = 10;
    [SerializeField]
    float _pitchAngle = 60;

    [Space(10)]
    [SerializeField]


    string _goalTagName = "GoalPoint";

    Camera _cam;//カメラ
    InputManager _inputManager;

    Transform _goalTf;
    Transform _targetTf;

    //カメラの角度
    Vector2 _camRot;
    Quaternion _camRotQuternion;

    void Start() {
        _inputManager=InputManager.Ins;
        _cam=GetComponent<Camera>();
        //_cam.tag="MainCamera";
        //Debugger.Log($"CamContoroller\\Start()\\{_cam}");

        //Goalのトランスフォームを取得
        _goalTf=GameObject.FindWithTag(_goalTagName)?.transform;
        if(!_goalTf) Debugger.LogError("GoalPointがありません");

    }

    void LateUpdate() {
        RotateCamera();//カクつき防止のためLateUpdateに書く
    }

    /// <summary>
    /// カメラの回転
    /// </summary>
    void RotateCamera() {
        if(_targetTf==null) {
            Debugger.LogError("_targeTfがnull");
            return;
        }
        //Debugger.Log("aaa");
        if(_cam==null) return;

        if(!_inputManager.IsLeftClick) {
            //マウスの座標からカメラの角度を決める
            _camRot.x-=_inputManager.MouseDelta.y*_camRotMagnification;
            _camRot.y+=_inputManager.MouseDelta.x*_camRotMagnification;
        }

        //カメラの上下回転に制限をかけ、反転を防止する
        _camRot.x=Mathf.Clamp(_camRot.x,-_pitchAngle,_pitchAngle);

        //カメラの角度を変更
        _camRotQuternion=Quaternion.Euler(_camRot.x,_camRot.y,0);
        _cam.transform.rotation=_camRotQuternion;

        //Playerの座標からカメラの回転方向に距離分だけ後に下がる
        Vector3 offset = _camRotQuternion*new Vector3(0,0,-_cameraDistance);
        _cam.transform.position=_targetTf.position+offset;
        //Debugger.Log($"targetTf:{_targetTf.position}");
    }

    /// <summary>
    /// カメラをPlayerに向ける
    /// </summary>
    public void SetTargetToPlayer() {
        Debugger.Log("SetTargetToPlayer");
        Debugger.Log(PlayerController.Ins);
        _targetTf=PlayerController.Ins.transform;
        Debugger.Log($"targetTf:{_targetTf.position}");
    }

    public void SetTargetToGoal() {
        Debugger.Log("SetTargetToGoal");
        _targetTf=_goalTf;
    }
}
