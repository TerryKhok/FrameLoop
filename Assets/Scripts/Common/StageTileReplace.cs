using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StageTileReplace : TileReplace
{

    private void Start()
    {
        //�^�C���}�b�v���擾���Ă���
        tilemap_out = GetComponent<Tilemap>();

        // �����蔻��̂Ȃ�Tilemap���쐬
        GameObject instance = Instantiate(new GameObject(), transform.position, transform.rotation, transform.parent);

        // �K�v�ȃR���|�[�l���g������
        tilemap_out_invisible = instance.AddComponent<Tilemap>();
        instance.AddComponent<TilemapRenderer>();

        //�^�C�����u���Ă���|�W�V������ۑ�
        allTileInfos = GetAllTilePositions(tilemap_out);
    }

    public override bool Replace(Vector3Int setPos, Vector3Int beforePos = new Vector3Int(), bool setInside = false)
    {
        if (!setInside) { return true; }

        //outside�̃^�C�����폜
        tilemap_out.SetTile(beforePos, null);

        //�����蔻��̂Ȃ�Tilemap�ɒu���Ȃ���
        tilemap_out_invisible.SetTile(beforePos, allTileInfos[beforePos]);

        return true;
    }

    public override void UnReplace()
    {
        //�^�C���}�b�v�����ׂăN���A
        tilemap_out.ClearAllTiles();
        tilemap_out_invisible.ClearAllTiles();

        //�^�C���}�b�v�̍Đݒu
        foreach (var tileInfo in allTileInfos)
        {
            tilemap_out.SetTile(tileInfo.Key,tileInfo.Value);
        }
    }
}
