using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WishList.Data;
using WishList.Models;

namespace WishList.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ItemController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            string currentLogedInUserId = "";
            var logedInUser = _userManager.GetUserAsync(HttpContext.User).Result;
            if (logedInUser != null)
            {
                currentLogedInUserId = _userManager.GetUserAsync(HttpContext.User).Result.Id;
            }
            var model = _context.Items.Where(i => i.User.Id == currentLogedInUserId).ToList();

            return View("Index", model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("Create");
        }

        [HttpPost]
        public IActionResult Create(Models.Item item)
        {
            var logedInUser = _userManager.GetUserAsync(HttpContext.User).Result;
            if (logedInUser != null)
            {
                item.User = logedInUser;
            }
            _context.Items.Add(item);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;
            var item = _context.Items.FirstOrDefault(e => e.Id == id);
            if (item.User.Id != user.Id)
                return Unauthorized();
            _context.Items.Remove(item);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
