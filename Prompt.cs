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
    public static bool Display(string prompt,bool enter, bool clear)
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

        if(Display(prompt,enter,clear)) userInput = System.Console.ReadLine();

        return userInput ?? string.Empty;
    }

    public static (bool good, int integer) GetInteger(string prompt, bool enter = false, bool clear = false)
    {
        if(Display(prompt,enter,clear))  return GloboWeb.Utility.Validator.ParseInteger(System.Console.ReadLine());

        return (false, -1);
    }

    public static bool TryAgain(string prompt = "Try again ('Y' or 'N')?")
    {
        bool gooResponse = false;
        bool tryAgain = false;

        while(!gooResponse)
        { 
            string userInput = GetString(prompt) ?? string.Empty;
            if(userInput == "Y" || userInput ==  "y")
            {
                gooResponse = true;
                tryAgain = true;
            }
            if(userInput == "N" || userInput == "n")
            {
                gooResponse = true;
                tryAgain = false;
            }
            if (!gooResponse)
            {
                Display("Invalid choice.  Please enter a valid response.",true,false);
            }
        }

        return tryAgain;
    }

    public static void Pause(
        string prompt = "Press any key to continue...", 
        bool enter = false, 
        bool clear = false)
    {
        Display(prompt,enter,clear);
        System.Console.ReadKey();
    }
}