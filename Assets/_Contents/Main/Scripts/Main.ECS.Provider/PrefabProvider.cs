using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace Main.ECS.Provider
{
    [RequiresEntityConversion, DisallowMultipleComponent]
    sealed class PrefabProvider : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
    {
        [Serializable]
        class PrefabDetail
        {
            public int ID = default;
            public GameObject Prefab = default;
        }

        [SerializeField] PrefabDetail[] _prefabDetails = default;
        public Dictionary<int, Entity> PrefabEntities { get; } = new Dictionary<int, Entity>();

        public void DeclareReferencedPrefabs(List<GameObject> gameObjects)
        {
            foreach (var detail in _prefabDetails)
            {
                gameObjects.Add(detail.Prefab);
            }
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            foreach (var detail in _prefabDetails)
            {
                PrefabEntities.Add(detail.ID, conversionSystem.GetPrimaryEntity(detail.Prefab));
            }
        }
    }
}
