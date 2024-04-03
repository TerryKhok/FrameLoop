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
        Debug.Log("onGoal");
        _clearCanvas.enabled = true;
    }
}
