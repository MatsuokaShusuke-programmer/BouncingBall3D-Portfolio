using UnityEngine;

/// <summary>
/// 移動制御クラス
/// ドラッグで跳ねる
/// 二段ジャンプができる
/// </summary>
public class PlayerController:SceneSingleton<PlayerController> {
    [Header("跳ねる力")]
    [SerializeField]
    float _minPower = 1;
    [SerializeField]
    float _maxPower = 10;
    [SerializeField]
    [Tooltip("1sあたりに上昇する跳ねる力の割合")]
    float _powerIncreaseRatio = 0.2f;
    [SerializeField]
    [Tooltip("カメラの向きの方向に与える力")]
    float _frontPowerWeight = 10;

    enum PlayerState {
        Idle,       //ジャンプしていない
        FirstJump,  //1回ジャンプした
        SecondJump  //2回ジャンプした
    }
    PlayerState _currentState=PlayerState.Idle;

    //現在のパワーの割合
    public float PowerRatio => Mathf.InverseLerp(_minPower,_maxPower,_currentPower);
    //ドラッグの量
    public Vector2 AccumulatedDragVector { get; private set; }

    public Transform MyTransform { get; private set; }

    Rigidbody _rb;

    GameManager _manager;
    InputManager _inputManager;
    TimeManager _timeManager;

    float _currentPower = 0;//現在の跳ねる力
    Vector3 _directionOfBounce = Vector2.zero;//跳ねる方向

    Vector2 _dragStartPos = Vector2.zero;//ドラッグし始めた座標

    string _graundTagName = "Ground";

    [SerializeField] GameObject _ballEffect;

    void Start() {
        MyTransform=transform;

        _manager =GameManager.Ins;
        _inputManager =InputManager.Ins;
        _timeManager=TimeManager.Ins;

        _rb=GetComponent<Rigidbody>();

        _currentPower=_minPower;

        _graundTagName=_manager.GroundTagName;
    }

    void Update() {
        Bounce();
    }

    /// <summary>
    /// Playerを跳ねさせる操作
    /// </summary>
    void Bounce() {
        if(_inputManager.IsLeftClickDown)
            AccumulatedDragVector=Vector2.zero;//ドラッグの蓄積をリセット

        //左クリックし続けてるとき
        if(_inputManager.IsLeftClick) {
            //ドラッグの量を蓄積
            AccumulatedDragVector+=_inputManager.MouseDelta;

            if(_currentPower<_maxPower) {
                //パワーを上げる
                _currentPower+=_maxPower*_powerIncreaseRatio*Time.deltaTime;
                _currentPower=_currentPower>_maxPower ? _maxPower : _currentPower;
            }

            if(_currentState==PlayerState.FirstJump)
                _timeManager.StartSlowMotion();
        }
        //左クリックから離したとき
        else if(_inputManager.IsLeftClickUp) {

            //跳ねる方向を計算
            _directionOfBounce
                -=new Vector3(
                    AccumulatedDragVector.x,AccumulatedDragVector.y
                );
            _directionOfBounce.z=_frontPowerWeight;
            _directionOfBounce
                =Camera.main.transform.rotation*_directionOfBounce;
            _directionOfBounce.Normalize();

            //二段ジャンプしてないなら
            if(_currentState!=PlayerState.SecondJump) {
                //跳ねる
                _rb.AddForce(_directionOfBounce*_currentPower,ForceMode.Impulse);
                _currentPower=_minPower;//跳ねる力の初期化
                _timeManager.ResetTimeScale();

                //ステータス更新
                switch(_currentState) {
                    case PlayerState.Idle:
                        _currentState=PlayerState.FirstJump;
                        break;

                    case PlayerState.FirstJump:
                        _currentState=PlayerState.SecondJump;
                        break;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision) {
        //地面にふれたとき
        if(collision.gameObject.CompareTag(_graundTagName)) {
            //Playerが1段ジャンプか2段ジャンプ中のとき
            if(_currentState==PlayerState.FirstJump||
                _currentState == PlayerState.SecondJump)
            {
                // エフェクトの生成と処理
                if (_ballEffect != null)
                {
                    // ぶつかった瞬間の接点を取得
                    ContactPoint contact = collision.contacts[0];

                    // 接点の位置に、地面の向きに合わせてエフェクトを生成
                    GameObject effect = Instantiate(
                        _ballEffect,
                        contact.point,
                        Quaternion.LookRotation(contact.normal)
                    );

                    // エフェクトが残り続けないように2秒後に消す
                    Destroy(effect, 2.0f);
                }
            }
            _currentState=PlayerState.Idle; //Playerの状態リセット
            //_timeManager.ResetTimeScale();
            TimeManager.Ins.ResetTimeScale();
        }
    }
}
