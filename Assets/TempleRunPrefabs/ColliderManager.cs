using UnityEngine;
using PlayerColorManager;
using TempleRun.Player;
using Unity.VisualScripting;
using System;

namespace TempleRun
{
    public class ColliderManager : MonoBehaviour
    {
        public string color;
        public GameObject player;

        public float triggerDistance = 2f;
        private bool playerInside = false;

        private void Update()
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance <= triggerDistance)
            {
                if (!playerInside)
                {
                    var playerColor = player.gameObject.GetComponent<PlayerController>().playerColor;

                    if (playerColor == this.color)

                    {
                        playerInside = false;
                        this.EqualColorCollision();
                    }
                    else
                    {
                        playerInside = true;
                        this.DifferentColorCollision();
                    }

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

        private void EqualColorCollision()
        {
            gameObject.SetActive(false);

        }

        private void DifferentColorCollision()
        {
            player.gameObject.GetComponent<PlayerController>().GameOver();
        }
    }
}