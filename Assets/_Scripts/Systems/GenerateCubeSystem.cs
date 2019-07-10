#region Author
///-----------------------------------------------------------------
///   Namespace:		YU.ECS
///   Class:			GenerateCubeSystem
///   Author: 		    yutian
///-----------------------------------------------------------------
#endregion

using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Burst;

namespace YU.ECS
{


    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class GenerateCubeSystem : JobComponentSystem
    {
        /// <summary>
        /// 这里用WithEntity的版本，可以获得Entity的index，妙用无穷
        /// </summary>
        [BurstCompile]
        struct GenerateJob : IJobForEachWithEntity<Translation>
        {
            public int isGene;
            public float3 initPosition;
            public int scale;
            public float stepLength;

            private float3 position;
            public void Execute(Entity entity, int index, ref Translation translation)
            {
                if (isGene == 1)
                {

                    position = new float3()
                    {
                        x = initPosition.x + stepLength * (index % scale),
                        y = initPosition.y + stepLength * math.floor(index % (scale * scale) / scale),
                        z = initPosition.z + stepLength * math.floor(index / (scale * scale))
                    };

                    translation = new Translation() { Value = position };
                }
            }
        }


        int isG = 0;
        int scale;
        float stepLength;
        float3 initPosition;
        protected override void OnCreate()
        {
            scale = Main.Instance.scale;
            stepLength = Main.Instance.stepLength;
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (Main.Instance.isGenerateWorld)
            {
                Main.Instance.isGenerateWorld = false;

                //能用ComponentType.ReadOnly标记只读就标，方便Job调度
                EntityQuery entityQuery = GetEntityQuery(ComponentType.ReadOnly<PrefabComponent>());

                NativeArray<PrefabComponent> prefabComponents = entityQuery.ToComponentDataArray<PrefabComponent>(Allocator.Persistent);

                var instance = prefabComponents[0].Prefab;

                float3 initialPosition = float3.zero;

                World.EntityManager.SetComponentData(instance, new Translation { Value = initialPosition });

                NativeArray<Entity> entities = new NativeArray<Entity>(64000,Allocator.Persistent);

                //在Job里用ECB会不停跳回主线程，实测还是直接在主线程用传NativeArray的Instantiate接口快
                World.EntityManager.Instantiate(instance, entities);

                World.EntityManager.DestroyEntity(instance);

                entities.Dispose();
                prefabComponents.Dispose();

                //控制只生成一次，也可以做个管理器管理System的开关，不过性能基本无影响
                isG = 1;
                
                //这里把初始点设为以摄像机为中心的三维矩阵的左下角
                initPosition = Camera.main.transform.position;
                initPosition = new float3(
                    initPosition.x - 0.5f * (scale - 1) * stepLength,
                    initPosition.y - 0.5f * (scale - 1) * stepLength,
                    initPosition.z - 0.5f * (scale - 1) * stepLength
                    );
            }

            var job = new GenerateJob
            {
                isGene = isG,
                initPosition = initPosition,
                scale = scale,
                stepLength = stepLength
            }.Schedule(this, inputDeps);

            isG = 0;

            
            return job;

        }


    }
}