using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace Rainbow.AStar
{
    public class RouteDisplay : MonoBehaviour
    {
        private AStar aStar;
        [Header("用于测试")]
        public Vector2Int startPos;
        public Vector2Int finishPos;
        public Tilemap displayMap;
        public TileBase displayTile;
        public bool displayStartAndFinish;
        public bool displayPath;

        private Stack<MovementStep> npcMovmentStepStack;


        [Header("测试移动NPC")]
        public NPCMovement npcMovement;
        public bool moveNPC;
        [SceneName] public string targetScene;
        public Vector2Int targetPos;
        public AnimationClip stopClip;


        private void Awake()
        {
            aStar = GetComponent<AStar>();
            npcMovmentStepStack = new Stack<MovementStep>();
        }

        private void Update()
        {
            ShowPathOnGridMap();

            if (moveNPC)
            {
                moveNPC = false;
                var schedule = new ScheduleDetails(0, 0, 0, 0, Season.Spring, targetScene, targetPos, true);
                npcMovement.BuildPath(schedule);
            }
        }

        private void ShowPathOnGridMap()
        {
            if (displayMap != null && displayTile != null)
            {
                if (displayStartAndFinish)
                {
                    displayMap.SetTile((Vector3Int)startPos, displayTile);
                    displayMap.SetTile((Vector3Int)finishPos, displayTile);
                }
                else
                {
                    displayMap.SetTile((Vector3Int)startPos, null);
                    displayMap.SetTile((Vector3Int)finishPos, null);
                }

                if (displayPath)
                {
                    var sceneName = SceneManager.GetActiveScene().name;

                    aStar.BuildPath(sceneName, startPos, finishPos, npcMovmentStepStack);

                    foreach (var step in npcMovmentStepStack)
                    {
                        displayMap.SetTile((Vector3Int)step.gridCoordinate, displayTile);
                    }
                }
                else
                {
                    if (npcMovmentStepStack.Count > 0)
                    {
                        foreach (var step in npcMovmentStepStack)
                        {
                            displayMap.SetTile((Vector3Int)step.gridCoordinate, null);
                        }
                        npcMovmentStepStack.Clear();
                    }
                }
            }
        }
    }
}
