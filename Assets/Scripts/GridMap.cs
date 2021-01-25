using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    //瓦片大小
    public Vector2 TileSize = new Vector2(0.8f, 0.8f);
    public Vector2 TileAmount = new Vector2(100, 100);

    //public Vector2 TileSize = new Vector2(1.0f, 1.0f);
    //public Vector2 TileAmount = new Vector2(60, 60);

    //public Vector2 TileSize = new Vector2(0.25f, 0.25f);
    //public Vector2 TileAmount = new Vector2(240, 240);

    //public Vector2 TileSize = new Vector2(1, 1);
    //public Vector2 TileAmount = new Vector2(60, 60);
    public GameObject regionSnippetPrefab;//用来表示区域的小片预制体

    public MapTile[,] tiles;

    [HideInInspector]
    public List<RegionSnippetInfo> regionSnippetsList;

    void Start()
    {
        tiles = new MapTile[(int)TileAmount.x, (int)TileAmount.y];
        for (int row = 0; row < TileAmount.x; row++)
        {
            for (int col = 0; col < TileAmount.y; col++)
            {
                tiles[row, col] = (new MapTile(row, col, TileSize));
            }
        }
        Debug.Log("初始化Tile");
    }

    public void CreateRegionSnippets(BuildingBase building)
    {
        Rect colRect = BuildingHelper.MakeRectOfCollider(building.GetComponentInChildren<Collider>());
        Vector2 buildingFootPoint = WorldPos2LogicPos(building.transform.position.x, building.transform.position.z);

        float fromX = colRect.position.x;
        float toX = colRect.position.x + colRect.width;
        float fromZ = colRect.position.y;
        float toZ = colRect.position.y + colRect.height;

        //Debug.DrawLine(new Vector3(fromX, 1, fromZ), new Vector3(toX, 1, fromZ), Color.red, 999999);
        //Debug.DrawLine(new Vector3(fromX, 1, toZ), new Vector3(toX, 1, toZ), Color.red, 999999);
        //Debug.DrawLine(new Vector3(fromX, 1, fromZ), new Vector3(fromX, 1, toZ), Color.red, 999999);
        //Debug.DrawLine(new Vector3(toX, 1, fromZ), new Vector3(toX, 1, toZ), Color.red, 999999);

        Vector2 minPoint = WorldPos2LogicPos(fromX, fromZ);
        Vector2 maxPoint = WorldPos2LogicPos(toX, toZ, CoordinateBound.Open);

        GameObject snippet;
        Vector2 snappedPos;
        RegionSnippetInfo snippetInfo;
        for (float x = minPoint.x; x <= maxPoint.x; x += 1)
        {
            for (float y = minPoint.y; y <= maxPoint.y; y += 1)
            {
                snippet = Instantiate(regionSnippetPrefab);
                snippet.transform.localScale = new Vector3(TileSize.x, TileSize.y, 1);

                snippet.gameObject.SetLayerRecursively("BuildingFollow");
                snappedPos = LogicPos2WorldPos((int)x, (int)y);

                snippetInfo = snippet.GetComponent<RegionSnippetInfo>();
                snippetInfo.relativePos = new Vector2(x - buildingFootPoint.x, y - buildingFootPoint.y);//相对于脚点的相对坐标

                snippet.transform.position = new Vector3(snappedPos.x, -0.1f, snappedPos.y);
                regionSnippetsList.Add(snippetInfo);
            }
        }
    }

    public void ClearRegionSnippets()
    {
        MapTile tile;
        foreach (var snippet in regionSnippetsList)
        {
            tile = GetTile((int)snippet.logicPos.x, (int)snippet.logicPos.y);
            tile.canBuild = false;
            //Debug.Log($"ClearRegionSnippets {snippet.GetComponent<RegionSnippetInfo>().logicPos} canBuild {tile.canBuild}}");
            Destroy(snippet.gameObject);
        }
        regionSnippetsList.RemoveAll(snippet => snippet) ;
    }

    public void RefreshRegionSnippets(Vector2 logicPos)
    {
        Vector2 tilePos;
        MapTile tile;
        RegionSnippetInfo snippetInfo;

        for (int i = 0; i < regionSnippetsList.Count; i++)
        {
            snippetInfo = regionSnippetsList[i];

            tilePos = logicPos + snippetInfo.relativePos;
            tile = GetTile(tilePos);
            //Debug.Log($"RefreshRegionSnippets {tilePos} canBuild {tile.canBuild}}");

            snippetInfo.SetCanBuild(tile.canBuild);
            snippetInfo.transform.position = new Vector3(tile.x, 0, tile.y);
            snippetInfo.logicPos = tilePos;
        }
    }

    public bool CheckCanBuild()
    {
        bool canBuild = true;
        foreach (var snippet in regionSnippetsList)
        {
            if (!snippet.canBuild)
            {
                canBuild = false;
                break;
            }
        }
        return canBuild;
    }

    public Vector2 LogicPos2WorldPos(Vector2 logicPos)
    {
        return LogicPos2WorldPos((int)logicPos.x, (int)logicPos.y);
    }

    public Vector2 LogicPos2WorldPos(int logicX, int logicY)
    {
        int dirX = 1;
        int dirY = 1;
        if (logicX < 0)
        {
            logicX = -logicX;
            dirX = -dirX;
        }
        if (logicY < 0)
        {
            logicY = -logicY;
            dirY = -dirY;
        }
        MapTile tile = GetTile(logicX, logicY);
        return new Vector2(tile.x * dirX, tile.y * dirY);
    }

    public Vector2 WorldPos2LogicPos(Vector2 worldPos, CoordinateBound bound = CoordinateBound.Close)
    {
        return WorldPos2LogicPos(worldPos.x, worldPos.y, bound);
    }

    public Vector2 WorldPos2LogicPos(float worldX, float worldY, CoordinateBound bound = CoordinateBound.Close)
    {
        int dirX = 1;
        int dirY = 1;
        if(worldX < 0)
        {
            worldX = -worldX;
            dirX = -dirX;
        }
        if (worldY < 0)
        {
            worldY = -worldY;
            dirY = -dirY;
        }

        float logicX = (int)((worldX + TileSize.x / 2) / TileSize.x);
        float logicY = (int)((worldY + TileSize.y / 2) / TileSize.y);

        if (bound == CoordinateBound.Open)
        {
            if (logicX * TileSize.x - TileSize.x / 2 == worldX)
            {
                logicX -= 1;
            }
            if (logicY * TileSize.y - TileSize.y / 2 == worldY)
            {
                logicY -= 1;
            }
        }

        return new Vector2(logicX * dirX,logicY * dirY);
    }

    public Vector3 World2SnappedPos(Vector3 worldPos)
    {
        return World2SnappedPos(worldPos.x, worldPos.y, worldPos.z);
    }

    public Vector3 World2SnappedPos(float x, float y, float z)
    {
        Vector2 logicPos = WorldPos2LogicPos(x, z);
        Vector2 stepPos = LogicPos2WorldPos((int)logicPos.x, (int)logicPos.y);
        return new Vector3(stepPos.x, y, stepPos.y);
    }

    public MapTile GetTile(int logicX, int logicY)
    {
        return tiles[logicX, logicY];
    }

    public MapTile GetTile(Vector2 logicPos)
    {
        return GetTile((int)logicPos.x, (int)logicPos.y);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;

        for (int col = 0; col < TileAmount.x; col++)
        {
            Gizmos.DrawLine(new Vector3(TileSize.x * (col - 0.5f), 0, -TileSize.y / 2), new Vector3(TileSize.x * (col - 0.5f), 0, TileSize.y * TileAmount.y - TileSize.y / 2));
        }
        for(int row = 0;row < TileAmount.y; row++)
        {
            Gizmos.DrawLine(new Vector3(-TileSize.x / 2,0,TileSize.y * (row - 0.5f)),new Vector3(TileSize.x * TileAmount.x - TileSize.x/2,0, TileSize.y * (row - 0.5f)));
        }
    }
}

public class MapTile
{
    public float x;
    public float y;

    public int logicX;
    public int logicY;

    public bool canBuild;
    public bool canWalk;

    public MapTile(int row, int col, Vector2 tileSize)
    {
        this.x = row * tileSize.x;
        this.y = col * tileSize.y;
        this.logicX = row;
        this.logicY = col;

        canBuild = true;
        canWalk = true;
    }
}

public enum CoordinateBound
{
    Open,
    Close
}