# Welcome

I'm really glad you're reading this because we need volunteer developers and other types of collaborators to help this project come to fruition.

I believe that any complex process, more or less long, that can be broken down into a series of steps and that must be carried out behind the scenes on a file, without compromising the usability of a given website, is liable to become one or more workflow nodes.

The project was started on September 15, 2021, and it is still very young. It does little, but it has great potential. Right now, there are only four workflow modes that categorize image files under different criteria (aspect ratio, resolution, year, and location). These workflow nodes have been created mainly as an example.

But you can create many more of them to do all kinds of things. If all goes well, this project has the prospect of massive growth; In fact, we may have to create separate Github projects to keep all workflows and workflow nodes organized in a manageable way.

With the help of people like you, this project can deliver an incredible amount of functionality over time to a large list of roles and stakeholders.

And please,

- If you're a junior developer read [this](./HI_JUNIOR_DEVELOPER.md) file
- If you're a student or self-taught individual read [this](./HI_STUDENT.md) file
- If you're an experienced developer read [this](./HI_EXPERIENCED_DEVELOPER.md) file

But regardless of who you are and what your personal circumstance is at the moment, this project will substantially increase your personal codebase and your skills' toolbox in order to be better prepared to face all kinds of situations in terms of workflows or unattended (Fire-and-Forget) jobs. 

Finally, please read our [Code of Conduct](./CODE_OF_CONDUCT.md) to keep our community approachable and respectable.

## Why to Contribute

There is often a high degree of coupling between business rules and microservices since such rules are implemented within the aforementioned microservices. For example, it is typical to see within a microservice that when a new user is registered, one more email is sent to that user immediately, and the call to the mail service is made from the same endpoint or method that is responsible for the registration of users.

In other applications, the situation may be somewhat better, and instead, the user registration endpoint can send a message to a message broker to inform another service (the email service), that a new user is just registered. 

In the two cases presented here, there is a certain degree of coupling between the business rule "send a welcome email to new users" and the microservice or application; every time such business rule changes for any reason, the code of the endpoint mentioned above will have to be modified ... and things can get worse with more complex business rules.

In my humble opinion, the responsibility for implementing business rules should rest exclusively with the workflow engine. In other words, microservices should only execute CRUD operations; where things should actually occur in the workflow engine. This means that somewhere there is an observer who is dedicated to launching events that cause the activation (directly or indirectly) of one or more workflows.

This project aims to create a robust, easily maintainable, and scalable workflow engine. In addition, it will also produce a long series of general-purpose workflows that can be very useful in a large number of scenarios.

Putting all the system workload in the operating system services is a safe bet. Windows services and windows daemons are strategic pieces of their respective operating systems and are going to be there for a long time. We can expect with confidence that the services we develop today, will be still compatible with the new operating version versions to come. 

I've been working with Windows services for many years and I can say that the services now have the same structure as they did years ago, meaning they are a very stable OS asset. Whatever you develop as an OS service is going to give many years of reliable service, and will be reusable through many programing languages and technologies for many years.

But depending of your personal circumstances you can have many other reasons to contribute. Please see the __Welcome section__ to learn more.

## New contributor guide

In this guide you will get an overview of the contribution workflow from opening an issue, creating a PR, reviewing, and merging the Pull Request (PR).

See the [README](../README.md) to get an overview of the project. See below some helpful resources to get you comfortable with the project.

### Some important resources:

- The [Welcome to C4Imaging Discussions!](/../../discussions/43) discussion, will keep you updated about any project developments and events.
- The [Conceptual Questions & Answers](/../../discussions/53) discussion, can help you to understand some basic concepts and vocabulary.
- The [Techical Questions & Answers](/../../discussions/53) discussion, can help you to solve doubts about any technical detail.
- The [Basic GitHub Commands](./docs/BASIC_GITHUB_COMMANDS.md) document, can help you to remember the most commonly used GitHub commands.
- The [Development Environment Setup](/../../docs/still_does_not_exist) document, explains the tools you???re going to need and how to get ready to code.
- The [Resources for Developers](/../../discussions/58)  discussion, will give you some useful resources and tools you can use during development.

### Skilsets

Below you can see the recommended skill sets to carry on your collaboration at different project levels.

