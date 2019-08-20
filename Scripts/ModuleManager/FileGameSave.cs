using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class FileGameSave
{
    public string sceneName; //场景名称
    public FilePlayer player = new FilePlayer(); //玩家
    public List<FileEnemy> enemys = new List<FileEnemy>(); //场景内敌人集合
    public FileOrcKing orcKing = new FileOrcKing(); //兽人首领
}

[Serializable]
public class FilePlayer
{
    //玩家当前位置与属性状态
    public float PosX; //位置
    public float PosY;
    public float PosZ;
    public float RotX; //旋转
    public float RotY;
    public float RotZ;
    public float RotW;
    public float atk; //攻击力
    public float hp; //体力值
    public float hpMax; //最大体力值
    public float sp; //耐力值
    public float spMax; //最大耐力值
    public float ep; //能量值 
    public float epMax; //最大能量值
    public int epPoint; //能量点
    public int epPointMax; //最大能量点
    public bool spUseup; //耐力耗尽
    //玩家摄像机位置与水平旋转
    public float cameraRotX; //旋转
    public float cameraRotY;
    public float cameraRotZ;
    public float cameraRotW;
    public float cameraDistance; //偏移向量长度
    public float cameraAngleLerp; //偏移向量垂直角度
}

[Serializable]
public class FileEnemy
{
    //敌人当前位置与属性状态
    public float PosX; //位置
    public float PosY;
    public float PosZ;
    public float RotX; //旋转
    public float RotY;
    public float RotZ;
    public float RotW;
    public float hp; //体力值
    public bool death; //死亡状态
}

[Serializable]
public class FileOrcKing
{
    //兽人首领当前位置与属性状态
    public float PosX; //位置
    public float PosY;
    public float PosZ;
    public float RotX; //旋转
    public float RotY;
    public float RotZ;
    public float RotW;
    public float hp; //体力值
    public bool death; //死亡状态
}
