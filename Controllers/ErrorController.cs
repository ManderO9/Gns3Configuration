using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;

namespace server.Controllers;

public class ErrorController : Controller
{

    /// <summary>
    /// Show the error page to the user
    /// </summary>
    /// <returns></returns>
     [Route("error/{message?}")]
    public IActionResult Error(string message)
    {
        // Set the error message to show to the user
        ViewBag.errorMessage = message;
        
        // Return the error page
        return View();
    }


}
