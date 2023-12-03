using UnityEngine;
using PlayerColorManager;
using TempleRun.Player;
using Unity.VisualScripting;
using System;

namespace TempleRun
{

    public class LightColor : MonoBehaviour
    {

        public string color;
        public GameObject player;

        public float triggerDistance = 5f;
        private bool playerInside = false;

        private void Update()
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance <= triggerDistance)
            {
                if (!playerInside)
                {

                    playerInside = true;
                    var colorIndex = Array.IndexOf(player.gameObject.GetComponent<PlayerController>().colors, color);
                    PlayerColorManager.PlayerColorManager.setPlayerColor(colorIndex, player.transform.GetChild(0).gameObject.GetComponent<Renderer>(),
                        player.gameObject.GetComponent<PlayerController>().playerColorMaterials);
                }
            }
            else
            {
                if (playerInside)
                {
                    playerInside = false;
                }
            }
        }
    }

}