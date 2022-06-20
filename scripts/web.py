import sys
import telnetlib
import time

# the number of arguments
number = len(sys.argv)

# local host
HOST = sys.argv[1]

# the port at which the console for the router is listening
PORT = int(sys.argv[2])

# telnet to that console
tn = telnetlib.Telnet(HOST,PORT)

# enter new line to make sure we are not blocked by anything
tn.write(str.encode("\r\n"))

# for each command
for n in range(3,number):
	# wait a little
	time.sleep(0.05)
	# run it
	tn.write(str.encode(sys.argv[n] + "\r\n"))


# exit the main function with this function
sys.exit()
