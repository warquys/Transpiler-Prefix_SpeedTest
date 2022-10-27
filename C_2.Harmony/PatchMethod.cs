using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace C_2.Harmony;

[HarmonyPatch]
static class PatchMethod
{

    [HarmonyTranspiler, HarmonyPatch(typeof(MethodsToPatch), nameof(MethodsToPatch.ReturnNumber1))]
    public static IEnumerable<CodeInstruction> TranspilerAdd(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var code = new Code();
        var newCodes = Program.GetCodeInstruction(() => code.NewCode());

        // Parse And Edit the newcodes
        RemoveThistagCode(newCodes);
        RemoveEndCode(newCodes); 

        newCodes.AddRange(instructions);
        instructions = newCodes;

        return instructions;
    }

    #region Transpiler Tool

    //Remove throw null
    private static void RemoveEndCode(List<CodeInstruction> codes)
    {
        var removeNext = false;
        for (int i = codes.Count - 1; i >= 0; i--)
        {
            if (removeNext)
            {
                codes.RemoveAt(i);
                removeNext = false;
                continue;
            }

            if (i < 1) return;

            var code = codes[i];
            var next = codes[i - 1];

            if (code.opcode == OpCodes.Throw && next.opcode == OpCodes.Ldnull)
            {
                codes.RemoveAt(i);
                removeNext = true;
                continue;
            }
        }
    }

    //Remove @this
    private static void RemoveThistagCode(List<CodeInstruction> codes)
    {
        for (int i = codes.Count - 1; i >= 0; i--)
        {
            var code = codes[i];
            if (code.opcode == OpCodes.Ldfld && code.operand is System.Reflection.FieldInfo rieldInfo && rieldInfo.Name == "this")
            {
                codes.RemoveAt(i);
            }
        }
    }
    #endregion

    [HarmonyPrefix, HarmonyPatch(typeof(MethodsToPatch), nameof(MethodsToPatch.ReturnNumber2))]
    public static void PatchAdd(MethodsToPatch __instance)
    {
        Console.WriteLine(__instance.j);
        __instance.j++;
    }
}

internal partial class Code : System.Dynamic.DynamicObject
{
    int j = 5;
    MethodsToPatch @this;

    [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
    public int NewCode()
    {
        Console.WriteLine(@this.j); //It is the same as: Console.WriteLine(j);
        @this.j++;                  //It is the same as: j++;
        throw null;                 //That is remove
    }

}