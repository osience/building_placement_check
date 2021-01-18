using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/BuildingConfig",fileName = "buildingx")]
public class BuildConfig : ScriptableObject
{
    public string id;//id
    public string name;//名字
    public GameObject prefab;//预制体
    public int vigor;//所需体力
}
