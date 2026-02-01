using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ドラッグの移動量をUIで可視化するクラス
/// InputManagerから提供される麻生座標の差分を計算することで線を表示
/// </summary>
public class PowerVisualizer:MonoBehaviour {
    [SerializeField]
    RectTransform _lineRect;//線のレクトトランスフォーム

    [SerializeField]
    RectTransform _powerGaugeRect;//パワーゲージのレクトトランスフォーム
    [SerializeField]
    float _minPowerGauge;
    [SerializeField]
    float _maxPowerGauge;

    InputManager _inputManager;

    Vector2 _dragVector;//ドラッグのベクトル

    float _angle;//線の角度
    float _length;//線の長さ

    Vector2 _powerGaugeSize;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        InitVisualizer();

        _inputManager =InputManager.Ins;
    }

    // Update is called once per frame
    void Update() {
        UpdatePowerDirectionLine();
        UpdatePowerGauge();
    }

    /// <summary>
    /// パワーの方向の線の更新
    /// </summary>
    void UpdatePowerDirectionLine() {
        if(_inputManager.IsLeftClick) {
            // PlayerController のインスタンス取得
            var player = PlayerController.Ins;
            if (player == null)
                return; // Player が存在しないなら処理しない

            // ドラッグ量を取得
            _dragVector = player.AccumulatedDragVector;

            //ドラッグのベクトルを取得
            _dragVector = PlayerController.Ins.AccumulatedDragVector;

            //ベクトルのラジアンを計算
            _angle
                =Mathf.Atan2(_dragVector.y,_dragVector.x)*Mathf.Rad2Deg;

            //UIの回転
            _lineRect.rotation=Quaternion.Euler(0,0,_angle);

            //ベクトルの長さを計算して線の長さをカエル
            _length=_dragVector.magnitude;
            _lineRect.sizeDelta=new Vector2(_length,_lineRect.sizeDelta.y);
        }
        else {
            //線を消す
            _lineRect.sizeDelta=new Vector2(0,_lineRect.sizeDelta.y);
        }
    }

    /// <summary>
    /// パワーゲージの更新
    /// </summary>
    void UpdatePowerGauge() {
        if(!PlayerController.Ins) return;

        if(_inputManager.IsLeftClick) {
            //ゲージを伸ばす
            _powerGaugeSize.x
                =(_maxPowerGauge-_minPowerGauge)*PlayerController.Ins.PowerRatio
                +_minPowerGauge;
            _powerGaugeRect.sizeDelta
                = new Vector2(_powerGaugeSize.x, _powerGaugeRect.sizeDelta.y);
        }
        else if(_inputManager.IsLeftClickUp) {
            //ゲージを0にする(非表示)
            _powerGaugeSize=new Vector2(0,_powerGaugeRect.sizeDelta.y);
            _powerGaugeRect.sizeDelta=_powerGaugeSize;
        }
    }

    /// <summary>
    /// 各表示のリセット
    /// </summary>
    void InitVisualizer() {
        //線の長さを0にする(実質的に非表示にする)
        if(_lineRect) _lineRect.sizeDelta=new Vector2(0,_lineRect.sizeDelta.y);
        else Debugger.LogError($"{nameof(_lineRect)}がnull");

        //ゲージのリセット
        if(_powerGaugeRect) {
            //サイズの初期化
            _powerGaugeSize=new Vector2(0,_powerGaugeRect.sizeDelta.y);
            _powerGaugeRect.sizeDelta=_powerGaugeSize;
            //座標の初期化
            _powerGaugeRect.anchoredPosition=new Vector2(-_maxPowerGauge/2,_powerGaugeRect.sizeDelta.y);
        }
        else Debugger.LogError($"{nameof(_powerGaugeRect)}がnull");
    }
}
