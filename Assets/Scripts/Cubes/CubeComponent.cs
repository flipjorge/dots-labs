using Unity.Entities;

public struct CubeComponent : IComponentData
{
    public float Speed;
    public float LifeTime;
    public float MaxLifeTime;
}