# Background

## About Workflows and Business Process Automation

A WorkFlow is like a kind of program or sequence of steps or phases, which is repeated regularly in a business environment, and in which people and / or software agents participate. There are several types of workflow, which I will talk about later.

Another possible definition of WorkFlow (more formal) is that it is a coordinated sequence of tasks in which different processing entities participate. These processing entities can be at different layers or levels within the system architecture, so they can be background workers, APIs, etc.

Each WorkFlow task runs in response to a WorkFlow managed business event. These events can be generated (triggered) by humans or by other processing entities. These events circulate or rather, flow, in the form of messages managed by a messaging engine (message broker). 

### APIs and/or Microservices Are Not Enough

Rest APIs provide only endpoints for users; they are not intended to carry out long processes.

The http requests that we send and receive from an API must be short (in time), a maximum of seconds, otherwise we will find ourselves faced with a huge nonsense: what is the point of launching an http request and having to wait minutes, even hours,  for that request to end?.

So we shouldn't try to get an API to do all the work. In these cases you have to resort to the __Background Workers__. In other words: the API will pass all the heavy lifting to a __background worker__; when it receives the job, it returns a __menssage__ of the type __"WORK_UNIT_STARTED"__ (or something similar) to the API. When the job is finished, the workflow will issue another message or notification to inform the API of this circumstance, with a message of the type __"WORK_UNIT_PROCESS_COMPLETED"__ or __"WORK_UNIT_PROCESS_FAILED"__, if an error has occurred during the process.

By the way, to implement this message flow we will need a __message broker__ like __RabbitMq__. Thus, both the API and the Backgrund Worker must be subscribed to such message broker.

In my opinion, if you are interested in Microservices architecture, you must first master the programming of operating system services or workers. Microservices and workers must work together and must communicate with each other through a __message broker__, as I have said before.

Many think only of APIs when they refer to microservices, but the truth is that a microservice can be several things: an API, an operating system service or (Daemon in Linux), a console application, etc.


### Tipical Workflow Processes

- __Documental Approval / Human Aproval Process:__ Common business processes that require someone to sign off on the data at certain stage. An approval workflow is a logical sequence of tasks, including human approval and rejections, to process data.
- __Data Processing:__ Data processing is a series of operations that are carried out on data, files and documents. These operations are traceable and can be organized sequentially or in parallel. __This project__ implement one of those. 
- __Long Running Processes:__ A persistent workflow that can can last for hours, days or months. For example, the workflow send an order to an external system and waits for a response. The durantion is less important than the fact that you potentially have to wait, which is the case in almost any situation where remote users or remote communications are involved. 

### Exsisting Solutions

To articulate the business process in a workflow environment, there are some interesting proposals out there:

- __Camunda:__  C# library. It works well with .Net Core 3.x
- __Elsa:__ C# library
- __WorkFlow Engine:__ C# Library. It works well with .Net Core 3.x
- __Hangfire:__ Is all about executing jobs on servers (shceduled jobs). Has a web-based dashbord. It can run on a server.
- __Microsoft Dynamics 365 Supply Chain Management:__ Difficult to learn; high learning curve, and you have to pay a subscription fee every month for it.

#### Problem with existing solutions

Must of them are great. But the problem is you have to choose one... and to be able to choose one properly, you may have to test all of them, and to do that you are going to need to learn about them. The learning curve will high, so the whole process will take some time eventually. 

If you're in a rush, which happens very often in software development, and if you are an experienced coder, probably you will provide yourself a solution from your own industry. 

WorkFlows are a crucial part of the business process. So you have to be careful about what dependencias you are going to stabish on the company codebase. So you're probably going to want to partner with a company that will assure you of long-term support. Many small independent developers are brilliant, but they can't guarantee that.

# About this project

This project implements a WorkFlow system that can execute business processes of various types and purposes and is mainly based on services / background workers for Windows 10 or Linux. It is developed with Visual Studio 2019 using the C # programming language. It is expected that its final version will allow end users to fully define any business process (WorkFlow) without having to resort to the development team.

In the current state of the project, each workflow is made up of an indeterminate number of services, which are responsible for executing each phase or state of the workflow to which they belong. These elements are called "Workflow nodes" in the system domain vocabulary.

To facilitate communication between the calling application and the workflow, each workflow has its own API. There are other software components involved at that level, which are described in the "System Architecture" section of this document.

