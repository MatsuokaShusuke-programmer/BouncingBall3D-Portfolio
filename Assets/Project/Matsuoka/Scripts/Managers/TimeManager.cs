using System;
using UnityEngine;

/// <summary>
/// 時間を管理
/// スロー演出の実行と解除を行う
/// </summary>
public class TimeManager:PersistentSingleton<TimeManager> {
    [SerializeField, Range(0,1)]
    float _slowTimeScale = 0.2f;//スローの倍率

    float _defaultFixedDeltaTime = 0.02f;

    /// <summary>
    /// ゲームの時間をスローにする
    /// </summary>
    public void StartSlowMotion() {
        Time.timeScale=_slowTimeScale;
        //部地理演算の更新頻度も時間に合わせて調整(カクつき防止)
        Time.fixedDeltaTime=_defaultFixedDeltaTime*Time.timeScale;
    }

    /// <summary>
    /// ゲームの時間を通常に戻す
    /// </summary>
    public void ResetTimeScale() {
        Time.timeScale=1.0f;
        Time.fixedDeltaTime=_defaultFixedDeltaTime;
    }
}
