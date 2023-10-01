using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRSample.Data;
using SignalRSample.Hubs;
using SignalRSample.Models;
using SignalRSample.ViewModel;
using System.Diagnostics;
using System.Security.Claims;

namespace SignalRSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHubContext<DeathlyHallowsHub> _deathlyHub;
        private readonly IHubContext<OrderHub> _orderHub;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger,
            IHubContext<DeathlyHallowsHub> deathlyHub,
            IHubContext<OrderHub> orderHub,
            ApplicationDbContext context)
        {
            _logger = logger;
            _deathlyHub = deathlyHub;
            _orderHub = orderHub;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> DeathlyHallows(string type)
        {
            if (SD.DealthyHallowRace.ContainsKey(type))
            {
                SD.DealthyHallowRace[type]++;
            }
            await _deathlyHub.Clients.All.SendAsync("updateDeathlyHallowCount",
                SD.DealthyHallowRace[SD.Cloak],
                 SD.DealthyHallowRace[SD.Stone],
                  SD.DealthyHallowRace[SD.Wand]);
            return Accepted();
        }

        public IActionResult Notification()
        {
            return View();
        }

        public IActionResult DethlyHallowRace()
        {
            return View();
        }

        public IActionResult HarryPotterHouse()
        {
            return View();
        }

        public IActionResult BasicChat()
        {
            return View();
        }
        [Authorize]
        public IActionResult Chat()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ChatVM chatVM = new ChatVM() 
            {
                UserId = userId,
                MaxRoomAllowed = 4,
                Rooms = _context.ChatRooms.ToList(),
            };
            return View(chatVM);
        }

        public IActionResult AdvancedChat()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ChatVM chatVM = new ChatVM()
            {
                UserId = userId,
                MaxRoomAllowed = 4,
                Rooms = _context.ChatRooms.ToList(),
            };
            return View(chatVM);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [ActionName("Order")]
        public  IActionResult Order()
        {
            string[] name = { "hossam", "Ben", "Jess", "Laura", "Ron" };
            string[] itemName = { "Food1", "Food2", "Food3", "Food4", "Food5" };

            Random rand = new();
            // Generate a random index less than the size of the array.  
            int index = rand.Next(name.Length);

            Order order = new()
            {
                Name = name[index],
                ItemName = itemName[index],
                Count = index
            };

            return View(order);
        }
        [ActionName("OrderList")]
        public IActionResult OrderList()
        {
            return View();
        }


        [ActionName("Order")]
        [HttpPost]
        public async Task<IActionResult> OrderPost(Order order)
        {

            _context.Orders.Add(order);
            _context.SaveChanges();
            await _orderHub.Clients.All.SendAsync("newOrder");
            return RedirectToAction(nameof(Order));
        }

        //APIs

        [HttpGet]
        public IActionResult GetAllOrder()
        {
            var productList = _context.Orders.ToList();
            return Json(new { data = productList });
        }
    }
}