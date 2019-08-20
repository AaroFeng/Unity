using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuliStateManager : NPCStateManagerBase
{
    protected override void SetStartState()
    {
        //添加状态
        AddState<NPCStateIdle>();
        AddState<NPCStatePatrol>();

        //设定状态属性值
        idleCDMin = 10.0f;
        idleCDMax = 15.0f;
        patrolSpeed = 1.2f;

        //初始状态
        ChangeState<NPCStateIdle>();
    }
}
