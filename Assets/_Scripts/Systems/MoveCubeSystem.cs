#region Author
///-----------------------------------------------------------------
///   Namespace:		YU.ECS
///   Class:			MoveCubeSystem
///   Author: 		    yutian
///-----------------------------------------------------------------
#endregion

using Unity.Mathematics;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Transforms;

namespace YU.ECS
{
    public class MoveCubeSystem : JobComponentSystem
    {
        [BurstCompile]
        struct JobProcess : IJobForEach<Translation>
        {
            public bool isEndMoveIn;
            public bool isEndMoveOut;
            public float velocityLength;
            public float3 curCenterPosition;

            //Job的执行函数，作用是控制cube的运动
            public void Execute(ref Translation translation)
            {
                if (isEndMoveIn)
                {
                    if (translation.Value.x > curCenterPosition.x)
                    {
                        translation.Value = translation.Value + new float3(-velocityLength * (translation.Value.x - curCenterPosition.x), 0f, 0f);
                    }
                    else
                    {
                        translation.Value = translation.Value + new float3(velocityLength * (curCenterPosition.x - translation.Value.x), 0f, 0f);
                    }

                    if (translation.Value.y > curCenterPosition.y)
                    {
                        translation.Value = translation.Value + new float3(0f, -velocityLength * (translation.Value.y - curCenterPosition.y), 0f);
                    }
                    else
                    {
                        translation.Value = translation.Value + new float3(0f, velocityLength * (curCenterPosition.y - translation.Value.y), 0f);
                    }

                    if (translation.Value.z > curCenterPosition.z)
                    {
                        translation.Value = translation.Value + new float3(0f, 0f, -velocityLength * (translation.Value.z - curCenterPosition.z));
                    }
                    else
                    {
                        translation.Value = translation.Value + new float3(0f, 0f, velocityLength * (curCenterPosition.z - translation.Value.z));
                    }
                }

                if (isEndMoveOut)
                {
                    if (translation.Value.x > curCenterPosition.x)
                    {
                        translation.Value = translation.Value + new float3(velocityLength * 2 * (translation.Value.x - curCenterPosition.x), 0f, 0f);
                    }
                    else
                    {
                        translation.Value = translation.Value + new float3(-velocityLength * 2 * (curCenterPosition.x - translation.Value.x), 0f, 0f);
                    }

                    if (translation.Value.y > curCenterPosition.y)
                    {
                        translation.Value = translation.Value + new float3(0f, velocityLength * 2 * (translation.Value.y - curCenterPosition.y), 0f);
                    }
                    else
                    {
                        translation.Value = translation.Value + new float3(0f, -velocityLength * 2 * (curCenterPosition.y - translation.Value.y), 0f);
                    }

                    if (translation.Value.z > curCenterPosition.z)
                    {
                        translation.Value = translation.Value + new float3(0f, 0f, velocityLength * 2 * (translation.Value.z - curCenterPosition.z));
                    }
                    else
                    {
                        translation.Value = translation.Value + new float3(0f, 0f, -velocityLength * 2 * (curCenterPosition.z - translation.Value.z));
                    }
                }

            }
        }

        float _velocityLength;
        protected override void OnCreate()
        {
            _velocityLength = 0.01f;
        }
        

        //系统会每帧调用这个函数
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {

            //初始化一个job
            var job = new JobProcess {
                isEndMoveIn = Main.Instance.isEndMoveIn,
                isEndMoveOut = Main.Instance.isEndMoveOut,
                velocityLength = _velocityLength,
                curCenterPosition = WorldGenerator.Instance.lastReplacePosition+new float3(0f,500f,2000f),
            };

            //开始job      
            return job.Schedule(this, inputDeps);
        }
    }
}