using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Storm_Chasers_Menu.UI
{
    class PlayerManager
    {
        public Style menuStyle = new Style();
        private Mods mod_instance;

        public Vector2 scrollPosition = Vector2.zero;
        public Rect optionScreen;

        PhotonPlayer selectedPlayerPre = null;

        public PlayerManager(Mods mod)
        {
            mod_instance = mod;
            optionScreen = new Rect(menuStyle.posRect.x + (menuStyle.widthSize + 20) * 1, menuStyle.posRect.y + 0f, menuStyle.widthSize + 10, 50f + 45 * 6);
        }

        public void ui_update()
        {
            if (PhotonNetwork.inRoom)
            {
                optionScreen = GUI.Window(1, optionScreen, drawSpawnList, "", menuStyle.BgStyle);

                GUI.Box(new Rect(menuStyle.posRect.x + (menuStyle.widthSize + 20) * 2, menuStyle.posRect.y + 0f, menuStyle.widthSize + 10, 50f + 45 * menuStyle.mulY), "", menuStyle.BgStyle);
                GUI.Label(new Rect(menuStyle.posRect.x + (menuStyle.widthSize + 20) * 2, menuStyle.posRect.y + 5f, menuStyle.widthSize + 10, 95f), "Manager\nMenu", menuStyle.LabelStyle);

                if (GUI.Button(menuStyle.BtnRect(1, 3), "Kick", menuStyle.BtnStyle))
                {
                    PhotonNetwork.CloseConnection(selectedPlayerPre);
                }

                if (GUI.Button(menuStyle.BtnRect(2, 3), "Set Host", menuStyle.BtnStyle))
                {
                    PhotonNetwork.SetMasterClient(selectedPlayerPre);
                }

                if (GUI.Button(menuStyle.BtnRect(3, 3), "Tornado Magnet", menuStyle.BtnStyle))
                {
                    StaticPatches.targetPlayer = mod_instance.getPlayerByPhoton(selectedPlayerPre);
                    StaticPatches.targetCar = mod_instance.getTruckByPhoton(selectedPlayerPre);
                    StaticPatches.followPlayer = true;
                    StaticPatches.tornadoMagnet = true;
                }

                if (GUI.Button(menuStyle.BtnRect(4, 3), "Go To", menuStyle.BtnStyle))
                {
                    Vector3 target = mod_instance.getPlayerByPhoton(selectedPlayerPre).transform.position;

                    if (mod_instance.isOwnerDrivingTruck(selectedPlayerPre))
                    {
                        target = mod_instance.getTruckByPhoton(selectedPlayerPre).transform.position;
                    }

                    if (mod_instance.localPlayer && mod_instance.localPlayer.mainCamera)
                    {
                        if (mod_instance.localPlayer.isInsideCar && mod_instance.localPlayer.isCarDriver)
                        {
                            mod_instance.localCar.transform.position = new Vector3(target.x, target.y + 4f, target.z);
                        }
                        else
                        {
                            mod_instance.localPlayer.transform.position = new Vector3(target.x, target.y + 1f, target.z);
                        }
                    }
                }

                if (GUI.Button(menuStyle.BtnRect(5, 3), "Bring", menuStyle.BtnStyle))
                {
                    Player target = mod_instance.getPlayerByPhoton(selectedPlayerPre);
                    Vector3 myPos = mod_instance.localPlayer.transform.position;
                    if (!mod_instance.getTruckByPhoton(selectedPlayerPre).isDriverSeatFree)
                    {
                        mod_instance.getTruckByPhoton(selectedPlayerPre).photonView.RPC("updatePosition", PhotonTargets.All, new object[] { myPos });
                    }
                    else
                    {
                        target.photonView.RPC("updatePosition", PhotonTargets.All, new object[] { myPos });
                    }
                }

                if (GUI.Button(menuStyle.BtnRect(6, 3), "Kill", menuStyle.BtnStyle))
                {
                    Player target = mod_instance.getPlayerByPhoton(selectedPlayerPre);
                    CarTornado targetTruck = mod_instance.getTruckByPhoton(selectedPlayerPre);
                    if (target != mod_instance.localPlayer)
                    {
                        target.DieRPC();
                    }
                    if (targetTruck != mod_instance.localCar)
                    {
                        targetTruck.instantiateDeathRPC();
                    }
                }

                if (GUI.Button(menuStyle.BtnRect(7, 3), "Deflate Tire", menuStyle.BtnStyle))
                {
                    Player target = mod_instance.getPlayerByPhoton(selectedPlayerPre);
                    CarTornado targetTruck = mod_instance.getTruckByPhoton(selectedPlayerPre);

                    bool[] tireDeflate = { false, false, false, false };
                    System.Random random = new System.Random();
                    tireDeflate[random.Next(4)] = true;

                    targetTruck.photonView.RPC("updateCarInfo", PhotonTargets.Others, new object[] {
                    targetTruck.getCarTeam(),
                    targetTruck.tornadoPodsCount,
                    targetTruck.portableGasCount,
                    targetTruck.replacementTireCount,
                    targetTruck.isEngineStarted,
                    GlobalValues.Instance.vehicleType,
                    GlobalValues.Instance.vehicleColor,
                    GlobalValues.Instance.vehicleRust,
                    GlobalValues.Instance.vehicleReflection,
                    targetTruck.isLightEnabled,
                    targetTruck.isSirenEnabled,
                    targetTruck.isGPSEnabled,
                    targetTruck.isWindCompassEnabled,
                    0,
                    targetTruck.onlineLock,
                    tireDeflate
                    });
                }

                if (GUI.Button(menuStyle.BtnRect(8, 3), "Smite", menuStyle.BtnStyle))
                {
                    Player target = mod_instance.getPlayerByPhoton(selectedPlayerPre);
                    CarTornado targetTruck = mod_instance.getTruckByPhoton(selectedPlayerPre);
                    Tornado targetTornado = null;

                    foreach(Tornado t in UnityEngine.Object.FindObjectsOfType<Tornado>())
                    {
                        if (t.isActiveAndEnabled)
                        {
                            targetTornado = t;
                            break;
                        }
                    }

                    if (targetTornado)
                    {
                        if (!targetTruck.isDriverSeatFree)
                        {
                            targetTornado.lightningStrikeRPC(new Vector2(targetTruck.transform.position.x, targetTruck.transform.position.z), targetTruck.transform.position.y);
                        } 
                        else
                        {
                            targetTornado.lightningStrikeRPC(new Vector2(target.transform.position.x, target.transform.position.z), target.transform.position.y);
                        }
                    }
                }
            }
        }


        void drawSpawnList(int WindowID)
        {
            List<PhotonPlayer> players = PhotonNetwork.playerList.ToList();
            scrollPosition = GUI.BeginScrollView(new Rect(0, 0, optionScreen.width, optionScreen.height), scrollPosition, new Rect(0, 0, optionScreen.width, 60 * players.Count));
            for (int i = 0; i < players.Count; i++)
            {
                if (GUI.Button(new Rect(0, 60 * i, optionScreen.width, 60), players[i].NickName, menuStyle.BtnStyle))
                {
                    selectedPlayerPre = players[i];
                }
            }
            GUI.EndScrollView();
        }
    }
}
