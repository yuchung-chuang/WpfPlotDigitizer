# PlotDigitizer 

A simple, powerful application that helps you digitize data from images.

![releases](https://img.shields.io/badge/release-v2.0-blue) 
![platform](https://img.shields.io/badge/platform-Windows11-blue) 
![.net](https://img.shields.io/badge/.net-8.0-blue) 
![lanugage](https://img.shields.io/badge/C%23-12-blue)

[![Build status](https://ci.appveyor.com/api/projects/status/github/alex1392/WpfPlotDigitizer?branch=master&svg=true)](https://ci.appveyor.com/project/alex1392/wpfplotdigitizer)

![license](https://img.shields.io/badge/license-MIT-blue)

<p align="center"> 
  <img src="https://raw.githubusercontent.com/alex1392/WpfPlotDigitizer/master/WpfPlotDigitizer/app/resources/icon_digitizer.ico" alt="icon" width="200"/> 
</p>

## Features

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
	<img src="https://github.com/yuchung-chuang/WpfPlotDigitizer/blob/v2/demoResources/load%20page.png?raw=true" alt="Load Image" width="400" />
	</p>

* **Auto detect chart axes**

	The program is able to detect the chart axes from the image automatically.
	You can also manually adjust the location of the axes.

	<p align="center"> 
	<img src="https://github.com/yuchung-chuang/WpfPlotDigitizer/blob/v2/demoResources/axis%20page.png?raw=true" alt="Auto Get Axis" width="400" />
	</p>

* **Auto detect axis limits**

	Incorporate optical character recognition (OCR) to detect the axis limits and labels automatically. 
	You can also manually edit the axis limits and labels.
	Log-scale axis is supported as well.

	<p align="center"> 
	<img src="https://github.com/yuchung-chuang/WpfPlotDigitizer/blob/v2/demoResources/range%20page2.png?raw=true" alt="Auto Get Axis" width="400" />
	</p>

* **Data filter by colors**

	Considering there may be multiple data types in different colors, it could be useful to filter out the data you don't need.

	<p align="center">
	<img src="https://github.com/yuchung-chuang/WpfPlotDigitizer/blob/v2/demoResources/filter%20page.png?raw=true" alt="Filter Image" width="400">
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
	<img src="https://github.com/yuchung-chuang/WpfPlotDigitizer/blob/v2/demoResources/edit%20page.png?raw=true" alt="Erase Image" width="400" />
	</p>


* **Adaptive data**

	Supports digitizing both discrete data and continuous data.
	
    * Continuous data: suitable for line chart or clustered data.
	<p align="center">
	<img src="https://github.com/yuchung-chuang/WpfPlotDigitizer/blob/v2/demoResources/data%20page.png?raw=true" alt="Adjust Data" width="400"/>
	</p>

	* Discrete data: suitable for isolated data points, the centroid of each data point is captured. 
	<p align="center">
	<img src="https://github.com/yuchung-chuang/WpfPlotDigitizer/blob/v2/demoResources/discrete%20data.png?raw=true" alt="Adjust Data" width="400"/>
	</p>

*  **Multiple export types**

	Supports export to a .csv or .txt file. The .csv file can be converted to .xlsx file by Excel.
  
* **Good UX design**

	* Modern user interface

	* Fluent operating experience

	* Simple work flow

  
## Install

Windows installer can be found in [releases page](https://github.com/alex1392/WpfPlotDigitizer/releases).
Portable version (no need to install, just an .exe file) is also provided.

## Supported Environments

Windows 10, 11

Legacy Windows (XP, 7, 8) requires .NET 8.0 runtime installed.

## Tech

PlotDigitizer uses a number of open source projects:

<img src="https://wpfui.lepo.co/images/wpfui.png" alt="WPF-UI Icon" width="50" /> <img src="https://avatars2.githubusercontent.com/u/2035816?s=460&v=4" alt="EmguCV Icon" width="50"/> <img src="https://avatars3.githubusercontent.com/u/3250496?s=200&v=4" alt="Fody Icon" width="50"/> 

## Author : C. Y. C.

<img  src="https://raw.githubusercontent.com/alex1392/WpfPlotDigitizer/master/WpfPlotDigitizer/app/resources/icon_cyc.png"  alt="cyc icon"  width="200"/>


License
----

MIT