#### Workflow Nodes Development Skill Set 
- A good level in .Net Core 3.x or newest versions and C# Language
- Experience creating Background Workers or Windows Services
- Familiarity or understanding of a number of development patterns
- A good understanding of multitasking and parallel programming. 
- GitHub basic commands familiarity
- Experience creating Background Workers or Windows Services

#### Workflow Development Skill Set 
- A good level in .Net Core 3.x or newest versions and C# Language
- Some basic experience with JavaScript and JQuery
- Experience in Web Development with .Net MVC pattern 
- Experience creating Background Workers or Windows Services
- Experience building Rest APIs with authentication and authorization by using JWT
- Experience in basic user's authentication and authorization
- The familiarity or understanding of a number of development patterns can be handy but not absolutely necessary.
- Experience with API documentation with Swagger
- Experience with most common API development tools like Postman
- A good level in SQL, particularly with MS SQL Server
- GitHub basic commands familiarity

#### Desktop client Apps
- Experience with Winforms or WPF
- GitHub basic commands familiarity
- Experience consuming Rest APIs 
- Experience creating Background Workers or Windows Services
- The familiarity or understanding of a number of development patterns can be handy but not absolutely necessary.
- GitHub basic commands familiarity

#### Desktop and Web Apps Frontend skillset
- User Experience (UX) expertise 
- Graphic Design

#### Other Skills
- Machine Learning with TensorFlow

### Coding Rules
1. We use Visual Studio (2019/2020).
2. We code in C# on .Net Core 3.x and NET 5.
3. We are committed to implement the SOLID principles.
4. Use development patterns only when is absolutely necessary. Keep it simple, especially when you are prototyping your software.
5. Use camel notation for variable names.
6. Use Pascal notation for class names.
7. Implement a clear separation of functionalities in your projects, using external classes. 
8. Take into account the reusability concept in your code.
9. If you implement a new math function, share it with other developers by placing it in the project's "Maths" library.
10. Sufficiently document your code by adding comments where necessary.
11. Produce unit tests if you can do it.
12. Be wise at naming functions, classes, variables, data entities, and API endpoints (That is the best development pattern ever).
13. We want all the projects to have the same structure and naming convention.
14. We Use the namespace and class name as the naming conventions for the system projects.
15. Add a README.md file to your project explaining what it does and how it does (with code samples)

