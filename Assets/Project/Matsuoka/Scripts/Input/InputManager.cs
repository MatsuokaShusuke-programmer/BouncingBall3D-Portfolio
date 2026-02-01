using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 全入力を一括管理するシングルトンクラス
/// </summary>
public class InputManager:PersistentSingleton<InputManager> {
    public Vector2 MouseDelta { get; private set; }

    public bool IsLeftClickDown { get; private set; }
    public bool IsLeftClickUp { get; private set; }
    public bool IsLeftClick { get; private set; }

    void Start() {
        Cursor.lockState=CursorLockMode.Locked;
        Cursor.visible=false;//カーソルを消す
    }

    void LateUpdate() {
        //クリックの押した・離した瞬間の初期化
        IsLeftClickDown=false;
        IsLeftClickUp=false;
    }

    /// <summary>
    /// マウスの座標の取得
    /// </summary>
    public void OnMouseDelta(InputAction.CallbackContext context) {
        MouseDelta=context.ReadValue<Vector2>();
    }

    /// <summary>
    /// 左クリック
    /// </summary>
    public void OnLeftClick(InputAction.CallbackContext context) {
        //左クリックを押したとき
        if(context.performed) {
            IsLeftClickDown=true;
            IsLeftClickUp=false;
            IsLeftClick=true;
        }
        //左クリックを離したとき
        else if(context.canceled) {
            IsLeftClickDown=false;
            IsLeftClickUp=true;
            IsLeftClick=false;
        }
    }
}
