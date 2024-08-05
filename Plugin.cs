using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Core.Logging.Interpolation;
using BepInEx.Logging;
using HarmonyLib;
using SOD.Common;
using SOD.Common.BepInEx;
using SOD.Common.Extensions;
using SOD.Common.Helpers;
using UnityEngine;
using SOD.Common.BepInEx.Configuration;
using SocietyFix;

namespace SocietyFix2;

public interface IConfigBindings
{
}

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Shadows of Doubt.exe")]
[BepInDependency("Venomaus.SOD.Common", (BepInDependency.DependencyFlags)1)]
public class Plugin : PluginController<Plugin, IConfigBindings>
{
    internal static ManualLogSource Logger;

    public override void Load()
    {
        Logger = PluginController<Plugin, IConfigBindings>.Log;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
}
