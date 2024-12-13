using TMPro;
using UnityEngine;
using System;

public class TypeWriteEffectUpdate : MonoBehaviour
{
    private string _typeText = string.Empty;

    // �Ώۂ̃e�L�X�g
    [SerializeField] private TMP_Text _text;

    // ���̕�����\������܂ł̎���[s]
    [SerializeField] private float _delayDuration = 0.1f;

    // ���o�����Ɏg�p��������ϐ�
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
        _text.text = _typeText; // �e�L�X�g�S�̂���x�Z�b�g
        _text.maxVisibleCharacters = 0; // �����\����0�����ɐݒ�
        _isRunning = true;
    }

    private void OnDisable()
    {
        _text.text = null;
        _isRunning = false;
    }

    private void Update()
    {
        // ���o���s���łȂ���Ή������Ȃ�
        if (!_isRunning) return;

        // ���̕����\���܂ł̎c�莞�ԍX�V
        _remainTime -= Time.deltaTime;
        if (_remainTime > 0) return;

        // �\�����镶����������₷
        ++_currentMaxVisibleCharacters;
        _text.maxVisibleCharacters = _currentMaxVisibleCharacters; // �\����������ݒ�
        _remainTime = _delayDuration;

        // "�b"�𖖔��ɒǉ�
        if (_isRunning)
        {
            _text.text = _typeText.Substring(0,_currentMaxVisibleCharacters-1)+ "�b"; // ���͒��J�[�\����\��
        }

        // ������S�ĕ\��������ҋ@��ԂɈڍs
        if (_currentMaxVisibleCharacters >= _typeText.Length)
        {
            _isRunning = false;
            _text.text = _typeText; // �ŏI��Ԃł�"�b"���폜
        }
    }
}
