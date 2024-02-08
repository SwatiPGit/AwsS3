using ConsumeAwsS3Api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ConsumeAwsS3Api.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Route("Home/Error/{StatusCode}")]
        public IActionResult Error(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorTitle = "The requested page does not exists.";
                    break;
                default:
                    ViewBag.ErrorTitle = "Unexpected error";
                    break;
            }
            return View("Error");
        }
        [Route("Home/Exception")]
        public IActionResult Exception()
        {
            ViewBag.ErrorTitle = "Unexpected error occured.";
            return View("Error");
        }
    }
}