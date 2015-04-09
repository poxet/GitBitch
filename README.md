# GitBitch
![Git bitch](https://theellipses.files.wordpress.com/2011/07/15477-18dg.jpeg)

Voice assist with an attitude.

# Project site
https://trello.com/b/onjII59D/gitbitch

# Install with chocolatey
choco install gitbitch -version 1.0.1

## Known chocolatey install issues
The chocolatey Git bitch installer requires you to have git installed. 
The git installing may hang/fail due to applications needed to be closed. Normally that pose no problem but in this case the list och applications that need top be closed are not shown to the user.
Instead you need to run the git installer without the /SILENT argument to make the GUI visible during install.

Usually the ssh-agent.exe process lock Git and cause this problem before trying anything else, kill ssh-agent with this powershell command

`Stop-Process -processname ssh-agent`

The retry the chocolatey installation
choco install gitbitch -version 1.0.1

In most cases that will fix the problem.