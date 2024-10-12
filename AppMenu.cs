using GloboWeb.Console.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloboWeb.Console;

public static class AppMenu
{
    private static IConfiguration _configuration;
    private static Menu _menu;

    static AppMenu()
    {
        try
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory)!.FullName)
                .AddJsonFile("appmenu.json")
                .Build();

            _menu = BuildMenu();
        }
        catch (Exception ex)
        {
            throw new Exception("Error creating the menu.",ex);
        }
    }

    private static Menu BuildMenu()
    {
        string? prompt = _configuration.GetSection("ConsoleAppMenu").GetSection("MenuPrompt").Value;
        prompt = (string.IsNullOrEmpty(prompt)) ? string.Empty : prompt;

        string? title = _configuration.GetSection("ConsoleAppMenu").GetSection("MenuTitle").Value;
        title = (string.IsNullOrEmpty(title)) ? string.Empty : title;

        _menu = new Menu();
        _configuration.GetSection("ConsoleAppMenu").Bind(_menu);
        
        return _menu;
    }

    /// <summary>
    /// Displays all menuds and sub-menus found in _menu
    /// </summary>
    public static void DisplayJsonMenu()
    {
        Message.Write(_menu.Title, true);
        Message.Write(_menu.Prompt, false);
        DisplayJsonMenuItems(_menu.Items!, 0);
    }

    private static void DisplayJsonMenuItems(List<MenuItem> menuItemss, int level)
    {
        foreach (var item in menuItemss)
        {
            Message.Write(new string(' ', level * 2) + $"{item.Choice}. {item.Text}");
            if (item.SubMenu != null) 
            {
                Message.Write(new string(' ', (level + 1) * 2) + item.SubMenu.Title);
                DisplayJsonMenuItems(item.SubMenu.Items!, level + 1);
            }
        }
    }

    /// <summary>
    /// Displays the Sub-menu of the MenuItem with Choice = choice
    /// </summary>
    /// <param name="choice"></param>
    /// <returns>false if SubMenu was null, otherwise true</returns>
    public static void DisplayMenu(Menu? menu = null)
    {
        menu ??= _menu;
        Message.Write(menu.Title, true);
        Message.Write(menu.Prompt, false);
        DisplayMenuItems(menu);
    }

    private static void DisplayMenuItems(Menu menu)
    {
        foreach (var item in menu.Items!)
        {
            Message.Write($"{item.Choice}. {item.Text}");
        }
    }

    /// <summary>
    /// Returns the first MenuItem found in _menu whose Choice = choice
    /// </summary>
    /// <param name="choice"></param>
    /// <returns>The MenuItem if found, null otherwise</returns>
    public static Menu? GetSubMenu(int choice, Menu? menu = null) 
    {
        menu ??= _menu;

        List<MenuItem>? items = menu.Items;

        if (items == null) return null;

        foreach (var item in items) 
        { 
            if(item.Choice == choice) return item.SubMenu;

            if (item.Action == "SubMenu" && item.SubMenu != null && item.SubMenu.Items != null)
                GetSubMenu(choice, item.SubMenu);
        }

        return null;
    }
}
