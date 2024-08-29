using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StageTileReplace : TileReplace
{

    private void Start()
    {
        //タイルマップを取得しておく
        tilemap_out = GetComponent<Tilemap>();

        // 当たり判定のないTilemapを作成
        GameObject instance = Instantiate(new GameObject(), transform.position, transform.rotation, transform.parent);

        // 必要なコンポーネントをつける
        tilemap_out_invisible = instance.AddComponent<Tilemap>();
        instance.AddComponent<TilemapRenderer>();

        //タイルが置いてあるポジションを保存
        allTileInfos = GetAllTilePositions(tilemap_out);
    }

    public override bool Replace(Vector3Int setPos, Vector3Int beforePos = new Vector3Int(), bool setInside = false)
    {
        if (!setInside) { return true; }

        //outsideのタイルを削除
        tilemap_out.SetTile(beforePos, null);

        //当たり判定のないTilemapに置きなおす
        tilemap_out_invisible.SetTile(beforePos, allTileInfos[beforePos]);

        return true;
    }

    public override void UnReplace()
    {
        //タイルマップをすべてクリア
        tilemap_out.ClearAllTiles();
        tilemap_out_invisible.ClearAllTiles();

        //タイルマップの再設置
        foreach (var tileInfo in allTileInfos)
        {
            tilemap_out.SetTile(tileInfo.Key,tileInfo.Value);
        }
    }
}
