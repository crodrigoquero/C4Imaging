# Acerca de este servicio

Este servicio categoriza imagenes por aspect ratio. Una vez instalado y en funcionamiento,
cada vez que se cree o se mueva una imagen a su directorio de trabajo (InBox) desde otro directorio, esta sera categorizada de forma inmediata por Aspect Ratio. El servicio crea una carpeta para cada uno de estos ratios y va colocando en cada una de ellas las imagines que se depositan en el mencioando directorio de trabajo.

En esta version, el directorio de trabajo o InBox es C:\Temp\workerservice. Este directorio puedeser configurado al instalar el servicio en Windows.

# Que tiene de especial este servicio?

Este servicio es le primero de una larga serie de servicios que tengo pensado desarrollar. En su estado actual puede ser empleado como plantilla de proyecto Visual Studio 2019 para crear otros servicios. 

Su estructura y comportamiento interno permite que una serie de servicios como este puedan
ser concatenados para crear un workflow o proceso mucho mas complejo. No sera necesario aplicar cambios en cada servicio de la cadena, ya que la estructura provista por este servicio se adaptara a todas las situaciones, tanto si el servicio en cuestion esta al principio, en medio o al final de la cadena.

El servicio lleva a cabo una accion muy concreta: categrozacion de imagenes por ratio de aspecto, nada especial. La accion principal se ejecuta dentro de una funcion asincrona (ProcessFileAsync). Pero en versiones futuras, tal accion podra ser sustituida por otra a traves de plugins, lo cual quiere decir que se podra modificar el comportamiento, la mision y la utilidad del servicio de forma dinamica a traves de una api, sin ncesidad de tener que instalar el servicio de nuevo. 

Tal caracterisica simplificara el proceso de desarrollo de cualquier workFlow, por extenso o complejo que sea. Permitira tambien a los usuarios poder configurar un workFlow segun sus necesidades concretas mediante una interfaz de usuario basada en web. Esto ultimo implica la generacion y compilacion dinamica de codigo, y una vez llegados a ese punto, puedo intuir que se podran hacer cosas verdaderamente interesantes. 



## Publicar el servicio

1- En visual studio 2019, seleccione la opccion "Publish" del menu "Build".
2- Selccione la opccion pulicar en una carpeta.
3- Seleccione la ubicacion de la carpeta. 
4- Pulse el boton "Publish"

Visual studio generara los binarios del servicio necesarios para su instalacion en windows.
Los comandos de isntalacion deben apuntar al directorio en el que residen los archivos 
generados durante este proceso de publicacion.

Es recomendable dedicar una carpeta especial para paquetes de sofrtware y dentro de esta, crear una carpeta para los servicios.

La notacion empleada para asignar nombres a los serrvicios es muy importante.

Por ejemplo se puede emplear la siguiente estructura:

- [Nombre del compania]
	- Aplicaciones
		- Servicios
			- [nombre del servicio]


## Instalar el servicio

sc.exe create C4Monitor binpath= "C:\Temp\workerservice\C4imagingNetCore.Workflow.Srv.exe C:\Temp\workerservice\WorkerInbox C:\Temp\workerservice\WorkerOutbox"

## Arrancar el servicio

start-service C4Monitor

## Parar el servicio

stop-service C4Monitor

## Borrar el servicio

sc.exe delete C4Monitor

## Configuar el servicio para auto-arranque automatico despues de error

Este servicio debe ser configurado para que arranque automaticamente tras un error de ejecucion. El servicio fallara cada vez que encuentre una nueva categoria de imagen. Esto es absolutamente normal.

Este comportamiento ha sido establecido intencionalmente de forma que cada vez que se encuentreuna nueva categoria el servicio vuelva a arrancar para poder asi establecer los watchers para cada nueva categroia.

El mencionado comportamiento debe ser establecido al isntalar el servicio en windows, mediante el siguiente comando:

SC.exe failure C4Monitor reset= 86400 actions= restart/1000/restart/1000/restart/1000

Como se puede ver, el comando establece el comportamiento del servicio ante los fallos. El
servicio volvera arrancar de nuevo inmediatamente tras porducirse un error de ejecucion.


## Asignar una categoria al servicio

sc.exe config c4Monitor group= workflows


# Considreaciones acerca de los nombres del los servicios

Sea cuidadoso en la asignacion de nombres de los servicios, procure que el nombre del servicio sea consistente 
con los otros servicios existnetetes usando notacion de dominio:

[nombre compania].wf.srv.[categoria].[funcionalidad/nombre servicio]

o bien:

Workflows.srv.[categoria].[funcionalidad/nombre servicio]

Ejemplos:

miCompania.wf.srv.img.categorization.byAspectRatio

workFlow.srv.img.categorization.byAspectRatio

Y asi, subsiguientes servicios prodrian ser nombrados como:

workFlow.srv.img.categorization.bySizeGroups

workFlow.srv.img.categorization.byCountry


