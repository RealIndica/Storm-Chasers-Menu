using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MelonLoader;
using Harmony;
using UnityEngine;
using System.Reflection;

namespace Storm_Chasers_Menu
{
    [Obfuscation(ApplyToMembers = false)]
    public class MainMod : MelonMod
    {

        /* TO DO
         * - Tornado Magnet X
         * - Click teleport by map position -
         * - Fix weird crashing issue (might just be game idk) ~
         */


        UI.MainMenu mainMenu;

        [Obfuscation(Exclude = false)]
        Mods mod_instance = new Mods();

        [Obfuscation(Exclude = false)]
        public static bool inGame = false;

        [Obfuscation(Exclude = false)]
        private float nextActionTime = 0.0f;

        [Obfuscation(Exclude = false)]
        public float period = 0.1f;

        public MainMod()
        {
            mainMenu = new UI.MainMenu(mod_instance);
            ModHandler.FixGameCompatibility();
        }

        [Obfuscation(Exclude = true)]
        public override void OnUpdate()
        {
            if (Time.time > nextActionTime)
            {
                nextActionTime += period;
                if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("Game"))
                {
                    inGame = true;
                } else
                {
                    inGame = false;
                    Mods.disableOnlinePerm();
                }
            }

            if (inGame)
            {
                mainMenu.UpdateThread();
                mod_instance.mod_update();
            }
        }

        [Obfuscation(Exclude = true)]
        public override void OnGUI()
        {
            if (inGame)
            {
                mainMenu.OnGUIMainMenu();
                mod_instance.gui_update();
            }
        }
    }
}
