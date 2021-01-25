using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void BuildCompleteHandler();
public class BuildingBase : MonoBehaviour
{

    [HideInInspector]
    public Vector3 offset; //建筑与格子坐标的偏移

    public void SetPosition(Vector3 position)
    {
        this.transform.position = position + offset;
    }
    //public List<GameObject> regionSnippets;
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}
