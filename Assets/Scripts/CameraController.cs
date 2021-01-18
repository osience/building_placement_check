using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public float panBorderThickness = 10f;
    public Vector2 panLimt;
    public float minY = 20f;
    public float maxY = 200f;
    public float scrollSpeed = 2000f;
    private void Update()
    {
        //Vector3 pos = this.transform.position;
        //if (Input.GetKey("w") || Input.mousePosition.y > Screen.height - panBorderThickness)
        //{
        //    pos.z += panSpeed * Time.deltaTime;
        //}
        //if (Input.GetKey("s") || Input.mousePosition.y < panBorderThickness)
        //{
        //    pos.z -= panSpeed * Time.deltaTime;
        //}
        //if (Input.GetKey("a") || Input.mousePosition.x < panBorderThickness)
        //{
        //    pos.x -= panSpeed * Time.deltaTime;
        //}
        //if (Input.GetKey("d") || Input.mousePosition.x > Screen.width - panBorderThickness)
        //{
        //    pos.x += panSpeed * Time.deltaTime;
        //}
        //pos.x = Mathf.Clamp(pos.x, -panLimt.x, panLimt.x);
        //pos.y = Mathf.Clamp(pos.y, minY, maxY);
        //pos.z = Mathf.Clamp(pos.z, -panLimt.y, panLimt.y);
        //this.transform.position = pos;

        Vector3 localPos = transform.localPosition;
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        localPos += scroll * transform.forward * 2000 * Time.deltaTime;
        transform.localPosition = localPos;
    }
}

