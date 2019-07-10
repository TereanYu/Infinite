#region Author
///-----------------------------------------------------------------
///   Namespace:		YU.ECS
///   Class:			WorldGenerator
///   Author: 		    yutian
///-----------------------------------------------------------------
#endregion

using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace YU.ECS
{
    /// <summary>
    /// 控制一个世界内部具体生成的东西
    /// </summary>
    public class WorldGenerator : MonoSingleton<WorldGenerator>
    {
        private int[,] m_heartShapeTable = new int[11, 11] {
            {0,0,1,1,0,0,0,1,1,0,0},
            {0,1,1,1,1,0,1,1,1,1,0},
            {1,1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1,1},
            {0,1,1,1,1,1,1,1,1,1,0},
            {0,0,1,1,1,1,1,1,1,0,0},
            {0,0,0,1,1,1,1,1,0,0,0},
            {0,0,0,0,1,1,1,0,0,0,0},
            {0,0,0,0,0,1,0,0,0,0,0},
        };

        private int[,] m_TriangleShapeTable = new int[11, 11] {
            {0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0},
            {1,1,1,1,1,1,1,1,1,1,1},
            {0,1,1,1,1,1,1,1,1,1,0},
            {0,0,1,1,1,1,1,1,1,0,0},
            {0,0,0,1,1,1,1,1,0,0,0},
            {0,0,0,0,1,1,1,0,0,0,0},
            {0,0,0,0,0,1,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0},
        };

        private int[,] m_manShapeTable = new int[39, 25] {
            {3,3,3,3,3,3,3,3,1,1,1,1,1,1,1,1,1,1,1,3,3,3,3,3,3},
            {3,3,3,3,3,3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,3,3,3,3},
            {3,3,3,3,3,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,1,3,3,3},
            {3,3,3,3,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,3,3},
            {3,3,3,1,1,1,1,1,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,1,3},
            {3,3,3,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {3,3,3,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {3,3,3,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,0,0,0,0,0,1},
            {3,3,3,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {3,3,3,3,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {3,3,3,3,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,3},
            {3,3,3,3,3,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,3},
            {3,3,3,3,3,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,3,3},
            {3,3,3,3,3,3,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,3,3,3},
            {3,3,3,3,3,3,3,1,0,0,0,0,0,0,0,0,0,0,0,1,3,3,3,3,3},
            {3,3,3,3,3,3,3,1,0,0,0,0,0,0,0,0,0,0,0,1,3,3,3,3,3},
            {3,3,3,3,3,3,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,3,3,3,3},
            {3,3,3,3,3,1,0,0,2,2,2,2,2,2,2,2,2,2,2,0,0,1,3,3,3},
            {3,3,3,3,1,0,0,0,2,2,2,2,2,2,2,2,2,2,2,0,0,0,1,3,3},
            {3,3,3,1,0,0,0,0,2,2,2,2,2,2,2,2,2,2,2,0,0,0,0,1,3},
            {3,3,1,0,0,0,0,0,2,2,2,2,2,2,2,2,2,2,2,0,0,0,0,1,3},
            {3,1,0,0,0,0,0,0,2,2,2,2,2,2,2,2,2,2,2,0,0,0,0,0,1},
            {3,1,0,0,0,0,0,0,2,2,2,2,2,2,2,2,2,2,2,0,0,0,0,0,1},
            {3,1,0,0,0,0,0,0,2,2,2,2,2,2,2,2,2,2,2,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,2,2,2,2,2,2,2,2,2,2,2,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,2,2,2,2,2,2,2,2,2,2,2,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,2,2,2,2,2,2,2,2,2,2,2,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,2,2,2,2,2,2,2,2,2,2,2,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1},
            {1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1},
            {1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1},
            {3,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,3},
            {3,3,3,3,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,3,3,3,3},
            {3,3,3,3,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,3,3,3,3},
            {3,3,3,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,3,3,3,3},
            {3,3,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,3,3,3,3},
            {3,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,3,3,3},
            {3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,3,3,3},
        };

        private int[,] m_womenShapeTable = new int[39, 25] {
            {3,3,3,3,3,3,3,1,1,1,1,1,1,1,1,1,1,1,1,3,3,3,3,3,3},
            {3,3,3,3,3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,3,3,3,3},
            {3,3,3,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,1,3,3,3},
            {3,3,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,3,3},
            {3,1,1,1,1,1,1,1,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,1,3},
            {3,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {3,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {3,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,0,0,0,0,0,1},
            {3,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {3,1,1,3,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {3,1,1,3,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,3},
            {3,1,1,3,3,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,3},
            {3,1,1,3,3,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,3,3},
            {3,1,1,3,3,3,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,3,3,3},
            {3,1,1,3,3,3,3,1,0,0,0,0,0,0,0,0,0,0,0,1,3,3,3,3,3},
            {3,1,1,3,3,3,3,1,0,0,0,0,0,0,0,0,0,0,0,1,3,3,3,3,3},
            {3,3,3,3,3,3,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,3,3,3,3},
            {3,3,3,3,3,1,0,0,2,2,2,2,2,2,2,2,2,2,2,0,0,1,3,3,3},
            {3,3,3,3,1,0,0,0,2,2,2,2,2,2,2,2,2,2,2,0,0,0,1,3,3},
            {3,3,3,1,0,0,0,0,2,2,2,2,2,2,2,2,2,2,2,0,0,0,0,1,3},
            {3,3,1,0,0,0,0,0,2,2,2,2,2,2,2,2,2,2,2,0,0,0,0,1,3},
            {3,1,0,0,0,0,0,0,2,2,2,2,2,2,2,2,2,2,2,0,0,0,0,0,1},
            {3,1,0,0,0,0,0,0,2,2,2,2,2,2,2,2,2,2,2,0,0,0,0,0,1},
            {3,1,0,0,0,0,0,0,2,2,2,2,2,2,2,2,2,2,2,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,2,2,2,2,2,2,2,2,2,2,2,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,2,2,2,2,2,2,2,2,2,2,2,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,2,2,2,2,2,2,2,2,2,2,2,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,2,2,2,2,2,2,2,2,2,2,2,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1},
            {1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1},
            {1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1},
            {3,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,3},
            {3,3,3,3,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,3,3,3,3},
            {3,3,3,3,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,3,3,3,3},
            {3,3,3,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,3,3,3,3},
            {3,3,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,3,3,3,3},
            {3,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,3,3,3},
            {3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,3,3,3},
        };

        [SerializeField]
        private GameObject outlineCube;
        [SerializeField]
        private GameObject skinCube;
        [SerializeField]
        private GameObject heartCube;


        [SerializeField]
        private Transform manHeartParent;
        [SerializeField]
        private Transform womenHeartParent;
        [SerializeField]
        private Transform manbodyParent;
        [SerializeField]
        private Transform womenbodyParent;
        [SerializeField]
        private Transform mainCamTran;

        [SerializeField]
        private Light spotLight;

        public Light pointLight;

        [HideInInspector]
        public bool isWatching = true;

        [HideInInspector]
        public bool isFailing = false;


        private Vector3 manParentInitPos;
        private Vector3 womenParentInitPos;
        private void Awake()
        {
            Physics.gravity = new Vector3(0f, 0f, 0f);
            manParentInitPos = manbodyParent.localPosition;
            womenParentInitPos = womenbodyParent.localPosition;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Space)&&Main.Instance.isStartGame&&isWatching&&!isFailing)
            {
                MoveThem();
            }
        }

        private float currDistance;
        private void LateUpdate()
        {
            //防开方
            currDistance = math.distancesq(lastReplacePosition, mainCamTran.position);

            if (currDistance < 25f && !isWatching&&!Main.Instance.isEndGaming)
            {//进入方块
                isWatching = true;
                Main.Instance.ChangeWorldColor();
            }
            else if(currDistance>100f)
            {
                isWatching = false;
            }
            
        }

        /// <summary>
        /// 教学关特殊生成
        /// </summary>
        public void GenerateTeachWorld()
        {
            transform.position = mainCamTran.position + mainCamTran.forward * 3f;
            transform.position += Vector3.up * 2f;
            PixelGenerator();
            HeartGenerator(m_heartShapeTable);
            HollowGenerator(m_TriangleShapeTable);
        }


        public void NearToNewCube(float3 position)
        {
            if (!Main.Instance.isEndGaming)
            {
                //如果被改动过就重新生成新的
                if (isBeChanged)
                {

                    isBeChanged = false;
                    Physics.gravity = Vector3.zero;
                    ClearAllCube();
                    manbodyParent.localPosition = manParentInitPos;
                    womenbodyParent.localPosition = womenParentInitPos;

                    PixelGenerator();
                    isFailing = false;
                    if (math.distancesq(position, Main.Instance.theEndPoint) < 1f)
                    {
                        HollowGenerator(m_heartShapeTable);
                        HeartGenerator(m_heartShapeTable);
                    }
                    else
                    {
                        StartRandomCellCreator();
                    }
                }

                ReplaceCube(position);
            }
        }

        [HideInInspector]
        public float3 lastReplacePosition;
        public void ReplaceCube(float3 position)
        {
            transform.position = position;
            lastReplacePosition = position;
            spotLight.color = UnityEngine.Random.ColorHSV(0.5f, 1f, 0.5f, 1f, 0.5f, 1f);
        }


        public int tryNum;
        private int2 manCheckPoint = new int2();
        private int2 womenCheckPoint = new int2();
        private List<int2> manHeartList = new List<int2>();
        private List<int2> wonmenHeartList = new List<int2>();
        /// <summary>
        /// 每进入一个Cube，随机生成两者的心脏
        /// </summary>
        public void StartRandomCellCreator()
        {
            isBeChanged = true;

            manHeartList.Clear();
            wonmenHeartList.Clear();

            manCheckPoint.x = UnityEngine.Random.Range(0, 11);
            manCheckPoint.y = UnityEngine.Random.Range(0, 11);

            womenCheckPoint.x = UnityEngine.Random.Range(0, 11);
            womenCheckPoint.y = UnityEngine.Random.Range(0, 11);

            manHeartList.Remove(manCheckPoint);
            wonmenHeartList.Remove(womenCheckPoint);

            addCanUsePoint(manCheckPoint,manHeartList);
            addCanUsePoint(womenCheckPoint, wonmenHeartList);

            manHeartObjects[11 * manCheckPoint.x + manCheckPoint.y].GetComponent<MeshRenderer>().sharedMaterial = HeartMaterial;
            Destroy(womenHeartObjects[11 * womenCheckPoint.x + womenCheckPoint.y]);

            for (int ii = 0; ii < tryNum-1; ii++)
            {
                manCheckPoint = manHeartList[UnityEngine.Random.Range(0, manHeartList.Count)];
                womenCheckPoint = wonmenHeartList[UnityEngine.Random.Range(0, wonmenHeartList.Count)];

                manHeartObjects[11 * manCheckPoint.x + manCheckPoint.y].GetComponent<MeshRenderer>().sharedMaterial = HeartMaterial;
                Destroy(womenHeartObjects[11 * womenCheckPoint.x + womenCheckPoint.y]);

                manHeartList.Remove(manCheckPoint);
                wonmenHeartList.Remove(womenCheckPoint);

                addCanUsePoint(manCheckPoint, manHeartList);
                addCanUsePoint(womenCheckPoint, wonmenHeartList);
            }
        }



        private void addCanUsePoint(int2 center, List<int2> heartList)
        {
            if (center.x + 1 < 11)
            {
                if (!heartList.Contains(new int2(center.x + 1, center.y)))
                {
                    heartList.Add(new int2(center.x + 1, center.y));
                }
            }
            if (center.y + 1 < 11)
            {
                if(!heartList.Contains(new int2(center.x, center.y + 1)))
                {
                    heartList.Add(new int2(center.x, center.y + 1));
                }
            }
            if (center.x - 1 >= 0)
            {
                if (!heartList.Contains(new int2(center.x - 1, center.y)))
                {
                    heartList.Add(new int2(center.x - 1, center.y));
                }
            }
            if (center.y - 1 >= 0)
            {
                if (!heartList.Contains(new int2(center.x, center.y - 1)))
                {
                    heartList.Add(new int2(center.x, center.y - 1));
                }
            }
        }


        public Material HeartMaterial;
        public void HeartGenerator(int[,] _shapeTable)
        {
            for (int ii = 0; ii < 11 ; ii++)
            {
                for (int jj = 0; jj < 11; jj++)
                {
                    if (_shapeTable[jj, ii] == 1)
                    {
                        if (manHeartObjects.Count!=0)
                        {
                            manHeartObjects[11 * ii + jj].GetComponent<MeshRenderer>().sharedMaterial = HeartMaterial;
                        }
                    }
                }
            }
        }

        public void HollowGenerator(int[,] _shapeTable)
        {
            for (int ii = 0; ii < 11; ii++)
            {
                for (int jj = 0; jj < 11; jj++)
                {
                    if (_shapeTable[jj, ii] == 1)
                    {
                        if (womenHeartObjects.Count != 0)
                        {
                            Destroy(womenHeartObjects[11 * ii + jj]);
                        }
                    }
                }
            }
        }

        private bool isBeChanged = false;
        public void MoveThem()
        {
            isBeChanged = true;
            manbodyParent.position -= manbodyParent.right * 0.07f;
            womenbodyParent.position += womenbodyParent.right * 0.07f;
            if ((manbodyParent.position.x - womenbodyParent.position.x) < 4.5f)
            {
                if (Main.Instance.CheckIfReachTheEnd())
                {
                    Main.Instance.Success();
                }
                else
                {
                    Fail();
                }
                
            }
        }

        public void Fail()
        {
            isBeChanged = true;
            isFailing = true;
            if (!Main.Instance.isTeaching)
            {
                Main.Instance.ShowTheWords();
                Physics.gravity = math.normalize(Main.Instance.theEndPoint - lastReplacePosition) * 0.2f;
            }
        }

        private List<GameObject> manHeartObjects = new List<GameObject>();
        private List<GameObject> manBodyObjects = new List<GameObject>();
        private List<GameObject> womenHeartObjects = new List<GameObject>();
        private List<GameObject> womenBodyObjects = new List<GameObject>();
        public void PixelGenerator()
        {

            for (int ii = 0; ii < 25; ii++)
            {
                for (int jj = 0; jj < 39; jj++)
                {
                    if (m_manShapeTable[jj, ii] == 1)
                    {
                        manBodyObjects.Add(Instantiate(outlineCube, new Vector3(manbodyParent.position.x - ii*0.1f, manbodyParent.position.y - jj * 0.1f, manbodyParent.position.z), outlineCube.transform.rotation, manbodyParent));
                    }
                    else if(m_manShapeTable[jj, ii] == 2)
                    {
                        manHeartObjects.Add(Instantiate(skinCube, new Vector3(manHeartParent.position.x - ii * 0.1f, manHeartParent.position.y - jj * 0.1f, manHeartParent.position.z), heartCube.transform.rotation, manHeartParent));
                    }
                    else if (m_manShapeTable[jj, ii] == 0)
                    {
                        manBodyObjects.Add(Instantiate(skinCube, new Vector3(manbodyParent.position.x - ii * 0.1f, manbodyParent.position.y - jj * 0.1f, manbodyParent.position.z), skinCube.transform.rotation, manbodyParent));
                    }

                }
            }

            for (int ii = 0; ii < 25; ii++)
            {
                for (int jj = 0; jj < 39; jj++)
                {
                    if (m_womenShapeTable[jj, ii] == 1)
                    {
                        womenBodyObjects.Add(Instantiate(outlineCube, new Vector3(womenbodyParent.position.x + ii * 0.1f, womenbodyParent.position.y - jj * 0.1f, womenbodyParent.position.z), outlineCube.transform.rotation, womenbodyParent));
                    }
                    else if (m_womenShapeTable[jj, ii] == 2)
                    {
                        womenHeartObjects.Add(Instantiate(skinCube, new Vector3(womenHeartParent.position.x + ii * 0.1f, womenHeartParent.position.y - jj * 0.1f, womenHeartParent.position.z), heartCube.transform.rotation, womenHeartParent));
                    }
                    else if (m_womenShapeTable[jj, ii] == 0)
                    {
                        womenBodyObjects.Add(Instantiate(skinCube, new Vector3(womenbodyParent.position.x + ii * 0.1f, womenbodyParent.position.y - jj * 0.1f, womenbodyParent.position.z), skinCube.transform.rotation, womenbodyParent));
                    }

                }
            }
            
        }

        public void ClearAllCube()
        {
            foreach (GameObject obj in manBodyObjects)
            {
                Destroy(obj);
            }
            foreach (GameObject obj in manHeartObjects)
            {
                Destroy(obj);
            }
            foreach (GameObject obj in womenBodyObjects)
            {
                Destroy(obj);
            }
            foreach (GameObject obj in womenHeartObjects)
            {
                Destroy(obj);
            }
            manBodyObjects.Clear();
            manHeartObjects.Clear();
            womenBodyObjects.Clear();
            womenHeartObjects.Clear();
        }
    }
}

