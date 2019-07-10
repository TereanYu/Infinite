#region Author
///-----------------------------------------------------------------
///   Namespace:		YU.ECS
///   Class:			Main
///   Author: 		    yutian
///-----------------------------------------------------------------
#endregion

using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


namespace YU.ECS {

    public class Main : MonoSingleton<Main>
    {
        [HideInInspector]
        public float lastLoadPositionX = 0f;
        [HideInInspector]
        public float lastLoadPositionY = 0f;
        [HideInInspector]
        public float lastLoadPositionZ = 0f;
        [HideInInspector]
        public bool isNeedToLoadXpos = false;
        [HideInInspector]
        public bool isNeedToLoadYpos = false;
        [HideInInspector]
        public bool isNeedToLoadZpos = false;
        [HideInInspector]
        public bool isNeedToLoadXneg = false;
        [HideInInspector]
        public bool isNeedToLoadYneg = false;
        [HideInInspector]
        public bool isNeedToLoadZneg = false;
        [HideInInspector]
        public Camera mainCamera;

        public int scale = 30;
        public float stepLength = 20f;
        [SerializeField]
        private int maxEndPointValue = 2;
        [SerializeField]
        private int minEndPointValue = 1;

        public bool isTeaching = true;
        public bool isStartGame = false;
        public bool isGenerateWorld = false;

        public FreeFlyCamera flyCamera;
        public GameObject background;
        public Material CubeMaterial;
        public float3 theEndPoint;

        private RandomDialogGenerator dialogGenerator;

        private void Awake()
        {
            Cursor.visible = false;
            dialogGenerator = GetComponent<RandomDialogGenerator>();
            mainCamera = Camera.main;

            InitGame();
        }

        private void InitGame()
        {
            //随机一个cube位置作为终点
            theEndPoint = new float3(
                UnityEngine.Random.Range(minEndPointValue, maxEndPointValue) * stepLength + stepLength * 0.5f,
                UnityEngine.Random.Range(minEndPointValue, maxEndPointValue) * stepLength + stepLength * 0.5f,
                UnityEngine.Random.Range(minEndPointValue, maxEndPointValue) * stepLength + stepLength * 0.5f
                );

            flyCamera._enableMovement = false;
            isGenerateWorld = true;
            isTeaching = true;
            mainCamera.backgroundColor = Color.white;
            CubeMaterial.SetColor("_EmissionColor", Color.black);
            WorldGenerator.Instance.GenerateTeachWorld();
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Success();
            }


            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }

            if (!isStartGame && Input.anyKeyDown)
            {
                isStartGame = true;
                UIManager.Instance.HideStartUI();
                InvokeRepeating("ShowTeachText", 0f, 6f);
            }

            if (!isNeedToLoadXpos && (mainCamera.transform.position.x - lastLoadPositionX) >= stepLength)
            {
                isNeedToLoadXpos = true;
                lastLoadPositionX += stepLength;
            }
            if (!isNeedToLoadYpos && (mainCamera.transform.position.y - lastLoadPositionY) >= stepLength)
            {
                isNeedToLoadYpos = true;
                lastLoadPositionY += stepLength;
            }
            if (!isNeedToLoadZpos && (mainCamera.transform.position.z - lastLoadPositionZ) >= stepLength)
            {
                isNeedToLoadZpos = true;
                lastLoadPositionZ += stepLength;
            }
            if (!isNeedToLoadXneg && (lastLoadPositionX - mainCamera.transform.position.x) >= stepLength)
            {
                isNeedToLoadXneg = true;
                lastLoadPositionX -= stepLength;
            }
            if (!isNeedToLoadYneg && (lastLoadPositionY - mainCamera.transform.position.y) >= stepLength)
            {
                isNeedToLoadYneg = true;
                lastLoadPositionY -= stepLength;
            }
            if (!isNeedToLoadZneg && (lastLoadPositionZ - mainCamera.transform.position.z) >= stepLength)
            {
                isNeedToLoadZneg = true;
                lastLoadPositionZ -= stepLength;
            }

        }

        private int currTeachID = 0;
        private List<string> teachTexts = new List<string>() {
            "按空格让他们靠近",
            "心的形状不契合,便是悲剧",
            "看来在这个时空,糟糕的结局，已经无法改变",
            "但是无数平行世界中，一定有我们想要的结局",
            "看，每个世界的碎片都会指引你方向",
            "WASD移动,SHIFT加速,去寻找那个世界吧"
        };
        public void ShowTeachText()
        {
            UIManager.Instance.ShowBottomText(teachTexts[currTeachID]);
            currTeachID++;
            if (currTeachID == 5)
            {
                Physics.gravity = math.normalize(theEndPoint -WorldGenerator.Instance.lastReplacePosition) * 0.1f;
            }
            if (currTeachID >= teachTexts.Count)
            {
                isTeaching = false;
                WorldGenerator.Instance.pointLight.gameObject.SetActive(true);
                flyCamera._enableMovement = true;
                CancelInvoke("ShowTeachText");
            }
        }

        private float curHValue;
        private float curSValue;
        private float curVValue;
        /// <summary>
        /// 除了教学关，可以改变世界颜色
        /// </summary>
        public void ChangeWorldColor()
        {
            if (!isTeaching)
            {
                curHValue = UnityEngine.Random.Range(0f, 1f);
                curSValue = UnityEngine.Random.Range(0f, 0.3f);
                curVValue = UnityEngine.Random.Range(0f, 0.3f);
                mainCamera.backgroundColor = Color.HSVToRGB(curHValue, curSValue, curVValue);
                CubeMaterial.SetColor("_EmissionColor", Color.HSVToRGB((curHValue + 0.4f) % 1, (curSValue + 0.7f) % 1, (curVValue + 0.7f) % 1));
            }
        }

        public void ShowTheWords()
        {
            dialogGenerator.GenerateRandomWords((int)(math.distance(mainCamera.transform.position,theEndPoint)/stepLength)+1);
        }

        
        public bool CheckIfReachTheEnd()
        {
            //防开方省性能
            return math.distancesq(mainCamera.transform.position, theEndPoint) < 25f;
        }

        [HideInInspector]
        public bool isEndMoveIn = false;
        [HideInInspector]
        public bool isEndMoveOut = false;
        [HideInInspector]
        public bool isEndGaming = false;
        public void Success()
        {
            isEndGaming = true;
            background.SetActive(false);
            flyCamera._enableMovement = false;
            StartCoroutine(WaitToEndMove());
        }

        private IEnumerator WaitToEndMove()
        {
            isEndMoveIn= true;
            isEndMoveOut = false;
            yield return new WaitForSeconds(10);
            isEndMoveIn = false;
            yield return new WaitForSeconds(2);
            isEndMoveOut = true;
            yield return new WaitForSeconds(8);
            UIManager.Instance.ShowSuccessText();
            yield return new WaitForSeconds(6);
            UIManager.Instance.ShowThanksText();
            yield return new WaitForSeconds(10);
            Application.Quit();

        }

    }
}

