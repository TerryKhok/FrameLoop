using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BreakablePlatform : MonoBehaviour
{
    [SerializeField, Tooltip("横幅"), Range(3, 20)]
    private int _width = 3;
    [SerializeField, Tooltip("左端用タイル")]
    private Tile _leftTile = null;
    [SerializeField, Tooltip("中心用タイル")]
    private Tile _centerTile = null;
    [SerializeField, Tooltip("右端用タイル")]
    private Tile _rightTile = null;
    [SerializeField]
    private GameObject _breakableTilemapPrefab = null;

    private Transform _prefabInstance = null;

    private SpriteRenderer _spriteRenderer = null;

    private static int i = 0;

    private static List<Fan> _allFanList = new List<Fan>();
    private static bool _isInit = false;

    public Transform PrefabInstance
    {
        get { return _prefabInstance; }
    }

    private void OnValidate()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.size = new Vector2(_width, 1);
    }

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.enabled = false;
        _isInit = false;

        FindAllFans();
        SetTiles();
    }

    private void Update()
    {
        if(_prefabInstance == null)
        {
            ResetAllFans();
            Destroy(this.gameObject,0);
        }
    }

    public void SetTiles()
    {
        Transform grid = GameObject.FindGameObjectWithTag("Grid").transform;

        if (_prefabInstance == null)
        {
            _prefabInstance = Instantiate(_breakableTilemapPrefab, grid).transform;
            _prefabInstance.name = i++.ToString();
        }

        //_position = transform.position;

        ChangeTiles();
    }

    public void ChangeTiles()
    {
        if(_prefabInstance == null) 
        {
            return;
        }

        var objects = GameObject.FindGameObjectsWithTag("BreakableParent");

        Tilemap tilemap = _prefabInstance.GetChild(0).GetComponent<Tilemap>();

        if(tilemap == null)
        {
            Debug.LogError("tilemap見つかりません");
            return;
        }

        tilemap.ClearAllTiles();
        Tile tile = _leftTile;

        Vector3 pos = transform.position;
        pos.y -= 1.0f;

        for (int i = 0; i < _width; i++)
        {
            if (i == _width - 1)
            {
                tile = _rightTile;
            }
            else if (i > 0)
            {
                tile = _centerTile;
            }

            Vector3 setPos = new Vector3(i - _width / 2.0f, 0, 0) + pos;

            Vector3Int intPos = Vector3Int.zero;
            intPos.x = (int)Math.Round(setPos.x,0,MidpointRounding.AwayFromZero);
            intPos.y = (int)Math.Round(setPos.y,0,MidpointRounding.AwayFromZero);
            intPos.z = (int)Math.Round(setPos.z,0,MidpointRounding.AwayFromZero);

            if(intPos.y < 0)
            {
                intPos.y += 1;
            }

            tilemap.SetTile(intPos, tile);
        }
        ResetAllFans();
    }

    private void FindAllFans()
    {
        if(_isInit)
        {
            return;
        }
        _isInit = true;

        _allFanList = GameObject.FindObjectsByType<Fan>(FindObjectsSortMode.None).ToList();
    }

    private void ResetAllFans()
    {
        //Debug.Log("resetAllFans");
        foreach(var fan in _allFanList)
        {
            if(fan == null)
            {
                continue;
            }
            fan.AsyncResetTiles();
            //fan.ResetTiles();
        }
    }
}
