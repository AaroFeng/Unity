using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCharacterBase : MonoBehaviour
{
    public Transform[] pathPoints; //巡逻路径点
    public Transform[] pathPointsIdle; //空闲状态路径点

    //NPC属性值
    protected string roleName; //角色名字
    public string RoleName { get { return roleName; } }
    protected float height; //角色高度
    public float Height { get { return height; } }

    void Start()
    {

    }

    void Update()
    {

    }
}
