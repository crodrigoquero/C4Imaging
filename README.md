## About this project

This project implements a WorkFlow system that can execute business processes of various types and purposes and is mainly based on services / background workers for Windows 10 or Linux. It is developed with Visual Studio 2019 using the C # programming language. It is expected that its final version will allow end users to fully define any business process (WorkFlow) without having to resort to the development team.

In the current state of the project, each workflow is made up of an indeterminate number of services, which are responsible for executing each phase or state of the workflow to which they belong. These elements are called "Workflow nodes" in the system domain vocabulary.

To facilitate communication between the calling application and the workflow, each workflow has its own API. There are other software components involved at that level, which are described in the "System Architecture" section of this document.

An example workflow is included that categorizes images in order to illustrate its operation with a practical case. Over time, other workflow nodes of general utility will be added that can be reused in a multitude of workflows.

The general purpose WorkFlow nodes created in this project can be useful by themselves, without necessarily being integrated into a WorkFlow; Not only are they interesting in a miservices / monolithic architecture, but they can also be useful on a desktop computer that has Windows 10 Home Edition installed. 

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

- __Ducumental Approval / Human Aproval Process:__ Common business processes that require someone to sign off on the data at certain stage. An approval workflow is a logical sequence of tasks, including human approval and rejections, to process data.
- __Data Processing:__ Data processing is a series of operations that are carroed out on data, files and documents. These operations are traceable and can be organized sequentially or in parallel. 
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

I you're in a rush, which happens very often in software development, and if you are an experienced coder, probably you will provide yourself a solution from your own industry. 

WorkFlows are a crucial part of the business process. So you have to be careful about what dependencias you are going to stabish on the company codebase. So you're probably going to want to partner with a company that will assure you of long-term support. Many small independent developers are brilliant, but they can't guarantee that.

## System Architecture

In this project, each workflow is composed by a series of services, which in the domain of this system are called __WorkFlow Nodes__. The functionality of each one of these nodes is very atomic and isolated, so a workflow node is reusable; such particular functionality can be useful in another WorkFlow.

Each WorkFlow node can be moved from one given location within the WorkFlow chain to another. This is possible because all of the WorkFlow nodes have exactly the same structure and behavior. A node that currently occupies the eighth place in the chain, can moved to the first place, and everything will continue to work perfectly without the need to make changes to any nodes' code.

Each WorkFlow node is nothing more and nothing less than a Windows 10 service (or a Linux Daemon). Visual Studio 2019 offers a project template to create these services in DotNet Core 3.x (worker service template).

### Basic Operation

Los servicios del sistema trabajan con ficheros, es decir aceptan ficheros como entrada y produce ficheros como salida. Los ficheros en la salida quedan agrupados o categorizados en directorios. Eso es todo. Tanto los ficheros de entrada como los de salida pueden ser imagenes, documentos de MS Word, documentos Json, documentos XML, etc., o una combinacion de todos ellos, dependiendo del caso de uso.

Asi pues, y dado que que los servicios trabajan solo con ficheros, estos monitorizan uno o varios directorios en particular; cada servicio monitoriza en directorio o los directorios de salida de su predecesor en la cadena del WorkFlow. El primer servicio monitoriza el directorio Inbox del workflow, que siempre tiene el mismo nombre (inbox). Cuando se ejecutada el primer servicio por primera vez este crea un directorio que tiene el mismo nombre del assembly servicio, y dentro de ese directorio se crea un subdirectorio llamado "InBox".

A medida que la ejecucion del WokrFlow avanza, esos ficheros se mueven de un directorio a otro, es decir desaparecen del directorio de salida del servicio anterior en el WorkFlow y aparecen el el directorio de salida del siguiente servicio del WorkFlow. En futuras versiones del sistema, esto ultimo no sera del todo cierto, ya que el sistema avanzara en complejidad y se puede redirigir la salida de un servicio a otro WorkFlow, si se dan determinadas condiciones. En tales casos, habra un nuevo componente en medio de cada sericio que se encargara de tomar ciertas decisiones y redirigir el trafico consecuentemente. 

Los directorios de salida pueden ser hacer referencia a categorias o afirmaciones en sus nombres. Esto da una idea del potencial del sistema.

En el nivel de los servicios no hay ningun motor de base de datos involucrado.

#### System Fault Tolerant 

Si el sistema cae, puede continuar la ejecucion desde el punto preciso en el que sistema fue detenido. Esto es asi porque lo primero que hace cada servicio es buscar buscar ya archivos presentes en su directorio de entrada para procesarlos antes de continuar su efecucion normal.  

#### Operational Premises

##### Se debe realizar una copia de respaldo de los archivos de entrada

Algunos workflows pueden realizar modificaciones en los archivos de entrada. Para poder evertir el proceso, se deben llevar a cabo copias de respaldo de los archivos de ntrada. 


##### Each workflow must have its own API

In the scenario that I am trying to describe, each workflow is made up of a series of Background Workers. The workflow has an API that allows applications to interact with it. Such API does more than being the workflow entry point, it also manages other things. Perhaps the API can implement an endpoint called "ProcessStatus" that accepts the __token__ of a process as a parameter. I will describe the API WorkFlow structure later. 

So yes, to allow the calling aplication to interact with a given workflow, and in order to simplify the things, such a workflow must have an API on its side. Otherwise the calling application will must to know a lot of internal details about every single workflow node (or service). So through encapsualation concept, all those details will remain hidden for the calling application. 

## Posible project evolution 

Perhaps in the medium term this will not continue to be the case, that is, in the future it is possible that there will be __a single API for all WorkFlows__. How is this possible? well, obviously the WorkFlow nodes will still exist, but an abstraction will be carried out such that a WorkFlow will be a data structure (probably using the __json__ format). Thus, a single main API can handle multiple WorkFlows. 

That circunstance leads us to the possibility that will be the user who'll define the contents of these structures through the aforementioned API, which is the same as saying that the users can define their own WorkFlows without the need for intervention from the software development team... Great.

Whichever direction the architecture takes, what is clear now is that some general purpose WorkFlow Nodes can be developed right now. Below I'll give you some ideas to develop some useful WorkFlow nodes (services).









