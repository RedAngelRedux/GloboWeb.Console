using GloboWeb.Console.Models;
using Microsoft.Extensions.Configuration;

using GloboWeb.Utility;

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
        DisplayMenuItems(menu);
        Message.BlankLine();
        Message.Write(menu.Prompt, false, false);
    }

    private static void DisplayMenuItems(Menu menu)
    {
        foreach (var item in menu.Items!)
        {
            Message.Write($"{item.Choice}. {item.Text}");
        }
    }

    private static void DisplayActionItem(MenuItem menuItem,string parentTitle)
    {
        //Message.Write(GetParentTitle(menuItem.Choice), true);
        Message.Write(parentTitle, true);
        Message.Write($"*** {menuItem.Text} ***",false);
        Message.BlankLine();
    }

    private static string? GetParentTitle(int choice, Menu? menu = null)
    {
        menu ??= _menu;

        List<MenuItem>? items = menu.Items;
        if (items == null) return null;

        string? title = menu.Title;

        foreach (var item in items) {

            if (item.Choice == choice) return title;

            if(item.Action == "SubMenu" && item.SubMenu != null && item.SubMenu.Items != null)
            {
                title = item.SubMenu.Title;
                GetParentTitle(choice, item.SubMenu);
            }
        }

        return null;
    }

    /// <summary>
    /// Returns the first Menu found in _menu whose Choice = choice
    /// </summary>
    /// <param name="choice"></param>
    /// <returns>The Menu if found, null otherwise</returns>
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

    private static MenuItem? GetMenuItemChosen(Menu? menu = null)
    {
        menu ??= _menu;

        (bool good, int choice) userInput = Prompt.GetInteger(" ");

        MenuItem? menuItemChosen = null;

        while (!userInput.good || (menu.Items != null && (menuItemChosen = menu.Items.FirstOrDefault(i => i.Choice == userInput.choice)) == null))
        {
            userInput = Prompt.GetInteger("That was not a valid option.  Please try again:  ");
        }

        return menuItemChosen;
    }

    public static string GetChoice()
    {
        MenuItem? menuItemChosen = GetMenuItemChosen();
        string menuTitle = _menu.Title ?? string.Empty;

        while(menuItemChosen != null && menuItemChosen.SubMenu != null) 
        {
            menuTitle = menuItemChosen.SubMenu.Title ?? string.Empty;
            DisplayMenu(menuItemChosen.SubMenu);
            menuItemChosen = GetMenuItemChosen(menuItemChosen.SubMenu);
        }

        DisplayActionItem(menuItemChosen!,menuTitle);

        return menuItemChosen!.Action.ToLower();
    }
}
