using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Storm_Chasers_Menu.UI
{
    public class SelfMenu2
    {
        public Style menuStyle = new Style();
        private Mods mod_instance;
        private SelfMenu parentMenu;

        public SelfMenu2(Mods mod, SelfMenu inputmenu)
        {
            mod_instance = mod;
            parentMenu = inputmenu;
        }

        public string GenerateName(int len)
        {
            System.Random r = new System.Random();
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
            string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
            string Name = "";
            Name += consonants[r.Next(consonants.Length)].ToUpper();
            Name += vowels[r.Next(vowels.Length)];
            int b = 2; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
            while (b < len)
            {
                Name += consonants[r.Next(consonants.Length)];
                b++;
                Name += vowels[r.Next(vowels.Length)];
                b++;
            }

            return Name;

        }

        string oldName = "";
        bool savedOldName = false;
        public void ui_update()
        {
            GUI.Box(new Rect(menuStyle.posRect.x + (menuStyle.widthSize + 20) * 2, menuStyle.posRect.y + 0f, menuStyle.widthSize + 10, 50f + 45 * menuStyle.mulY), "", menuStyle.BgStyle);
            GUI.Label(new Rect(menuStyle.posRect.x + (menuStyle.widthSize + 20) * 2, menuStyle.posRect.y + 5f, menuStyle.widthSize + 10, 95f), "Self\nMenu (2)", menuStyle.LabelStyle);

            if (GUI.Button(menuStyle.BtnRect(1, 3), "Randomize Name", menuStyle.BtnStyle))
            {
                if (!savedOldName)
                {
                    oldName = PhotonNetwork.player.NickName;
                }
                savedOldName = true;
                PhotonNetwork.player.NickName = GenerateName(7);
            }

            if (GUI.Button(menuStyle.BtnRect(2, 3), "Restore Name", menuStyle.BtnStyle))
            {
                if (savedOldName)
                PhotonNetwork.player.NickName = oldName;
            }


            if (StaticPatches.noMapBoundaries)
            {
                if (GUI.Button(menuStyle.BtnRect(3, 3), "No Bounaries", menuStyle.OnStyle))
                {
                    StaticPatches.noMapBoundaries = !StaticPatches.noMapBoundaries;
                }
            }
            else
            {
                if (GUI.Button(menuStyle.BtnRect(3, 3), "No Bounaries", menuStyle.OffStyle))
                {
                    StaticPatches.noMapBoundaries = !StaticPatches.noMapBoundaries;
                }
            }

            if (GUI.Button(menuStyle.BtnRect(4, 3), "Kill Self", menuStyle.BtnStyle))
            {
                if (mod_instance.localPlayer.isInsideCar)
                {
                    mod_instance.localCar.instantiateDeathRPC();
                }
                mod_instance.localPlayer.DieRPC();
            }

            if (GUI.Button(menuStyle.BtnRect(5, 3), "Fix Radar", menuStyle.BtnStyle))
            {
                foreach (HeatmapRender H in mod_instance.gameController.heatmapRenders)
                {
                    H.Reset = true;
                }
            }
        }
    }
}
