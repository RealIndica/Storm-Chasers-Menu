using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityEngine;
using MelonLoader;

namespace Storm_Chasers_Menu.UI
{
    public class SelfMenu
    {
        public Style menuStyle = new Style();
        private Mods mod_instance;
        private SelfMenu2 _selfMenu2;

        public SelfMenu(Mods mod)
        {
            mod_instance = mod;
            _selfMenu2 = new SelfMenu2(mod_instance, this);
            _selfMenu2.menuStyle.Start();
        }

        public void ui_update()
        {
            GUI.Box(new Rect(menuStyle.posRect.x + menuStyle.widthSize + 20, menuStyle.posRect.y + 0f, menuStyle.widthSize + 10, 50f + 45 * menuStyle.mulY), "", menuStyle.BgStyle);
            GUI.Label(new Rect(menuStyle.posRect.x + menuStyle.widthSize + 20, menuStyle.posRect.y + 5f, menuStyle.widthSize + 10, 95f), "Self\nMenu", menuStyle.LabelStyle);

            if (GUI.Button(menuStyle.BtnRect(1, 2), "Force Host", menuStyle.BtnStyle))
            {
                mod_instance._forceHost();
            }

            if (GUI.Button(menuStyle.BtnRect(2, 2), "Give $1,000", menuStyle.BtnStyle))
            {
                mod_instance.gameController.earnMoney(1000);
            }

            if (GUI.Button(menuStyle.BtnRect(3, 2), "Max Fuel", menuStyle.BtnStyle))
            {
                mod_instance.localCar.fuel = 100f;
            }

            if (GUI.Button(menuStyle.BtnRect(4, 2), "Fix Truck", menuStyle.BtnStyle))
            {
                mod_instance.localCar.replacementTireCount = 100;
                mod_instance.localCar.stuckInMudProbability = 0f;
                mod_instance.localCar.toggleFlatTireRPC(0, false);
                mod_instance.localCar.toggleFlatTireRPC(1, false);
                mod_instance.localCar.toggleFlatTireRPC(2, false);
                mod_instance.localCar.toggleFlatTireRPC(3, false);
                mod_instance.localCar.repairCar();
                mod_instance.localCar.repairWindowsRPC();
                mod_instance.localCar.GetComponent<EVP.VehicleDamage>().Repair();
                mod_instance.localCar.health = mod_instance.localCar.maxHealth;
            }

            if (GUI.Button(menuStyle.BtnRect(5, 2), "Heal Self", menuStyle.BtnStyle))
            {
                mod_instance.localPlayer.GetComponent<vp_FPPlayerDamageHandler>().CurrentHealth = mod_instance.localPlayer.GetComponent<vp_FPPlayerDamageHandler>().MaxHealth;
            }

            if (mod_instance.godMode)
            {
                if (GUI.Button(menuStyle.BtnRect(6, 2), "God Mode", menuStyle.OnStyle))
                {
                    mod_instance.godMode = !mod_instance.godMode;
                }
            } 
            else
            {
                if (GUI.Button(menuStyle.BtnRect(6, 2), "God Mode", menuStyle.OffStyle))
                {
                    mod_instance.godMode = !mod_instance.godMode;
                }
            }

            if (mod_instance.fastTruck)
            {
                if (GUI.Button(menuStyle.BtnRect(7, 2), "Fast Truck", menuStyle.OnStyle))
                {
                    mod_instance.fastTruck = !mod_instance.fastTruck;
                    mod_instance.fastTruckManage();
                }
            }
            else
            {
                if (GUI.Button(menuStyle.BtnRect(7, 2), "Fast Truck", menuStyle.OffStyle))
                {
                    mod_instance.fastTruck = !mod_instance.fastTruck;
                    mod_instance.fastTruckManage();
                }
            }

            _selfMenu2.ui_update();
        }
    }
}
