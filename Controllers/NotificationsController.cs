using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace server.Controllers;

public class NotificationsController : Controller
{
    #region Private Members
    
    /// <summary>
    /// Shows if there are new notifications available
    /// </summary>
    private static bool NewNotifications = false;

    #endregion

    #region Public Properties

    /// <summary>
    /// Contains the new notifications that the user has not retrieved yet
    /// </summary>
    public static List<Notification> Notifications{get; set;} = new List<Notification>();
    
    #endregion

    #region Public Methods
    
    public static void AddNotification(string message, NotificationType type){
        // Add the notification to the list of notifications
        Notifications.Add(new Notification(message, type));

        // Set new notifications to true
        NewNotifications = true;
    }
    
    #endregion


    [Route("/GetNewNotifications")]
    public IActionResult GetNewNotifications()
    {
        // If we have no new notifications
        if (!NewNotifications)
            // Return nothing
            return Ok(new {empty = true});
            
        // Otherwise...
        // Set new notifications to false
        NewNotifications = false;
        
        // Get the current notifications
        var newNotifications = Notifications;

        // Remove all the existing notifications
        Notifications = new List<Notification>();

        // Return the new notifications
        return Ok(new {empty = false, notifications = newNotifications});
    }

}
