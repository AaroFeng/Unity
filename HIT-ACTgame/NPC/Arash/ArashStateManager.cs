using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArashStateManager : NPCStateManagerBase
{
    protected override void SetStartState()
    {
        //添加状态
        AddState<NPCStateIdle>();
        AddState<NPCStatePatrol>();

        //设定状态属性值
        idleCDMin = 20.0f;
        idleCDMax = 35.0f;
        patrolSpeed = 3.0f;

        //初始状态
        ChangeState<NPCStateIdle>();
    }
}
