using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector2 lastLogicPos;
    public GridMap map;
    private GameObject buildingObj;
    private BuildingBase building;

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
        //return;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit,Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
        {
            Vector3 hitPoint = hit.point;
            if (buildingObj != null)
            {
                Vector2 mouseLogicPos = map.WorldPos2LogicPos(hitPoint.x, hitPoint.z);
                if (lastLogicPos == default(Vector2) || mouseLogicPos != lastLogicPos)
                {
                    lastLogicPos = mouseLogicPos;
                    Vector2 snappedPos = map.LogicPos2WorldPos(mouseLogicPos);

                    building.SetPosition(new Vector3(snappedPos.x, 0, snappedPos.y));
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
        if(buildingObj != null)
        {
            return;
        }
        buildingObj = Instantiate(cfg.prefab,new Vector3(10.5f,0,10.3f),Quaternion.identity);

        building = buildingObj.GetComponent<BuildingBase>();
        building.offset = building.transform.position - map.World2SnappedPos(building.transform.position);

        buildingObj.SetAlphaRecursively(0.2f);
        buildingObj.SetLayerRecursively("BuildingFollow");
        map.CreateRegionSnippets(building);
    }

    private void BuildComplete() 
    {
        buildingObj.SetAlphaRecursively(1.0f);
        buildingObj.SetLayerRecursively("Default");
        buildingObj = null;
        building = null;
        map.ClearRegionSnippets();
    }
}
