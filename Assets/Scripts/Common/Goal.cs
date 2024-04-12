using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField,Tooltip("クリア時に表示するキャンバス")]
    private Canvas _clearCanvas = null;

    private void Start()
    {
        _clearCanvas.enabled = false;
    }

    public void OnGoal()
    {
        _clearCanvas.enabled = true;
    }
}
