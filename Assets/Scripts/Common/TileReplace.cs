using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/*  ProjectName :FrameLoop
 *  ClassName   :TileTeplace
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :Breakableオブジェクトをフレームの有無に合わせて再配置
 *               
 *  Created     :2024/05/31
 */
public class TileReplace : MonoBehaviour
{
    protected Tilemap tilemap_out;
    protected Tilemap tilemap_out_invisible;
    protected Tilemap tilemap_in;
    protected Tilemap tilemap_in_invisible;
    protected List<Vector3Int> allTilePositions;
    protected TileBase tile = null;

    void Start()
    {
        //タイルマップを取得しておく
        var tilemaps = GetComponentsInChildren<Tilemap>();
        tilemap_out = tilemaps[0] != null ? tilemaps[0] : null;
        tilemap_out_invisible = tilemaps[1] != null ? tilemaps[1] : null;
        tilemap_in = tilemaps[2] != null ? tilemaps[2] : null;
        tilemap_in_invisible = tilemaps[3] != null ? tilemaps[3] : null;

        //タイルが置いてあるポジションを保存
        allTilePositions = GetAllTilePositions(tilemap_out);
    }

    public virtual bool Replace(Vector3Int setPos, Vector3Int beforePos = new Vector3Int(), bool setInside = false)
    {
        if(setInside)
        {
            //outsideのタイルを削除
            tilemap_out.SetTile(beforePos, null);

            //insideに置きなおす
            tilemap_in.SetTile(beforePos, tile);
            //ループ後のポジションに追加
            tilemap_in_invisible.SetTile(setPos, tile);
        }
        else
        {
            //ループ後のポジションに追加
            tilemap_out_invisible.SetTile(setPos, tile);
        }

        return false;
    }

    public virtual void UnReplace()
    {
        //タイルマップをすべてクリア
        tilemap_in.ClearAllTiles();
        tilemap_in_invisible.ClearAllTiles();
        tilemap_out.ClearAllTiles();
        tilemap_out_invisible.ClearAllTiles();

        //タイルマップの再設置
        foreach(var position in allTilePositions)
        {
            tilemap_out.SetTile(position, tile);
        }
    }

    protected List<Vector3Int> GetAllTilePositions(Tilemap tilemap)
    {
        List<Vector3Int> tilePositions = new List<Vector3Int>();

        // Tilemapのバウンディングボックスの範囲を取得
        BoundsInt bounds = tilemap.cellBounds;

        // バウンディングボックス内のすべての位置をチェック
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                for (int z = bounds.zMin; z < bounds.zMax; z++)
                {
                    Vector3Int pos = new Vector3Int(x, y, z);
                    if (tilemap.HasTile(pos))
                    {
                        tilePositions.Add(pos);

                        if(tile == null)
                        {
                            tile = tilemap.GetTile(pos);
                        }
                    }
                }
            }
        }

        return tilePositions;
    }

    private void OnDestroy()
    {
        if (ParticleFrameScript.Instance != null)
        {

            ParticleFrameScript.Instance.SendDestroyMsg(this);
        }
        }
}
