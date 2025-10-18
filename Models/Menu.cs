using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloboWeb.Console.Models;

public class Menu()
{
    public string? MenuTitle { get; set; }
    public string? MenuPrompt { get; set; }
    public List<MenuItem>? MenuItems { get; set; }
}
