using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using SOD.Common.BepInEx;
using SocietyFix;

namespace SocietyFix2;

public interface IConfigBindings {}

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Shadows of Doubt.exe")]
[BepInDependency("Venomaus.SOD.Common", (BepInDependency.DependencyFlags)1)]
public class Plugin : PluginController<Plugin, IConfigBindings>
{
    public static ManualLogSource Logger;

    public override void Load()
    {
        Logger = PluginController<Plugin, IConfigBindings>.Log;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        var harmony = new Harmony($"{MyPluginInfo.PLUGIN_GUID}");
        harmony.PatchAll();
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is patched!");
    }
}

[HarmonyPatch(typeof(Human), "SetSexualityAndGender")]
public class Human_SetSexualityAndGender
{
    public static void Postfix(Human __instance)
    {
        if (SessionData.Instance.isFloorEdit || CityConstructor.Instance.generateNew)
        {
            if (__instance.gender != __instance.birthGender)
            {
                Plugin.Logger.LogInfo($"Fixing gender for: ID={__instance.humanID} [{__instance.gender} -> {__instance.birthGender}]");
                __instance.gender = __instance.birthGender;
            }

            __instance.attractedTo.Clear();
            __instance.homosexuality = 0f;

            switch (__instance.gender)
            {
                case Human.Gender.male:
                    __instance.genderScale = 0f;
                    __instance.attractedTo.Add(Human.Gender.female);
                    break;

                case Human.Gender.female:
                    __instance.genderScale = 1f;
                    __instance.attractedTo.Add(Human.Gender.male);
                    break;
            }
        }
    }
}

[HarmonyPatch(typeof(Human), "SetFootwear")]
public class Human_SetFootwear
{
    public static void Postfix(Human __instance, Human.ShoeType newType)
    {
        if (SessionData.Instance.isFloorEdit || CityConstructor.Instance.generateNew)
        {
            if (__instance.gender == Human.Gender.male && __instance.footwear == Human.ShoeType.heel)
            {
                Plugin.Logger.LogInfo($"Fixing footwear for: ID={__instance.humanID} [{__instance.footwear} -> {Human.ShoeType.normal}]");
                __instance.footwear = Human.ShoeType.normal;
            }
        }
    }
}
