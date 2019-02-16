
# <img src="https://raw.githubusercontent.com/alex1392/WpfPlotDigitizer/master/WpfPlotDigitizer/app/resources/icon_digitizer.ico" alt="icon" width="50"/> Plot Digitizer 數據擷取器

  

一個能夠數位化影像資料的簡單小程式。

  

### 使用情境

當你在熬夜趕論文報告時，突然需要對某篇paper上的資料分析，偏偏paper只附了一張如下圖般的影像：

<p align="center"> 
<img src="https://raw.githubusercontent.com/alex1392/WpfPlotDigitizer/master/Images/DemoData.png" alt="Demo Data" width="300" />
</p>

這下麻煩了，圖中數以千計的資料點，怎麼樣也沒辦法短時間內人工轉譯出座標來。更何況座標軸還是對數尺度......

  

這個時候就是使用Plot Digitizer的最佳時機，不到30秒就可以解決問題！

  

## 程式特色

* **自動抓取座標軸**

程式中點兩下左鍵即可抓取座標軸，也允許手動調整座標軸位置。

<p align="center"> 
<img src="https://raw.githubusercontent.com/alex1392/WpfPlotDigitizer/master/Images/AxisPageDemo.gif" alt="Auto Get Axis" width="300" />
</p>
  

* **內建顏色過濾器**

考慮到影像中可能有多種顏色的資料，顏色過濾器可以很方便的過濾掉不要的資料。

<p align="center">
<img src="https://raw.githubusercontent.com/alex1392/WpfPlotDigitizer/master/Images/FilterPageDemo.gif" alt="Filter Image" width="300">
</p>
  

*  **提供影像編輯功能**

內建橡皮擦工具，可以直接修掉影像中的雜訊或不要的資料。也提供復原/重做功能，方便修改。


<p align="center">
<img src="https://raw.githubusercontent.com/alex1392/WpfPlotDigitizer/master/Images/ErasePageDemo.gif" alt="Erase Image" width="300" />
</p>

  

* **可調整數據點大小**
 
考慮到每張圖的資料點大小不同，程式中可以手動將資料點調整到最適合的大小，以保證取得的資料與原始資料完全相符。

<p align="center">
<img src="https://raw.githubusercontent.com/alex1392/WpfPlotDigitizer/master/Images/DataPageDemo.gif" alt="Adjust Data" width="300"/>
</p>

  

*  **內建教學模式** 

程式中所有頁面與控制項皆有Tooltip與教學模式，方便使用者快速上手。

初次開啟程式時會自動開啟教學模式，之後可以透過右上角的「?」按鈕再次開啟教學。

<p align="center">
<img src="https://raw.githubusercontent.com/alex1392/WpfPlotDigitizer/master/Images/TutorialDemo.gif" alt="Tutorial" width="300"/>
</p>

  

* **支援中文與英文介面** 

開啟程式時會自動根據系統語言決定語言，也可以透過右上角的「文A」按鈕在執行中切換語言。

<p align="center">
<img src="https://raw.githubusercontent.com/alex1392/WpfPlotDigitizer/master/Images/LangDemo.gif" alt="Language" width="300"/>
</p>

  

*  **支援多種資料輸出格式**

  

目前可支援輸出Excel檔、.csv檔、.txt檔，並支援對數尺度的座標軸。

<p align="center">
<img src="https://raw.githubusercontent.com/alex1392/WpfPlotDigitizer/master/Images/SavePageDemo.gif" alt="Output" width="300"/>
</p>

  
* **良好的UX設計**

	* 使用Material Design精美介面

	* 流暢的操作體驗

	* 簡單的工作流程

<p align="center">
<img src="https://raw.githubusercontent.com/alex1392/WpfPlotDigitizer/master/Images/DemoImages.png" alt="Pages" width="300"/>
</p>

  

## 使用方式

  

1. 到[releases](https://github.com/alex1392/WpfPlotDigitizer/releases)頁面下載最新版的WpfPlotDigitizer.rar

2. 解壓縮

3. 雙擊exe檔

4. Wow!

  

## 適用環境

  

Windows 10

Windows XP, 7, 8 (需安裝 .Net Framework 4.6.1)

  

## 技術資源

  

本程式使用Material Design及MahApps.Metro設計畫面風格，EmguCV進行影像處理，OxyPlot繪製資料。

  

<img  src="https://raw.githubusercontent.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit/master/web/images/MD4XAML64.png"  alt="Material Design In Xaml Toolkit Icon"  width="50"/>  <img  src="https://user-images.githubusercontent.com/658431/30968270-0e3a855e-a45f-11e7-862b-8d92ebd301ad.png"  alt="MahApps Icon"  width="50"/>  <img  src="https://avatars2.githubusercontent.com/u/2035816?s=460&v=4"  alt="EmguCV Icon"  width="50"/>  <img  src="https://avatars3.githubusercontent.com/u/3250496?s=200&v=4"  alt="Fody Icon"  width="50"/>  <img  src="https://avatars3.githubusercontent.com/u/8432466?s=200&v=4"  alt="OxyPlot Icon"  width="50"/>

  

## 作者 : C. Y. C

  

<img  src="https://raw.githubusercontent.com/alex1392/WpfPlotDigitizer/master/WpfPlotDigitizer/app/resources/icon_cyc.png"  alt="cyc icon"  width="200"/>