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

    public static void FixTrans(Human human)
    {
        if (human.gender != human.birthGender)
        {
            Plugin.Logger.LogInfo($"Fixing gender for: ID={human.humanID} [{human.gender} -> {human.birthGender}]");
            human.gender = human.birthGender;
        }
    }

    public static void FixSexuality(Human human)
    {
        human.attractedTo.Clear();
        human.homosexuality = 0f;

        switch (human.gender)
        {
            case Human.Gender.male:
                human.genderScale = 0f;
                human.attractedTo.Add(Human.Gender.female);
                break;

            case Human.Gender.female:
                human.genderScale = 1f;
                human.attractedTo.Add(Human.Gender.male);
                break;
        }
    }
}

[HarmonyPatch(typeof(Human), "SetSexualityAndGender")]
public class Human_SetSexualityAndGender
{
    public static void Postfix(Human __instance)
    {
        if (SessionData.Instance.isFloorEdit || CityConstructor.Instance.generateNew)
        {
            Plugin.FixTrans(__instance);
            Plugin.FixSexuality(__instance);
        }
    }
}

[HarmonyPatch(typeof(Human), "GenerateSuitableGenderAndSexualityForParnter")]
public class Human_GenerateSuitableGenderAndSexualityForParnter
{
    public static void Postfix(Human __instance, Citizen newPartner)
    {
        if (SessionData.Instance.isFloorEdit || CityConstructor.Instance.generateNew)
        {
            if (__instance.birthGender != newPartner.birthGender)
            {
                return;
            }
            
            switch (newPartner.birthGender)
            {
                case Human.Gender.male:
                    __instance.birthGender = Human.Gender.female;
                    break;

                case Human.Gender.female:
                    __instance.birthGender = Human.Gender.male;
                    break;
            }

            Plugin.FixTrans(__instance);
            Plugin.FixSexuality(__instance);

            Plugin.Logger.LogInfo($"Fixing homosexuality for: ID={__instance.humanID} {__instance.gender} (partner: ID={newPartner.humanID} {newPartner.gender})");
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
