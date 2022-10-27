using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace C_2.Harmony;

internal class Program
{
    const uint DelimitationSize = 50;
    const uint MinDelimitationSize = 2;
    const uint numberOfCalls = 1000000;

    static void Main(string[] args)
    {
        var c = new MethodsToPatch();
        var stopWatch = new Stopwatch();

        TryPatch();
        stopWatch.Start();


#if TESTTRANSPILER_RELEASE || TESTTRANSPILER_DEBUG
        TestReturnNumber1(c);
#endif

#if TESTPREFIX_RELEASE || TESTPREFIX_DEBUG
        TestReturnNumber2(c);
#endif

        stopWatch.Stop();
        WrithConsoleDelimitation("END TEST");
        TimeSpan ts = stopWatch.Elapsed;
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
        Console.WriteLine("RunTime " + elapsedTime);

    }

    static void TestReturnNumber1(MethodsToPatch methods) //Transpiler
    {
        for (uint i = 0; i < numberOfCalls; i++)
        {
            methods.ReturnNumber1();
        }
    }

    static void TestReturnNumber2(MethodsToPatch methods) //Prefix
    {
        for (uint i = 0; i < numberOfCalls; i++)
        {
            methods.ReturnNumber2();
        }
    }

    static void TryPatch()
    {
        try
        {
            var instance = new HarmonyLib.Harmony("C_2.Harmony");
            instance.PatchAll();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }


#region utils

    public static void WrithConsoleDelimitation(string message, char delimitation = '-')
    {
        const uint spaceCharForMessage = DelimitationSize - MinDelimitationSize * 2;
        if (message.Length > spaceCharForMessage)
            throw new NotImplementedException("Do it");
        var charCount = MinDelimitationSize + (spaceCharForMessage - message.Length) / 2f;
        var delChar = new string(delimitation, (int)charCount);
        var ligne = delChar + message + delChar;
        if (ligne.Length < DelimitationSize) ligne += delimitation;
        Console.WriteLine(ligne);
    }

    /// <summary>
    /// Given a lambda expression calling a method, print in console the codes intructions
    /// </summary>
    /// <param name="expression">The lambda expression using the method</param>
    public static void PrintCodesInstruction(Expression<Action> expression)
    {
        var method = SymbolExtensions.GetMethodInfo(expression);
        var codes = PatchProcessor.GetOriginalInstructions(method);

        PrintCodesInstruction(codes, method);
    }

    /// <summary>
    /// Print in console the codes intructions
    /// </summary>
    /// <param name="instructions">instruction to print</param>
    /// <param name="method">method to print</param>
    public static void PrintCodesInstruction(IEnumerable<CodeInstruction> instructions, MethodInfo method)
        => PrintCodesInstruction(instructions, method.FullDescription());

    /// <summary>
    /// Print in console the codes intructions
    /// </summary>
    /// <param name="instructions">instruction to print</param>
    /// <param name="method">Name of the method to print</param>
    public static void PrintCodesInstruction(IEnumerable<CodeInstruction> instructions, string method)
    {
        Console.WriteLine($"Code of :\n{method}");
        Console.WriteLine("{");
        foreach (var code in instructions)
            Console.WriteLine("\t" + code.ToString());
        Console.WriteLine("}");
    }

    /// <summary>
    /// Given a lambda expression calling to get the code of the designated method as list of <see cref="CodeInstruction"/>
    /// </summary>
    /// <param name="expression">The lambda expression using the method</param>
    /// <returns>The <see cref="CodeInstruction"/> list of the method</returns>
    public static List<CodeInstruction> GetCodeInstruction(Expression<Action> expression)
    {
        var method = SymbolExtensions.GetMethodInfo(expression);

        return PatchProcessor.GetOriginalInstructions(method);
    }
#endregion

}
