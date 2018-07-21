using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public struct DoorBase
{
    public Transform trans;
    public MapType type;
    public DoorBase( Transform position , MapType typeKind)
    {
        trans = position;
        type = typeKind;
    }
}
public enum MapType{
    road,
    room,
}
public class MapMgr : MonoBehaviour {

    private List<DoorBase> doorList = new List<DoorBase>();
    
    public List<GameObject> roomPrefab = new List<GameObject>();
    public List<GameObject> roadPrefab = new List<GameObject>();
    int roadLen;
    int roomLen;
    void Start()
    {
        Transform obj = this.transform.GetChild(0);
        SetPosition(obj);
        roadLen = this.roadPrefab.Count;
        roomLen = this.roomPrefab.Count;
    }
    const float eps = 1e-4f;
    int step = 0;
    void Update() { 
    }

    public void Next()
    { 
        DoorBase top = doorList[0];
        doorList.RemoveAt(0);
        GameObject target;          
        if (top.type == MapType.road)   
        {
            int idx = UnityEngine.Random.Range(0, roomLen );
            target = roomPrefab[idx];
        }
        else
        {
            int idx = UnityEngine.Random.Range(0, roadLen );
            target = roadPrefab[idx];
        }
        GameObject nextPrefab = GameObject.Instantiate(target, new Vector3(0,0,0), Quaternion.identity) as GameObject;
        nextPrefab.transform.SetParent(this.gameObject.transform);
        Transform trans = nextPrefab.transform.GetChild(0);
        if( NumberEqual( -top.trans.eulerAngles.y + trans.eulerAngles.y,180) || NumberEqual(-top.trans.eulerAngles.y + trans.eulerAngles.y, -180))
        {
        }
        else
        {
            int aim = 0;
            if (top.trans.eulerAngles.y > 0)
            {
                aim = (int)(top.trans.eulerAngles.y)- 180 ;
            }
            else
            {
                aim = (int)(top.trans.eulerAngles.y)+ 180 ;
            }
            MyRotate(nextPrefab.transform, new Vector3(0, aim - (int)(trans.eulerAngles.y), 0));
        }
        Debug.Log(top.trans.position + "  " + trans.position);
        nextPrefab.transform.position = nextPrefab.transform.position + (top.trans.position - trans.position);
        SetPosition(nextPrefab.transform, 1);
    }   
    private void MyRotate(Transform trans,Vector3 vec)
    {
        int y = (int)(vec.y+trans.eulerAngles.y);
        while (y > 180)
        {
            y -= 360;
        }
        while (y <= -180)
        {
            y += 360;
        }
        Vector3 res = new Vector3((int)trans.eulerAngles.x  , y ,(int) trans.eulerAngles.z );
        trans.eulerAngles = res;
    }   
    private void SetPosition(Transform obj,int index=-1)
    {
        int count = 0;
        foreach(Transform child in obj)
        {
            if (child.tag == "out")
            {
                count++;
                if (count == index)
                    continue;
                doorList.Add(new DoorBase(child, (MapType)Enum.Parse(typeof(MapType), obj.tag.ToString())));
            }
        }
    }
    private bool NumberEqual(float a,float b)
    {
        if (Mathf.Abs(a - b) <= eps)
        {
            return true;
        }
        else return false;
    }

}   

