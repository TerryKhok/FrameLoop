using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField]
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
