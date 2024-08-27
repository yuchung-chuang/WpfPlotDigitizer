# WpfPlotDigitizer 

A simple, powerful application that helps you digitize data from images.

![releases](https://img.shields.io/github/release-pre/alex1392/WpfPlotDigitizer.svg?style=flat) 
![platform](https://img.shields.io/badge/platform-windows-informational.svg) 
![.net](https://img.shields.io/badge/.net->=4.6.1-informational.svg) 
![lanugage](https://img.shields.io/badge/C%23-7.0-informational.svg)
![IDE](https://img.shields.io/badge/IDE-vs2017-informational.svg)

[![Build status](https://ci.appveyor.com/api/projects/status/github/alex1392/WpfPlotDigitizer?branch=master&svg=true)](https://ci.appveyor.com/project/alex1392/wpfplotdigitizer)

![license](https://img.shields.io/github/license/alex1392/WpfPlotDigitizer.svg?style=flat)

<p align="center"> 
  <img src="https://raw.githubusercontent.com/alex1392/WpfPlotDigitizer/master/WpfPlotDigitizer/app/resources/icon_digitizer.ico" alt="icon" width="200"/> 
</p>

## Features

* **Detect axes automatically**

Let the program detect axes automagically by double click on the image.
You can also manually adjust the location of axes.

<p align="center"> 
<img src="https://raw.githubusercontent.com/alex1392/WpfPlotDigitizer/master/DemoResources/AxisPageDemo.gif" alt="Auto Get Axis" width="400" />
</p>
  

* **Color filtering**

Considering there may be multiple data types in different colors, it could be useful to filter out the data you don't need.

<p align="center">
<img src="https://raw.githubusercontent.com/alex1392/WpfPlotDigitizer/master/DemoResources/FilterPageDemo.gif" alt="Filter Image" width="400">
</p>
  

*  **Image editing**

You can directly erase any noise or data you don't need.
Undo/redo function are provided for your convenience.

<p align="center">
<img src="https://raw.githubusercontent.com/alex1392/WpfPlotDigitizer/master/DemoResources/ErasePageDemo.gif" alt="Erase Image" width="400" />
</p>


* **Data size adapting**
 
Considering the size of data point may be different in each image, you could manually adjust the data size to ensure the digitized data is consistent with the original data.

<p align="center">
<img src="https://raw.githubusercontent.com/alex1392/WpfPlotDigitizer/master/DemoResources/DataPageDemo.gif" alt="Adjust Data" width="400"/>
</p>


*  **Tutorial mode** 

Every pages and controls in the program are provided with tooltips.
You can also toggle the tutorial by pressing "?" button in the top-right corner.

<p align="center">
<img src="https://raw.githubusercontent.com/alex1392/WpfPlotDigitizer/master/DemoResources/TutorialDemo.gif" alt="Tutorial" width="400"/>
</p>

* **Supporting Chinese and English** 

The program can automatically determine the language from your location, nevertheless, you could also change the language by pressing the "文A" button in the top-right corner.

<p align="center">
<img src="https://raw.githubusercontent.com/alex1392/WpfPlotDigitizer/master/DemoResources/LangDemo.gif" alt="Language" width="400"/>
</p>

*  **Supporting multiple file types**

You can output .xlsx, .csv, or .txt files.

<p align="center">
<img src="https://raw.githubusercontent.com/alex1392/WpfPlotDigitizer/master/DemoResources/SavePageDemo.gif" alt="Output" width="400"/>
</p>

  
* **Good UX design**

	* The interface is designed according to Material Design guildline

	* Fluent operating experience

	* Simple work flow

<p align="center">
<img src="https://raw.githubusercontent.com/alex1392/WpfPlotDigitizer/master/DemoResources/DemoImages.png" alt="Pages" width="400"/>
</p>

  
## Usage

No need to install, just download [WpfPlotDigitizer.rar](https://github.com/alex1392/WpfPlotDigitizer/releases) from release page, unzip the file, and hit WpfPlotDigitizer.exe :)

## Supported Environments

Windows 10

Windows XP, 7, 8 (Need .Net Framework 4.6.1 or above)

## Tech

WpfPlotDigitizer uses a number of open source projects:

<img src="https://avatars.githubusercontent.com/u/16655083?s=200&v=4" alt="Material Design In Xaml Toolkit Icon" width="50"/> <img src="https://user-images.githubusercontent.com/658431/30968270-0e3a855e-a45f-11e7-862b-8d92ebd301ad.png" alt="MahApps Icon" width="50"/> <img src="https://avatars2.githubusercontent.com/u/2035816?s=460&v=4" alt="EmguCV Icon" width="50"/> <img src="https://avatars3.githubusercontent.com/u/3250496?s=200&v=4" alt="Fody Icon" width="50"/> 
<img src="https://avatars3.githubusercontent.com/u/8432466?s=200&v=4" alt="OxyPlot Icon" width="50"/> 

## Author : C. Y. C.

<img  src="https://raw.githubusercontent.com/alex1392/WpfPlotDigitizer/master/WpfPlotDigitizer/app/resources/icon_cyc.png"  alt="cyc icon"  width="200"/>


License
----

MIT
