using System.Collections;
using System.Collections.Generic;
using Bee.Enums;
using UnityEngine;
using System.Linq;

namespace Bee.Enemies
{
    public class PathFinderAi : MonoBehaviour
    {
        //[Tooltip("Comb Object")]
        //[SerializeField]
        //protected Transform Tartget;

        [Tooltip("All the possibles paths for an enemy follow")]
        [SerializeField]
        protected EnemyPath[] PossiblePaths;

        [Tooltip("Enemy speed")]
        [SerializeField]
        protected float SpeedMove;

        protected Transform[] PathChosen { get { return PossiblePaths[ChosenWay].PointsToWalk; } }

        [Tooltip("The chosen way")]
        [SerializeField]
        private int ChosenWay = 0;

        protected bool BeingAttacked;

        /// <summary>
        /// Index for the waypoint for Enemy walk
        /// </summary>
        [SerializeField]
        private int CurrentWayIndex = 0;

        void Start()
        {
            LoadAllPossiblePaths();
            ChoosePath();
        }

        void Update()
        {
            if (PossiblePaths == null || PossiblePaths.Length == 0 || PathChosen.Length == 0)
                return;

            Move();
        }

        private void Move()
        {

            //if (Retreat == true)
            //{
            //    WayIndex = 0;

            //    float randomWay = Random.Range(0, 2);
            //    if (randomWay == 1)
            //        ChoosenWay += 1;

            //    else if (randomWay == 2)
            //        ChoosenWay -= 1;

            //    if (ChoosenWay < 0)
            //        ChoosenWay = PossiblePaths.Length;
            //}

            var hasReached = transform.position == PathChosen[CurrentWayIndex].transform.position;

            if (CurrentWayIndex == 0 && BeingAttacked && hasReached)
            {
                Destroy(gameObject);
                return;
            }

            // Verify if it doesn't arrive in the final index    
            if (CurrentWayIndex >= PathChosen.Length - 1) // && Retreat == false
                Destroy(gameObject);

            transform.position = Vector2.MoveTowards(
                transform.position,
                PathChosen[CurrentWayIndex].transform.position,
                SpeedMove * Time.deltaTime
            );

            if (hasReached)
                CurrentWayIndex += BeingAttacked ? -1 : 1;

            return;
        }

        private void LoadAllPossiblePaths()
        {
            var allPossiblesPaths = GameObject.FindGameObjectsWithTag(Tags.EnemyPath);

            PossiblePaths = new EnemyPath[allPossiblesPaths.Length];

            for (int index = 0; index < allPossiblesPaths.Length; index++)
            {
                var currentPath = allPossiblesPaths[index];

                var allPointsToWalk = currentPath.GetComponentsInChildren<Transform>();

                allPointsToWalk = allPointsToWalk.Skip(1).ToArray();
                
                PossiblePaths[index] = new EnemyPath(allPointsToWalk);
            }
        }

        private void ChoosePath()
        {
            var chosen = Random.Range(0, PossiblePaths.Length - 1);
            ChosenWay = chosen;

            transform.position = PossiblePaths[ChosenWay].PointsToWalk[CurrentWayIndex].transform.position;
        }
    }
}
