using System;
using UnityEngine;

namespace HURTHax.Extensions
{
    public static class AimExtensions
    {
        public static float Gravity;

        static AimExtensions()
        {
            Gravity = Math.Abs(Physics.gravity.y);
        }


        public static NetworkEntityManagerPlayerProxy UpdateTargetSelector(NetworkEntityManagerPlayerProxy[] players, NetworkEntityManagerPlayerOwner owner)
        {
            NetworkEntityManagerPlayerProxy target = null;
            var mouseDistance = Mathf.Infinity; // dystans myszki do gracza 

            foreach (var player in players)
            {
                if (player == null) continue;
                if (BaseSettings.GetSettings.Friends.Contains(player.GetProxyName())) continue;
                var distance = Vector3.Distance(owner.transform.position, player.transform.position);
                if (distance > BaseSettings.GetSettings.EspSettings.Range) continue;


                var distanceVec3 = Vector2.Distance(Input.mousePosition, Camera.main.WorldToScreenPoint(player.transform.position));

                Debug.Log($"{player.GetProxyName()} {distanceVec3}");

                if (distanceVec3 < mouseDistance)
                {
                    mouseDistance = distanceVec3;
                    target = player;
                }
            }
            if (target != null)
                Debug.Log($"{target.GetProxyName()} {mouseDistance}");
            return target;
        }


        public static void AimCorrection(NetworkEntityManagerPlayerProxy target, ref Vector3 vector, float distance, float bulletSpeed, float gravity = 10f) //9,81f // 10 in game
        {
            vector = vector + target.GetVelocity() * (distance / Math.Abs(bulletSpeed));
            vector = vector - new Vector3(0f, 0f, 0f) * (distance / Math.Abs(bulletSpeed)); // vector to nasza predkosc
            var mDistanse = distance / Math.Abs(bulletSpeed);
            vector.y += 0.5f * Gravity * mDistanse * mDistanse;
        }

        public static void AimAtVec3(this FPSInputController controller, Vector3 position)
        {
            var mouse = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
            var target = position - mouse;
            var angle = Quaternion.LookRotation(target);
            controller.ResetViewAngle(angle);
        }
    }
}