<p align="center">
    <img src="./.github/logo-ordermaker.png" height="100%">
</p>

<p align="center">
  <img alt="GitHub" src="https://img.shields.io/badge/licence-MIT-green">
  <img alt="GitHub" src="https://img.shields.io/badge/platform-.Net%207.0%20%7C%20Windows%20%7C%20IIS-blue">  
  <img alt="GitHub" src="https://img.shields.io/badge/database-MySql%208.00-blue">  
</p>

<hr>

<strong>MTD OrderMaker</strong> is a simple web application that provides ability to make a knowledge base, query management system or any solutions based on forms and fields in a short time. 

MTD OrderMaker includes the Configurator module, provides administrators to create forms around which interaction functions automatically created — List (search and filtering), Editor, View and Print, and the ability to export data to Excel. Also, the user management module providers you to set the level of access rights for each user for each form. 

## Who is using 

- Freelancers - automate the business processes of their customers.
- System administrators or IT specialists of small companies handle tasks related to order management.
- Back-end programmers use the MTD OrderMaker as an intermediary to simplify the collection of data from users into their data processing systems.

## How to start
ℹ️ _This manual is intended for system administrators_

1. Download and install .Net Core 7.0 for Windows OS depending on the operating system, also download and install the Hosting Bundle.
2. Download and install MySql Community Server 8.0 ([recommendation 8.0.15](https://downloads.mysql.com/archives/get/p/23/file/mysql-8.0.15-winx64.zip)). The MySql user you will use to connect from the web application must have Database Creator permissions.  
3. Download the OrderMaker [latest release](https://github.com/olegbruev/OrderMakerServer/releases) in the publish.zip file archive.
4. Unzip it and create file appsettings.json in same folder.  Use the appsettings.Template.json file to understand what settings you need to specify.
5. Run MtdKey.OrderMaker.exe file. Then open browser and type address https://localhost:5001. Log in with the username and password that you specify in the appsettings.json file.
6. Open the Configurator and Form Template Builder. Create a form. Then use the Action button to create the Part form (section). Open the created section and then create some field using Action button.
7. Open the Users menu and Policy Template Designer. Create a policy and name it Default. Use the "Action" button and click "Select All".  Click Save button.
8. Open the "Users" menu and the "Accounts" module (list of users). Click your account name and select the Default value in the Policy field. Click the Save button.

After all this, you will open the Desktop menu item and see your first database.

<p align="center">
      <img src="./.github/desktop2.png" width="650">
</p>


**Good luck building complex applications without programming!**:crossed_fingers:

## License

The MTD OrderMaker web application is free and open-source software and starting from version 2.0 is released under the MIT license. 

## Third-party integrations

List of vendors that are not in the [Dependency graph](https://github.com/olegbruev/OrderMakerServer/network/dependencies):

| Library               | Description                                            |
| --------------------- | ------------------------------------------------------ |
| [xdan]                | DateTimePicker jQuery plugin select date and time.     |
| [moment]              | A JavaScript date library for parsing, validating, manipulating, and formatting dates. |



[xdan]:https://github.com/xdan/datetimepicker
[moment]:https://github.com/moment/moment/
