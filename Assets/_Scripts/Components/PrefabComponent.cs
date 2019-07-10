#region Author
///-----------------------------------------------------------------
///   Namespace:		YU.ECS
///   Class:			PrefabComponent
///   Author: 		    yutian
///-----------------------------------------------------------------
#endregion

using Unity.Entities;

public struct PrefabComponent : IComponentData
{
    public Entity Prefab;
}
