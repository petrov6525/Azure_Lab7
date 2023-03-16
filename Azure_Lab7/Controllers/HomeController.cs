using Azure_Lab7.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Azure_Lab7.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private Address address;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }




        public async Task<IActionResult> GetAddressById(int id)
        {
            ViewBag.address=await PostgreDbService.GetAddressById(id);
            ViewBag.books = await PostgreDbService.GetAllBooks();
            ViewBag.addresses = await PostgreDbService.GetAllAddresses();

            return View("Index");
        }



        public async Task<IActionResult> AddAddress(Address address)
        {

            await PostgreDbService.TryAddAddress(address);



            return RedirectToAction("Index");
        }


        public async Task<IActionResult> AddBook(Book book)
        {
            bool isBookAdded=await PostgreDbService.TryAddBook(book);

            await Console.Out.WriteLineAsync(isBookAdded.ToString());


            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Index()
        {
            //await TryCreateTables();

            //await PostgreDbService.TryDropTable("books");

            //await PostgreDbService.TryDropTable("address");

            ViewBag.books = await PostgreDbService.GetAllBooks();
            ViewBag.addresses = await PostgreDbService.GetAllAddresses();

            return View();
        }


        private async Task TryCreateTables()
        {
            bool isCreateDB = await PostgreDbService.TryCreateTables();

            if (isCreateDB) await Console.Out.WriteLineAsync("Created");
            else await Console.Out.WriteLineAsync("False to create");
        }














        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}