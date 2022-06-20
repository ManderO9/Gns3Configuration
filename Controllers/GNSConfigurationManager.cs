using System.Diagnostics;

namespace server
{
    public class GNSConfigurationManager
    {
        /// <summary>
        /// The path to the directory where the python scripts exist
        /// </summary>
        public static string DirectoryPath { get; set; } = "scripts\\";

        /// <summary>
        /// Runs a python script and send some data to it as command line arguments
        /// </summary>
        /// <param name="scriptName">The name of the file containing the script to run</param>
        /// <param name="hostIpAddress">The ip address of the console of the switch/router to configure</param>
        /// <param name="port">The port number the console is listening to</param>
        /// <param name="commands">The list of commands we want our code to execute</param>
        public static void RunScript(string scriptName,string hostIpAddress, string port, List<string> commands)
        {
            // The path of the python program we want to run
            var programPath = DirectoryPath + scriptName;

            // Create the command we want to execute
            var command = "py " + programPath + " " + hostIpAddress + " " + port;

            // Add the list of commands we want to execute in our python code
            commands.ForEach(cmd =>
            {
                command += " \"" + cmd + "\"" ;
            });
            
            // Create the start info for the process
            var startInfo = new ProcessStartInfo("cmd", $"/c {command}") { CreateNoWindow = false };

            // Start the process
            Process.Start(startInfo);
            
        }
    }
}
