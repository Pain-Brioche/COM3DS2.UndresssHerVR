using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Diagnostics;



// This is the major & minor version with an asterisk (*) appended to auto increment numbers.
[assembly: AssemblyVersion(COM3D2.UndresssHerVR.PluginInfo.PLUGIN_VERSION + ".*")]

// These two lines tell your plugin to not give a flying fuck about accessing private variables/classes whatever.
// It requires a publicized stubb of the library with those private objects though. 
[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]


namespace COM3D2.UndresssHerVR
{
    public static class PluginInfo
    {
        // The name of this assembly.
        public const string PLUGIN_GUID = "UndressHerVR";
        // The name of this plugin.
        public const string PLUGIN_NAME = "UndressHer VR";
        // The version of this plugin.
        public const string PLUGIN_VERSION = "2.0.0";
    }
}



namespace COM3D2.UndresssHerVR
{
    // This is the metadata set for your plugin.
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public sealed class UndresssHerVR2 : BaseUnityPlugin
    {
        public static UndresssHerVR2 Instance { get; private set; }
        internal static new ManualLogSource Logger => Instance?.logger;
        private ManualLogSource logger => base.Logger;

        private static ConfigEntry<KeyCode> modifierKey;
        private static ConfigEntry<KeyCode> keyShortcut;
        KeyboardShortcut keyboardShortcut;


        private static readonly string jsonPath = BepInEx.Paths.ConfigPath + "\\UndressHer.profiles.json";
        private List<UndressProfile> profileList = new();

        private int currentProfileIndex = 0;

        private void Awake()
        {
            Instance = this;

            // Loading from JSon
            if (File.Exists(jsonPath))
            {
                string json = File.ReadAllText(jsonPath);
                profileList = JsonConvert.DeserializeObject<List<UndressProfile>>(json);
            }
            // Create a new default JSon
            else
            {
                profileList.Add(new UndressProfile());
                File.WriteAllText(jsonPath, JsonConvert.SerializeObject(profileList, Formatting.Indented));
            }

            //config
            keyShortcut = Config.Bind("Shortcuts", "Key", KeyCode.Q, "Main shortcut");
            modifierKey = Config.Bind("Shortcuts", "Modifier", KeyCode.LeftControl, "modifier key leave empty if you don't want any");

            UpdateShortcut();

            Config.SettingChanged += UpdateShortcut;
        }

        private void UpdateShortcut(object sender = null, SettingChangedEventArgs settingChanged = null)
        {
            if (modifierKey.Value == KeyCode.None)
            {
                keyboardShortcut = new KeyboardShortcut(keyShortcut.Value);
            }
            else
            {
                KeyCode[] modifier = { modifierKey.Value };

                keyboardShortcut = new KeyboardShortcut(keyShortcut.Value, modifier);
            }
        }

        private void Update()
        {
            // VR Controllers
            /* Key description:
            GRIP                = Hand Grip (Both Oculus & Vive)
            TRIGGER             = Index Triger (Both Oculus & Vive)
            VIRTUAL_R_CLICK     = Joystick Click (Oculus) Trackpad press (Vive)
            VIRTUAL_L_CLICK     = A/X (Oculus)  ? (Vive)
            VIRTUAL_MENU        = B/Y (Oculus) Menu (Vive)
            */

            if (GameMain.Instance.VRMode)
            {
                AVRControllerButtons isVRControllerLeft = GameMain.Instance.OvrMgr.GetVRControllerButtons(true);
                AVRControllerButtons isVRControllerRight = GameMain.Instance.OvrMgr.GetVRControllerButtons(false);

                if (isVRControllerLeft.GetPress(AVRControllerButtons.BTN.GRIP) && isVRControllerLeft.GetPressDown(AVRControllerButtons.BTN.VIRTUAL_L_CLICK))
                {
                    //Logger.LogInfo("UndressHerVR Left");
                    Undress();
                }
                if (isVRControllerRight.GetPress(AVRControllerButtons.BTN.GRIP) && isVRControllerRight.GetPressDown(AVRControllerButtons.BTN.VIRTUAL_L_CLICK))
                {
                    //Logger.LogInfo("UndressHerVR Right");
                    Undress();
                }
            }

            // Keyboard
            if (keyboardShortcut.IsDown())
            {
                Undress();
            }
        }

        private void Undress()
        {
            UndressProfile undressProfile = GetNextProfile();

            if (undressProfile == null)
            {
                Dress();
                return;
            }
            Logger.LogInfo($"Using profile: {undressProfile.ProfileName}");


            int maidCount = 0;
            foreach (TBody body in GetBody())
            {
                maidCount++;

                body.m_hFoceHide[TBody.SlotID.wear] =  !undressProfile.Wear;
                body.m_hFoceHide[TBody.SlotID.skirt] =  !undressProfile.Skirt;
                body.m_hFoceHide[TBody.SlotID.onepiece] =  !undressProfile.Onepiece;
                body.m_hFoceHide[TBody.SlotID.mizugi] =  !undressProfile.Mizugi;
                body.m_hFoceHide[TBody.SlotID.bra] =  !undressProfile.Bra;
                body.m_hFoceHide[TBody.SlotID.panz] =  !undressProfile.Panz;
                body.m_hFoceHide[TBody.SlotID.stkg] =  !undressProfile.Stkg;
                body.m_hFoceHide[TBody.SlotID.shoes] =  !undressProfile.Shoes;
                body.m_hFoceHide[TBody.SlotID.headset] =  !undressProfile.Headset;
                body.m_hFoceHide[TBody.SlotID.accHat] =  !undressProfile.AccHat;
                body.m_hFoceHide[TBody.SlotID.accHead] =  !undressProfile.AccHead;
                body.m_hFoceHide[TBody.SlotID.accKubi] =  !undressProfile.AccKubi;
                body.m_hFoceHide[TBody.SlotID.accKubiwa] =  !undressProfile.AccKubiwa;
                body.m_hFoceHide[TBody.SlotID.glove] =  !undressProfile.Glove;
                body.m_hFoceHide[TBody.SlotID.accUde] =  !undressProfile.AccUde;
                body.m_hFoceHide[TBody.SlotID.accAshi] =  !undressProfile.AccAshi;
                body.m_hFoceHide[TBody.SlotID.accSenaka] =  !undressProfile.AccSenaka;
                body.m_hFoceHide[TBody.SlotID.accShippo] =  !undressProfile.AccShippo;
                body.m_hFoceHide[TBody.SlotID.accHana] =  !undressProfile.AccHana;
                body.m_hFoceHide[TBody.SlotID.megane] =  !undressProfile.Megane;

                body.FixMaskFlag();
                body.FixVisibleFlag();
            }

            Logger.LogInfo($"{maidCount} maid(s) undressed.");
        }

        private void Dress()
        {
            foreach (TBody body in GetBody())
            {
                body.SetMaskMode(TBody.MaskMode.None);                
            }
        }

        private UndressProfile GetNextProfile()
        {
            if (currentProfileIndex >= profileList.Count)
            {
                currentProfileIndex = 0;
                return null;                
            }

            UndressProfile profile = profileList[currentProfileIndex];
            currentProfileIndex++;
            return profile;
        }

        private TBody[] GetBody()
        {
            TBody[] tBody = GameMain.Instance.CharacterMgr.GetStockMaidList().Where(maid => maid.body0 != null && maid.body0.boMaid && maid.Visible).Select(maid => maid.body0).ToArray();
            return tBody;
        }
    }
}
