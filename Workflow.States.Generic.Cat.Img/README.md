# About this Workflow State

This worklow state __categorizes images by aspect ratio__. Once installed and running, every time an image is created or moved to your working directory (InBox) from another directory, it will be immediately categorized by Aspect Ratio. The worklow state creates a folder for each of these ratios and places in each one of them the images that are deposited in the mentioned working directory.

In this first version, the working directory or InBox is C:\Temp\workerservice. This directory can be configured when installing the service on Windows.

## What is special about this workflow state?
__This workflow state is the first in a long series of states that I plan to develop__. In its current version it can be used as a Visual Studio 2019 project template to create other services.

Its structure and internal behavior allow a series of workflow states like this one to be __concatenated to create a much more complex workflow or process__. It will not be necessary to apply changes to each workflow state in the chain whatever position the state occupies in the workflow satates chain, since the structure provided by this state will adapt to all situations, whether the workflow state in question was placed at the beginning, in the middle, or at the end of the chain.

This workflow state carries out a very specific action: categorization of images by aspect ratio, nothing special. The main action is executed inside an asynchronous function (ProcessFileAsync). But in future versions, such action may be replaced through __workflow node's plugins__, which means that the behavior, mission and utility of the workflow state can be dynamically modified through an api, without the need to install state again.

Such a feature will __simplify the development process of any workFlow__, no matter how extensive or complex it may be. It will also allow users to configure a workFlow according to their specific needs through a desktop or web-based user interface. The latter implies the dynamic generation and compilation of code, and once we have reached that point, I can guess that really interesting things can be done.


## Publish the workflow state

1. In visual studio 2019, select the "Publish" option from the "Build" menu.
2. Select the "Publish to folder" option.
3. Select the folder location. 
4. Press the "Publish" button.

Visual studio generara los binarios del servicio necesarios para su instalacion en windows.

Keep into account that ...
- The installation commands __must point__ to the directory in which the files generated during this publishing process reside.
- It is advisable to dedicate a __special folder__ for software packages and within this, create a folder for services.
- The __notation__ used to assign names to services is very important.

For example, the following structure can be used:

- [Company Name]
	- Applications
		- Services
			- WorkFlows
				- [Workflow #1]
				- [Workflow #2]
				- [ ... ]
				- [Workflow n]
				- Generic WorkFlow Sates
					- [Generic WorkFlow State category]
						- [WorkFlow State Name] --> Put your workflow state here!
			- [Other services (non workflows / workflow states)]


## Installation 
```ssh
sc.exe create C4Monitor binpath= "C:\Temp\workerservice\C4imagingNetCore.Workflow.Srv.exe C:\Temp\workerservice\WorkerInbox C:\Temp\workerservice\WorkerOutbox"
```
As can be seen, the name of the service and the parameters necessary for the workflow state to start (the same ones that we write on the command line when we run the service as a console application) are passed in the same string.

## To Start Up the Workflow node
```ssh
start-service C4Monitor
```
## To Stop the Workflow node
```ssh
stop-service C4Monitor
```
## Workflow node removal
```ssh
sc.exe delete C4Monitor
```
## Configure the service for automatic autostart after failure

This service must be configured to start automatically after a runtime error. The service will "crash" every time it finds a new image category. This is absolutely normal.

This behavior has been intentionally established so that each time a new category is found the service will restart in order to establish the watchers for each new category (This strategy simplifies the internal logic of the state, by taking advantage of the automatic autostart feature of Windows services).

The aforementioned behavior must be established when installing the service on windows, using the following command:
```ssh
SC.exe failure C4Monitor reset= 86400 actions= restart/1000/restart/1000/restart/1000
```
As you can see, the command establishes the behavior of the service in the event of failures. The service will start again immediately after a runtime error occurs.


## Assign "workflows" category to the Workflow State:
```ssh
sc.exe config c4Monitor group= workflows
```

# Considerations about service names

Be careful when naming the services, make sure the name of the service is consistent with the other existing services using domain notation:

**Workflows.states.Generic.[functionality group acronym].[low state name]_**

Examples:

__Workflows.states.Generic.cat.Img.ByAspectRatio__

__Workflows.states.Generic.cat.Img.ByResolution__

__Workflows.states.Generic.cat.Img.BySizeGroup__

__Workflows.states.Generic.cat.Img.ByLocation__

__Workflows.states.Generic.cat.Img.ByDateYear__

... and so on and so forth.

## The importance of properly naming things

Notice the __"Generic" word__ in the workflow state name, which indicates that this state does not satisfies any particular company or corporation requirement but can be useful everywhere. In other words; if the "generic" word was no there, it means that the workflow node has been created to fulfill a particular business or processing requirement.

If the name of the workflow state is well established, it gives us all an exact idea of what it is doing. In the previous examples, we can conclude that this state categorizes image files by aspct ratio. If instead of the __acronym "Img"__ we had placed the __acronym "Doc"__, we could know that this state can process __MS-Word documents, XML, JSON, etc.__, which gives us a fairly clear idea of what the state is. doing internally.

## Next Steps

I would like to develop some more functions before moving to version 2:

- Send images in batches using zip files for parallel processing.
- Develop other similar services to catalog images with other criteria and thus be able to generate a complete image categorization workflow as an example.
- API for workflow control.

### What the version 2 is about

- Implementation of dynamic loading of process plugins (i.e. extendable plugIns).
- Then, I'll proceed migrate the already existing workflow states to that new paradigm.
