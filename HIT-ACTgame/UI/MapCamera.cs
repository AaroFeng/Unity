using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCamera : MonoBehaviour
{
    Transform player; //玩家
    Transform mapArrow; //玩家箭头

    void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        mapArrow = (Instantiate(Resources.Load("Prefab/UI/MapArrow"), transform.position, transform.rotation) as GameObject).transform;
	}
	
	void Update ()
    {
        //摄像机跟随玩家
        Vector3 cameraPos = player.position;
        cameraPos.y += 35.0f;
        transform.position = cameraPos;
        //玩家箭头跟随玩家旋转
        Vector3 arrowPos = player.position;
        arrowPos.y += 25.0f;
        mapArrow.position = arrowPos;
        mapArrow.localRotation = Quaternion.Euler(-90, 0, player.rotation.eulerAngles.y);
	}
}
