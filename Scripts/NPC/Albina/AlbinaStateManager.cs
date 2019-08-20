using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlbinaStateManager : NPCStateManagerBase
{
    protected override void SetStartState()
    {
        //添加状态
        AddState<NPCStateIdle>();
        AddState<NPCStatePatrol>();

        //设定状态属性值
        idleCDMin = 30.0f;
        idleCDMax = 50.0f;
        patrolSpeed = 1.0f;

        //初始状态
        ChangeState<NPCStateIdle>();
    }
}
