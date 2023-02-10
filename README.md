# ElseFortyWebApp
Web Application developed for elseforty a unity 3D tools publisher on unity asset store .
This project is an application for managing blog posts and bug reports. It utilizes Azure CosmosDB for storing data and includes a web application for managing blog posts, as well as a bug reporting system built using durable functions.  

### Authentication:
There is currently no user authentication implemented for this project. Roles (regular client vs. admin) are determined by adding a secret query key value to the URL.  

### Data storage:
All blog posts and bugs data are stored in an Azure CosmosDB database.  

### Blog:
The blog section is entirely handled by the MVC web application logic.  

### Bug Reporting:
The bug reporting system utilizes durable functions to handle user interactions. When a client submits a new bug report, a new durable orchestration client is created to wait for an admin to mark the bug as resolved. Admins can access the resolution form from the bugs list and, once filled out and submitted, raise a "Resolved" event to the orchestrator which marks the bug as resolved. If an admin does not fill out the bug resolution within 7 days, the bug is marked with the tag "Timeout." An escalation process can be implemented in this case.  

### Diagram
![ElseForty web application schema](https://i.imgur.com/POarlwD.png) 
