using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct SpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        foreach (var spawner in SystemAPI.Query<RefRW<Spawner>>())
            ProcessSpawner(ref state, spawner, ecb);
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    private void ProcessSpawner(ref SystemState state, RefRW<Spawner> spawner, EntityCommandBuffer ecb)
    {
        if (!(spawner.ValueRO.NextSpawnTime < SystemAPI.Time.ElapsedTime)) return;
        
        var newEntity = state.EntityManager.Instantiate(spawner.ValueRO.Prefab);

        var uniqueSeed = (uint)newEntity.Index + (uint)newEntity.Version * 397;
        var random = Random.CreateFromIndex(uniqueSeed);
        var direction = random.NextFloat3Direction();
        
        var transform = LocalTransform.FromPosition(spawner.ValueRO.SpawnPosition);
        transform.Rotation = quaternion.Euler(direction);
        
        ecb.SetComponent(newEntity, transform);
            
        ecb.AddComponent(newEntity, new CubeComponent()
        {
            Speed = 8,
            LifeTime = 0,
            MaxLifeTime = 5
        });
            
        spawner.ValueRW.NextSpawnTime = (float)SystemAPI.Time.ElapsedTime + spawner.ValueRO.SpawnRate;
    }
}