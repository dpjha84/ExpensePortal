# ExpensePortal
1. Create a Database in SQL Server with name "ExpenseDB". Let all the settings be default.
2. Unzip the solution zip and goto ExpensePortal\ExpensePortal\Web.Config
3. Look for string "Data Source=" and "data source=". You should see 2 matching entries.
4. Change the data source value to your SQL Server instance name. Currently "DESKTOP-O80CRIA" is my SQL Server instance name.
5. You need to convert Data Source=DESKTOP-O80CRIA to Data Source=<YourSQLServerInstanceName>
6. Similarly you have to do for data source=DESKTOP-O80CRIA to data dource=<YourSQLServerInstanceName>
7. Open ExpensePortal.sln from ExpensePortal folder and build the solution.
7. Once build is successful, you can host application in IIS or directly run from IIS express.
8. For quick testing we will use IIS express, simpally do "View in Browser" on project from Visual Studio or do "F5" from Visual Studio.
9. Once UI is loaded in browser, before navigating anywhere in UI, Go to SQL Server Management Studio, navigate to ExpenseDB DB and verify you have following tables
   in ExpenseDb created - AspNetRoles, AspNetUserClaims, AspNetUserLogins, AspNetUserRoles, AspNetUsers.
10. Execute "CreateDbObjects.sql" script present in ExpensePortal folder on ExpenseDB DB.
10.Refresh UI and go to Register link in top right.
11. Create a user of role Employee and region Asia.
12. Create a user of role Team Lead and region Asia.
13. Create a user of role Employee and region Europe.
14. Create a user of role Team Lead and region Europe.
15. Now login with Asian Employee user and submit an expense.
16. Now login with Asian team lead user in another browser and you will see an expense for review.
17. Note at the same time if you login with Europian team lead in another browser, you will not see anything to review.
18. A team lead can only review the expense which is raised from an employee from his/her region.
19. While reviewing an expense, team lead can go to view details, check receipt and then take action of Approve or reject. He can also put a comment while approving or rejecting an expense.
20. Once expense is reviewed (approved or rejected), expense will be updated in the UI session of employee who had raised that expense.
