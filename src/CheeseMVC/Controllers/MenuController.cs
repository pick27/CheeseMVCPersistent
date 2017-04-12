using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CheeseMVC.Data;
using CheeseMVC.Models;
using CheeseMVC.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CheeseMVC.Controllers
{
    public class MenuController : Controller
    {
        private readonly CheeseDbContext context;

        public MenuController(CheeseDbContext dbContext)
        {
            context = dbContext;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            IList<Menu> menus = context.Menus.ToList();
            return View(menus);
        }

        public IActionResult Add()
        {
            AddMenuViewModel addMenuViewModel = new AddMenuViewModel();
            return View(addMenuViewModel);
        }

        [HttpPost]
        public IActionResult Add(AddMenuViewModel addMenuViewModel)
        {
            if(ModelState.IsValid)
            {
                Menu newMenu = new Menu
                {
                    Name = addMenuViewModel.Name
                };

                context.Menus.Add(newMenu);
                context.SaveChanges();

                return Redirect("/Menu/ViewMenu/" + newMenu.ID);
            }
            return View(addMenuViewModel);
        }

        public IActionResult ViewMenu(int id)
        {
            Menu Menu = context.Menus.Single(m => m.ID == id);
            List<CheeseMenu> items = context
                .CheeseMenus.Include(item => item.Cheeses)
                .Where(cm => cm.MenuID == id)
                .ToList();

            ViewMenuViewModel viewMenuViewModel = new ViewMenuViewModel();
            viewMenuViewModel.Menu = Menu;
            viewMenuViewModel.Items = items;

            return View(viewMenuViewModel);
        }

        [HttpGet]
        public IActionResult AddItem(int id)
        {
            Menu menu = context.Menus.Single(m => m.ID == id);

            AddMenuItemViewModel addMenuItemViewModel = new AddMenuItemViewModel(id, context.Cheeses);
            addMenuItemViewModel.Menu = menu;

            return View(addMenuItemViewModel);
        }

        [HttpPost]
        public IActionResult AddItem(AddMenuItemViewModel addMenuItemViewModel)
        {
            if (ModelState.IsValid)
            {
                int menuID = addMenuItemViewModel.MenuID;
                int cheeseID = addMenuItemViewModel.CheeseID;

                IList<CheeseMenu> existingItems = context.CheeseMenus
                        .Where(cm => cm.CheeseID == cheeseID)
                        .Where(cm => cm.MenuID == menuID).ToList();
                if (existingItems.Count == 0)
                {
                    CheeseMenu cheeseMenu = new CheeseMenu {
                        CheeseID = cheeseID,
                        MenuID = menuID
                    };
                    context.CheeseMenus.Add(cheeseMenu);
                    context.SaveChanges();
                }
                return Redirect("/Menu/ViewMenu/" + menuID);
            }
            return View(addMenuItemViewModel);
        }
    }
}
