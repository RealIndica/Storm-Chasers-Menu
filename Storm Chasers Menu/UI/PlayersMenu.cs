using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Storm_Chasers_Menu.UI
{
    public class PlayersMenu
    {
        public Style menuStyle = new Style();
        private Mods mod_instance;

        public PlayersMenu(Mods mod)
        {
            mod_instance = mod;
        }

        public void ui_update()
        {
            GUI.Box(new Rect(menuStyle.posRect.x + menuStyle.widthSize + 20, menuStyle.posRect.y + 0f, menuStyle.widthSize + 10, 50f + 45 * menuStyle.mulY), "", menuStyle.BgStyle);
            GUI.Label(new Rect(menuStyle.posRect.x + menuStyle.widthSize + 20, menuStyle.posRect.y + 5f, menuStyle.widthSize + 10, 95f), "Player\nMenu", menuStyle.LabelStyle);

            if (GUI.Button(menuStyle.BtnRect(1, 2), "Kill Everyone", menuStyle.BtnStyle))
            {
                Player[] players = mod_instance.gameController.otherPlayers.ToArray();
                foreach (Player p in players)
                {
                    p.photonView.RPC("Die", PhotonTargets.All, null);
                }
            }

            if (GUI.Button(menuStyle.BtnRect(2, 2), "Bring all", menuStyle.BtnStyle))
            {
                Player[] players = mod_instance.gameController.otherPlayers.ToArray();
                Vector3 myPos = mod_instance.localPlayer.transform.position;
                foreach (Player p in players)
                {
                    if (p.isInsideCar)
                    {
                        p.getInteractCar().photonView.RPC("updatePosition", PhotonTargets.All, new object[] { myPos });
                    } 
                    else
                    {
                        p.photonView.RPC("updatePosition", PhotonTargets.All, new object[] { myPos });
                    }
                }
            }

            if (GUI.Button(menuStyle.BtnRect(3, 2), "All $10,000", menuStyle.BtnStyle))
            {
                Player[] players = mod_instance.gameController.otherPlayers.ToArray();
                foreach (Player p in players)
                {
                    p.photonView.RPC("receiveMoney", PhotonTargets.All, new object[] { (int)10000 });
                }
            }

            if (GUI.Button(menuStyle.BtnRect(4, 2), "Restart Server", menuStyle.BtnStyle))
            {
                mod_instance.gameController.currentVotationType = GameController.VotationType.RESET;
                for (int i = 0; i < 20; i++)
                {
                    mod_instance.gameController.photonView.RPC("receiveVote", PhotonTargets.All, new object[] { i, GameController.VotationType.RESET });
                }
            }

            if (GUI.Button(menuStyle.BtnRect(5, 2), "Force Sleep", menuStyle.BtnStyle))
            {
                mod_instance.gameController.currentVotationType = GameController.VotationType.SLEEP;
                for (int i = 0; i < 20; i++)
                {
                    mod_instance.gameController.photonView.RPC("receiveVote", PhotonTargets.All, new object[] { i, GameController.VotationType.SLEEP });
                }
            }

            if (GUI.Button(menuStyle.BtnRect(6, 2), "Explode Trucks", menuStyle.BtnStyle))
            {
                Player[] players = mod_instance.gameController.otherPlayers.ToArray();
                foreach (Player p in players)
                {
                    mod_instance.getTruckByPhoton(p.photonView.owner).instantiateDeathRPC();
                }
            }

            if (GUI.Button(menuStyle.BtnRect(7, 2), "5000 Score All", menuStyle.BtnStyle))
            {              
                foreach (PhotonPlayer p in PhotonNetwork.playerList)
                {
                    p.AddScore(5000);                  
                }
            }
        }
    }
}
