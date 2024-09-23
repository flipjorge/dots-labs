using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
public partial struct CubeSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        foreach (var (cube, transform, entity) in SystemAPI.Query<RefRW<CubeComponent>, RefRW<LocalTransform>>().WithEntityAccess())
        {
            var localTransform = transform.ValueRO;
            var upDirection = localTransform.Up();
            
            transform.ValueRW.Position += upDirection * cube.ValueRO.Speed * SystemAPI.Time.DeltaTime;

            cube.ValueRW.LifeTime += SystemAPI.Time.DeltaTime;
            if (cube.ValueRO.LifeTime > cube.ValueRO.MaxLifeTime)
            {
                ecb.DestroyEntity(entity);
            }
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}