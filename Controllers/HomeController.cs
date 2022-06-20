using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;

namespace server.Controllers;

public class HomeController : Controller
{
    #region Private Members

    ILogger<HomeController> mLogger;

    #endregion

    #region Public Properties

    /// <summary>
    /// The script to run when we want to configure a machine
    /// </summary>
    public string ScriptName { get; set; } = "web.py";

    /// <summary>
    /// The list of commands to run in the script
    /// </summary>
    public List<string> Commands { get; set; } = new List<string>();

    /// <summary>
    /// The ip address of the machine we want to run the code on
    /// </summary>
    public string HostIpAddress { get; set; } = String.Empty;

    /// <summary>
    /// The port number at which we can configure the console 
    /// </summary>
    public string PortNumber = String.Empty;

    #endregion

    #region Public Actions

    /// <summary>
    /// Show the main page to the user
    /// </summary>
    /// <returns></returns>
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// Tries to add a static route in the routers routing table
    /// </summary>
    /// <param name="hostIpAddress">The ip address of the current machine</param>
    /// <param name="port">The port to the console </param>
    /// <param name="network">The network address to route to</param>
    /// <param name="mask">The mask of the network address</param>
    /// <param name="nextHop">The ip address of the next hop router to go through to the network</param>
    /// <returns></returns>
    public IActionResult StaticRoute(string hostIpAddress, string port, string network, string mask, string nextHop)
    {
        // If we passed in an unvalid network address
        if (!IsValidIpAddress(network))
            // Return an error to the user
            return ShowError("invalid network address");

        // If the mask is invalid
        if (!IsValidIpAddress(mask))
            // Return an error to the user
            return ShowError("invalid mask");

        // If the next hop ip address is invalid
        if (!IsValidIpAddress(nextHop))
            // Return an error to the user
            return ShowError("next hop address is invalid");

        // Set the ip address of the host we want to connect to
        HostIpAddress = hostIpAddress;

        // Set the port number we want to access the console from
        PortNumber = port;

        // Get the commands to execute
        this.Commands = GenerateStaticRouteCommands(network, mask, nextHop);

        // Log it 
        mLogger.LogInformation("we are adding a static route");

        // Execute the commands
        var result = HandleCommandsExecution();

        // If execution was successful
        if (result)
        {
            // If we finished with no problems
            return new OkObjectResult(null);
        }
        // Otherwise
        else
        {
            // TODO: add error page
            return ShowError("something went wrong during execution of commands");
        }

    }

    #endregion

    #region Constructor

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="logger"></param>
    public HomeController(ILogger<HomeController> logger)
    {
        mLogger = logger;
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Handles the execution of the commands
    /// </summary>
    /// <returns>True if execution has succeeded, false otherwise</returns>
    private bool HandleCommandsExecution()
    {
        try
        {
            // Send the commands to confiugure a specific machine
            GNSConfigurationManager.RunScript(ScriptName, HostIpAddress, PortNumber, Commands);
        }
        catch (Exception ex)
        {
            // Log the error
            mLogger.LogCritical(ex.Message);

            // Return false indicating that the configuration has not succeeded
            return false;
        }

        // Return true indicating that the configuration has succeeded
        return true;
    }

    /// <summary>
    /// Returns if the passed in address is a valid ip address
    /// </summary>
    /// <param name="address">The address to check</param>
    /// <returns>True if it's valid</returns>
    private bool IsValidIpAddress(string address)
    {
        // The pattern to match an IP address    
        var Pattern = @"^([0-9]|00[0-9]|0[0-9]|0[0-9][0-9]|[0-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[0-9][0-9]|0[0-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$";

        // Regular Expression object    
        var check = new Regex(Pattern);

        // If the IP address is empty...
        if (string.IsNullOrEmpty(address))
            // Return false
            return false;

        // Otherwise...
        else
            // Return if the IP address matches the pattern
            return check.IsMatch(address, 0);
    }

    #endregion

    #region Commands Generation Helpers

    /// <summary>
    /// Generates a list of commands that will add a static route in a router
    /// </summary>
    /// <param name="network">The network address to route to</param>
    /// <param name="mask">The mask of the network address</param>
    /// <param name="nextHop">The ip address of the next hop router to go through to the network</param>
    /// <returns>The list of commands to execute</returns>
    private List<string> GenerateStaticRouteCommands(string network, string mask, string nextHop)
    {
        // Create list of commands
        var commands = new List<string>();

        // Enable console
        commands.Add("en");

        // Configure terminal
        commands.Add("conf t");

        // Add static address
        commands.Add("ip route " + network + " " + mask + " " + nextHop);

        // Exit back to enable mode
        commands.Add("exit");

        return commands;
    }

    #endregion

    #region Error Handler

    /// <summary>
    /// Shows an error to the user when something went wrong
    /// </summary>
    /// <param name="errorMessage">The error message to show</param>
    public IActionResult ShowError(string errorMessage)
    {
        return Redirect("/error/" + errorMessage);
    }

    #endregion
}
