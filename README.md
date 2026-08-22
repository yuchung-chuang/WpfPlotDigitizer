# PlotDigitizer 

<p align="center"> 
	<img src="demoResources/icon_digitizer.png" alt="icon" width="200"/>
</p>

A simple, powerful application that helps you digitize data from images.

![platform](https://img.shields.io/badge/platform-Windows%20%7C%20Web-blue) 
[![CI](https://github.com/yuchung-chuang/PlotDigitizer/actions/workflows/ci.yml/badge.svg)](https://github.com/yuchung-chuang/PlotDigitizer/actions/workflows/ci.yml)
[![Web deployment](https://github.com/yuchung-chuang/PlotDigitizer/actions/workflows/web-cd.yml/badge.svg)](https://github.com/yuchung-chuang/PlotDigitizer/actions/workflows/web-cd.yml)
![.net](https://img.shields.io/badge/.net-8.0-blue) 
![license](https://img.shields.io/badge/license-MIT-blue)

## Install

Windows installers and portable versions (single .exe file) can be found in [releases page](https://github.com/alex1392/WpfPlotDigitizer/releases). Legacy Windows (XP, 7, 8) requires .NET 8.0 runtime installed.

Web version is available at [PlotDigitizer Web](https://plot-digitizer-g0embmcpc2deg5ah.francecentral-01.azurewebsites.net/).

There is currently no installer project in this repo (the legacy Visual Studio Installer Projects `.vdproj` was removed, since it can only be built from full Visual Studio with that extension installed). A future Windows installer should use the [WiX Toolset](https://wixtoolset.org/) instead, which ships a `dotnet`-buildable SDK-style project and produces a real `.msi` without requiring Visual Studio.

## Use cases

<p align="center"> 
<img src="demoResources/schematics.png" alt="Plot Digitizer Schematics" width="800" />
</p>

The data in journal articles or conference papers are usually published in the form of images, which creates a barrier for people who want to perform further statistical analysis. This application, Plot Digitizer, helps you extract data from any chart with ease, and includes advanced features such as OCR, editing, and filtering.

## Features

* **Auto detect chart axes**

	The program is able to detect the chart axes from the image automatically.
	You can also manually adjust the location of the axes.

	<p align="center"> 
	<img src="demoResources/axis%20page.png" alt="Auto Get Axis" width="400" />
	</p>

* **Auto detect axis limits**

	Incorporate optical character recognition (OCR) to detect the axis limits and labels automatically. 
	You can also manually edit the axis limits and labels.
	Log-scale axis is supported as well.

	<p align="center"> 
	<img src="demoResources/range%20page2.png" alt="Auto Get Axis" width="400" />
	</p>

* **Data filter by colors**

	Considering there may be multiple data types in different colors, it could be useful to filter out the data you don't need.

	<p align="center">
	<img src="demoResources/filter%20page.png" alt="Filter Image" width="400">
	</p>
	

*  **Image editing**

	You can directly erase any noise or data you don't need.

	Currently the program supports the following functionalities:
	* Undo/redo function
	* Editting history
	* Pen tool 
	* Eraser tool
	* Rectangle selection tool
	* Polygon selection tool
	* Clear border function

	<p align="center">
	<img src="demoResources/edit%20page.png" alt="Erase Image" width="400" />
	</p>


* **Adaptive data**

	Supports digitizing both discrete data and continuous data.
	
    * Continuous data: suitable for line chart or clustered data.
	<p align="center">
	<img src="demoResources/data%20page.png" alt="Adjust Data" width="400"/>
	</p>

	* Discrete data: suitable for isolated data points, the centroid of each data point is captured. 
	<p align="center">
	<img src="demoResources/discrete%20data.png" alt="Adjust Data" width="400"/>
	</p>

* **Modern UX design**

	* Modern user interface

	* Fluent operating experience

	* Simple work flow

* **Load images with ease**

	Support multiple image loading methods, including:
	* Browse 
	* Copy & paste 
	* Drag & drop

	Support multiple image sources, including:
	* local image file
	* image metadata (from clipboard or microsoft office) 
	* online image (download from the image's Url address)

	<p align="center"> 
	<img src="demoResources/load%20page.png" alt="Load Image" width="400" />
	</p>

*  **Multiple export types**

	Supports export to a .csv or .txt file. The .csv file can be converted to .xlsx file by Excel.
    
## Tech

PlotDigitizer uses a number of open source projects:

<img src="https://wpfui.lepo.co/images/wpfui.png" alt="WPF-UI Icon" width="50" /> <img src="https://avatars2.githubusercontent.com/u/2035816?s=460&v=4" alt="EmguCV Icon" width="50"/> <img src="https://avatars3.githubusercontent.com/u/3250496?s=200&v=4" alt="Fody Icon" width="50"/> 

## Author : C. Y. C.

<img  src="demoResources/icon_cyc.png"  alt="cyc icon"  width="200"/>


License
----

MIT
