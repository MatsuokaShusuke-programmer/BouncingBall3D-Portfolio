using TMPro;
using UnityEngine;

public class Goal : MonoBehaviour
{
    //ゴール用のパーティクルシステム
    [SerializeField] ParticleSystem goalParticle;

    [Header("Text関連")]
    [SerializeField] TextMeshProUGUI goalText;
    [SerializeField] TextMeshProUGUI nextText;
    [SerializeField] private CameraContoller cameraController;
    GameManager gameManager;
     private void Start()
    {
        //テキスト達を非表示
        goalText.enabled = false;
        nextText.enabled = false;
        cameraController = FindObjectOfType<CameraContoller>();
        gameManager = GameManager.Ins;
    }
    private void OnTriggerEnter(Collider other)
    {
        //Playerタグの物が触れたら紙吹雪を出してテキストを表示
        //カメラをGoalPointに向かせる
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance.PlayGoalClip();
            goalParticle.Play();
            goalText.enabled = true;
            nextText.enabled = true;
            cameraController.SetTargetToGoal();
            gameManager.Goal();
        }
        
    }
}
