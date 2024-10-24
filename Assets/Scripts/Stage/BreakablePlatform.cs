using System;
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

        SetTiles();
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
        Vector3Int intPos = new Vector3Int((int)pos.x, (int)pos.y-1, (int)pos.z);

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

            tilemap.SetTile(new Vector3Int(i - _width / 2, 0, 0) + intPos, tile);
        }
    }

    private void OnDestroy()
    {
        if(_prefabInstance != null )
        {
            Destroy(_prefabInstance);
        }
    }
}
