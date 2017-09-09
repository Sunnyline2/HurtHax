using System;
using System.Collections;
using System.Collections.Generic;
using HURTHax.Extensions;
using UnityEngine;

namespace HURTHax
{
    public class Menu : MonoBehaviour
    {
        private bool _frameAimKey;
        public AnimalStatManagerClient[] Animals;
        public FPSInputController FpsInputController;
        public HashSet<GenericDeviceUsableInterfaceClient> LootCrates = new HashSet<GenericDeviceUsableInterfaceClient>();
        public NetworkEntityManagerPlayerOwner ManagerPlayerOwner;
        public NetworkEntityManagerPlayerProxy[] Players;
        public DestroyInTime[] ResourceNodes;
        public HashSet<GenericDeviceUsableInterfaceClient> Stakes = new HashSet<GenericDeviceUsableInterfaceClient>();
        public NetworkEntityManagerPlayerProxy Target;
        public GenericDeviceUsableInterfaceClient[] UsableItems;
        public HashSet<GenericDeviceUsableInterfaceClient> Wrecks = new HashSet<GenericDeviceUsableInterfaceClient>();


        private void Start()
        {
            StartCoroutine(UpdatePlayers());
            StartCoroutine(UpdateResouceNodes());
            StartCoroutine(UpdateAnimals());
            StartCoroutine(UpdateUsableItems());
            BaseSettings.GetSettings.IsDebug = true;
        }

        private void Update()
        {
            #region FindingObjects

            if (ManagerPlayerOwner == null)
                ManagerPlayerOwner = FindObjectOfType<NetworkEntityManagerPlayerOwner>();

            if (FpsInputController == null)
                FpsInputController = FindObjectOfType<FPSInputController>();

            #endregion

            #region GUIKeys

            if (Input.GetKeyDown(KeyCode.F5))
                BaseSettings.GetSettings.ShowEspMenu = !BaseSettings.GetSettings.ShowEspMenu;

            if (Input.GetKeyDown(KeyCode.F6))
                BaseSettings.GetSettings.ShowAimbotMenu = !BaseSettings.GetSettings.ShowAimbotMenu;

            #endregion

            #region Dodaj znajomych

            if (Input.GetKeyDown(KeyCode.F3))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    var proxyName = hit.transform.GetComponent<DisplayProxyName>();
                    if (proxyName != null)
                    {
                        BaseSettings.GetSettings.Friends.Add(proxyName.Name);
                        Debug.Log($"{proxyName.Name} now is your friend!");
                    }
                }
            }

            #endregion

            #region Usuń znajomych

            if (Input.GetKey(KeyCode.F3) && Input.GetKeyDown(KeyCode.F4))
                BaseSettings.GetSettings.Friends = new List<string>();

            #endregion

            //if (Input.GetKeyDown(KeyCode.Z))
            //    foreach (var monoBehaviour in ManagerPlayerOwner.GetComponents<Component>())
            //        print(monoBehaviour);

            if (Input.GetMouseButton(3) && BaseSettings.GetSettings.AimBotSettings.IsEnabled)
                if (_frameAimKey && Target != null)
                    HuamnAimbot(Target);
                else
                    HuamnAimbot();
            else
                _frameAimKey = false;


