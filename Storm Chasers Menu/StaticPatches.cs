using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Harmony;
using System.Reflection;

namespace Storm_Chasers_Menu
{
    public static class StaticPatches
    {
        public static bool noMapBoundaries = false;
        public static bool disableOnline = false;

        public static bool tornadoMagnet = false;
        public static bool followPlayer = false;
        public static Player targetPlayer;
        public static CarTornado targetCar;
        public static Vector3 tornadoDirection;

        [HarmonyPatch(typeof(NoiseMove), "Update")]
        private class tornadoMagnetHandle
        {
            [Obfuscation(Exclude = true)]
            private static bool Prefix([Obfuscation(Exclude = true)] NoiseMove __instance)
            {
                if (tornadoMagnet)
                {
                    var prop1 = __instance.GetType().GetField("tornado", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    Tornado tornado = prop1.GetValue(__instance) as Tornado;

                    MethodInfo updatePosition = __instance.GetType().GetMethod("updatePosition", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (!__instance.useGeneralWindDirection)
                    {
                        GameController.Instance.windDirection = __instance.targetDirection;
                        GameController.Instance.windSpeed = __instance.targetMaxSpeed;
                    }
                    else
                    {
                        Vector3 localPlayer = tornadoDirection - tornado.transform.position;

                        if (followPlayer)
                        {
                            if (targetPlayer || targetCar)
                            {
                                if (targetCar.isDriverSeatFree)
                                {
                                    tornadoDirection = targetPlayer.transform.position;
                                } else
                                {
                                    tornadoDirection = targetCar.transform.position;
                                }
                                localPlayer = tornadoDirection - tornado.transform.position;
                            }
                        }

                        localPlayer.y = 0f;
                        localPlayer.Normalize();
                        __instance.targetDirection = new Vector2(localPlayer.x, localPlayer.z);
                        __instance.targetMaxSpeed = GameController.Instance.windSpeed * 0.8f;
                    }
                    __instance.currentDirection = Vector3.Lerp(__instance.currentDirection, __instance.targetDirection, __instance.acceleration * 1f * Time.deltaTime);
                    updatePosition.Invoke(__instance, new object[] { __instance.currentDirection, true });
                    if (__instance.GetComponent<Tornado>().tornadoType == Tornado.TornadoType.REAL && (__instance.transform.position.x > GameController.Instance.getPlayableSquare().xMax || __instance.transform.position.x < GameController.Instance.getPlayableSquare().xMin || __instance.transform.position.z > GameController.Instance.getPlayableSquare().yMax || __instance.transform.position.z < GameController.Instance.getPlayableSquare().yMin))
                    {
                        if (__instance.GetComponent<Tornado>().state == Tornado.TornadoState.ENABLED)
                        {
                            __instance.GetComponent<Tornado>().timer = Mathf.Max(__instance.GetComponent<Tornado>().timer, __instance.GetComponent<Tornado>().durationTime * 0.8f);
                        }
                        else if (__instance.GetComponent<Tornado>().state == Tornado.TornadoState.GROWING)
                        {
                            __instance.GetComponent<Tornado>().disableTornadoRPC();
                        }
                    }
                    __instance.currentSpeed = Mathf.Lerp(__instance.currentSpeed, __instance.targetMaxSpeed, __instance.acceleration * Time.deltaTime);
                    __instance.transform.position = new Vector3(__instance.transform.position.x, Terrain.activeTerrain.SampleHeight(tornado.tornadoColumn.transform.position) + Terrain.activeTerrain.transform.position.y, __instance.transform.position.z);
                    return false;
                } 
                else
                {
                    return true;
                }              
            }
        }

        [HarmonyPatch(typeof(Player), "checkMapSizeLimits")]
        private class noBoundariesPlayer
        {
            [Obfuscation(Exclude = true)]
            private static bool Prefix()
            {
                if (noMapBoundaries)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        [HarmonyPatch(typeof(CarTornado), "checkMapSizeLimits")]
        private class noBoundariesCar
        {
            [Obfuscation(Exclude = true)]
            private static bool Prefix()
            {
                if (noMapBoundaries)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        [HarmonyPatch(typeof(CarController), "checkMapSizeLimits")]
        private class noBoundariesCarController
        {
            [Obfuscation(Exclude = true)]
            private static bool Prefix()
            {
                if (noMapBoundaries)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        [HarmonyPatch(typeof(MainUIMenu), "SetInitialInfoText")]
        private class customWelcomeText
        {
            [Obfuscation(Exclude = true)]
            private static void Postfix()
            {
                UnityEngine.UI.Text info = GameObject.FindObjectOfType<MainUIMenu>().PlayfabInfoText;
                info.supportRichText = true;
                info.text = "<color=#ffff00>Storm Chaser Mod Menu</color> - <color=#7d7d7d>version " + typeof(MainMod).Assembly.GetName().Version + "</color>\n\nThank you for using my mod! If you have any issues, please contact me on Discord @ <color=#850aff>Indica#1259</color>";

                if (disableOnline)
                {
                    GameObject.FindObjectOfType<MainUIMenu>().OnlineRoomsButtons.transform.Find("PublicOnlineButton").GetComponent<UnityEngine.UI.Button>().interactable = false;
                    GameObject.FindObjectOfType<MainUIMenu>().OnlineRoomsButtons.transform.Find("JoinPrivateButton").GetComponent<UnityEngine.UI.Button>().interactable = false;
                }
            }
        }

    }
}
