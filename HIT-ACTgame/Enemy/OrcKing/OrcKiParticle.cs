using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcKiParticle : ParticleBase
{
    public void Play(OrcKiState orcKiState)
    {
        switch (orcKiState)
        {
            case OrcKiState.Attack:
                ParticlePlay(particleList[0]); //播放粒子效果组
                break;
            case OrcKiState.Damage:
                RandomPositionDirection(particleList[1]);
                ParticlePlay(particleList[1]);
                break;
            case OrcKiState.Repel:
                ParticlePlay(particleList[2]);
                break;
            case OrcKiState.FlameJet:
                ParticlePlay(particleList[3]);
                break;
            case OrcKiState.BulletShoot:
                ParticlePlay(particleList[4]);
                break;
            default:
                //Debug.Log(playerState.ToString() + ":无此类型粒子效果组");
                break;
        }
    }

    public void Stop(OrcKiState orcKiState)
    {
        switch (orcKiState)
        {
            case OrcKiState.Attack:
                ParticleStop(particleList[0]); //停止粒子效果组
                break;
            case OrcKiState.Damage:
                ParticleStop(particleList[1]);
                break;
            case OrcKiState.Repel:
                ParticleStop(particleList[2]); //停止粒子效果组
                break;
            case OrcKiState.FlameJet:
                ParticleStop(particleList[3]);
                break;
            case OrcKiState.BulletShoot:
                ParticleStop(particleList[4]);
                break;
            default:
                //Debug.Log(playerState.ToString() + ":无此类型粒子效果组");
                break;
        }
    }

    void RandomPositionDirection(GameObject particles) //受伤溅血 随机 位置与方向
    {
        Transform par = particles.transform;
        par.localPosition = new Vector3(0, Random.Range(0.5f, 2.0f), 0); //随机高度
        par.localRotation = Quaternion.Euler(Random.Range(-60, 60), Random.Range(0, 360), 0); //随机方向
    }
}