An example workflow is included that categorizes images in order to illustrate its operation with a practical case. Over time, other workflow nodes of general utility will be added that can be reused in a multitude of workflows.

The type of WorkFlow that this project implements is a __Data WorkFlow__ (see "Tipical WorkFlow Processes" section for more details).

## Project Domain Vocabulary

- __WorkFlow Node__: In this project domain, a workflow node is just a Windows Service with a particular structure and behaviour. We can also call them as __WorkFlow States__.
- __WorkFlow__: A series a __WorkFlow States__ working together, organised in a sequence which is intended to achieve a final result.
- __Work Order__: Text file containing a data structure in json format which describes a piece or sa sets of pieces of work or __tasks__ to be executed by a __state__ (service). 
- __Activities__: Concrete actions execute inside a state. Each activity performs a different task, such as running a script, sending notifications, or requesting approvals. Activities can succeed or fail dependendng of the result of a task execution, which can result in actions performed by other activities. 
- __States__: Workflow states are used for dividing workflows into smaller stages. In the domain in this project, a state is a windows service.  States represent stages in the lifecycle of a business process and can be named with any word that makes sense to you and your organization. Typical examples include Dispatched, Approved, Rejected, Completed, Confirmed, Pending, etc. So they are __verbs in past tense__.
- __Transitions__: The movement of a task from one state to another state is called a state transition. The number of states a task can move to, from a particular state is called possible state transitions. Transitions define the processing path of the workflow, depending on conditions defined in each activity. All conditions in an activity must have a transition and all transitions must have a connection to another activity or to the End activity. Transitions are used in complex workflows. They involves some sort of decision mechanism.
- __Commands__: The commands are used to __move__ an Item (task) from one state to another. When I said "move" I meant that can be a coceptual move, not necessarily a phisical move. You can think of commands as links between states. Take all the states we already saw (_verbs in past tense__), turn them into simple present verbs, and you have all the possible commands of a given workflow. In programming terms, Commands are functions which can potentially send a state ina form of message to a mesage broker queue. 
- __Tasks__: Task is simple and atomic unit of work. Examples of task are running a script, sending notifications, or requesting approvals, etc. You can conceptually and programatically divide a __Work Order__ into tasks.
- __Actors__: Users and groups of users (roles). A work order is usually associated to an actor.
- __Decision Tree__: According to wikipedia, a decision tree is *"a flowchart-like structure in which each internal node represents a "test" on an attribute (eg whether a coin flip comes up heads or tails), each branch represents the outcome of the test, and each leaf node represents a class label (decision taken after computing all attributes). The paths from root to leaf represent classification rules"*. You can think of workflows as a sort of decision trees.

