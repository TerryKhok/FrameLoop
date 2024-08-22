using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/*  ProjectName :FrameLoop
 *  ClassName   :TileTeplace
 *  Creator     :Fujishita.Arashi
 *  
 *  Summary     :Breakable�I�u�W�F�N�g���t���[���̗L���ɍ��킹�čĔz�u
 *               
 *  Created     :2024/05/31
 */
public class TileReplace : MonoBehaviour
{
    protected Tilemap tilemap_out;
    protected Tilemap tilemap_out_invisible;
    protected Tilemap tilemap_in;
    protected Tilemap tilemap_in_invisible;
    protected Dictionary<Vector3Int,TileBase> allTileInfos;

    void Start()
    {
        //�^�C���}�b�v���擾���Ă���
        var tilemaps = GetComponentsInChildren<Tilemap>();
        tilemap_out = tilemaps[0] != null ? tilemaps[0] : null;
        tilemap_out_invisible = tilemaps[1] != null ? tilemaps[1] : null;
        tilemap_in = tilemaps[2] != null ? tilemaps[2] : null;
        tilemap_in_invisible = tilemaps[3] != null ? tilemaps[3] : null;

        //�^�C�����u���Ă���|�W�V������ۑ�
        allTileInfos = GetAllTilePositions(tilemap_out);
    }

    public virtual bool Replace(Vector3Int setPos, Vector3Int beforePos = new Vector3Int(), bool setInside = false)
    {
        if(setInside)
        {
            //outside�̃^�C�����폜
            tilemap_out.SetTile(beforePos, null);

            //inside�ɒu���Ȃ���
            tilemap_in.SetTile(beforePos, allTileInfos[beforePos]);
            //���[�v��̃|�W�V�����ɒǉ�
            tilemap_in_invisible.SetTile(setPos, allTileInfos[beforePos]);
        }
        else
        {
            //���[�v��̃|�W�V�����ɒǉ�
            tilemap_out_invisible.SetTile(setPos, allTileInfos[beforePos]);
        }

        return false;
    }

    public virtual void UnReplace()
    {
        //�^�C���}�b�v�����ׂăN���A
        tilemap_in.ClearAllTiles();
        tilemap_in_invisible.ClearAllTiles();
        tilemap_out.ClearAllTiles();
        tilemap_out_invisible.ClearAllTiles();

        //�^�C���}�b�v�̍Đݒu
        foreach(var tileInfo in allTileInfos)
        {
            tilemap_out.SetTile(tileInfo.Key, tileInfo.Value);
        }
    }

    protected Dictionary<Vector3Int,TileBase> GetAllTilePositions(Tilemap tilemap)
    {
        Dictionary<Vector3Int,TileBase> tilePositions = new Dictionary<Vector3Int, TileBase>();

        // Tilemap�̃o�E���f�B���O�{�b�N�X�͈̔͂��擾
        BoundsInt bounds = tilemap.cellBounds;

        // �o�E���f�B���O�{�b�N�X���̂��ׂĂ̈ʒu���`�F�b�N
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                for (int z = bounds.zMin; z < bounds.zMax; z++)
                {
                    Vector3Int pos = new Vector3Int(x, y, z);
                    if (tilemap.HasTile(pos))
                    {
                        tilePositions.Add(pos, tilemap.GetTile(pos));
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
