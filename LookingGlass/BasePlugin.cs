using BepInEx;
using LookingGlass.AutoSortItems;
using LookingGlass.BuffTimers;
using LookingGlass.CommandItemCount;
using LookingGlass.CommandWindowBlur;
using LookingGlass.DPSMeterStuff;
using LookingGlass.EquipTimerFix;
using LookingGlass.EscapeToCloseMenu;
using LookingGlass.HiddenItems;
using LookingGlass.HidePickupNotifs;
using LookingGlass.ItemCounters;
using LookingGlass.ItemStatsNameSpace;
using LookingGlass.ResizeCommandWindow;
using LookingGlass.StatsDisplay;
using RiskOfOptions;
using RoR2;
using System.Collections;
using System.IO;
using UnityEngine;

namespace LookingGlass
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class BasePlugin : BaseUnityPlugin
    {
        internal static BasePlugin instance;
        internal AutoSortItemsClass autoSortItems;
        internal NoWindowBlur noWindowBlur;
        internal ButtonsToCloseMenu buttonsToCloseMenu;
        internal HidePickupNotifications hidePickupNotifications;
        internal ItemStats itemStats;
        internal CommandItemCountClass commandItemCountClass;
        internal ModifyCommandWindow resizeCommandWindowClass;
        internal StatsDisplayClass statsDisplayClass;
        internal DPSMeter dpsMeter;
        internal ItemCounter itemCounter;
        internal BuffTimersClass buffTimers;
        internal CooldownFixer equipFixer;
        internal UnHiddenItems unHiddenItems;
        
        public static byte[] logo;
        public static Sprite logo2;
        
        public static BepInEx.PluginInfo Plugin { get; private set; }
        
        public void Awake()
        {
            Plugin = Info;
            Log.Init(Logger);
            instance = this;
            
            try
            {
                string folderName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Info.Location), "icons");
                int i = UnityEngine.Random.Range(0, Directory.GetFiles(folderName).Length);
                int i2 = 0;
                foreach (var file in Directory.GetFiles(folderName))
                {
                    if (i2 == i)
                    {
                        logo = File.ReadAllBytes(file);
                    }
                    i2++;
                }
                Texture2D t = LoadTexture(logo, 256, 256);
                logo2 = Sprite.Create(t, new Rect(0.00f, 0.00f, t.width, t.height), new Vector2(0, 0), 100, 1, SpriteMeshType.Tight, new Vector4(0, 0, 0, 0), true);
                ModSettingsManager.SetModIcon(logo2);
            }
            catch (System.Exception)
            {
            }
            
            Assets.LoadAllAssetBundles();

            autoSortItems = new AutoSortItemsClass();
            noWindowBlur = new NoWindowBlur();
            buttonsToCloseMenu = new ButtonsToCloseMenu();
            hidePickupNotifications = new HidePickupNotifications();
            commandItemCountClass = new CommandItemCountClass();
            resizeCommandWindowClass = new ModifyCommandWindow();
            statsDisplayClass = new StatsDisplayClass();
            buffTimers = new BuffTimersClass();
            dpsMeter = new DPSMeter();
            itemCounter = new ItemCounter();
            equipFixer = new CooldownFixer();
            unHiddenItems = new UnHiddenItems();
            StartCoroutine(CheckPlayerStats());
            ItemCatalog.availability.CallWhenAvailable(() =>
            {
                itemStats = new ItemStats();
            });
        }

        private void Update()
        {
            if (ButtonsToCloseMenu.buttonsToClickOnMove.Count != 0 && Input.anyKeyDown && !Input.GetMouseButtonDown(0))
            {
                ButtonsToCloseMenu.CloseMenuAfterFrame();
            }
            dpsMeter.Update();
        }
        
        IEnumerator CheckPlayerStats()
        {
            yield return new WaitForSeconds(StatsDisplayClass.statsDisplayUpdateInterval.Value);
            statsDisplayClass.CalculateStuff();
            StartCoroutine(CheckPlayerStats());
        }
        public static Texture2D LoadTexture(byte[] bytes, int width, int height)
        {
            Texture2D Tex2D = new Texture2D(width, height, TextureFormat.ARGB32, false, false);
            Tex2D.LoadImage(bytes);
            Tex2D.filterMode = FilterMode.Point;
            return Tex2D;
        }
    }
}
