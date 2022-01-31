using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityEngine;
using MelonLoader;

namespace Storm_Chasers_Menu.UI
{
    public class MainMenu
    {
        /// fields
        public bool ShowHide = true, isStart;
        public UI.Menu selectedMenu = Menu.None;
        private bool menuInitialized = false;

        Style menuStyle = new Style();

        SelfMenu _selfMenu;
        PlayersMenu _serverMenu;
        TornadoMenu _tornadoMenu;
        SpawnMenu _spawnMenu;
        VisualMenu _visualMenu;
        PlayerManager _playerManager;

        Mods mod_instance;

        public MainMenu(Mods mod)
        {
            mod_instance = mod;
            _selfMenu = new SelfMenu(mod_instance);
            _serverMenu = new PlayersMenu(mod_instance);
            _tornadoMenu = new TornadoMenu(mod_instance);
            _spawnMenu = new SpawnMenu(mod_instance);
            _visualMenu = new VisualMenu(mod_instance);
            _playerManager = new PlayerManager(mod_instance);
        }

        public void OnGUIMainMenu()
        {
            if (!isStart)
            {
                menuStyle.Start();
                _selfMenu.menuStyle.Start();
                _serverMenu.menuStyle.Start();
                _tornadoMenu.menuStyle.Start();
                _spawnMenu.menuStyle.Start();
                _visualMenu.menuStyle.Start();
                _playerManager.menuStyle.Start();
                isStart = true;
            }


            if (ShowHide)
            {
                //Cursor.lockState = 0;
                //Cursor.visible = true;
                MainMenuShow();
            }
            else
            {
                //Cursor.lockState = CursorLockMode.Locked;
                //Cursor.visible = false;
            }
        }


        public void HandleMenuSelection(UI.Menu sel)
        {
            switch (sel)
            {
                case Menu.None:
                    break;
                case Menu.Self:
                    _selfMenu.ui_update();
                    break;
                case Menu.Players:
                    _serverMenu.ui_update();
                    break;
                case Menu.Tornadoes:
                    _tornadoMenu.ui_update();
                    break;
                case Menu.Spawner:
                    _spawnMenu.ui_update();
                    break;
                case Menu.Visual:
                    _visualMenu.ui_update();
                    break;
                case Menu.PlayerMngr:
                    _playerManager.ui_update();
                    break;
            }
        }

        public void MainMenuShow()
        {
            GUI.Box(new Rect(menuStyle.posRect.x, menuStyle.posRect.y + 0f, menuStyle.widthSize + 10, 50f + 45 * menuStyle.mulY), "", menuStyle.BgStyle);
            GUI.Label(new Rect(menuStyle.posRect.x, menuStyle.posRect.y + 5f, menuStyle.widthSize + 10, 95f), "Storm Chasers Menu\nIndica#1259", menuStyle.LabelStyle);

            GUI.Box(new Rect(menuStyle.posRect.x, menuStyle.posRect.y + 65f + 45 * menuStyle.mulY, menuStyle.widthSize + 10, 50f), "", menuStyle.BgStyle);
            GUI.Label(new Rect(menuStyle.posRect.x, menuStyle.posRect.y + 70f + 45 * menuStyle.mulY, menuStyle.widthSize + 10, 50f), "<color=yellow>Host : " + mod_instance._hostName() + "\nPlayers : " + mod_instance._playerCount() + "</color>", menuStyle.LabelStyle);

            if (GUI.Button(menuStyle.BtnRect(1), "Self", menuStyle.BtnStyle))
            {
                selectedMenu = Menu.Self;
            }

            if (GUI.Button(menuStyle.BtnRect(2), "Server", menuStyle.BtnStyle))
            {
                selectedMenu = Menu.Players;
            }

            if (GUI.Button(menuStyle.BtnRect(3), "Tornadoes", menuStyle.BtnStyle))
            {
                selectedMenu = Menu.Tornadoes;
            }

            if (GUI.Button(menuStyle.BtnRect(4), "Spawner", menuStyle.BtnStyle))
            {
                selectedMenu = Menu.Spawner;
            }

            if (GUI.Button(menuStyle.BtnRect(5), "Visual", menuStyle.BtnStyle))
            {
                selectedMenu = Menu.Visual;
            }

            if (GUI.Button(menuStyle.BtnRect(6), "Manager", menuStyle.BtnStyle))
            {
                selectedMenu = Menu.PlayerMngr;
            }

            if (GUI.Button(menuStyle.BtnRect(7), "Hide", menuStyle.BtnStyle))
            {
                selectedMenu = Menu.None;
            }

            HandleMenuSelection(selectedMenu);

            if (!menuInitialized)
            {
                MelonLogger.Msg("Storm Chasers Menu Initialized!");
                menuInitialized = true;
            }
        }

        public void UpdateThread()
        {
            if (Input.GetKeyDown(KeyCode.Insert))
                ShowHide = !ShowHide;
        }
    }
}