*REMARK: Perhaps you like to have a look on the theory of [State Machine WorkFlows](https://docs.microsoft.com/en-us/dotnet/framework/windows-workflow-foundation/state-machine-workflows) article from Microsoft.*

*REMARK: In this early stages of the project development, I'm not going to implement all of these concepts, but is good to start getting familiar with them from now on. Any contribution or ammendment in the conceptual layer of this project will be appreciated.*

## System Architecture

In this project, each workflow is composed by a series of services, which in the domain of this system are called __WorkFlow Nodes__. The functionality of each one of these nodes is very atomic and isolated, so a workflow node is reusable; such particular functionality can be useful in another WorkFlow.

Each WorkFlow node can be moved from one given location within the WorkFlow chain to another. This is possible because all of the WorkFlow nodes have exactly the same structure and behavior. A node that currently occupies the eighth place in the chain, can moved to the first place, and everything will continue to work perfectly without the need to make changes to any nodes' code.

Each WorkFlow node is nothing more and nothing less than a Windows 10 service (or a Linux Daemon). Visual Studio 2019 offers a project template to create these services in DotNet Core 3.x (worker service template).

### The Visual Studio Solution projects

1. __C4ImagingNetCore.Notifier.Logging__: Obstraction layer to decouple logging provider
2. __C4ImagingNetCore__:  [removal inminent] All the project functionality will moved to "Helpers" project.
3. __C4ImagingNetCore.BackEnd__: [removal inminent] All the project functionality will moved to "Helpers" project.
4. __C4ImagingNetCore.Helpers__: Cotains a few helper classes and enums.
5. __C4ImagingNetCore.UI__:[removal inminent] Console app for manual testing in the early stages of the project
6. __C4ImagingNetCore.Workflow.Srv__: The first WorkFlow Node I've created.

*__REMARKS__: A better project naming convention must be applied (domain-based also). This list will be updated periodically.*

### System Components Summary

Below you can see the system components list and the the current development status of each of them: 

- [ ] __WorkFlow__: A data structure / entity (Json) that hold a collection of workflow nodes and other related properties, which lives into the WorkFlow API domain, and is managed by it.
- [X] __WorkFlow Node__: Windows Service with a particular structure and behavior. It is the main component of the Workflow.
- [ ] __WorkFlow Node Installer (Windows)__: Allows end users to Install WorkFlows locally. Includes desktop win UI.
- [ ] __WorkFlow Watcher__: Monitors the all the WorkFlow nodes and sends status messages to a message broker queue.
- [ ] __WorkFlow Manager__: Installs, starts, stops, remove and organises WorkFlow Nodes.
- [ ] __WorkFlow Scheduler__: Sets execution time for work orders / processes.
- [ ] __WorkFlow API__: API Rest that allow applications to use a given workflow. Each WorkFlow API is associated with one and just one WorkFlow.
- [ ] __WorkFlow Node Task PlugIn__: A binary module that can be dinamically loaded at runtime by a given service. The plugIn has just one public method that executes a single task asyncronously. Is used to modiffy the behavior of a service. This a key system software component that makes the whole system versatile and flexible. Please have a loock at [this issue](/../../issues/2).
- [ ] __WorkFlow Node Message Broker PlugIn__: Allows a WorkFlow Node to send status messages to a message broker queue. 
- [ ] __Work Order__: Text file containing a data structure in json format which describes a piece of work or job to be executed by a service. These work orders are produced by the WorkFlow api when the calling app send a work request. Then, the WorkFlow api delivers the work order to the service by writing the work order file into the service InBox. 
- [ ] __Work Flow Monitor UI__: 
    - [ ] __Windows Desktop application (Winforms/WPF)__: Allows final users to monitor a single workflow or a set of them in some deplyment scenarios (see issues / ehacements section for more datails about this component). Please have a loock at [this issue](/../../issues/6).
    - [ ] __Console Application (Win/Linux)__: Text-based version of the aforementioned Windows Desktop App.

*__REMARKS__: This list will be updated periodically, until the backend architecture will get fully defined and stable. Please keep into account that because the system's building process is still in the architecture design level, that all these components are still subjected to analisys and appraisal. So some of them can ptentially get removed, renamed or redefined. Also, new componenets can be added.*

### Basic Operation

The workflow services work with files, that is, they accept files as input and produce files as output. The files in the output are grouped or categorized in directories. That is all. Both the input and output files can be images, MS Word documents, Json documents, XML documents, datasets, etc., or a combination of all of them, depending on the use case.

Thus, and since the services work only with files, they monitor one or more directories in particular, each service monitors the output directory or directories of its predecessor in the WorkFlow chain. The first service monitors the workflow's Inbox directory, which always has the same name (inbox). When the first service is executed for the first time, it creates a directory that has the same name as the service assembly, and within that directory a subdirectory called "InBox" is created.

As the WorkFlow execution progresses, these files move from one directory to another, that is, they disappear from the output directory of the previous WorkFlow service and appear in the output directory of the next WorkFlow service. In future versions of the system, this will not be always entirely true, since the system will progress in complexity and the output of a service can be redirected to another WorkFlow, if certain conditions are met. In such cases, there will be a new component in the middle of each service that will be in charge of making certain decisions and redirecting traffic accordingly.

Output directories can referrence to categories or astertions into their names. This gives an idea of the potential of the system.

#### System caracteristics

1. __WorkFlow Nodes are fault tolerant__: If the system crashes, the execution can be continued from the precise point where the system was stopped. This is so because the first thing each service does is look for files already present in its input directory to process them before continuing its normal execution. 

2. __WorkFlow Nodes has a configurable startup__: Services can be configured at install time in order to modify certain operational behaviors, by passing some arguments from the cmmand line args array. For esample, a given service can be instructed to process only certain types of files by passing as an parameter/argument the list of allowed file extensions. You can also change the default input and output directory. 

3. __WorkFlow Nodes are moveable__: Services can occupy any position within the WorkFlow's chain. The position that a service occupies does not affect its internal operation; It is not necessary to make any changes to its internal structure if such service needs to be moved to a different position than its current one within the WorkFlow.

4. __WorkFlow Nodes can be useful by themselves__: The general purpose WorkFlow nodes created in this project can be useful by themselves, without necessarily being integrated into a WorkFlow; Not only are they interesting in a miservices / monolithic architecture, but they can also be useful on a desktop computer that has Windows 10 Home Edition installed. 

5. __Services can be configured to change its behaviour__: They can be configured work with single files, groups of files (in zip format) or with work Orders. The default input an output directories can be configured too in order to facilitate its linking to other WorkFlows. Also, the accepted file type into the service's InBox can be configured too. 

#### Operational Premises

1. __A backup copy of the input files must be made__: Some workflows can make modifications to the input files. So as norm, and in order to be able reverse or cancel the process, backup copies of the input files must be made before the service process begins if the service is going to make changes on those files. 

2. __Each workflow must have its own API__: In the scenario that I am trying to describe, each workflow is made up of a series of Background Workers. The workflow has an API that allows applications to interact with it. Such API does more than being the workflow entry point, it also manages other things. Perhaps the API can implement an endpoint called "ProcessStatus" that accepts the __token__ of a process as a parameter. I will describe the API WorkFlow structure later. So yes, to allow the calling aplication to interact with a given workflow, and in order to simplify the things, such a workflow must have an API on its side. Otherwise the calling application will must to know a lot of internal details about every single workflow node (or service). So through encapsualation concept, all those details will remain hidden for the calling application.

3. __No database engine in services level__: At the service level there is no database engine directly involved. WorkFlow Nodes on this type of WorkFlow (data WorkFlow) doesn't need to use a Db for its internal functioning. But they can use indirectlya Db on its process in order to get the neccessary data to complete certain task. 

4. __Cannot install two services with the same name__: No comments. Windows will not let you to do such a thing. 

5. __Services are isolated workers__: No Message Broker in services level; services can't comunicate to each other, or to any other system. They don't have external dependencies to carry out their main (and unique) task.

6. __WokrFlow Nodes does just one thing and does it well__: The WokrFlow Nodes (services) only process files to parform a concrete task on them, and keep an operational log during their their lifes. They are not responsible for launching notifications of any kind, which is work for other components of the system, which are responsible for monitoring, configuring and managing the services of a certain WorkFlow. Using classic workflow vocabulary, we can say that workflow nodes are responsible for one state; their state. 

7. __Each WorkFlow must have its own Message Queue__: Such a queue must be managed by the proper workflow components (i.e. Workflow controller or WorkFlow observer).

8. __Every WorkFlow node can be installed independently__: Every WorkFlow node must have its own and idependent installer. This policy allows any user to be able to install locally any WorkFlow Node. The insllation proceess must be __extremely simple__.

### Some Deployment Configurations

The system components can be deployed __partially or totally__ in different ways to fulfill different users and business scenarios needs. Please have a loock at [this issue](/../../issues/7).

## Final Goals

1. The final system must be able to futfill all the possible configurations (see "Some Deployment Configurations" section for more details), and be effcient in all of the possible use cases and scenarios. 
2. A series o general purpose and always-useful WorkFlow Nodes will be constructed (on the main brnach of the project), for demonstration purpouses and to enrich the system. The list of those must grow over time. The most of them will futfill different general pupopurpose needs in the daily basis on any company, in a way that the project will gain value over time.
3. All the WorkFlow Nodes must have its own release at GitHub, so the final user can install them independently.
4. Kubernetes scripts and Docker images must be available to carry out any deployment mentioned on the point #1. 
5. Some Kubernetes scripts and Dcoker Images can be automatically generated by the corresponding [*undefined*] tool.

## Next Steps

Perhaps in the medium term, there will be __a single API for all WorkFlows__. How is this possible? well, a new abstraction layer will be added in a way that a WorkFlow will become a data structure (probably using the __json__ format). Thus, a single main API can handle multiple WorkFlows. 

That circunstance leads us to the possibility that will be the user who'll define the contents of such a structure through the aforementioned API, which is the same as saying that the users can define their own WorkFlows without any futher intervention from the software development team... Great.

Whichever direction the architecture takes, what is clear now is that some general purpose WorkFlow Nodes can be developed right now. Below I'll give you some ideas to develop some useful WorkFlow nodes (services).









