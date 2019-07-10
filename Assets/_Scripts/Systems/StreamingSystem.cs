#region Author
///-----------------------------------------------------------------
///   Namespace:		YU.ECS
///   Class:			StreamingSystem
///   Author: 		    yutian
///-----------------------------------------------------------------
#endregion

using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;


namespace YU.ECS
{
    //UpdateInGroup可以控制System间的执行顺序
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public class StreamingSystem : JobComponentSystem
    {
        //这个System用来动态加载世界，当玩家移动一定程度时，将远端的一层cube移到前进方向那一端，效率最高
        [BurstCompile]
        struct StreamingJob : IJobForEachWithEntity<Translation>
        {
            public int scale;
            public float stepLength;
            public float3 currCamPosition;
            [ReadOnly]public NativeArray<int> loadType;
            [NativeDisableParallelForRestriction] [WriteOnly] public NativeArray<float> isNearCube;
            [NativeDisableParallelForRestriction] [ReadOnly] public NativeArray<int> lastNearCubeIndex;

            private float3 position;
            public void Execute(Entity entity, int index, ref Translation translation)
            {
                if (index!= lastNearCubeIndex[0]&&math.distance(currCamPosition, translation.Value)< stepLength * 0.5f)
                {
                    isNearCube[0] = 1;
                    isNearCube[1] = translation.Value.x;
                    isNearCube[2] = translation.Value.y;
                    isNearCube[3] = translation.Value.z;
                    isNearCube[4] = index;
                }

                if (loadType[0] == 1)
                {
                    if (translation.Value.x <= (currCamPosition.x - ((scale - 1) * 0.5f+1) * stepLength))
                    {
                        position = new float3()
                        {
                            x = translation.Value.x + scale * stepLength,
                            y = translation.Value.y,
                            z = translation.Value.z
                        };
                        translation = new Translation() { Value = position };
                    }
                }
                if (loadType[1] == 1)
                {
                    if (translation.Value.y <= (currCamPosition.y - ((scale - 1) * 0.5f + 1) * stepLength))
                    {
                        position = new float3()
                        {
                            x = translation.Value.x,
                            y = translation.Value.y + scale * stepLength,
                            z = translation.Value.z
                        };
                        translation = new Translation() { Value = position };
                    }
                }
                if (loadType[2] == 1)
                {
                    if (translation.Value.z <= (currCamPosition.z - ((scale - 1) * 0.5f + 1) * stepLength))
                    {
                        position = new float3()
                        {
                            x = translation.Value.x,
                            y = translation.Value.y,
                            z = translation.Value.z + scale * stepLength
                        };
                        translation = new Translation() { Value = position };
                    }
                }
                if (loadType[3] == 1)
                {
                    if (translation.Value.x >= (currCamPosition.x + ((scale - 1) * 0.5f + 1) * stepLength))
                    {
                        position = new float3()
                        {
                            x = translation.Value.x - scale * stepLength,
                            y = translation.Value.y,
                            z = translation.Value.z 
                        };
                        translation = new Translation() { Value = position };
                    }
                }
                if (loadType[4] == 1)
                {
                    if (translation.Value.y >= (currCamPosition.y + ((scale - 1) * 0.5f + 1) * stepLength))
                    {
                        position = new float3()
                        {
                            x = translation.Value.x,
                            y = translation.Value.y - scale * stepLength,
                            z = translation.Value.z
                        };
                        translation = new Translation() { Value = position };
                    }
                }
                if (loadType[5] == 1)
                {
                    if (translation.Value.z >= (currCamPosition.z + ((scale - 1) * 0.5f + 1) * stepLength))
                    {
                        position = new float3()
                        {
                            x = translation.Value.x,
                            y = translation.Value.y,
                            z = translation.Value.z - scale * stepLength
                        };
                        translation = new Translation() { Value = position };
                    }
                }

            }
        }


        int scale;
        float3 currCamPosition = float3.zero;
        [DeallocateOnJobCompletion] NativeArray<int> loadType = new NativeArray<int>(6, Allocator.Persistent);
        [DeallocateOnJobCompletion] NativeArray<float> isNearCube = new NativeArray<float>(5, Allocator.Persistent);
        //用来双缓冲，防止race condition
        [DeallocateOnJobCompletion] NativeArray<int> lastNearCubeIndex = new NativeArray<int>(1, Allocator.Persistent);

        protected override void OnCreate()
        {
            scale = Main.Instance.scale;
        }

        protected override void OnDestroy()
        {
            loadType.Dispose();
            isNearCube.Dispose();
            lastNearCubeIndex.Dispose();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {

            if (isNearCube[0] == 1)
            {
                lastNearCubeIndex[0] = (int)isNearCube[4];
                WorldGenerator.Instance.NearToNewCube(new float3(isNearCube[1], isNearCube[2], isNearCube[3]));
            }
            for (int ii = 0; ii < 6; ii++)
            {
                loadType[ii] = 0;
            }
            isNearCube[0] = 0;

            if (Main.Instance.isNeedToLoadXpos)
            {
                loadType[0] = 1;
                Main.Instance.isNeedToLoadXpos = false;
            }
            if (Main.Instance.isNeedToLoadYpos)
            {
                loadType[1] = 1;
                Main.Instance.isNeedToLoadYpos = false;
            }
            if (Main.Instance.isNeedToLoadZpos)
            {
                loadType[2] = 1;
                Main.Instance.isNeedToLoadZpos = false;
            }
            if (Main.Instance.isNeedToLoadXneg)
            {
                loadType[3] = 1;
                Main.Instance.isNeedToLoadXneg = false;
            }
            if (Main.Instance.isNeedToLoadYneg)
            {
                loadType[4] = 1;
                Main.Instance.isNeedToLoadYneg = false;
            }
            if (Main.Instance.isNeedToLoadZneg)
            {
                loadType[5] = 1;
                Main.Instance.isNeedToLoadZneg = false;
            }

            currCamPosition = Main.Instance.mainCamera.transform.position;

            var job = new StreamingJob
            {
                scale = scale,
                stepLength = Main.Instance.stepLength,
                loadType = loadType,
                currCamPosition = currCamPosition,
                isNearCube = isNearCube,
                lastNearCubeIndex = lastNearCubeIndex

            }.Schedule(this, inputDeps);


            return job;
        }
    }
}