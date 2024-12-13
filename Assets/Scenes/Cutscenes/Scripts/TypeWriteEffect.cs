using TMPro;
using UnityEngine;
using System;

public class TypeWriteEffectUpdate : MonoBehaviour
{
    private string _typeText = string.Empty;

    // 対象のテキスト
    [SerializeField] private TMP_Text _text;

    // 次の文字を表示するまでの時間[s]
    [SerializeField] private float _delayDuration = 0.1f;

    // 演出処理に使用する内部変数
    private bool _isRunning;
    private float _remainTime;
    private int _currentMaxVisibleCharacters;

    private void Awake()
    {
        _typeText = _text.text;
    }

    private void OnEnable()
    {
        _remainTime = _delayDuration;
        _text.text = _typeText; // テキスト全体を一度セット
        _text.maxVisibleCharacters = 0; // 初期表示を0文字に設定
        _isRunning = true;
    }

    private void OnDisable()
    {
        _text.text = null;
        _isRunning = false;
    }

    private void Update()
    {
        // 演出実行中でなければ何もしない
        if (!_isRunning) return;

        // 次の文字表示までの残り時間更新
        _remainTime -= Time.deltaTime;
        if (_remainTime > 0) return;

        // 表示する文字数を一つ増やす
        ++_currentMaxVisibleCharacters;
        _text.maxVisibleCharacters = _currentMaxVisibleCharacters; // 表示文字数を設定
        _remainTime = _delayDuration;

        // "｜"を末尾に追加
        if (_isRunning)
        {
            _text.text = _typeText.Substring(0,_currentMaxVisibleCharacters-1)+ "｜"; // 入力中カーソルを表示
        }

        // 文字を全て表示したら待機状態に移行
        if (_currentMaxVisibleCharacters >= _typeText.Length)
        {
            _isRunning = false;
            _text.text = _typeText; // 最終状態では"｜"を削除
        }
    }
}
