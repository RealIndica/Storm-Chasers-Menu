using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
using System.Collections;

namespace Storm_Chasers_Menu.UI
{
    public class TornadoMenu2
    {
        public Style menuStyle = new Style();
        private Mods mod_instance;
        private TornadoMenu parentMenu;

        public TornadoMenu2(Mods mod, TornadoMenu inputmenu)
        {
            mod_instance = mod;
            parentMenu = inputmenu;
        }

        System.Random _R = new System.Random();
        T RandomEnumValue<T>()
        {
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(_R.Next(v.Length));
        }

        public void ui_update()
        {
            GUI.Box(new Rect(menuStyle.posRect.x + (menuStyle.widthSize + 20) * 2, menuStyle.posRect.y + 0f, menuStyle.widthSize + 10, 50f + 45 * menuStyle.mulY), "", menuStyle.BgStyle);
            GUI.Label(new Rect(menuStyle.posRect.x + (menuStyle.widthSize + 20) * 2, menuStyle.posRect.y + 5f, menuStyle.widthSize + 10, 95f), "Tornado\nMenu (2)", menuStyle.LabelStyle);

            if (GUI.Button(menuStyle.BtnRect(1, 3), "Destroy Tornadoes", menuStyle.BtnStyle))
            {
                Tornado[] ts = mod_instance.gameController.getTornados().ToArray();
                foreach (Tornado t in ts)
                {
                    t.StopAllCoroutines();
                    mod_instance.gameController.removeTornado(t);
                    PhotonNetwork.Destroy(t.gameObject);
                }
            }

            if (GUI.Button(menuStyle.BtnRect(2, 3), "Hail Mary", menuStyle.BtnStyle))
            {
                for (int i = 0; i < 10; i++)
                {
                    parentMenu.spawnTornado(RandomEnumValue<Tornado.TornadoCategory>());
                }
            }

            if (GUI.Button(menuStyle.BtnRect(3, 3), "All EF5", menuStyle.BtnStyle))
            {
                Tornado[] ts = UnityEngine.Object.FindObjectsOfType<Tornado>();
                foreach (Tornado t in ts)
                {
                    t.photonView.RPC("changeCategory", PhotonTargets.All, new object[]{(int)5, Tornado.TornadoCategory.EF5});
                }
            }

            if (GUI.Button(menuStyle.BtnRect(4, 3), "RC Tornado", menuStyle.BtnStyle))
            {
                mod_instance.gameController.spawnersOnlineVersus.SetActive(true);
                
                PlayerSpawner playerSpawner = mod_instance.gameController.getLocalPlayerRespawner();
                Vector3 position = position = mod_instance.localPlayer.transform.position;
                UnityEngine.Object.Instantiate<GameObject>(playerSpawner.tornadoControllerPrefab, position, Quaternion.identity);
                PhotonNetwork.Destroy(mod_instance.gameController.getLocalPlayer().gameObject);
            }

            if (GUI.Button(menuStyle.BtnRect(5, 3), "Stop RC Tornado", menuStyle.BtnStyle))
            {
                mod_instance.gameController.spawnersOnlineVersus.SetActive(false);
                mod_instance.gameController.getLocalTornadoController().toggleGameUI(false);
                PlayerSpawner playerSpawner = mod_instance.gameController.getLocalPlayerRespawner();
                Vector3 position = playerSpawner.transform.position;
                GameObject gameObject = PhotonNetwork.Instantiate("PlayerTornado", position, playerSpawner.transform.rotation, 0);
                if (playerSpawner.transform.parent.GetComponentInChildren<Home>() != null)
                {
                    GameController.Instance.home = playerSpawner.transform.parent.GetComponentInChildren<Home>();
                    GameController.Instance.home.setAsPlayerHome();
                }
                if (gameObject.GetComponent<vp_FPPlayerEventHandler>() != null)
                {
                    gameObject.GetComponent<vp_FPPlayerEventHandler>().Rotation.Set(new Vector2(0f, playerSpawner.transform.rotation.eulerAngles.y));
                }
                PhotonNetwork.Destroy(mod_instance.gameController.getLocalTornadoController().gameObject);
            }

            if (GUI.Button(menuStyle.BtnRect(6, 3), "RC Enable Options", menuStyle.BtnStyle))
            {
                upgradeRCTornado();
            }

            if (GUI.Button(menuStyle.BtnRect(7, 3), "Activate Tornadoes", menuStyle.BtnStyle))
            {
                Tornado[] ts = UnityEngine.Object.FindObjectsOfType<Tornado>();
                foreach (Tornado t in ts)
                {
                    if (t.tornadoType == Tornado.TornadoType.REAL && t.state == Tornado.TornadoState.DISABLED)
                    {
                        t.photonView.RPC("changeEnableTornado", PhotonTargets.All, new object[0]);
                    }
                }
            }

            if (StaticPatches.tornadoMagnet)
            {
                if (GUI.Button(menuStyle.BtnRect(8, 3), "Tornado Magnet", menuStyle.OnStyle))
                {
                    StaticPatches.tornadoMagnet = !StaticPatches.tornadoMagnet;
                }
            } 
            else
            {
                if (GUI.Button(menuStyle.BtnRect(8, 3), "Tornado Magnet", menuStyle.OffStyle))
                {
                    StaticPatches.tornadoDirection = mod_instance.localPlayer.transform.position;
                    StaticPatches.followPlayer = true;
                    StaticPatches.targetPlayer = mod_instance.localPlayer;
                    StaticPatches.targetCar = mod_instance.localCar;
                    StaticPatches.tornadoMagnet = !StaticPatches.tornadoMagnet;
                }
            }
        }

        private void upgradeRCTornado()
        {
            var prop1 = mod_instance.tornadoController.GetType().GetField("CATEGORY_SCORE_EF1", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var prop2 = mod_instance.tornadoController.GetType().GetField("CATEGORY_SCORE_EF2", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var prop3 = mod_instance.tornadoController.GetType().GetField("CATEGORY_SCORE_EF3", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var prop4 = mod_instance.tornadoController.GetType().GetField("CATEGORY_SCORE_EF4", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var prop5 = mod_instance.tornadoController.GetType().GetField("CATEGORY_SCORE_EF5", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            prop1.SetValue(mod_instance.tornadoController, 0);
            prop2.SetValue(mod_instance.tornadoController, 0);
            prop3.SetValue(mod_instance.tornadoController, 0);
            prop4.SetValue(mod_instance.tornadoController, 0);
            prop5.SetValue(mod_instance.tornadoController, 0);
            mod_instance.gameController.getLocalTornadoController().toggleGameUI(true);
        }
    }
}
