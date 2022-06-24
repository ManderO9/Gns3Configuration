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
        // If we passed in an invalid network address
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

    /// <summary>
    /// Configures an interface, gives it an ip address and turns it on
    /// </summary>
    /// <param name="hostIpAddress">The ip address of the current machine</param>
    /// <param name="port">The port to the console </param>
    /// <param name="interfaceName">The name of the interface</param>
    /// <param name="interfaceIpAddress">The ip address to give to this interface</param>
    /// <param name="interfaceMask">The mask we want to give to this interface</param>
    /// <returns></returns>
    public IActionResult ConfigureInterface(string hostIpAddress, string port, string interfaceName, string interfaceIpAddress, string interfaceMask)
    {
        // If we passed in an invalid ip address
        if (!IsValidIpAddress(interfaceIpAddress))
            // Return an error to the user
            return ShowError("invalid IP address for the interface");

        // If the mask is invalid
        if (!IsValidIpAddress(interfaceMask))
            // Return an error to the user
            return ShowError("invalid mask for the interface");

        // Set the ip address of the host we want to connect to
        HostIpAddress = hostIpAddress;

        // Set the port number we want to access the console from
        PortNumber = port;

        // Get the commands to execute
        this.Commands = GenerateInterfaceConfigurationCommands(interfaceName, interfaceIpAddress, interfaceMask);

        // Log it 
        mLogger.LogInformation("we are configuring an interface, giving it an ip address and turning it on");

        // Execute the commands
        var result = HandleCommandsExecution();

        // If execution was successful
        if (result)
        {
            // If we finished with no problems
            return Ok(null);
        }
        // Otherwise
        else
        {
            // TODO: add error page
            return ShowError("something went wrong during execution of commands");
        }

    }

    /// <summary>
    /// Adds a network to the RIP routing table and configures RIP in the current router
    /// </summary>
    /// <param name="hostIpAddress">The ip address of the current machine</param>
    /// <param name="port">The port to the console </param>
    /// <param name="ripNetwork">The network ip address to add to the RIP routing table</param>
    /// <returns></returns>
    public IActionResult AddRipNetwork(string hostIpAddress, string port, string ripNetwork)
    {
        // If we passed in an invalid network
        if (!IsValidIpAddress(ripNetwork))
            // Return an error to the user
            return ShowError("invalid IP address for the provided network");

        // Set the ip address of the host we want to connect to
        HostIpAddress = hostIpAddress;

        // Set the port number we want to access the console from
        PortNumber = port;

        // Get the commands to execute
        this.Commands = GenerateRIPConfigurationCommands(ripNetwork);

        // Log it 
        mLogger.LogInformation("we are configuring RIP, adding the network: " + ripNetwork);

        // Execute the commands
        var result = HandleCommandsExecution();

        // If execution was successful
        if (result)
        {
            // If we finished with no problems
            return Ok(null);
        }
        // Otherwise
        else
        {
            // TODO: add error page
            return ShowError("something went wrong during execution of commands");
        }

    }

    public IActionResult AddOSPFNetwork(string hostIpAddress, string port, string OSPFNetwork, string WildcardMask, string id, string area)
    {
        // If we passed in an invalid network
        if (!IsValidIpAddress(OSPFNetwork))
            // Return an error to the user
            return ShowError("invalid IP address for the provided network");

        // Set the ip address of the host we want to connect to
        HostIpAddress = hostIpAddress;

        // Set the port number we want to access the console from
        PortNumber = port;


        // Get the commands to execute
        this.Commands = GenerateOSPFConfigurationCommands(OSPFNetwork, WildcardMask, id, area);

        // Log it 
        mLogger.LogInformation("we are configuring OSPF, adding the network: " + OSPFNetwork);

        // Execute the commands
        var result = HandleCommandsExecution();

        // If execution was successful
        if (result)
        {
            // If we finished with no problems
            return Ok(null);
        }
        // Otherwise
        else
        {
            // TODO: add error page
            return ShowError("something went wrong during execution of commands");
        }

    }

    public IActionResult ConfigurePcInterface(string hostIpAddress, string port, string PcIpAddress, string GateWay)
        {
            if (!IsValidIpAddress(GateWay))
                // Return an error to the user
                return ShowError("invalid gateway");

            if (!IsValidIpAddress(PcIpAddress))
                // Return an error to the user
                return ShowError("invalid address/mask for pc");

            // Set the ip address of the host we want to connect to
            HostIpAddress = hostIpAddress;

            // Set the port number we want to access the console from
            PortNumber = port;

            // Get the commands to execute
            this.Commands = GeneratePcInterfaceConfiguration(PcIpAddress,GateWay);

            // Log it 
            mLogger.LogInformation("we are configuring Pc interface ");

            // Execute the commands
            var result = HandleCommandsExecution();

            // If execution was successful
            if (result)
            {
                // If we finished with no problems
                return Ok(null);
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
        // If we don't have a host
        if (string.IsNullOrEmpty(HostIpAddress))
        {
            // Show error to the user
            NotificationsController.AddNotification("No host entered", NotificationType.Error);

            // Return unsuccessful
            return false;
        }
        
        // If we don't have a port
        if (string.IsNullOrEmpty(PortNumber))
        {
            // Show error to the user
            NotificationsController.AddNotification("No port number entered", NotificationType.Error);

            // Return unsuccessful
            return false;
        }

        // For each command we gonna enter
        Commands.ForEach(command=>{
            // Display it to the user
            NotificationsController.AddNotification(command,NotificationType.NewCommand);
        });

        try
        {
            // Send the commands to confiugure a specific machine
            GNSConfigurationManager.RunScript(ScriptName, HostIpAddress, PortNumber, Commands);
        }
        catch (Exception ex)
        {
            // Log the error
            mLogger.LogCritical("error when running the python script: ");
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

        // Exit from config mode
        commands.Add("exit");

        // Return the commands
        return commands;
    }

    /// <summary>
    /// Generates a list of commands that will give give an interface an ip address and turns it on
    /// </summary>
    /// <param name="interfaceName">The name of the interface</param>
    /// <param name="interfaceIpAddress">The ip address to give to this interface</param>
    /// <param name="interfaceMask">The mask we want to give to this interface</param>
    /// <returns></returns>
    private List<string> GenerateInterfaceConfigurationCommands(string interfaceName, string interfaceIpAddress, string interfaceMask)
    {
        // Create list of commands
        var commands = new List<string>();

        // Enable console
        commands.Add("en");

        // Configure terminal
        commands.Add("conf t");

        // Open interface configuratin mode
        commands.Add("int " + interfaceName);

        // Give it an ip address
        commands.Add("ip address " + interfaceIpAddress + " " + interfaceMask);

        // Turn it on
        commands.Add("no shutdown");

        // Exit back to config mode
        commands.Add("exit");

        // Go back to enable mode
        commands.Add("end");

        // Return the commands
        return commands;
    }

    /// <summary>
    /// Generates a list of commands that will configure RIP in the router and add a network to it's routing table
    /// </summary>
    /// <param name="ripNetwork">The network ip address to add to the RIP routing table</param>
    /// <returns></returns>
    private List<string> GenerateRIPConfigurationCommands(string ripNetwork)
    {
        // Create list of commands
        var commands = new List<string>();

        // Enable console
        commands.Add("en");

        // Configure terminal
        commands.Add("conf t");

        // Open rip config mode
        commands.Add("router rip");

        // Add a network
        commands.Add("network " + ripNetwork);

        // Exit back to config mode
        commands.Add("exit");

        // Go back to enable mode
        commands.Add("end");

        // Return the commands
        return commands;
    }
    private List<string> GenerateOSPFConfigurationCommands(string OSPFNetwork, string WildcardMask, string id, string area)
    {
        // Create list of commands
        var commands = new List<string>();

        // Enable console
        commands.Add("en");

        // Configure terminal
        commands.Add("conf t");

        // Open ospf config mode
        commands.Add("router ospf " + id);

        // Add a network
        commands.Add("network " + OSPFNetwork + " " + WildcardMask + " area " + area);

        // Exit back to config mode
        commands.Add("exit");

        // Go back to enable mode
        commands.Add("end");

        // Return the commands
        return commands;
    }
        
    private List<string> GeneratePcInterfaceConfiguration(string PcIpAddress, string GateWay)
        {
            // Create list of commands
            var commands = new List<string>();

            // Give it an ip address
            commands.Add("ip " + PcIpAddress);

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
        // Show the error to the user
        NotificationsController.AddNotification(errorMessage, NotificationType.Error);
        
        // Return nothing
        return Ok(null);
        //return Redirect("/error/" + errorMessage);
    }

    #endregion
}
