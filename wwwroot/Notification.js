



// Listen for page loading
document.addEventListener("DOMContentLoaded", function () {

    // After that the page loads

    // Create a timer to check if there are new notifications
    var myIntervale = setInterval(getNotifications, 1000);

    // Timer to remove existing notifications after a certain period of time
    var removeNotificationsTimer = setInterval(removeNotifications, 3000);

    // Get the list of notifications container
    var notificationsContainer = document.getElementsByClassName("NotificationsContainer")[0];

    // Removes old notifications from the list of notifications in the UI
    function removeNotifications() {

        // If there are no notifications 
        if (notificationsContainer.children.length == 0)
            // Don't do anything
            return;

        // Get the first element from the notifications container
        var firstElement = notificationsContainer.children.item(0);

        // Make it disappear
        firstElement.classList.remove("appear");
        firstElement.classList.add("disappear");

        // After delay, remove it from the container
        fireAndForget(() => { notificationsContainer.removeChild(firstElement) }, 520)

    }

    // Gets notifications from the server and shows them to the user
    function getNotifications() {

        // Get the list of new notifications
        fetch("https://localhost:5005/GetNewNotifications")
            .then(async function (response) {

                // Get the response data
                var data = await response.json();

                // If there was no new notifications
                if (data.empty)
                    // Don't do anything
                    return;

                // Get list of new notifications
                var newNotifs = data.notifications;

                var delay = 1;

                // Foreach new notification
                newNotifs.forEach(notification => {
                    // Get the type of the notification
                    var type = notification.type;

                    // Get the message of the notification
                    var message = notification.message;

                    // Increment the delay between messages
                    delay++;

                    // Show the notification to the user
                    fireAndForget(() => { ShowNotification(message, type) }, delay * 100);
                    console.log(delay)
                });


            })
    }

    // Shows a notification to the user
    function ShowNotification(message, type) {

        // The class to add to the notification to syle it
        var classType = "";

        // Switch the type of the notification
        switch (type) {
            case 0:
                // Set class as Command
                classType = "Command";
                // Return
                break;
            case 1:
                // Set class as Error
                classType = "Error";
                // Return
                break;
            case 2:
                // Set class as none
                classType = "";
                // Return
                break;
            default:
                // No type specified, an error has happened
                console.log("no type for the notification has been specified");
                break;
        }

        // Create a new notifications element
        var newNotification = document.createElement("div");

        // Add the message to it
        newNotification.innerHTML = message;

        // Add the style of the specific type of notification
        newNotification.classList.add(classType)

        // Add it to the notifications container
        notificationsContainer.appendChild(newNotification);

        // After delay, show it
        fireAndForget(() => { newNotification.classList.add("appear"); }, 100)

    }

    // Runs a function after a fiven time in milliseconds   
    function fireAndForget(callBack, time) {
        // Create an interval 
        var inter = setInterval(function () {

            // Run the function afer the delay
            callBack();

            // Clear the interval
            clearInterval(inter);
        }, time);
    }

});
