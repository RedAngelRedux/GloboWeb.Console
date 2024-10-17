using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GloboWeb.Utility;

namespace GloboWeb.Console;

public static class Prompt
{
    private static bool DisplayPrompt(string prompt,bool enter, bool clear)
    {
        //if (string.IsNullOrWhiteSpace(prompt)) return false;

        if (clear) System.Console.Clear();

        if (enter) System.Console.WriteLine(prompt);
        else System.Console.Write(prompt);
         
        return true;
    }

    public static string GetString(string prompt, bool enter = false, bool clear = false)
    {
        string? userInput = null;

        if(DisplayPrompt(prompt,enter,clear)) userInput = System.Console.ReadLine();

        return userInput ?? string.Empty;
    }

    public static (bool good, int integer) GetInteger(string prompt, bool enter = false, bool clear = false)
    {
        if(DisplayPrompt(prompt,enter,clear))  return GloboWeb.Utility.Validator.ParseInteger(System.Console.ReadLine());

        return (false, -1);
    }

    public static void Pause(
        string prompt = "Press any key to continue...", 
        bool enter = false, 
        bool clear = false)
    {
        DisplayPrompt(prompt,enter,clear);
        System.Console.ReadKey();
    }
}