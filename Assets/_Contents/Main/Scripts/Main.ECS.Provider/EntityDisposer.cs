using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Main.ECS.Provider
{
    public class EntityDisposer : IDisposable
    {
        readonly EntityManager _entityManager;
        NativeArray<Entity> _entities;

        public EntityDisposer(EntityManager entityManager, NativeArray<Entity> entities)
        {
            _entityManager = entityManager;
            _entities = new NativeArray<Entity>(entities, Allocator.Persistent);
        }

        public void Dispose()
        {
            try
            {
                foreach (var entity in _entities)
                {
                    _entityManager.DestroyEntity(entity);
                }
            }
            catch (NullReferenceException e)
            {
                // アプリ終了タイミングでDisposeが呼び出されるとEntityManagerが先に破棄されている事があり、
                // 内部的にNullReferenceExceptionが飛んでくるのでNativeArrayが正しく破棄されるように例外を潰しておく.
                // FIXME: 逆にアプリ終了以外で呼び出されたら想定外なので適切に対処すること.
                Debug.LogWarning($"    >>> {e}, {e.Message}");
            }

            _entities.Dispose();
        }
    }
}
