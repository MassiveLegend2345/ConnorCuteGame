

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Game.NavigationTutorial
{

    [RequireComponent(typeof(NavMeshAgent))]

    public class NPC : MonoBehaviour
    {

        [HideInInspector]
        public NavMeshAgent Agent;


        public float CurrentSpeed
        {

            get { return Agent.velocity.magnitude; }
        }

        private void Awake()
        {

            Agent = GetComponent<NavMeshAgent>();

        }
      }
}        