            if (Input.GetMouseButton(4))
            {
                HuamnAimbot();
            }
        }

        private void OnGUI()
        {
            try
            {
                #region Drawing

                if (BaseSettings.GetSettings.ShowEspMenu)
                    Drawing.DrawEspWindow();

                if (BaseSettings.GetSettings.ShowAimbotMenu)
                    Drawing.DrawAimWindow();


                if (BaseSettings.GetSettings.EspSettings.IsEnabled)
                {
                    if (BaseSettings.GetSettings.EspSettings.DrawPlayers && Players != null)
                        DrawPlayers();
                    if (BaseSettings.GetSettings.EspSettings.DrawResouces && ResourceNodes != null)
                        DrawResouces();
                    if (BaseSettings.GetSettings.EspSettings.DrawAnimals && Animals != null)
                        DrawAnimals();
                    if (BaseSettings.GetSettings.EspSettings.DrawLootCrates && UsableItems != null)
                        DrawCrates();
                    if (BaseSettings.GetSettings.EspSettings.DrawLootCrates && UsableItems != null)
                        DrawWrecks();
                }


                if (Input.GetKey(KeyCode.LeftAlt))
                    if (UsableItems != null)
                        foreach (var stake in UsableItems)
                            try
                            {
                                if (stake == null)
                                    continue;

                                var distance = Vector3.Distance(ManagerPlayerOwner.transform.position, stake.transform.position);

                                if (distance > BaseSettings.GetSettings.EspSettings.Range)
                                    continue;

                                var wtsPlayer = Camera.main.WorldToScreenPoint(stake.transform.position);
                                if (wtsPlayer.z < 0.0)
                                    continue;
                                Drawing.DrawString(new Vector2(wtsPlayer.x, Screen.height - wtsPlayer.y), Color.white, Drawing.TextFlags.TEXT_FLAG_CENTERED, $"{stake.name} [{Math.Round(distance)}m]");
                            }
                            catch (Exception e)
                            {
                                Debug.LogWarning(e);
                            }

                #endregion

                #region RaycastHit

                if (Input.GetMouseButtonDown(4))
                {
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out var hit))
                    {
                        print("Dumping objects!");
                        foreach (var component in hit.transform.GetComponents<Component>())
                            print(component);
                    }
                }

                #endregion
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }

        private void OnApplicationQuit()
        {
            BaseSettings.SaveSettings();
        }


        private void HuamnAimbot(NetworkEntityManagerPlayerProxy myTarget = null)
        {
            try
            {
                var target = myTarget ?? AimExtensions.UpdateTargetSelector(Players, ManagerPlayerOwner);

                if (target != null && target.IsNullOrDestroyed() == false)
                {
                    var weapon = ManagerPlayerOwner.GetWeaponCode();
                    Vector3 bodyPosition;

                    switch (weapon)
                    {
                        case ItemCode.Shotgun:
                            bodyPosition = target.GetBone(EHitboxItem.Chest).transform.position;
                            break;
                        case ItemCode.Spear:
                            bodyPosition = target.GetBone(EHitboxItem.Heart).transform.position;
                            break;
                        default:
                            bodyPosition = target.GetBone(EHitboxItem.Head).transform.position;
                            break;
                    }

                    if (weapon != ItemCode.None)
                        AimExtensions.AimCorrection(target, ref bodyPosition, Vector3.Distance(bodyPosition, ManagerPlayerOwner.transform.position), ManagerPlayerOwner.GetWeaponBulletSpeed());


                    FpsInputController.AimAtVec3(bodyPosition);
                    _frameAimKey = true;
                    Target = target;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }


        private void UpdateCrates()
        {
            //LootCrates.Clear();
            //cleaup

            foreach (var crate in LootCrates)
                if (crate.IsNullOrDestroyed())
                    LootCrates.Remove(crate);


            foreach (var item in UsableItems)
            {
                if (item == null) continue;
                if (LootCrates.Contains(item)) continue;
                if (item.name.ToLower().Contains("loot"))
                    LootCrates.Add(item);
            }
        }

        private void FindWrecks()
        {
            Wrecks.Clear();
            foreach (var item in Wrecks)
            {
                if (item == null) continue;
                if (item.name.ToLower().Contains("goat") || item.name.ToLower().Contains("roach") || item.name.ToLower().Contains("kanga")) Wrecks.Add(item);
            }
        }

        private void FindStakes()
        {
            Stakes.Clear();
            foreach (var stake in Stakes)
            {
                if (stake == null) continue;
                if (stake.name.ToLower().Contains("ship")) Stakes.Add(stake);
            }
        }

        #region Whiles

        public IEnumerator UpdatePlayers()
        {
            if (BaseSettings.GetSettings.IsDebug)
                Debug.Log("UpdateManagerPlayerProxy is running");
            while (true)
            {
                try
                {
                    if (BaseSettings.GetSettings.EspSettings.IsEnabled && BaseSettings.GetSettings.EspSettings.DrawPlayers)
                        Players = FindObjectsOfType<NetworkEntityManagerPlayerProxy>();
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception in UpdateManagerPlayerProxy! " + e);
                }
                yield return new WaitForSeconds(1f);
            }
        }

        public IEnumerator UpdateUsableItems()
        {
            if (BaseSettings.GetSettings.IsDebug)
                Debug.Log("UpdateUsableItems is running");
            while (true)
            {
                try
                {
                    if (BaseSettings.GetSettings.EspSettings.IsEnabled && (BaseSettings.GetSettings.EspSettings.DrawLootCrates || BaseSettings.GetSettings.EspSettings.DrawWrecks || BaseSettings.GetSettings.EspSettings.DrawOwnershipStakes))
                    {
                        UsableItems = FindObjectsOfType<GenericDeviceUsableInterfaceClient>();

                        if (BaseSettings.GetSettings.EspSettings.DrawLootCrates)
                            UpdateCrates();
                        if (BaseSettings.GetSettings.EspSettings.DrawWrecks)
                            FindWrecks();
                        if (BaseSettings.GetSettings.EspSettings.DrawOwnershipStakes)
                            FindStakes();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception in UpdateUsableItems! " + e);
                }
                yield return new WaitForSeconds(5f);
            }
        }

        public IEnumerator UpdateAnimals()
        {
            if (BaseSettings.GetSettings.IsDebug)
                Debug.Log("UpdateAnimals is running");
            while (true)
            {
                try
                {
                    if (BaseSettings.GetSettings.EspSettings.IsEnabled && BaseSettings.GetSettings.EspSettings.DrawAnimals)

                        Animals = FindObjectsOfType<AnimalStatManagerClient>();
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception in UpdateAnimals! " + e);
                }
                yield return new WaitForSeconds(5f);
            }
        }

        public IEnumerator UpdateResouceNodes()
        {
            if (BaseSettings.GetSettings.IsDebug)
                Debug.Log("UpdateResouceNodes is running");
            while (true)
            {
                try
                {
                    if (BaseSettings.GetSettings.EspSettings.IsEnabled && BaseSettings.GetSettings.EspSettings.DrawResouces)
                        ResourceNodes = FindObjectsOfType<DestroyInTime>();
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception in UpdateResouceNodes! " + e);
                }
                yield return new WaitForSeconds(5f);
            }
        }

        #endregion

        #region Draw

        private void DrawPlayers()
        {
            foreach (var playerProxy in Players)
                try
                {
                    if (playerProxy == null)
                        continue;
                    var distance = Vector3.Distance(ManagerPlayerOwner.transform.position, playerProxy.transform.position);

                    if (distance > BaseSettings.GetSettings.EspSettings.Range)
                        continue;

                    var wtsPlayer = Camera.main.WorldToScreenPoint(playerProxy.transform.position);
                    if (wtsPlayer.z < 0.0)
                        continue;

                    var nameProxy = playerProxy.GetComponent<DisplayProxyName>();
                    var description = $"{nameProxy.Name} [{Math.Round(distance)}m]";

                    var color = BaseSettings.GetSettings.Friends.Exists(s => s.Equals(nameProxy.Name)) ? Color.green : Color.red;


                    var height = (Target.GetBone(EHitboxItem.Head).transform.position - Target.GetBone(EHitboxItem.LeftFoot).transform.position).x;
                    var width = height / 2.6f;

                    Drawing.DrawString(new Vector2(wtsPlayer.x, Screen.height - wtsPlayer.y), color, Drawing.TextFlags.TEXT_FLAG_CENTERED, description);
                    Drawing.DrawBoxOutlines(new Vector2(wtsPlayer.x, Screen.height - wtsPlayer.y), new Vector2(height, width), 1f, color);
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }
        }

        private void DrawAnimals()
        {
            foreach (var animal in Animals)
                try
                {
                    if (animal == null)
                        continue;

                    var distance = Vector3.Distance(ManagerPlayerOwner.transform.position, animal.transform.position);

                    if (distance > BaseSettings.GetSettings.EspSettings.Range)
                        continue;

                    var wtsPlayer = Camera.main.WorldToScreenPoint(animal.transform.position);
                    if (wtsPlayer.z < 0.0)
                        continue;

                    Drawing.DrawString(new Vector2(wtsPlayer.x, Screen.height - wtsPlayer.y), Color.magenta, Drawing.TextFlags.TEXT_FLAG_CENTERED, $"{animal.name} [{Math.Round(distance)}m]");
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }
        }

        private void DrawResouces()
        {
            foreach (var res in ResourceNodes)
                try
                {
                    if (res == null)
                        continue;

                    var distance = Vector3.Distance(ManagerPlayerOwner.transform.position, res.transform.position);

                    if (distance > BaseSettings.GetSettings.EspSettings.Range)
                        continue;

                    var wtsPlayer = Camera.main.WorldToScreenPoint(res.transform.position);
                    if (wtsPlayer.z < 0.0)
                        continue;

                    Drawing.DrawString(new Vector2(wtsPlayer.x, Screen.height - wtsPlayer.y), Color.cyan, Drawing.TextFlags.TEXT_FLAG_CENTERED, $"{res.name} [{Math.Round(distance)}m]");
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }
        }

        private void DrawCrates()
        {
            foreach (var crate in LootCrates)
                try
                {
                    if (crate == null)
                        continue;

                    var distance = Vector3.Distance(ManagerPlayerOwner.transform.position, crate.transform.position);

                    if (distance > BaseSettings.GetSettings.EspSettings.Range)
                        continue;

                    var wtsPlayer = Camera.main.WorldToScreenPoint(crate.transform.position);
                    if (wtsPlayer.z < 0.0)
                        continue;
                    Drawing.DrawString(new Vector2(wtsPlayer.x, Screen.height - wtsPlayer.y), Color.yellow, Drawing.TextFlags.TEXT_FLAG_CENTERED, $"{crate.name} [{Math.Round(distance)}m]");
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }
        }

        private void DrawWrecks()
        {
            foreach (var crate in Wrecks)
                try
                {
                    if (crate == null)
                        continue;

                    var distance = Vector3.Distance(ManagerPlayerOwner.transform.position, crate.transform.position);

                    if (distance > BaseSettings.GetSettings.EspSettings.Range)
                        continue;

                    var wtsPlayer = Camera.main.WorldToScreenPoint(crate.transform.position);
                    if (wtsPlayer.z < 0.0)
                        continue;
                    Drawing.DrawString(new Vector2(wtsPlayer.x, Screen.height - wtsPlayer.y), Color.yellow, Drawing.TextFlags.TEXT_FLAG_CENTERED, $"{crate.name} [{Math.Round(distance)}m]");
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }
        }

        #endregion
    }
}
