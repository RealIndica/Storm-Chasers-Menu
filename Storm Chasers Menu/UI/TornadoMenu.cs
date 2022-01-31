using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Storm_Chasers_Menu.UI
{
    public class TornadoMenu
    {
        public Style menuStyle = new Style();
        private Mods mod_instance;
        private TornadoMenu2 _menu2;

        public bool isOnline = false;

        public TornadoMenu(Mods mod)
        {
            mod_instance = mod;
            _menu2 = new TornadoMenu2(mod_instance, this);
            _menu2.menuStyle.Start();
        }

        public void spawnTornado(Tornado.TornadoCategory type)
        {
            if (isOnline)
            {
                spawnSupercellMod(mod_instance.tornadoSpawner, type);
            }
            else
            {
                spawnSupercellMod(mod_instance.offlineTornadoSpawner, type);
            }
        }

        public Vector3 getRandomPos(TornadoSpawner spawner)
        {
            return new Vector3(UnityEngine.Random.Range(GameController.Instance.getPlayableSquare().xMin * 0.75f, GameController.Instance.getPlayableSquare().xMax * 0.75f), spawner.transform.position.y, UnityEngine.Random.Range(GameController.Instance.getPlayableSquare().yMin * 0.75f, GameController.Instance.getPlayableSquare().yMax * 0.75f));
        }

        public void spawnSupercellMod(TornadoSpawner spawner, Tornado.TornadoCategory category, bool randomGen = false, bool hail = false, float duration = 1200f, float thunderAmount = 0f, bool generalWind = true, bool randomWind = true, float rainamt = 0f) 
        {
            Tornado component = PhotonNetwork.InstantiateSceneObject(spawner.tornadoPrefab.name, getRandomPos(spawner), spawner.transform.rotation, 0, null).GetComponent<Tornado>();
            component.tornadoType = Tornado.TornadoType.REAL;
            component.state = Tornado.TornadoState.ENABLED;
            component.radarWindDirectionInside = (UnityEngine.Random.value > 0.5f);
            component.randomGeneration = randomGen;
            component.columnTouchingGroundPercent = 10;
            component.currentThunderstormAmount = 2f;
            component.emitHail = hail;
            component.category = category;
            component.rainAmount = rainamt;
            component.columnFormationSpeed = spawner.columnFormationSpeed;
            component.wallCloudHeight = spawner.wallCloudHeight;
            component.startTime = spawner.enableTornadoTime;
            component.durationTime = duration;
            component.thunderstormAmount = thunderAmount;
            component.groundHeight *= 1.5f;
            GameController.Instance.addTornado(component);
            if (!generalWind)
            {
                component.GetComponent<NoiseMove>().useGeneralWindDirection = false;
                component.GetComponent<NoiseMove>().targetDirection = spawner.windDirection;
                component.GetComponent<NoiseMove>().targetMaxSpeed = spawner.windMaxSpeed;
                if (randomWind)
                {
                    component.GetComponent<NoiseMove>().generateRandomProperties();
                }
            }
            component.updateTornadoSettings();
            component.spawnerId = spawner.GetInstanceID();
            spawner.checkAndSpawnSurroundFakeSupercell(component);
            component.updateTornadoWarnings();
        }

        public void ui_update()
        {
            GUI.Box(new Rect(menuStyle.posRect.x + menuStyle.widthSize + 20, menuStyle.posRect.y + 0f, menuStyle.widthSize + 10, 50f + 45 * menuStyle.mulY), "", menuStyle.BgStyle);
            GUI.Label(new Rect(menuStyle.posRect.x + menuStyle.widthSize + 20, menuStyle.posRect.y + 5f, menuStyle.widthSize + 10, 95f), "Tornado\nMenu", menuStyle.LabelStyle);

            if (mod_instance.gameController.spawnersOnlineFreeRoaming.GetActive())
            {
                isOnline = true;
            } else
            {
                isOnline = false;
            }

            if (GUI.Button(menuStyle.BtnRect(1, 2), "Spawn EF0", menuStyle.BtnStyle))
            {
                spawnTornado(Tornado.TornadoCategory.EF0);
            }

            if (GUI.Button(menuStyle.BtnRect(2, 2), "Spawn EF1", menuStyle.BtnStyle))
            {
                spawnTornado(Tornado.TornadoCategory.EF1);
            }

            if (GUI.Button(menuStyle.BtnRect(3, 2), "Spawn EF2", menuStyle.BtnStyle))
            {
                spawnTornado(Tornado.TornadoCategory.EF2);
            }

            if (GUI.Button(menuStyle.BtnRect(4, 2), "Spawn EF3", menuStyle.BtnStyle))
            {
                spawnTornado(Tornado.TornadoCategory.EF3);
            }

            if (GUI.Button(menuStyle.BtnRect(5, 2), "Spawn EF4", menuStyle.BtnStyle))
            {
                spawnTornado(Tornado.TornadoCategory.EF4);
            }

            if (GUI.Button(menuStyle.BtnRect(6, 2), "Spawn EF5", menuStyle.BtnStyle))
            {
                spawnTornado(Tornado.TornadoCategory.EF5);
            }

            if (GUI.Button(menuStyle.BtnRect(7, 2), "Teleport Tornadoes", menuStyle.BtnStyle))
            {
                Tornado[] ts = UnityEngine.Object.FindObjectsOfType<Tornado>();
                foreach (Tornado t in ts)
                {
                    if (t.tornadoType == Tornado.TornadoType.REAL && t.tornadoColumn)
                    {
                        t.transform.position = mod_instance.localPlayer.transform.position;
                        t.updateTornadoSettings();
                    }
                }
            }

            if (GUI.Button(menuStyle.BtnRect(8, 2), "Kill Tornadoes", menuStyle.BtnStyle))
            {
                Tornado[] ts = mod_instance.gameController.getTornados().ToArray();
                foreach (Tornado t in ts)
                {
                    t.disableTornadoRPC();
                }
            }

            _menu2.ui_update();
        }
    }
}
