# Introduction 
Welcome to the Modern software architecture. Here you will find many, many concepts that will require knowledge.


# Getting Started
I truly recommend you read the following articles and understand the concepts behide them.

- [Solid principles](https://dotnettutorials.net/course/solid-design-principles/)  
This principles are important to keep our code organized, clean and with a very well defined function
- [DDD - Domain Driven Design](https://www.codeproject.com/articles/1131462/domain-driven-design-my-top-best-practices)  
Domain driven design is a code design pattern that guide us on the business issues and bring their namings, concepts and problems into code
- [Hexagonal Architecture](https://medium.com/@odinnou/hexagonal-architecture-in-net-the-fastest-right-way-df93bec46bff)  
This is a modern way to create a software architecture that connects with many different ports, much more reliable than the traditional 3 layers architecture.
- [CQRS](https://martinfowler.com/bliki/CQRS.html)  
Commands changes data while queries just read them. They need to be separated in code and eventually fisicaly. But why?
- [Domain events](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-events-design-implementation)  
An event is something that has happened in the past. A domain event is, something that happened in the domain that you want other parts of the same domain (in-process) to be aware of. The notified parts usually react somehow to the events.
- [Naming conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/identifier-names)  
We don't need to spend time thinking in naming, variable names and etc. Microsoft already did and we just need to follow.
- [FluentValidations](https://docs.fluentvalidation.net/en/latest/)  
It's still under implementation, but it's good to know the concept and how to use the features
- [Result Pattern](https://www.milanjovanovic.tech/blog/functional-error-handling-in-dotnet-with-the-result-pattern)  
Stop throwing exceptions when they are not an exception! The result pattern is the right way to build a fast and stable code!
- [Fluent Results](https://github.com/altmann/FluentResults)  
This is the library used to implement the Result Pattern, add failures, validation errors and much more.
- [Git Flow](https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow)  
It's very important that you knows how to manage the code and versions the right way. How to create a branch, how to name it and manage the Pull Requests.  
![Git flow](ReadmeFiles/GitFlow.png)

# Solution Overview
This solution is designed using the Hexagonal Architecture, Domain Driven Design, Mediator Pattern, SOLID and Clean Code.
![Hexagonal Architecture](ReadmeFiles/HexagonalArchitecture.png)

In this solution we can see some folders and each one with a segregated responsibility:

![Solution overview](ReadmeFiles/SolutionOverview.png)  

- _Solution Items  
Contains files related to this solution like this readme, diagrams and other non-technical documents

- Adapters  
Following the Hexagonal Concepts, contains the many adapters we can have in the solution.

    - Services  
    The services that can be connected to the solution like SendGrid, to send emails. Sharepoint, to save and retrieve data and many others. Each one with just one single responsibility. Each folder should represent one external Service, for exemple: Workday, Salesforce, MOH, Meta, and etc. Inside each folder you may have multiple services regarding that specific service. Like: 
    -- Google (library project)
    --- Maps (class)
    --- Auth (class)
    --- Youtube (class)

    - Stream  
    This layer is used to stream data outside sending to the a Bus service. In our case, we send the data to the Azure Service Bus and if needed, we can implement another library project to stream to other platform/service.

    - Web  
    This is another adapter that is responsible for exposing and receiving data. In our case it's a functions project but can be an API, a PHP (sic!) application or something else. (But never java, please!)

- Core  
This is where the business rules are created. This is the core, the heart of this type of solution. Here we can find two projects:
    - Application  
    Responsbile for orquestrating multiple adapters and hydrating the domains.
    - Domain  
    The DDD domain concept containing the business objects, aggreates, entities and more. Here is where the decisions are made and the Domain Events produced to trigger actions in the application.

- Tests  
Where we can find the Tests projects, each one responsible for their respective layer or requirement.   





# Contribute
TODO: Explain how other users and developers can contribute to make your code better. 

If you want to learn more about creating good readme files then refer the following [guidelines](https://docs.microsoft.com/en-us/azure/devops/repos/git/create-a-readme?view=azure-devops). You can also seek inspiration from the below readme files:
- [ASP.NET Core](https://github.com/aspnet/Home)
- [Visual Studio Code](https://github.com/Microsoft/vscode)
- [Chakra Core](https://github.com/Microsoft/ChakraCore)


# Project Specific

Install the [Azure Functions Core Tools](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=windows%2Ccsharp%2Cbash) dependency using the following command line 
choco install azure-functions-core-tools

Create a new Mongo container to save the project data. Use the following command line
docker run --name OptimusAIS -p 27017:27017 -e MONGO_INITDB_ROOT_USERNAME= -e MONGO_INITDB_ROOT_PASSWORD= -d mongo


# Naming Convention
TODO: Write the naming convention details

Imperative names for functions, means that it will change something later
FSI_ChangeUserName - This function will change the user name somewhere (probably this is an HTTP function or a Service Bus function)

The topics should be named in the past, because they represent an event that happened in the past

FSI_ChangeUserName -> Topic will be FSI_UserNameChanged 

When the FSI_UserNameChanged we need to send this data to the SAP

FSI_UserNameChanged_SendToSAP

DataSentToSAP