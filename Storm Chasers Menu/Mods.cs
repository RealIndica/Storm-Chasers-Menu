using System;
using UnityEngine;
using System.Threading;
using MelonLoader;
using EVP;
using System.Linq;
using System.Collections.Generic;
using Harmony;
using System.Collections;
using System.Reflection;

namespace Storm_Chasers_Menu
{
    public class Mods
    {
        public Player localPlayer;
        public CarTornado localCar;
        public TornadoController tornadoController;
        public GameController gameController;
        public TornadoSpawner offlineTornadoSpawner;
        public TornadoSpawner tornadoSpawner;
        public LaptopMapNavigationUI laptopMenuBase;

        public List<Player> esp_players = new List<Player>();
        public List<CarTornado> esp_trucks = new List<CarTornado>();
        public List<Tornado> esp_tornados = new List<Tornado>();

        public List<string> prefabs = new List<string>();
        public List<GameObject> spawnedPrefabs = new List<GameObject>();
        public bool loadedPrefabs = false;

        public bool godMode = false;
        public bool fastTruck = false;
        public bool PlayerESP = false;
        public bool TruckESP = false;
        public bool TornadoESP = false;
        public bool Crosshair = false;
        public bool fastRCtornado = false;

        //Frame Rate
        private int m_frameCounter = 0;
        private float m_timeCounter = 0.0f;
        private float m_lastFramerate = 0.0f;
        private float m_refreshTime = 0.5f;

        private bool fps_clearNearestPerFrame = false;
        private bool fps_destroyMode = false;
      
        private Vector2 realTerrainSize = new Vector2(10240, 10240);

        private bool isLaptopMapShown = false;
        

        public Mods()
        {
            new Thread(delegate ()
            {
                loadObjects();
            }).Start();
        }

        public void _forceHost()
        {
            if (PhotonNetwork.inRoom)
                PhotonNetwork.SetMasterClient(PhotonNetwork.player);
        }

        public int _playerCount()
        {
            if (PhotonNetwork.inRoom)
            {
                return PhotonNetwork.room.PlayerCount;
            }
            else
            {
                return 1;
            }
        }

        public bool isOwnerDrivingTruck(PhotonPlayer p)
        {
            return !getTruckByPhoton(p).isDriverSeatFree;
        }

        public CarTornado getTruckByPhoton(PhotonPlayer p)
        {
            CarTornado ret = localCar;
            foreach (CarTornado px in esp_trucks)
            {
                if (px.photonView.owner.NickName == p.NickName)
                {
                    ret = px;
                }
            }

            return ret;
        }

        public Player getPlayerByPhoton(PhotonPlayer p)
        {
            Player ret = localPlayer;
            foreach (Player px in esp_players)
            {
                if (px.photonView.owner.NickName == p.NickName)
                {
                    ret = px;
                }
            }

            return ret;
        }

        public Vector3 getMyPos()
        {
            if (localPlayer && localPlayer.mainCamera)
            {
                if (localPlayer.isInsideCar && localPlayer.isCarDriver)
                {
                    return localCar.transform.position;
                }
                else
                {
                    return localPlayer.transform.position;
                }
            } else
            {
                return new Vector3(0, 0, 0);
            }
        }

        public string _hostName()
        {
            if (PhotonNetwork.inRoom)
            {
                return PhotonNetwork.masterClient.NickName;
            }
            else
            {
                return "N/A";
            }
        }

        public void fastTruckManage()
        {
            if (fastTruck)
            {
                if (localCar)
                {
                    localCar.GetComponent<VehicleController>().aeroDrag = -5f;
                }
            }
            else
            {
                if (localCar)
                {
                    localCar.GetComponent<VehicleController>().aeroDrag = 0.2f;
                }
            }
        }

        public static void disableOnlinePerm()
        {
            if (!MainMod.inGame && StaticPatches.disableOnline)
            {
                if (GameObject.FindObjectOfType<MainUIMenu>())
                {
                    GameObject.FindObjectOfType<MainUIMenu>().OnlineRoomsButtons.transform.Find("PublicOnlineButton").GetComponent<UnityEngine.UI.Button>().interactable = false;
                    GameObject.FindObjectOfType<MainUIMenu>().OnlineRoomsButtons.transform.Find("JoinPrivateButton").GetComponent<UnityEngine.UI.Button>().interactable = false;
                }
            }
        }



