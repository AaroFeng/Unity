using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DummyStateManager : EnemyStateManagerBase
{
    protected override void SetStartState()
    {
        //添加状态
        AddState<EnemyStateIdle>();
        AddState<EnemyStatePatrol>();
        AddState<EnemyStateDamage>();
        //初始状态
        ChangeState<EnemyStateIdle>();
    }
}
