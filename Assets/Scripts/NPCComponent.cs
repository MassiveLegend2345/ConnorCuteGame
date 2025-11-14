using System.Collections.Generic;
using UnityEngine;

namespace Game.NavigationTutorial
{


    public class NPCComponent : MonoBehaviour
    {
        public NPC npc; 



        protected virtual void Awake()
        {
            npc = GetComponentInParent<NPC>();
        }
    }
}