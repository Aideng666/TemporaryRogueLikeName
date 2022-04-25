using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] ParticleSystem dashParticleStart;
    [SerializeField] ParticleSystem dashParticleEnd;

    ParticleSystem currentParticle;

    public static ParticleManager Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    public GameObject SpawnParticle(ParticleTypes type, Vector3 position)
    {
        switch (type)
        {
            case ParticleTypes.DashStart:

                currentParticle = Instantiate(dashParticleStart, position, Quaternion.Euler(-90, 0, 0));

                break;

            case ParticleTypes.DashEnd:

                currentParticle = Instantiate(dashParticleEnd, position, Quaternion.Euler(-90, 0, 0));

                break;
        }

        return currentParticle.gameObject;
    }
}
