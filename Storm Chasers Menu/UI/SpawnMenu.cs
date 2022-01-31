using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Storm_Chasers_Menu.UI
{
    public class SpawnMenu
    {
        public Style menuStyle = new Style();
        private Mods mod_instance;

        public Vector2 scrollPosition = Vector2.zero;
        public Rect spawnScreen;

        string selectedSpawnPre = "";

        public SpawnMenu(Mods mod)
        {
            mod_instance = mod;
            spawnScreen = new Rect(menuStyle.posRect.x + (menuStyle.widthSize + 20) * 1, menuStyle.posRect.y + 0f, menuStyle.widthSize + 10, 50f + 45 * 6);
        }

        public void ui_update()
        {
            if (mod_instance.loadedPrefabs)
            {
                spawnScreen = GUI.Window(1, spawnScreen, drawSpawnList, "", menuStyle.BgStyle);

                GUI.Box(new Rect(menuStyle.posRect.x + (menuStyle.widthSize + 20) * 2, menuStyle.posRect.y + 0f, menuStyle.widthSize + 10, 50f + 45 * menuStyle.mulY), "", menuStyle.BgStyle);
                GUI.Label(new Rect(menuStyle.posRect.x + (menuStyle.widthSize + 20) * 2, menuStyle.posRect.y + 5f, menuStyle.widthSize + 10, 95f), "Spawner\nMenu", menuStyle.LabelStyle);

                if (GUI.Button(menuStyle.BtnRect(1, 3), "Spawn", menuStyle.BtnStyle))
                {
                    GameObject spawned = PhotonNetwork.Instantiate(selectedSpawnPre, mod_instance.localPlayer.transform.position, mod_instance.localPlayer.transform.rotation, 0);
                    mod_instance.spawnedPrefabs.Add(spawned);
                }

                if (GUI.Button(menuStyle.BtnRect(2, 3), "Remove Spawned", menuStyle.BtnStyle))
                {
                    foreach (GameObject g in mod_instance.spawnedPrefabs)
                    {
                        if (g)
                        {
                            PhotonNetwork.Destroy(g);
                        }
                    }

                    mod_instance.spawnedPrefabs.Clear();
                }
            }
        }

        void drawSpawnList(int WindowID)
        {
            scrollPosition = GUI.BeginScrollView(new Rect(0, 0, spawnScreen.width, spawnScreen.height), scrollPosition, new Rect(0, 0, spawnScreen.width, 60 * mod_instance.prefabs.Count));
            for (int i = 0; i < mod_instance.prefabs.Count; i++)
            {
                if (GUI.Button(new Rect(0, 60 * i, spawnScreen.width, 60), mod_instance.prefabs[i], menuStyle.BtnStyle))
                {
                    selectedSpawnPre = mod_instance.prefabs[i];
                }
            }
            GUI.EndScrollView();
        }
    }
}
