using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector2 lastLogicPos;
    public GridMap map;
    [HideInInspector]
    public GameObject underCreatingBuilding;

    private void Awake()
    {
        if(Instance == null) 
        {
            Instance = this;   
        }
        else if(Instance != this)
        {
            Destroy(this);
        }
    }

    public static Player Instance { get; private set; }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit,Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
        {
            Vector3 hitPoint = hit.point;
            if (underCreatingBuilding != null)
            {
                Vector2 mouseLogicPos = map.WorldPos2LogicPos(hitPoint.x, hitPoint.z);
                if (lastLogicPos == default(Vector2) || mouseLogicPos != lastLogicPos)
                {
                    lastLogicPos = mouseLogicPos;
                    Vector2 snappedPos = map.LogicPos2WorldPos(mouseLogicPos);

                    underCreatingBuilding.transform.position = new Vector3(snappedPos.x, 0, snappedPos.y);
                    map.RefreshRegionSnippets(mouseLogicPos);
                }

                if (Input.GetMouseButton(0))
                {
                    if (map.CheckCanBuild())
                    {
                        BuildComplete();
                    }
                    else
                    {
                        Debug.Log("所需空间不足");
                    }
                }
            }
        }
    }

    public void BuildStart(BuildConfig cfg) 
    {
        if(cfg == null)
        {
            return;
        }
        if(underCreatingBuilding != null)
        {
            return;
        }
        underCreatingBuilding = Instantiate(cfg.prefab,new Vector3(0,0,0),Quaternion.identity);
        BuildingHelper.SetBuildingAlpha(underCreatingBuilding, 0.2f);
        BuildingHelper.SetBuildingLayer(underCreatingBuilding,"BuildingFollow");
        map.CreateRegionSnippets(underCreatingBuilding);
    }

    private void BuildComplete() 
    {
        BuildingHelper.SetBuildingAlpha(underCreatingBuilding, 1.0f);
        BuildingHelper.SetBuildingLayer(underCreatingBuilding, "Default");
        underCreatingBuilding = null;
        map.ClearRegionSnippets();
    }
}
