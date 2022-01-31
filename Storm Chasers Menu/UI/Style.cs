using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Unity;

namespace Storm_Chasers_Menu.UI
{
    public class Style
    {
        public GUIStyle BgStyle, OnStyle, OffStyle, LabelStyle, BtnStyle, BtnStyle1, BtnStyle2, BtnStyle3;
        public float delay = 0, widthSize = 300; //Change size here
        // Texture
        public Texture2D ontexture, onpresstexture, offtexture, offpresstexture, backtexture, btntexture, btnpresstexture;
        public Texture2D NewTexture2D { get { return new Texture2D(1, 1); } }

        // Position (x, y, width, height)
        // Change only x and y
        public Rect posRect = new Rect(0, 150, 0, 0);

        // Remember Y position
        public int btnY, mulY;

        // Remember X position
        public int btnX, mulX;

        /// Rect for buttons
        // Auto position buttons. There is no need to change it 
        public Rect BtnRect(int y = 1, int x = 1, bool multiplyBtn = false)
        {
            mulY = y;
            mulX = x;

            int finalX = 1;

            if (x >= 2)
            {
                for (int i = 1; i < x; i++)
                {
                    finalX += (int)widthSize + 20;
                }
            }

            if (multiplyBtn)
            {
                btnY = 5 + 45 * y;
                return new Rect(posRect.x + 5 * x, posRect.y + 5 + 45 * y, widthSize - 90, 40);
            }

            return new Rect(posRect.x + 4f + finalX, posRect.y + 5 + 45 * y, widthSize, 40);
        }

        /// Load GUIStyle
        public void Start()
        {
            if (BgStyle == null)
            {
                BgStyle = new GUIStyle();
                BgStyle.normal.background = BackTexture;
                BgStyle.onNormal.background = BackTexture;
                BgStyle.active.background = BackTexture;
                BgStyle.onActive.background = BackTexture;
                BgStyle.normal.textColor = Color.white;
                BgStyle.onNormal.textColor = Color.white;
                BgStyle.active.textColor = Color.white;
                BgStyle.onActive.textColor = Color.white;
                BgStyle.fontSize = 18;
                BgStyle.fontStyle = FontStyle.Normal;
                BgStyle.alignment = TextAnchor.UpperCenter;
            }

            if (LabelStyle == null)
            {
                LabelStyle = new GUIStyle();
                LabelStyle.normal.textColor = Color.white;
                LabelStyle.onNormal.textColor = Color.white;
                LabelStyle.active.textColor = Color.white;
                LabelStyle.onActive.textColor = Color.white;
                LabelStyle.fontSize = 18;
                LabelStyle.fontStyle = FontStyle.Normal;
                LabelStyle.alignment = TextAnchor.UpperCenter;
            }

            if (OffStyle == null)
            {
                OffStyle = new GUIStyle();
                OffStyle.normal.background = OffTexture;
                OffStyle.onNormal.background = OffTexture;
                OffStyle.active.background = OffPressTexture;
                OffStyle.onActive.background = OffPressTexture;
                OffStyle.normal.textColor = Color.white;
                OffStyle.onNormal.textColor = Color.white;
                OffStyle.active.textColor = Color.white;
                OffStyle.onActive.textColor = Color.white;
                OffStyle.fontSize = 18;
                OffStyle.fontStyle = FontStyle.Normal;
                OffStyle.alignment = TextAnchor.MiddleCenter;
            }

            if (OnStyle == null)
            {
                OnStyle = new GUIStyle();
                OnStyle.normal.background = OnTexture;
                OnStyle.onNormal.background = OnTexture;
                OnStyle.active.background = OnPressTexture;
                OnStyle.onActive.background = OnPressTexture;
                OnStyle.normal.textColor = Color.white;
                OnStyle.onNormal.textColor = Color.white;
                OnStyle.active.textColor = Color.white;
                OnStyle.onActive.textColor = Color.white;
                OnStyle.fontSize = 18;
                OnStyle.fontStyle = FontStyle.Normal;
                OnStyle.alignment = TextAnchor.MiddleCenter;
            }

            if (BtnStyle == null)
            {
                BtnStyle = new GUIStyle();
                BtnStyle.normal.background = BtnTexture;
                BtnStyle.onNormal.background = BtnTexture;
                BtnStyle.active.background = BtnPressTexture;
                BtnStyle.onActive.background = BtnPressTexture;
                BtnStyle.normal.textColor = Color.white;
                BtnStyle.onNormal.textColor = Color.white;
                BtnStyle.active.textColor = Color.white;
                BtnStyle.onActive.textColor = Color.white;
                BtnStyle.fontSize = 18;
                BtnStyle.fontStyle = FontStyle.Normal;
                BtnStyle.alignment = TextAnchor.MiddleCenter;
            }
        }

        /// Textures
        // Use material colors and convert hex code to rbg https://www.materialpalette.com/colors
        public Texture2D BtnTexture
        {
            get
            {
                if (btntexture == null)
                {
                    btntexture = NewTexture2D;
                    btntexture.SetPixel(0, 0, new Color32(106, 27, 154, 255));
                    btntexture.Apply();
                }
                return btntexture;
            }
        }

        public Texture2D BtnPressTexture
        {
            get
            {
                if (btnpresstexture == null)
                {
                    btnpresstexture = NewTexture2D;
                    btnpresstexture.SetPixel(0, 0, new Color32(123, 31, 162, 255));
                    btnpresstexture.Apply();
                }
                return btnpresstexture;
            }
        }

        public Texture2D OnPressTexture
        {
            get
            {
                if (onpresstexture == null)
                {
                    onpresstexture = NewTexture2D;
                    onpresstexture.SetPixel(0, 0, new Color32(56, 142, 60, 255));
                    onpresstexture.Apply();
                }
                return onpresstexture;
            }
        }

        public Texture2D OnTexture
        {
            get
            {
                if (ontexture == null)
                {
                    ontexture = NewTexture2D;
                    ontexture.SetPixel(0, 0, new Color32(76, 175, 80, 255));
                    ontexture.Apply();
                }
                return ontexture;
            }
        }

        public Texture2D OffPressTexture
        {
            get
            {
                if (offpresstexture == null)
                {
                    offpresstexture = NewTexture2D;
                    offpresstexture.SetPixel(0, 0, new Color32(211, 47, 47, 255));
                    offpresstexture.Apply();
                }
                return offpresstexture;
            }
        }

        public Texture2D OffTexture
        {
            get
            {
                if (offtexture == null)
                {
                    offtexture = NewTexture2D;
                    offtexture.SetPixel(0, 0, new Color32(244, 67, 54, 255));
                    offtexture.Apply();
                }
                return offtexture;
            }
        }

        public Texture2D BackTexture
        {
            get
            {
                if (backtexture == null)
                {
                    backtexture = NewTexture2D;
                    backtexture.SetPixel(0, 0, new Color32(42, 42, 42, 200));
                    backtexture.Apply();
                }
                return backtexture;
            }
        }
    }
}
