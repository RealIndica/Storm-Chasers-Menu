using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Storm_Chasers_Menu.UI
{
    public class VisualMenu
    {
        public Style menuStyle = new Style();
        private Mods mod_instance;

        public VisualMenu(Mods mod)
        {
            mod_instance = mod;
        }

        public void ui_update()
        {
            GUI.Box(new Rect(menuStyle.posRect.x + menuStyle.widthSize + 20, menuStyle.posRect.y + 0f, menuStyle.widthSize + 10, 50f + 45 * menuStyle.mulY), "", menuStyle.BgStyle);
            GUI.Label(new Rect(menuStyle.posRect.x + menuStyle.widthSize + 20, menuStyle.posRect.y + 5f, menuStyle.widthSize + 10, 95f), "Visual\nMenu", menuStyle.LabelStyle);

            if (mod_instance.PlayerESP)
            {
                if (GUI.Button(menuStyle.BtnRect(1, 2), "Player ESP", menuStyle.OnStyle))
                {
                    mod_instance.PlayerESP = !mod_instance.PlayerESP;
                }
            } 
            else
            {
                if (GUI.Button(menuStyle.BtnRect(1, 2), "Player ESP", menuStyle.OffStyle))
                {
                    mod_instance.PlayerESP = !mod_instance.PlayerESP;
                }
            }

            if (mod_instance.TruckESP)
            {
                if (GUI.Button(menuStyle.BtnRect(2, 2), "Truck ESP", menuStyle.OnStyle))
                {
                    mod_instance.TruckESP = !mod_instance.TruckESP;
                }
            }
            else
            {
                if (GUI.Button(menuStyle.BtnRect(2, 2), "Truck ESP", menuStyle.OffStyle))
                {
                    mod_instance.TruckESP = !mod_instance.TruckESP;
                }
            }

            if (mod_instance.TornadoESP)
            {
                if (GUI.Button(menuStyle.BtnRect(3, 2), "Tornado ESP", menuStyle.OnStyle))
                {
                    mod_instance.TornadoESP = !mod_instance.TornadoESP;
                }
            }
            else
            {
                if (GUI.Button(menuStyle.BtnRect(3, 2), "Tornado ESP", menuStyle.OffStyle))
                {
                    mod_instance.TornadoESP = !mod_instance.TornadoESP;
                }
            }
        }
    }
}