        private class optimizeObject
        {
            public GameObject _object { get; set; }
            public float distanceFromLocal { get; set; }
        }

        [Obfuscation(Exclude = true)]
        class optimizeCompare : IComparer
        {
            [Obfuscation(Exclude = true)]
            public int Compare(object x, object y)
            {
                return Comparer<float>.Default.Compare(((optimizeObject)x).distanceFromLocal, ((optimizeObject)y).distanceFromLocal);
            }
        }

        GameObject[] organizeObjectsByDistance(GameObject[] objects)
        {
            optimizeObject[] pre = new optimizeObject[objects.Length];
            List<GameObject> organized = new List<GameObject>();

            int idx = 0; 
            foreach (GameObject obj in objects)
            {
                float curDist = Vector3.Distance(obj.transform.position, getMyPos());
                pre[idx] = new optimizeObject() { _object = obj, distanceFromLocal = curDist };
                idx++;
            }

            Array.Sort(pre, new optimizeCompare());

            foreach (optimizeObject o in pre)
            {
                organized.Add(o._object);
            }

            return organized.ToArray();
        }

        private void clear_Debris()
        {
            try
            {
                GameObject environment_Gobj = GameObject.Find("Environment");
                List<Transform> allTransforms = environment_Gobj.GetComponentsInChildren<Transform>().ToList();
                List<GameObject> removableObjects = new List<GameObject>();
                List<GameObject> removableNearestFirst = new List<GameObject>();

                foreach (GameObject obj in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                {
                    allTransforms.Add(obj.transform);
                }

                foreach (Transform t in allTransforms)
                {
                    GameObject cur_object = t.gameObject;
                    if (cur_object.GetComponent<MeshRenderer>() && cur_object.GetComponent<StaticTornadoObject>())
                    {
                        if (cur_object.GetComponent<MeshRenderer>().enabled && cur_object.GetComponent<StaticTornadoObject>().isActivated())
                        {
                            removableObjects.Add(cur_object);
                        }
                    }
                }

                if (removableObjects.Any())
                {
                    removableNearestFirst = organizeObjectsByDistance(removableObjects.ToArray()).ToList();

                    if (!fps_clearNearestPerFrame)
                    {
                        int removeAmount = (int)Math.Round((double)(removableNearestFirst.Count / 8), 0);
                        if (removeAmount != 0)
                        {
                            for (int i = 0; i <= removeAmount; i++)
                            {
                                if (!fps_destroyMode)
                                {
                                    removableNearestFirst[i].GetComponent<MeshRenderer>().enabled = false;
                                    removableNearestFirst[i].GetComponent<StaticTornadoObject>().enabled = false;
                                    removableNearestFirst[i].GetComponent<BoxCollider>().enabled = false;
                                    removableNearestFirst[i].SetActive(false);
                                } else
                                {
                                    GameObject.Destroy(removableNearestFirst[i]);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!fps_destroyMode)
                        {
                            removableNearestFirst[0].GetComponent<MeshRenderer>().enabled = false;
                            removableNearestFirst[0].GetComponent<StaticTornadoObject>().enabled = false;
                            removableNearestFirst[0].GetComponent<BoxCollider>().enabled = false;
                            removableNearestFirst[0].SetActive(false);
                        } else
                        {
                            GameObject.Destroy(removableNearestFirst[0]);
                        }
                    }
                }
            }
            catch (Exception)
            { /*sometimes "System.ObjectDisposedException" occures, even though i don't see how it should so this is an easy fix :) */ }
        }

        private void fps_monitor()
        {
            if (m_timeCounter < m_refreshTime)
            {
                m_timeCounter += Time.deltaTime;
                m_frameCounter++;
            }
            else
            {
                m_lastFramerate = (float)m_frameCounter / m_timeCounter;
                m_frameCounter = 0;
                m_timeCounter = 0.0f;
                if (m_lastFramerate < 24.0f)
                {
                    if (fps_clearNearestPerFrame)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            clear_Debris();
                        }
                    } else
                    {
                        clear_Debris();
                        Resources.UnloadUnusedAssets();
                    }
                }
            }
        }

        private void handleMapTP() //called on Input.GetMouseButtonDown(1) in Update
        {
            localCar.GetComponent<Rigidbody>().isKinematic = true;
            LaptopUIMenu curMenu = laptopMenuBase.transform.parent.parent.GetComponent<LaptopUIMenu>();
            var prop1 = curMenu.GetType().GetField("currentTabSelected", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if ((int)prop1.GetValue(curMenu) == 0 && laptopMenuBase.transform.parent.parent.gameObject.activeSelf)
            {
                Camera baseCam = laptopMenuBase.GPSCameras[0];
                int layerMask = LayerMask.GetMask("Terrain", "Road"); //12 and 14

                Vector2 normalizedPoint = new Vector2(0.5f, 0.5f);

                var renderRay = baseCam.ViewportPointToRay(normalizedPoint);

                RaycastHit raycastHit;
                if (Physics.Raycast(renderRay, out raycastHit, float.MaxValue, layerMask))
                {
                    localCar.transform.position = new Vector3(raycastHit.point.x, raycastHit.point.y + 5f, raycastHit.point.z + 15f);
                }
            }

            localCar.GetComponent<Rigidbody>().isKinematic = false;
        }

        public void mod_update()
        {
            if (gameController)
            {
                //fps_monitor();

                if (laptopMenuBase && laptopMenuBase.transform.parent.parent.gameObject.activeSelf)
                {
                    isLaptopMapShown = true;
                } else
                {
                    isLaptopMapShown = false;
                }
            }

            if (godMode)
            {
                if (localPlayer)
                {
                    localPlayer.GetComponent<vp_FPPlayerDamageHandler>().CurrentHealth = localPlayer.GetComponent<vp_FPPlayerDamageHandler>().MaxHealth;
                    localPlayer.GetComponent<vp_FPPlayerDamageHandler>().AllowFallDamage = false;
                }
                if (localCar)
                {
                    localCar.health = localCar.maxHealth;
                }
            }
            else
            {
                if (localPlayer)
                {
                    localPlayer.GetComponent<vp_FPPlayerDamageHandler>().AllowFallDamage = true;
                }
            }

            if (Input.GetKey(KeyCode.LeftAlt))
            {
                Crosshair = true;
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit raycastHit;

                    if (localPlayer && localPlayer.mainCamera && !isLaptopMapShown)
                    {
                        if (localPlayer.isInsideCar && localPlayer.isCarDriver)
                        {
                            if (Physics.Raycast(localPlayer.mainCamera.transform.position, localPlayer.mainCamera.transform.forward, out raycastHit, float.MaxValue))
                            {
                                localCar.transform.position = new Vector3(raycastHit.point.x, raycastHit.point.y + 1f, raycastHit.point.z);
                            }
                        }
                        else
                        {
                            if (Physics.Raycast(localPlayer.mainCamera.transform.position, localPlayer.mainCamera.transform.forward, out raycastHit, float.MaxValue))
                            {
                                localPlayer.transform.position = new Vector3(raycastHit.point.x, raycastHit.point.y + 1f, raycastHit.point.z);
                            }
                        }
                    }
                }

                if (Input.GetMouseButtonDown(1))
                {
                    if (isLaptopMapShown)
                    {
                        handleMapTP();
                    }
                    else
                    {
                        RaycastHit raycastHit;
                        if (localPlayer && localPlayer.mainCamera)
                        {
                            if (Physics.Raycast(localPlayer.mainCamera.transform.position, localPlayer.mainCamera.transform.forward, out raycastHit, float.MaxValue))
                            {
                                StaticPatches.followPlayer = false;
                                StaticPatches.tornadoDirection = new Vector3(raycastHit.point.x, raycastHit.point.y + 1f, raycastHit.point.z);
                                localPlayer.think("Updated Tornado Magnet Position!");
                            }
                        }
                    }
                }
            } 
            else
            {
                Crosshair = false;
            }
        }

        public void gui_update()
        {
            if (PlayerESP)
            {
                foreach (Player p in esp_players)
                {
                    if (p)
                    {
                        Vector3 vector = Camera.main.WorldToScreenPoint(p.transform.position);
                        if (vector.z > 0)
                        {
                            vector.y = (float)Screen.height - (vector.y + 1f);
                            GUI.color = Color.green;
                            GUI.Label(new Rect(new Vector2(vector.x, vector.y), new Vector2(100f, 100f)), p.photonView.owner.NickName + "\nHealth : " + p.getCurrentHealth().ToString("0"));
                        }
                    }
                }
            }

            if (TruckESP)
            {
                foreach (CarTornado c in esp_trucks)
                {
                    if (c)
                    {
                        Vector3 vector = Camera.main.WorldToScreenPoint(c.transform.position);
                        Renderer[] rA = c.transform.Find("Heavy Duty Truck").Find("Heavy Duty Truck LOD2").GetComponentsInChildren<Renderer>();
                        if (vector.z > 0)
                        {
                            vector.y = (float)Screen.height - (vector.y + 1f);
                            GUI.color = Color.blue;
                            GUI.Label(new Rect(new Vector2(vector.x, vector.y), new Vector2(100f, 100f)), "Health : " + c.health.ToString() + "\nFuel : " + c.fuel.ToString("0.00"));

                            foreach (Renderer r in rA)
                                Make3DBox(r.bounds.center, r.bounds.size, Color.blue);
                        }
                    }

                }
            }

            if (TornadoESP)
            {
                foreach (Tornado t in esp_tornados)
                {
                    if (t)
                    {
                        if (t.tornadoType == Tornado.TornadoType.REAL && t.tornadoColumn)
                        {
                            Vector3 vector = Camera.main.WorldToScreenPoint(t.transform.position);
                            Renderer[] rA = t.transform.Find("Column").GetComponentsInChildren<Renderer>();
                            if (vector.z > 0)
                            {
                                vector.y = (float)Screen.height - (vector.y + 1f);
                                GUI.color = Color.red;
                                GUI.Label(new Rect(new Vector2(vector.x, vector.y), new Vector2(100f, 100f)), "Category : EF" + t.category.ToString("d") + "\nTime : " + t.timer.ToString("0") + "/" + t.durationTime.ToString("0"));

                                foreach (Renderer r in rA)
                                    Make3DBox(r.bounds.center, r.bounds.size, Color.red);
                            }
                        }
                    }
                }
            }

            if (Crosshair)
            {
                DrawingAPI.DrawLine(new Vector2((Screen.width / 2) - 15, Screen.height / 2), new Vector2((Screen.width / 2) + 15, Screen.height / 2), Color.green, 2.0f, false);
                DrawingAPI.DrawLine(new Vector2(Screen.width / 2, (Screen.height / 2) - 15), new Vector2(Screen.width / 2, (Screen.height / 2) + 15), Color.green, 2.0f, false);
            }
        }

        private static void MakeESPLine(Vector3 center, float x1, float y1, float z1, float x2, float y2, float z2, Color color)
        {
            Vector3 pointPos1 = new Vector3(center.x + x1, center.y + y1, center.z + z1);
            Vector3 pointPos2 = new Vector3(center.x + x2, center.y + y2, center.z + z2);
            Vector3 xy1 = Camera.main.WorldToScreenPoint(pointPos1);
            Vector3 xy2 = Camera.main.WorldToScreenPoint(pointPos2);
            if ((xy1.z > 0) && (xy2.z > 0))
                DrawingAPI.DrawLine(new Vector2(xy1.x, Screen.height - xy1.y), new Vector2(xy2.x, Screen.height - xy2.y), color, 1.0f, false);
        }
        private static void Make3DBox(Vector3 center, Vector3 size, Color color)
        {
            //clockwise

            //bottom
            MakeESPLine(center, -size.x / 2, -size.y / 2, size.z / 2, size.x / 2, -size.y / 2, size.z / 2, color);
            MakeESPLine(center, size.x / 2, -size.y / 2, size.z / 2, size.x / 2, -size.y / 2, -size.z / 2, color);
            MakeESPLine(center, size.x / 2, -size.y / 2, -size.z / 2, -size.x / 2, -size.y / 2, -size.z / 2, color);
            MakeESPLine(center, -size.x / 2, -size.y / 2, -size.z / 2, -size.x / 2, -size.y / 2, size.z / 2, color);

            //middle
            MakeESPLine(center, -size.x / 2, -size.y / 2, size.z / 2, -size.x / 2, size.y / 2, size.z / 2, color);
            MakeESPLine(center, size.x / 2, -size.y / 2, size.z / 2, size.x / 2, size.y / 2, size.z / 2, color);
            MakeESPLine(center, size.x / 2, -size.y / 2, -size.z / 2, size.x / 2, size.y / 2, -size.z / 2, color);
            MakeESPLine(center, -size.x / 2, -size.y / 2, -size.z / 2, -size.x / 2, size.y / 2, -size.z / 2, color);

            //top
            MakeESPLine(center, -size.x / 2, size.y / 2, size.z / 2, size.x / 2, size.y / 2, size.z / 2, color);
            MakeESPLine(center, size.x / 2, size.y / 2, size.z / 2, size.x / 2, size.y / 2, -size.z / 2, color);
            MakeESPLine(center, size.x / 2, size.y / 2, -size.z / 2, -size.x / 2, size.y / 2, -size.z / 2, color);
            MakeESPLine(center, -size.x / 2, size.y / 2, -size.z / 2, -size.x / 2, size.y / 2, size.z / 2, color);
        }

        private void loadPrefabNames()
        {
            string[] prefabsList =
            {
                "_None_Post",
"Angled_Post",
"Arc1_Rail",
"BarbedWire_Rail",
"BentCylinder_Post",
"bootcamp fence prefab",
"Bootcamp Fence prefabs",
"BoxFlashPrefab",
"BoxPrefab",
"BoxSection_Rail",
"BoxyWithBase_Post",
"Boy",
"Brick_Post",
"Bridge Medieval Prefab",
"bridge simple prefab",
"bridge_01_2_lane_entrance_1_lane",
"bridge_01_2_lane_entrance_1_lane_0",
"bridge_01_2_lane_exit_1_lane",
"bridge_01_2_lane_exit_1_lane_0",
"bridge_01_pilar",
"BrokenFence_Post",
"Chicken",
"ClassicRoadsideBox_Post",
"ClassicRoadsideBox_Rail",
"ClassicRoadsideGreen_Post",
"ClassicRoadsideGreen_Rail",
"CleanWall_Post",
"CleanWood_Post",
"CleanWoodVert_Post",
"ClickMarkerObj",
"concrete barrier prefab",
"concrete barrier simple prefab",
"ConcreteMouldy_Panel_Rail",
"ConcreteMouldy_Post",
"Cow",
"CrossFence_Panel_Rail",
"crosswalk 1 prefab",
"crosswalk 1 prefab_0",
"crosswalk 2 prefab",
"crosswalk 2 prefab_0",
"CrowdFence_Panel_Rail",
"Cube",
"curbStone prefab",
"custom roundabout prefab",
"custom roundabout prefab_0",
"custom t crossing prefab",
"custom t crossing prefab_0",
"CylinderSlim_Rail",
"Cylindrical_Post",
"Cylindrical_Rail",
"CylindricalPlane_Rail",
"CylindricalTapered_Post",
"DarkDirtyWood_Panel_Rail",
"DarkWoodPlank_Rail",
"Default T Crossing",
"Default X Crossing",
"DefaultGrassPatch",
"DiamondNew2_Panel_Rail",
"Electric_column_1",
"Electric_column_end",
"Electric_column_wire_to_small",
"ER Road Network",
"ERProjectLog",
"ERProRoad",
"ERSideObjectsLog",
"ExplosionBig",
"GalvanisedBoxy_Post",
"Gilded1_Panel_Rail",
"Girl",
"GlobalValues",
"guard rail",
"guard rail prefab",
"guard rail_0",
"halfpipe",
"halfpipe_0",
"Horizon2_Post",
"lamppost",
"LightningStrike",
"MapCell",
"MetalBase_Post",
"MetalCrossFence_Panel_Rail",
"MetalFence_Post",
"Mission01",
"Mission02",
"Mission03",
"Mission04",
"Mission05",
"Mission06",
"Mission07",
"Mission08",
"monsterprefab",
"Motorway_2L_Bridge1_prefab",
"muktargame",
"My Robot Kyle",
"OrangelSafety_Panel_Rail",
"Ornate1_Panel_Rail",
"Ornate1Wall_Post",
"OwnershipCube",
"OwnershipSphere",
"Palm",
"Paneled_Panel_Rail",
"ParticlesLight",
"Physics Box",
"Picket_Post",
"Picket_Rail",
"PicketSub_Post",
"PickupCharacter",
"Pig",
"Plain_Rail",
"Player",
"Player UI",
"PlayerTornado",
"Pointed1_Post",
"PointerPrefab",
"PostA2 1",
"PostA2",
"PowerlinePolePrefab",
"PowerLineWirePrefab",
"Pyramid_Post",
"RailA_Rail",
"RailAMetal_Rail",
"RailAMetalBlack_Rail",
"RearWindshield Broken",
"RoadBarrier_Panel_Rail",
"RoadBarrier_Post",
"Robot Kyle 2D",
"Robot Kyle Mecanim",
"Robot Kyle RPG",
"roundabout",
"Roundabout 4 connections",
"RTHolder",
"SciFi3_Rail",
"SciFiFence_Post",
"Sheep",
"Simple_SubJoiner",
"SimpleBevelPost4NoSmooth_Post",
"SimpleBlock_Panel_Rail",
"single lane dirt crossing prefab",
"single lane dirt crossing prefab_0",
"single lane sidewalk switch prefab",
"single lane sidewalk switch prefab_0",
"SlimCylinder_Post",
"SpikeySub_Post",
"SquareWire_Panel_Rail",
"Tornado",
"TornadoController",
"TornadoPod",
"Train Crossing Single Lane",
"Train Crossing Single Lane_0",
"trainConcrete",
"TwoPlusOne_Panel_Rail",
"TwoPlusOneGreen_Panel_Rail",
"VehiclePickup",
"VirtualSnapshotFilmSLR",
"vp_Input",
"Wall",
"Wall End",
"WaterBasicDaytime",
"WaterProDaytime",
"WhitePointy_Post",
"Window Left Broken",
"WindowLeftRear Broken",
"WindowRight Broken",
"WindowRightRear Broken",
"windshield Broken",
"wirefence prefab",
"WoodBlock_Post",
"WoodenArc_Rail",
"WoodenPicket_Post",
"WoodLog_Post",
"WoodLog_Rail",
"WoodLogBase_Post",
"WoodPLank_Post",
"WoodPLank_Rail",
"X crossing"
            };

            prefabs = prefabsList.ToList();

        }

        private void loadObjects()
        {
            while (true)
            {
                try
                {
                    gameController = GameController.Instance;
                    localPlayer = gameController.getLocalPlayer();
                    localCar = gameController.getLocalCar();
                    tornadoController = gameController.getLocalTornadoController();
                    offlineTornadoSpawner = gameController.spawnersFreeRoaming.GetComponentInChildren<TornadoSpawner>();
                    tornadoSpawner = gameController.spawnersOnlineFreeRoaming.GetComponentInChildren<TornadoSpawner>();
                    laptopMenuBase = GameObject.FindObjectOfType<LaptopMapNavigationUI>();


                    esp_players = Enumerable.ToList<Player>(UnityEngine.Object.FindObjectsOfType<Player>()); //also player list
                    
                    esp_trucks = Enumerable.ToList<CarTornado>(UnityEngine.Object.FindObjectsOfType<CarTornado>()); //also truck list

                    if (TornadoESP)
                    {
                        esp_tornados = Enumerable.ToList<Tornado>(UnityEngine.Object.FindObjectsOfType<Tornado>());
                    }

                    if (gameController)
                    {
                        if (!loadedPrefabs)
                        {
                            loadPrefabNames();
                            loadedPrefabs = true;
                        }
                    }
                }
                catch (Exception) { }
                Thread.Sleep(5000);
            }
        }
    }
}
