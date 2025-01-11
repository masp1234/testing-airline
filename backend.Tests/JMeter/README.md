## Installation of JMeter

###### For Windows

https://tejaksha-k.medium.com/a-step-by-step-guide-how-to-install-apache-jmeter-on-macos-6a9eb8bf3463

##### For Mac 

https://tejaksha-k.medium.com/a-step-by-step-guide-how-to-install-apache-jmeter-on-macos-6a9eb8bf3463

## Installing required plugins

###### Plugin Manager
Put the jmeter-plugins-manager-1.10.jar file from the lib folder into `apache-jmeter-5.*.*/lib/ext` where you saved JMeter on your computer.

Alternatively, follow the guide here
https://jmeter-plugins.org/wiki/PluginsManager/#Installation-and-Usage.

Restart JMeter to and you should see `Plugins Manager` in the `Options` if it was installed correctly.

###### Custom  Thread Groups
To download the `Custom Thread Groups` plugin, follow the steps below (after installing the Plugin Manager).
- Restart JMeter.
- Go to `Options`.
- Click on `Plugins Manager`.
- Click on hte `Available Plugins` tab.
- Search for `Custom Thread Groups`.
- Click on it, and then click `Apply Changes and Restart JMeter` located in the bottom of the window.

## Running the tests

### Required setup
Remember to start the backend and the database before running the tests.

### Using the GUI

##### For Windows

Execute the `jmeter.bat` file in the bin folder of the Jmeter directory.

##### For Mac

If you followed the installation guide from earlier, you should be able to open the GUI by running the command `jmeter`

Now you just click on the play button that says `start` when you mouse over it.

### Using the CLI

###### Add JMeter to PATH

Locate the /bin/ folder in the JMeter directory. Copy the path and add it to your `System environment variables`

If you added JMeter to the path correctly, you should be able to run the following command
`jmeter -n -t TestPlans/AirlineProject_LoadTesting-000132.jmx

