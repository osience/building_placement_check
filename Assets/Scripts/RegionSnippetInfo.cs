using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionSnippetInfo : MonoBehaviour
{
    public Color legalRegionColor;//合法区域颜色
    public Color illegalRegionColor;//不合法区域颜色

    [HideInInspector]
    public bool canBuild;

    //[HideInInspector]
    public Vector2 logicPos;

    //相对于建筑脚点的坐标
    [HideInInspector]
    public Vector2 relativePos;

    private Renderer render;

    private void Awake()
    {
        render = GetComponent<Renderer>();
        SetCanBuild(true);
    }

    public void SetCanBuild(bool canBuild)
    {
        if(this.canBuild == canBuild)
        {
            return;
        }
        this.canBuild = canBuild;
        render.material.SetColor("_TintColor", canBuild ? legalRegionColor : illegalRegionColor);
    }
}