#### Bibliography
[Best Practices for Rest Api Design](https://stackoverflow.blog/2020/03/02/best-practices-for-rest-api-design/) article from Stack Overflow

## How Can I Contribute?
You can contribute in several ways:

- [X] Sharing Your Ideas about new workflow Nodes Development
- [X] Finding New Use Cases For Workflow Nodes
- [X] Reporting Bugs
- [X] Suggesting Enhancements
- [ ] Investigating about Other C# Workflow packages
- [X] Improving the System Documentation
- [ ] Improving the System Dcouemntation Diagrams
- [ ] Creating TensorFlow Models
- [ ] Providing advice to code migration to newer C# versions 
- [ ] Providing sample data/files to help to test new workflow nodes and new system features
- [ ] Helping to create Kubernetes deployments and Github releases
- [ ] Helping to implement or update components' continuous delivery with Jenkins or similar tools
- [ ] Creating automated tests for different projects

*REMARK: The already documented __ways of contribution__ has been marked with a checkbox on the previous list.*

*REMARK: In order to facilitate some of these ways of contribution, I still need to create some new templates.*

In the below sections I will explain each of those in detail.

### Ok, But... where to start exactly?
You can start by creating workflow states' plugins. In this project, a plugin receives a single file and generates an output after it proceeds to analyze it. Such output can be another file containing a JSON structure or any other kind of data, or a label that defines a category; That's it. Although the concept is extraordinarily simple, the aforementioned process can be very complex; not always, by the way.

So you don't need to create the entire workflow node/service structure; you can go right to the point and concentrate on the functionality, in the file's processing. The project framework takes care of you about what happens before and after such a process gets executed.  

In this [document](../../PLUGINS_QUICKSTART_GUIDE.md) you can find a quickstart guide to create your first plugin.

Once you get bored with creating plugIns, you can start creating entire workflows; so, you're going to put some workflow nodes together into a chain to carry out a more complex process. A workflow is nothing more than a windows service with a particular structure and behavior, which is capable of communicating with the workflow nodes that you created before.

In this [document](../../WORKFLOWS_QUICKSTART_GUIDE.md) you can find a quick start guide to create your first workflow.

And once you have created your first workflow, you need to provide users with an entry point to use it, i.e. a rest API or a desktop app, so you can then develop one of those.

### discussions
All of these contributing ways are articulated by the GitHub discussions facility. There's a lot of them. In the below sections, you're going to know when and how to use them.

#### Discussion life cycle
All discussion has a life cycle, i.e. they start for whatever reason, they have an evolution over time, and finally, they must finish (ASAP), and they must lead us to a conclusion.

So, a discussion is unconcluded until there's a conclusion. A discussion can get closed without it meaning it was concluded. If we can find a lot of unconcluded discussions, that can mean that something is not working properly. Perhaps we are dealing with a very difficult issue that nobody can solve, indicating that we need to redirect our effort to another development area, trying to find a different approach.

The conclusion is the last entry in the discussion, and almost everybody must agree on it. Before the conclusion gets published, maybe a kind of survey must be carried out to retrieve everybody???s final opinion. 

### Sharing Your Ideas about new workflow Nodes Development
If you're on the belief that you have a great idea regarding a new workflow state, this section guides you through the process of sharing such types of ideas for the project. 

Before creating an idea report, please check this [discussion](/../../discussions/46) as you might find out that you don't need to create one. When you are creating an idea report, please include as many details as possible. Fill out the required template, the information it asks for helps us to achieve a complete understanding of your proposal.

#### Before Submitting A New Idea
[TODO]

#### How Do I Submit A New Idea Properly?
[TODO]

### Investigating about Other C# Workflow packages
[TODO]

### Finding New Use Cases For Workflow Nodes
Not only by writing code that you can contribute; you can also contribute to improving the project documentation. Perhaps you???re a final user in a company or you're involved in a workflows-related project of any kind at the moment, or you were involved in one of those in the past. So you can tell us what you think is, or was a good use case.

#### Before Submitting A New Use Case Report
- Make sure you completely understand the system and what it does. 
- Check the proper [discussion](/../../discussions/50) to make sure that a similar use case was not reported before by someone else.
- Do not get confused about a use case and a New Workflow Node Idea. The main differences between these both is that New Use Case Report is about an entire workflow and doesn't cover any technical details.

#### How Do I Submit A New Use Case Report Properly?
- A clear and descriptive title for the discussion entry to identify the New Use Case.
- Include some details about the type of company business or work environment such workflow must live in.
- A generic description of the workflow.
- State if the workflow must run at a specific periodicity (hourly, daily, weekly, monthly, etc.), or on the contrary, it is activated through a certain event or trigger.
- A detailed description of the workflow state-by-state in as many details as possible.
- Include some graphical representations (flow diagrams) to illustrate the workflow general process.
- Specific examples to illustrate each workflow state. 
- Include some details about the involved human roles.
- Include a glossary of terms from the domain of the business, corporation, agency, or work environment in which the workflow is or should be executed.
- Include screenshots to demonstrate the steps or point out the part of the system to which the suggestion is related to.
- Explain why this new use case would be useful to the system users.

### Reporting Bugs
This section guides you through submitting a bug report for the project. Following these guidelines helps maintainers and the community understand your report, reproduce the behavior, and find related reports.

Before creating bug reports, please check this list as you might find out that you don't need to create one. When you are creating a bug report, please include as many details as possible. Fill out the required template, the information it asks for helps us resolve issues faster.

Note: If you find a Closed issue that seems like it is the same thing that you're experiencing, open a new issue and include a link to the original issue in the body of your new one.

#### Before Submitting A Bug Report
- Check the debugging guide. You might be able to find the cause of the problem and fix things yourself. Most importantly, check if you can reproduce the problem in the latest version of the project.
- Check the discussions for a list of common questions and problems.
- Perform a cursory search to see if the problem has already been reported. If it has and the issue is still open, add a comment to the existing issue instead of opening a new one.

#### How Do I Submit A (Good) Bug Report?
- Bugs are tracked as GitHub issues. 
- Explain the problem and include additional details to help maintainers reproduce the problem:
    - Use a clear and descriptive title for the issue to identify the problem.
    - Describe the exact steps which reproduce the problem in as many details as possible. 
    - Provide specific examples to demonstrate the steps. Include links to files or GitHub projects.
    - Describe the behavior you observed after following the steps and point out what exactly is the problem with that behavior.
    - Explain which behavior you expected to see instead and why.
    - Include screenshots that show you following the described steps to demonstrate the problem.
- Include details about your configuration and environment by answering the following questions:
    - What's the name and version of the OS you're using?
    - Are you running the system in a virtual machine? If so, which VM software are you using?
    - Which NuGet packages do you have installed?

### Suggesting Enhancements
This section guides you through submitting an enhancement suggestion, including completely new system components and workflow nodes. Following these guidelines helps maintainers and the community understand your suggestion and find related suggestions.

Before creating enhancement suggestions, please check the proper discussion as you might find out that you don't need to create one. When you are creating an enhancement suggestion, please include as many details as possible. Fill in the template, including the steps that you imagine you would take if the feature you're requesting existed. 

- To suggest the adoption of a certain development pattern in any component through this [discussion](/../../discussions/52).
- For Workflow API Enhacements, have a look on this [discussion](/../../discussions/47).
- For Workflow Components Enhacements have a look on this [discussion](/../../discussions/49).

*REMARK: Other discussions will be added to this list in the future.*

#### Before Submitting An Enhancement Suggestion
- Check the debugging guide for tips ??? you might discover that the enhancement is already available. 
- Check if there's already a system component that provides that enhancement.
- Determine which project the enhancement should be suggested in.
- Perform a cursory search to see if the enhancement has already been suggested. If it has, adds a comment to the existing issue instead of opening a new one.

#### How Do I Submit A (Good) Enhancement Suggestion?
Enhancement suggestions are tracked as discussions. After you've determined which project your enhancement suggestion is related to, create a discussion entry on that project and provide the following information:

- A clear and descriptive title for the discussion entry to identify the suggestion.
- A step-by-step description of the suggested enhancement in as many details as possible.
- State the list of necessary (new) NuGet Packages.
- Describe the current behavior and explain which behavior you expected to see instead and why.
- Include screenshots to demonstrate the steps or point out the part of the system to which the suggestion is related.
- Explain why this enhancement would be useful to the system users.

### Improving the System Documentation
[TODO]

### Your First Code Contribution
Unsure where to begin contributing? You can start by looking through these beginner and help-wanted issues:

- Beginner issues - issues that should only require a few lines of code, and a test or two.
- Help wanted issues - issues that should be a bit more involved than beginner issues.

#### If you are still unsure about how to collaborate
You have an idea that is complex but at the same time infeasible, unstructured... If that is the case, please have a look on the [Wishful thinking?](/../../discussions/55) discussion.

*REMARK: Both issue lists are sorted by a total number of comments. While not perfect, a number of comments is a reasonable proxy for the impact a given change will have.*


## Pull Requests
The process described here has several goals:

- Maintain system's quality
- Fix problems that are important to users
- Engage the community in working toward the best possible Data Workflows
- Enable a sustainable system for system's maintainers to review contributions

Please follow these steps to have your contribution considered by the maintainers:

- Follow all instructions in the template
- Follow the style guides
- After you submit your pull request, verify that all status checks are passing
- What if the status checks are failing?
- While the prerequisites above must be satisfied prior to your pull request being reviewed, the reviewer(s) may ask you to complete additional design work, tests, or other changes before your pull request can be ultimately accepted.

### Pull Request Procedure
When you're done making the changes, open a pull request, often referred to as a PR, and:

- Fill out the "Ready for review" template so we can review your PR. This template helps reviewers understand your changes and the purpose of your pull request.
- Don't forget to link your PR to the issue if you are solving one.
- Enable the checkbox to allow maintainer edits so the branch can be updated for a merge. Once you submit your PR, a Docs team member will review your proposal. We may ask questions or request additional information.
- We may ask for changes to be made before a PR can be merged, either using suggested changes or pull request comments. You can apply suggested changes directly through the UI. You can make any other changes in your fork, then commit them to your branch.
- As you update your PR and apply changes, mark each conversation as resolved.