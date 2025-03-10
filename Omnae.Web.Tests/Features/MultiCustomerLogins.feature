Feature: Multi Customer Logins
    As a customer
    I want to manage all my corporation logins information.

Scenario: A new Customer must have a Admintration user - Self-Serve
   Given A new Customer is self creating a user in Omnae System 
	When a new user is created for this Customer and the company data
    Then this user should be a "CustomerAdmin" acount
    Then this user should be associate to this Customer

Scenario: A customer with existing user account or email domain should not be able to create duplicates
   Given A customer with existing user account associated with email address or email domain 
    When this customer attempts to create a new account
    Then The system will reject the new account creation
     And Customer will be notified that the account already exists
     And associated account admin will be notified of the atempted account creation, given option to approve user
    
Scenario: A customer with an existing company account should not be able to create duplicates
   Given A customer with existing Company account associated 
	When this customer attempts to create a new account
    Then The system will reject the new account creation
    
#Scenario: A new Customer must have a Admintration user - Admin Create
#    When A new Customer user is creat by the OnameAdmin 
#    Then this user should be a "CustomerAdmin" acount
#    Then this user should be associate to this Customer
#    
#Scenario: A CustomerAdmin can add new users for HIS customer account
#    When A customeradmin creates a new user, provides email address, first/last name, mobile number for 2FA (optional)
#    Then The user will be generated.
#    Then The user should be in the same customer account as the admin
#    Then The system will generate a temporary password and send request to new user email to change the password.
#
#Scenario: A CustomerAdmin can add new users for HIS customer account but they are disabled
#    When A customeradmin creates a new user, provides email address, first/last name, mobile number for 2FA (optional)
#     And this user is created before and is disabled to this Company
#    Then the customeradmin should be prompt to confirm that he wants to reenable this user
#    Then the user should be enabled again and his password should be reset.
#    Then The user should be in the same customer account as the admin
#    Then The system will generate a temporary password and send request to new user email to change the password.
#
#Scenario: A CustomerAdmin can add new users for HIS customer account but they are disabled (from toher company)
#    When A customeradmin creates a new user, provides email address, first/last name, mobile number for 2FA (optional)
#     And this user is created before and is enabled or disabled in another Company
#    Then the customeradmin should be prompt "This email is used by another company, please use another email for this user".
#    
#Scenario: A customeradmin wants to create many users for HIS customer account
#    When A customeradmin wants to create many users for HIS customer account
#     And A spreadsheet collects email address, first/last name, mobile number for 2FA (optional) for each user in a single row
#    Then The user will be generated.
#    Then The users should be in the same customer account as the admin
#    Then The system will generate a temporary password for each user and send request to new user emails to change the password.
#
#Scenario: A CustomerAdmin can remove users for HIS customer account
#    When A customeradmin wants to remove a user(s) account
#    Then The user should be disabled
#    Then The user should not be able to log in to the account
#    Then The user should not receive any notifications about the change of parts
#    
#Scenario: A customeradmin can update user information (name, email, phone number, force password reset)
#    When A customeradmin wishes to update user information
#    Then A user should be updated with that information
#    
#Scenario: A user wishes to update their account phone number
#    When A user wishes to update their accountphone number
#    Then The system updates the phone number
#    Then The system sends an SMS confirmation token to the user to validate
#
#Scenario: A customeradmin should be able to list all active users in HIS company
#    When A customeradmin wants to see all their users login activity
#    Then The system will display users login activity
#    
#Scenario: Taskdata relating to a suspended user must be re-assigned
#    When A user is suspended
#    Then live taskdata must be re-assigned to an active user
