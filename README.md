# WindowsServiceControl

This app simply starts and stops a specific Windows service. The service name is included in the code for security reasons. There is no extra config file.
The app has to be run as admin, to being able to control services, when UAC is enabled.

I did not manage to start the app automatically with elevated priviledges, when using Intune.

Currently the strings are for using the app as VPN Service start and stop. They are not generic.