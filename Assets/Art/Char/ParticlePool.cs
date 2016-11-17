using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// The Particle Pool keeps a circular buffer of particle systems that can be spawned at a given location,
/// the particle system will remain at the spawned location until the buffer index hits the system again.
/// This means this class is not safe to be used by more than one user since many callers may get the same system referenced.
/// 
/// </summary>
public class ParticlePool : MonoBehaviour {


    public ParticleData[] particleData;

    [Serializable]
    public class ParticleData {
        public ParticleSystem particleSystemPrefab;
        public int numberOfSystemsToStore;
    }

    private Dictionary<string, int> indexes;
    private Dictionary<string, ParticleData> particleDataDict;
    private Dictionary<string, List<ParticleSystem>> particles;

	void Start () {
        indexes = new Dictionary<string, int>();
        particleDataDict = new Dictionary<string, ParticleData>();
        particles = new Dictionary<string, List<ParticleSystem>>();
        for(int i = 0; i < particleData.Length; i++) {
            List<ParticleSystem> list = new List<ParticleSystem>(particleData[i].numberOfSystemsToStore);
            for(int j = 0; j < particleData[i].numberOfSystemsToStore; j++) {
                ParticleSystem particleSystem = (ParticleSystem)Instantiate(particleData[i].particleSystemPrefab);
                particleSystem.transform.parent = this.transform;
                particleSystem.gameObject.SetActive(false);
                list.Add(particleSystem);
            }
            particles.Add(particleData[i].particleSystemPrefab.name, list);
            indexes.Add(particleData[i].particleSystemPrefab.name, 0);
            particleDataDict.Add(particleData[i].particleSystemPrefab.name, particleData[i]);
        }
    }

    public void showParticleSystemAt(string particleSystemName, Vector3 position) {
        ParticleData particleData = particleDataDict[particleSystemName];
        if(particleData != null) {
            indexes[particleSystemName] = (indexes[particleSystemName] + 1) % particles[particleSystemName].Count;
            ParticleSystem particleSystemToShow = particles[particleSystemName][indexes[particleSystemName]];

            if(particleSystemToShow.gameObject.activeSelf) {
                particleSystemToShow.gameObject.SetActive(false);
            }
            particleSystemToShow.transform.position = position;
            particleSystemToShow.transform.rotation = Quaternion.identity;
            particleSystemToShow.gameObject.SetActive(true);
        }
    }

    public int NumberOfParticleSystems() {
        return particleData.Length;
    }

    public GameObject GetParticleGameObject (int index) {
        return particleData[index].particleSystemPrefab.gameObject;
    }
}
