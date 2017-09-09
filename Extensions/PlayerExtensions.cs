using System;
using UnityEngine;

namespace HURTHax.Extensions
{
    public static class PlayerExtensions
    {
        /// <summary>
        ///     Get target name
        /// </summary>
        /// <param name="managerBase">Value type</param>
        /// <returns>Name</returns>
        public static string GetProxyName<T>(this T managerBase) where T : NetworkEntityManagerBase
        {
            try
            {
                return managerBase.GetComponent<DisplayProxyName>().Name;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        /// <summary>
        ///     Get single bone
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="managerBase">Value type</param>
        /// <param name="item">Bone to find</param>
        /// <returns>Single bone</returns>
        public static HitboxIdentifier GetBone<T>(this T managerBase, EHitboxItem item) where T : NetworkEntityManagerBase
        {
            try
            {
                foreach (var bone in GetBones(managerBase))
                    if (bone.Item == item)
                        return bone;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
            throw new NullReferenceException(nameof(HitboxIdentifier));
        }

        /// <summary>
        ///     Get weapon id
        /// </summary>
        /// <param name="managerBase">Value type</param>
        /// <returns>Weapon ID</returns>
        public static int GetWeaponId<T>(this T managerBase) where T : NetworkEntityManagerBase
        {
            try
            {
                return managerBase.GetComponentInChildren<CharacterAnimationInputs>(true).GetWeaponType();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        /// <summary>
        ///     Get current weapon bullet speed
        /// </summary>
        /// <param name="managerBase">Value type</param>
        /// <returns>Bullet speed</returns>
        public static float GetWeaponBulletSpeed<T>(this T managerBase) where T : NetworkEntityManagerBase
        {
            switch (GetWeaponCode(managerBase))
            {
                case ItemCode.BoltAction:
                    return 900f;
                case ItemCode.Bow:
                    return 100; //100 default
                case ItemCode.Shotgun:
                    return 900f;
                case ItemCode.Spear:
                    return 45;
                default:
                    return 900f;
            }
        }

        /// <summary>
        ///     Get weapon code
        /// </summary>
        /// <param name="managerBase">Value type</param>
        /// <returns>Current weapon code</returns>
        public static ItemCode GetWeaponCode<T>(this T managerBase) where T : NetworkEntityManagerBase
        {
            switch (GetWeaponId(managerBase))
            {
                case 3:
                    return ItemCode.BoltAction;
                case 2:
                    return ItemCode.Bow;
                case 10:
                    return ItemCode.Shotgun;
                case 12:
                    return ItemCode.Spear;
                default:
                    return ItemCode.None;
            }
        }

        /// <summary>
        ///     Get all bones from target
        /// </summary>
        /// <param name="managerBase">Value type</param>
        /// <returns>Bone array</returns>
        public static HitboxIdentifier[] GetBones<T>(this T managerBase) where T : NetworkEntityManagerBase
        {
            try
            {
                return managerBase.GetComponentsInChildren<HitboxIdentifier>();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        /// <summary>
        ///     Get current proxy time
        /// </summary>
        /// <param name="managerBase">Value type</param>
        /// <returns>Current proxy time</returns>
        public static double GetProxyTime<T>(this T managerBase) where T : NetworkEntityManagerBase
        {
            try
            {
                return managerBase.GetComponentInParent<NetworkEntityManagerBase>().EntityTimeFluid;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        /// <summary>
        ///     Get NetworkEntityManagerPlayerProxy velocity
        /// </summary>
        /// <param name="target">Target</param>
        /// <returns>Current velocity</returns>
        public static Vector3 GetVelocity(this NetworkEntityManagerPlayerProxy target)
        {
            try
            {
                var netMovementProxy = target.GetComponent<NetMovementProxy>();
                var time = GetProxyTime(target);
                var delta = GetProxyTime(target) - time;

                HistoryBufferTimeContainerNonSeq<NetMovementProxyState> historyBufferTimeContainerNonSeq;
                HistoryBufferTimeContainerNonSeq<NetMovementProxyState> historyBufferTimeContainerNonSeq2;
                if (netMovementProxy._history.FindTime(time + delta, out historyBufferTimeContainerNonSeq, out historyBufferTimeContainerNonSeq2) || historyBufferTimeContainerNonSeq != null && historyBufferTimeContainerNonSeq2 != null)
                {
                    var num3 = historyBufferTimeContainerNonSeq != historyBufferTimeContainerNonSeq2 ? historyBufferTimeContainerNonSeq.Time - historyBufferTimeContainerNonSeq2.Time : 1.0;
                    return (historyBufferTimeContainerNonSeq.Data.Position - historyBufferTimeContainerNonSeq2.Data.Position) / (float) (num3 * 0.05f);
                }
                return new Vector3(0f, 0f, 0f);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }
    }
}