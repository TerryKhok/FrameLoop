using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerTitleMotion : MonoBehaviour
{
    [SerializeField,Tooltip("ワープ先の座標")]
    private List<Vector3> _warpTo = new List<Vector3>();

    public enum LessOrGreater { Less, Greater};

    [System.Serializable]
    public struct POS
    {
        public LessOrGreater lessOrGreater;
        public float posx; 
    }

    [SerializeField, Tooltip("ワープするx座標")]
    private POS _warpPos;

    [System.Serializable]
    public struct POS_METHOD
    {
        public POS pos;
        public int setX;
        public float wait;
    }

    [SerializeField,Tooltip("方向を変える座標")]
    private List<POS_METHOD> _flipPosX = new List<POS_METHOD>();

    private Transform _transform;
    private PlayerMove _playerMove;

    private int _warpIndex = 0, _moveIndex = 0;

    private int _moveX = 1;

    private float _currentWaitTime = 0, _elapsedTime = 0;

    private void Start()
    {
        _transform = transform;
        _playerMove = GetComponent<PlayerMove>();
    }

    private void FixedUpdate()
    {
        _playerMove.SetMove(_moveX);

        _elapsedTime += Time.fixedDeltaTime;

        if(_elapsedTime < _currentWaitTime) { return; }

        switch (_warpPos.lessOrGreater)
        {
            case LessOrGreater.Less:
                if(_transform.position.x < _warpPos.posx)
                {
                    Warp();
                }
                break;

            case LessOrGreater.Greater:
                if (_transform.position.x > _warpPos.posx)
                {
                    Warp();
                }
                break;
        }

        if(_moveIndex == _flipPosX.Count) { return; }
        switch (_flipPosX[_moveIndex].pos.lessOrGreater)
        {
            case LessOrGreater.Less:
                if(_transform.position.x < _flipPosX[_moveIndex].pos.posx)
                {
                    _currentWaitTime = _flipPosX[_moveIndex].wait;
                    Move(_flipPosX[_moveIndex].setX);
                }
                break;
            case LessOrGreater.Greater:
                if (_transform.position.x > _flipPosX[_moveIndex].pos.posx)
                {

                    _currentWaitTime = _flipPosX[_moveIndex].wait;
                    Move(_flipPosX[_moveIndex].setX);
                }
                break;
        }
    }

    private void Warp()
    {
        _transform.position = _warpTo[_warpIndex];
        _warpIndex++;

        if (_warpIndex >= _warpTo.Count)
        {
            _warpIndex = 0;
            _moveIndex = 0;
        }
    }

    private void Move(int setX)
    {
        _moveX = setX;
        _moveIndex++;

        _elapsedTime = 0;
    }
}
