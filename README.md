# Clinic-Automation-System
This is .NET based Web Application using ASP.NET (MVC) framework

 CAS system (henceforth called system) is a .NET based web application and it provides an important customer interface to the Clinic operations. Prior to this system a lot of processes we handled manually and consumed a lot of time and resources. Based on the requirements this system has bee designed to enable users to have easy access to the common information about the clinic. Using this, users will now be able to search for more information on the prescribed drugs. Patients will be able to interact with their physicians directly via an inbuilt messaging system. The system also has the provision for physicians and salespersons to order for more drugs directly with the suppliers. Suppliers in turn can view the orders placed in the system and approve it as well.
In short it supports a part of the information infrastructure and workflow management to support the day to day activities of a clinic. Based on the various surveys conducted prior to using this system, it will now allow a majority of information to be available for patients online thus reducing the intake of telephonic queries at the clinic. It also forms a common platform for Physicians, Patients, Suppliers and Salesperson to perform their tasks efficiently and faster.


The following major flows defined in the Clinic Automation System are listed below.
    Create / Modify / Delete - Patient (admin users)
    Create / Modify / Delete - Physician (admin users)
    Create / Modify / Delete - Supplier (admin users)
    Create / Modify / Delete - Salesperson (admin users)
    Edit Profile (Patient)
    Create / View  Patient History (Physician)
    Place a purchase order (Salesmen / Physician)
    View Pending orders (Supplier)
    Search Product (Guest)
    View Product Inventory (Guest)
    Compose New Message (Patient / Physician / Supplier / Salesman).
    View Message Inbox (Patient / Physician / Supplier / Salesman)
    
    
It shall be used for centralized Session Management, User authentication, authorization and logging.
The admin user is the only user who will have the ability to create new users for the system. Users will be provided with the login credentials in order to access this system. This process is offline and not in scope for the system. Once a user logs into the system, the system will validate the user against the information already in the database. Once validated, the user enters the system and will view the appropriate left navigation based on the above user type. 
An error message will appear if the validations fail during login. If a user accesses any page which is not meant for his/her role type, an appropriate error message will be displayed. Exception handling will be used to throw appropriate error messages to the user. 
For logging exceptions and application information, create a static logger class, which will handle all the Log writing functionalities.
Note: The inbuilt Login controls provided by .NET can also be used.    
