using GloboWeb.Console.Models;
using Microsoft.Extensions.Configuration;

using GloboWeb.Utility;

namespace GloboWeb.Console;

public class AppMenu
{
    private IConfiguration _configuration;
    private Menu _menu;

    public AppMenu(IConfiguration configuration)
    {
        try
        {
            _configuration = configuration;
            //string basepath = Directory.GetParent(AppContext.BaseDirectory)!.FullName;
            //_configuration = new ConfigurationBuilder()
            //    .SetBasePath(basepath)
            //    //.AddJsonFile("appmenu.json")
            //    .Build();

            _menu = BuildMenu();
        }
        catch (Exception ex)
        {
            throw new Exception("Error creating the menu.",ex);
        }
    }

    private Menu BuildMenu()
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
    public void DisplayJsonMenu()
    {
        Message.Write(_menu.MenuTitle, true);
        Message.Write(_menu.MenuPrompt, false);
        DisplayJsonMenuItems(_menu.MenuItems!, 0);
    }

    private static void DisplayJsonMenuItems(List<MenuItem> menuItemss, int level)
    {
        foreach (var item in menuItemss)
        {
            Message.Write(new string(' ', level * 2) + $"{item.Choice}. {item.Text}");
            if (item.SubMenu != null) 
            {
                Message.Write(new string(' ', (level + 1) * 2) + item.SubMenu.MenuTitle);
                DisplayJsonMenuItems(item.SubMenu.MenuItems!, level + 1);
            }
        }
    }

    /// <summary>
    /// Displays the Sub-menu of the MenuItem with Choice = choice
    /// </summary>
    /// <param name="choice"></param>
    /// <returns>false if SubMenu was null, otherwise true</returns>
    public void DisplayMenu(Menu? menu = null)
    {
        menu ??= _menu;
        Message.Write(menu.MenuTitle, true);
        DisplayMenuItems(menu);
        Message.BlankLine();
        Message.Write(menu.MenuPrompt, false, false);
    }

    private static void DisplayMenuItems(Menu menu)
    {
        foreach (var item in menu.MenuItems!)
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

    private string? GetParentTitle(int choice, Menu? menu = null)
    {
        menu ??= _menu;

        List<MenuItem>? items = menu.MenuItems;
        if (items == null) return null;

        string? title = menu.MenuTitle;

        foreach (var item in items) {

            if (item.Choice == choice) return title;

            if(item.Action == "SubMenu" && item.SubMenu != null && item.SubMenu.MenuItems != null)
            {
                title = item.SubMenu.MenuTitle;
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
    public Menu? GetSubMenu(int choice, Menu? menu = null) 
    {
        menu ??= _menu;

        List<MenuItem>? items = menu.MenuItems;

        if (items == null) return null;

        foreach (var item in items) 
        { 
            if(item.Choice == choice) return item.SubMenu;

            if (item.Action == "SubMenu" && item.SubMenu != null && item.SubMenu.MenuItems != null)
                GetSubMenu(choice, item.SubMenu);
        }

        return null;
    }

    private MenuItem? GetMenuItemChosen(Menu? menu = null)
    {
        menu ??= _menu;

        (bool good, int choice) userInput = Prompt.GetInteger(" ");

        MenuItem? menuItemChosen = null;

        while (!userInput.good || (menu.MenuItems != null && (menuItemChosen = menu.MenuItems.FirstOrDefault(i => i.Choice == userInput.choice)) == null))
        {
            userInput = Prompt.GetInteger("That was not a valid option.  Please try again:  ");
        }

        return menuItemChosen;
    }

    public string GetChoice()
    {
        MenuItem? menuItemChosen = GetMenuItemChosen();
        string menuTitle = _menu.MenuTitle ?? string.Empty;

        while(menuItemChosen != null && menuItemChosen.SubMenu != null) 
        {
            menuTitle = menuItemChosen.SubMenu.MenuTitle ?? string.Empty;
            DisplayMenu(menuItemChosen.SubMenu);
            menuItemChosen = GetMenuItemChosen(menuItemChosen.SubMenu);
        }

        DisplayActionItem(menuItemChosen!,menuTitle);

        return menuItemChosen!.Action.ToLower();
    }
}
