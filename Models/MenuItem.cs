using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloboWeb.Console.Models;

public class MenuItem (int choice, string text, string action)
{
    public int Choice { get; set; } = choice;
    public string Text { get; set; } = text;
    public string Action { get; set; } = action;
    public Menu? SubMenu { get; set; } = null;
}
