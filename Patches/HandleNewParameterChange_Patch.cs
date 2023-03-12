using HarmonyLib;
using Kitchen;
using KitchenData;
using System;
using System.Reflection;

namespace KitchenCustomDifficulty.Patches
{
    [HarmonyPatch]
    static class HandleNewParameterChange_Patch
    {
        static MethodBase TargetMethod()
        {
            Type type = AccessTools.FirstInner(typeof(HandleNewParameterChange), t => t.Name.Contains("c__DisplayClass_OnUpdate_LambdaJob0"));
            return AccessTools.FirstMethod(type, method => method.Name.Contains("OriginalLambdaBody"));
        }

        static void Postfix(CNewParameterChange change)
        {
            if (!GameData.Main.TryGet<UnlockCard>(change.ID, out var output))
            {
                return;
            }
            if (output.Effects.Count <= change.Index)
            {
                return;
            }
            if (!(output.Effects[change.Index] is ParameterEffect parameterEffect))
            {
                return;
            }
            GroupSizeController.HandleVanillaParameterChange(parameterEffect);
            Main.LogInfo("HandleParamChange");
        }
    }
}
