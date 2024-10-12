using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloboWeb.Console;

public static class Message
{
    public static void Write(string? message, bool clear = false, bool enter = true)
    {
        if(clear) System.Console.Clear();

        message = message ?? string.Empty;
        if(enter) System.Console.WriteLine(message);
        else System.Console.Write(message);
    }

    public static void BlankLine()
    {
        System.Console.WriteLine();
    }
}
