using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DummyCharacter : EnemyCharacterBase
{
    void Awake()
    {
        //设定属性值
        enemyName = "训练假人"; //名字
        height = 2.2f; //身高
        hpMax = 300; //最大体力值

        //设定状态属性值
        idleCDMin = 5.0f;
        idleCDMax = 5.0f;
        patrolSpeed = 1.2f;
    }
}
