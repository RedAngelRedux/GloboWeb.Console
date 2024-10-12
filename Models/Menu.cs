using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloboWeb.Console.Models;

public class Menu()
{
    public string? Title { get; set; }
    public string? Prompt { get; set; }
    public List<MenuItem>? Items { get; set; }
}
