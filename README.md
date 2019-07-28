# Rich Client Platform for .NET

The Rich Client Platform for .NET is similar to the NetBeans Platform and the Eclipse RCP for Java, enabling development of windows-based applications on top of the IDE containing dockable panels and documents. The platform is derived from the open-source FlashDevelop IDE for developing Flash Actionscript - http://www.flashdevelop.org.

The main characteristic of the platform is its plugin architecture which enable modularity or separation of concerns by encapsulating the code for each dockable panel and document into individual plugins within a well-defined interface. Each plugin is loaded separately at runtime and can be activated or deactivated depending on the use, allowing for great extensibility and flexibility. 

Ensure that the whole solution is built before running to load all the plugins. A sample plugin is included to demonstrate the barebone implementation of a plugin.

