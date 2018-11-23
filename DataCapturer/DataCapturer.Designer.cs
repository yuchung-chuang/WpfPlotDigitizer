using MetroFramework;
using MyLibrary.Classes;
using System.Drawing;

namespace DataCapturer
{
	partial class DataCapturer
	{
		/// <summary>
		/// 設計工具所需的變數。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 清除任何使用中的資源。
		/// </summary>
		/// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form 設計工具產生的程式碼

		/// <summary>
		/// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
		/// 這個方法的內容。
		/// </summary>
		private void InitializeComponent()
		{
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataCapturer));
      this.Tooltip = new MetroFramework.Components.MetroToolTip();
      this.ButtonSave = new MetroFramework.Controls.MetroButton();
      this.ButtonBrowse = new MetroFramework.Controls.MetroButton();
      this.CheckBoxYLog = new System.Windows.Forms.CheckBox();
      this.TextBoxYBase = new System.Windows.Forms.TextBox();
      this.CheckBoxXLog = new System.Windows.Forms.CheckBox();
      this.TextBoxXBase = new System.Windows.Forms.TextBox();
      this.TextBoxYlo = new System.Windows.Forms.TextBox();
      this.TextBoxYhi = new System.Windows.Forms.TextBox();
      this.TextBoxXhi = new System.Windows.Forms.TextBox();
      this.TextBoxXlo = new System.Windows.Forms.TextBox();
      this.metroLabel7 = new MetroFramework.Controls.MetroLabel();
      this.metroLabel6 = new MetroFramework.Controls.MetroLabel();
      this.metroLabel5 = new MetroFramework.Controls.MetroLabel();
      this.metroLabel4 = new MetroFramework.Controls.MetroLabel();
      this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
      this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
      this.PictureBoxGetAxis = new System.Windows.Forms.PictureBox();
      this.PictureBoxFilter = new System.Windows.Forms.PictureBox();
      this.PictureBoxOutput = new System.Windows.Forms.PictureBox();
      this.SliderAxisOffset = new MyLibrary.Controls.Slider();
      this.SliderAxLengthY = new MyLibrary.Controls.Slider();
      this.SliderAxLengthX = new MyLibrary.Controls.Slider();
      this.RangeSliderBlue = new MyLibrary.Controls.RangeSlider();
      this.RangeSliderGreen = new MyLibrary.Controls.RangeSlider();
      this.RangeSliderRed = new MyLibrary.Controls.RangeSlider();
      this.PictureBoxInput = new System.Windows.Forms.PictureBox();
      this.ImageViewerSetAxLim = new MyLibrary.Controls.ImageViewer();
      this.RedoButton = new System.Windows.Forms.PictureBox();
      this.UndoButton = new System.Windows.Forms.PictureBox();
      this.ButtonNext = new MetroFramework.Controls.MetroButton();
      this.ButtonBack = new MetroFramework.Controls.MetroButton();
      this.TabPage6 = new MetroFramework.Controls.MetroTabPage();
      this.DataGridView = new System.Windows.Forms.DataGridView();
      this.TabPage5 = new MetroFramework.Controls.MetroTabPage();
      this.ImageViewerErase = new MyLibrary.Controls.ImageViewer();
      this.TabPage4 = new MetroFramework.Controls.MetroTabPage();
      this.metroPanel1 = new MetroFramework.Controls.MetroPanel();
      this.TabPage3 = new MetroFramework.Controls.MetroTabPage();
      this.TabPage1 = new MetroFramework.Controls.MetroTabPage();
      this.TabControlMain = new MetroFramework.Controls.MetroTabControl();
      this.TabPage2 = new MetroFramework.Controls.MetroTabPage();
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.BackgroundWorker = new System.ComponentModel.BackgroundWorker();
      this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
      ((System.ComponentModel.ISupportInitialize)(this.PictureBoxGetAxis)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.PictureBoxFilter)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.PictureBoxOutput)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.PictureBoxInput)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.RedoButton)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.UndoButton)).BeginInit();
      this.TabPage6.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.DataGridView)).BeginInit();
      this.TabPage5.SuspendLayout();
      this.TabPage4.SuspendLayout();
      this.metroPanel1.SuspendLayout();
      this.TabPage3.SuspendLayout();
      this.TabPage1.SuspendLayout();
      this.TabControlMain.SuspendLayout();
      this.TabPage2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // Tooltip
      // 
      this.Tooltip.AutoPopDelay = 5000;
      this.Tooltip.InitialDelay = 1000;
      this.Tooltip.MetroFont = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Tooltip.ReshowDelay = 1000;
      this.Tooltip.Style = MetroFramework.MetroColorStyle.Blue;
      this.Tooltip.StyleManager = null;
      this.Tooltip.Theme = MetroFramework.MetroThemeStyle.Light;
      // 
      // ButtonSave
      // 
      this.ButtonSave.Highlight = true;
      this.ButtonSave.Location = new System.Drawing.Point(415, 510);
      this.ButtonSave.MetroFont = new System.Drawing.Font("Segoe UI Light", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
      this.ButtonSave.Name = "ButtonSave";
      this.ButtonSave.Size = new System.Drawing.Size(129, 46);
      this.ButtonSave.Style = MetroFramework.MetroColorStyle.Blue;
      this.ButtonSave.StyleManager = null;
      this.ButtonSave.TabIndex = 1;
      this.ButtonSave.Text = "Save";
      this.ButtonSave.Theme = MetroFramework.MetroThemeStyle.Light;
      this.Tooltip.SetToolTip(this.ButtonSave, "Save your data.");
      this.ButtonSave.Click += new System.EventHandler(this.ButtonSave_Click);
      // 
      // ButtonBrowse
      // 
      this.ButtonBrowse.Highlight = true;
      this.ButtonBrowse.Location = new System.Drawing.Point(415, 510);
      this.ButtonBrowse.MetroFont = new System.Drawing.Font("Segoe UI Light", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
      this.ButtonBrowse.Name = "ButtonBrowse";
      this.ButtonBrowse.Size = new System.Drawing.Size(129, 46);
      this.ButtonBrowse.Style = MetroFramework.MetroColorStyle.Blue;
      this.ButtonBrowse.StyleManager = null;
      this.ButtonBrowse.TabIndex = 0;
      this.ButtonBrowse.Text = "Browse";
      this.ButtonBrowse.Theme = MetroFramework.MetroThemeStyle.Light;
      this.Tooltip.SetToolTip(this.ButtonBrowse, "Browse the image to capture its data.\r\n");
      this.ButtonBrowse.Click += new System.EventHandler(this.ButtonBrowse_Click);
      // 
      // CheckBoxYLog
      // 
      this.CheckBoxYLog.AutoSize = true;
      this.CheckBoxYLog.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this.CheckBoxYLog.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.CheckBoxYLog.Location = new System.Drawing.Point(49, 278);
      this.CheckBoxYLog.Name = "CheckBoxYLog";
      this.CheckBoxYLog.Size = new System.Drawing.Size(190, 36);
      this.CheckBoxYLog.TabIndex = 3;
      this.CheckBoxYLog.Text = "    Y Log Scale";
      this.Tooltip.SetToolTip(this.CheckBoxYLog, "Check if the Y axis is in logarithmic scale.");
      this.CheckBoxYLog.UseVisualStyleBackColor = false;
      this.CheckBoxYLog.CheckedChanged += new System.EventHandler(this.CheckBoxYLog_CheckedChanged);
      // 
      // TextBoxYBase
      // 
      this.TextBoxYBase.BackColor = System.Drawing.SystemColors.Control;
      this.TextBoxYBase.Enabled = false;
      this.TextBoxYBase.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.TextBoxYBase.Location = new System.Drawing.Point(112, 223);
      this.TextBoxYBase.Name = "TextBoxYBase";
      this.TextBoxYBase.Size = new System.Drawing.Size(127, 39);
      this.TextBoxYBase.TabIndex = 4;
      this.TextBoxYBase.Text = "10";
      this.TextBoxYBase.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      this.Tooltip.SetToolTip(this.TextBoxYBase, "Set the base number of Y axis.");
      this.TextBoxYBase.TextChanged += new System.EventHandler(this.TextBoxYBase_TextChanged);
      // 
      // CheckBoxXLog
      // 
      this.CheckBoxXLog.AutoSize = true;
      this.CheckBoxXLog.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this.CheckBoxXLog.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.CheckBoxXLog.Location = new System.Drawing.Point(481, 532);
      this.CheckBoxXLog.Name = "CheckBoxXLog";
      this.CheckBoxXLog.Size = new System.Drawing.Size(191, 36);
      this.CheckBoxXLog.TabIndex = 7;
      this.CheckBoxXLog.Text = "    X Log Scale";
      this.Tooltip.SetToolTip(this.CheckBoxXLog, "Check if the X axis is in logarithmic scale.");
      this.CheckBoxXLog.UseVisualStyleBackColor = false;
      this.CheckBoxXLog.CheckedChanged += new System.EventHandler(this.CheckBoxXLog_CheckedChanged);
      // 
      // TextBoxXBase
      // 
      this.TextBoxXBase.BackColor = System.Drawing.SystemColors.Control;
      this.TextBoxXBase.Enabled = false;
      this.TextBoxXBase.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.TextBoxXBase.Location = new System.Drawing.Point(545, 474);
      this.TextBoxXBase.Name = "TextBoxXBase";
      this.TextBoxXBase.Size = new System.Drawing.Size(127, 39);
      this.TextBoxXBase.TabIndex = 8;
      this.TextBoxXBase.Text = "10";
      this.TextBoxXBase.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      this.Tooltip.SetToolTip(this.TextBoxXBase, "Set the base number of X axis.");
      this.TextBoxXBase.TextChanged += new System.EventHandler(this.TextBoxXBase_TextChanged);
      // 
      // TextBoxYlo
      // 
      this.TextBoxYlo.BackColor = System.Drawing.SystemColors.Control;
      this.TextBoxYlo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.TextBoxYlo.Location = new System.Drawing.Point(112, 402);
      this.TextBoxYlo.Name = "TextBoxYlo";
      this.TextBoxYlo.Size = new System.Drawing.Size(127, 39);
      this.TextBoxYlo.TabIndex = 2;
      this.TextBoxYlo.Text = "0";
      this.TextBoxYlo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      this.Tooltip.SetToolTip(this.TextBoxYlo, "Set the minimum value of the Y axis.");
      this.TextBoxYlo.TextChanged += new System.EventHandler(this.TextBoxYlo_TextChanged);
      // 
      // TextBoxYhi
      // 
      this.TextBoxYhi.BackColor = System.Drawing.SystemColors.Control;
      this.TextBoxYhi.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.TextBoxYhi.Location = new System.Drawing.Point(112, 50);
      this.TextBoxYhi.Name = "TextBoxYhi";
      this.TextBoxYhi.Size = new System.Drawing.Size(127, 39);
      this.TextBoxYhi.TabIndex = 1;
      this.TextBoxYhi.Text = "1";
      this.TextBoxYhi.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      this.Tooltip.SetToolTip(this.TextBoxYhi, "Set the maximum value of the Y axis.");
      this.TextBoxYhi.TextChanged += new System.EventHandler(this.TextBoxYhi_TextChanged);
      // 
      // TextBoxXhi
      // 
      this.TextBoxXhi.BackColor = System.Drawing.SystemColors.Control;
      this.TextBoxXhi.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.TextBoxXhi.Location = new System.Drawing.Point(784, 472);
      this.TextBoxXhi.Name = "TextBoxXhi";
      this.TextBoxXhi.Size = new System.Drawing.Size(127, 39);
      this.TextBoxXhi.TabIndex = 6;
      this.TextBoxXhi.Text = "1";
      this.TextBoxXhi.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      this.Tooltip.SetToolTip(this.TextBoxXhi, "Set the maximum value of the X axis.");
      this.TextBoxXhi.TextChanged += new System.EventHandler(this.TextBoxXhi_TextChanged);
      // 
      // TextBoxXlo
      // 
      this.TextBoxXlo.BackColor = System.Drawing.SystemColors.Control;
      this.TextBoxXlo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.TextBoxXlo.Location = new System.Drawing.Point(270, 472);
      this.TextBoxXlo.Name = "TextBoxXlo";
      this.TextBoxXlo.Size = new System.Drawing.Size(127, 39);
      this.TextBoxXlo.TabIndex = 5;
      this.TextBoxXlo.Text = "0";
      this.TextBoxXlo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      this.Tooltip.SetToolTip(this.TextBoxXlo, "Set the minimum value of the X axis.");
      this.TextBoxXlo.TextChanged += new System.EventHandler(this.TextBoxXlo_TextChanged);
      // 
      // metroLabel7
      // 
      this.metroLabel7.Anchor = System.Windows.Forms.AnchorStyles.Top;
      this.metroLabel7.AutoSize = true;
      this.metroLabel7.CustomBackground = false;
      this.metroLabel7.LabelMode = MetroFramework.Controls.MetroLabelMode.Default;
      this.metroLabel7.Location = new System.Drawing.Point(15, 225);
      this.metroLabel7.MetroFont = new System.Drawing.Font("Segoe UI Light", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
      this.metroLabel7.Name = "metroLabel7";
      this.metroLabel7.Size = new System.Drawing.Size(91, 35);
      this.metroLabel7.Style = MetroFramework.MetroColorStyle.Blue;
      this.metroLabel7.StyleManager = null;
      this.metroLabel7.TabIndex = 17;
      this.metroLabel7.Text = "YBase :";
      this.metroLabel7.Theme = MetroFramework.MetroThemeStyle.Light;
      this.Tooltip.SetToolTip(this.metroLabel7, "Set the base number of Y axis.");
      this.metroLabel7.UseStyleColors = false;
      // 
      // metroLabel6
      // 
      this.metroLabel6.Anchor = System.Windows.Forms.AnchorStyles.Top;
      this.metroLabel6.AutoSize = true;
      this.metroLabel6.CustomBackground = false;
      this.metroLabel6.LabelMode = MetroFramework.Controls.MetroLabelMode.Default;
      this.metroLabel6.Location = new System.Drawing.Point(448, 476);
      this.metroLabel6.MetroFont = new System.Drawing.Font("Segoe UI Light", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
      this.metroLabel6.Name = "metroLabel6";
      this.metroLabel6.Size = new System.Drawing.Size(91, 35);
      this.metroLabel6.Style = MetroFramework.MetroColorStyle.Blue;
      this.metroLabel6.StyleManager = null;
      this.metroLabel6.TabIndex = 14;
      this.metroLabel6.Text = "XBase :";
      this.metroLabel6.Theme = MetroFramework.MetroThemeStyle.Light;
      this.Tooltip.SetToolTip(this.metroLabel6, "Set the base number of X axis.");
      this.metroLabel6.UseStyleColors = false;
      // 
      // metroLabel5
      // 
      this.metroLabel5.Anchor = System.Windows.Forms.AnchorStyles.Top;
      this.metroLabel5.AutoSize = true;
      this.metroLabel5.CustomBackground = false;
      this.metroLabel5.LabelMode = MetroFramework.Controls.MetroLabelMode.Default;
      this.metroLabel5.Location = new System.Drawing.Point(45, 404);
      this.metroLabel5.MetroFont = new System.Drawing.Font("Segoe UI Light", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
      this.metroLabel5.Name = "metroLabel5";
      this.metroLabel5.Size = new System.Drawing.Size(61, 35);
      this.metroLabel5.Style = MetroFramework.MetroColorStyle.Blue;
      this.metroLabel5.StyleManager = null;
      this.metroLabel5.TabIndex = 10;
      this.metroLabel5.Text = "Ylo :";
      this.metroLabel5.Theme = MetroFramework.MetroThemeStyle.Light;
      this.Tooltip.SetToolTip(this.metroLabel5, "Set the minimum value of the Y axis.");
      this.metroLabel5.UseStyleColors = false;
      // 
      // metroLabel4
      // 
      this.metroLabel4.Anchor = System.Windows.Forms.AnchorStyles.Top;
      this.metroLabel4.AutoSize = true;
      this.metroLabel4.CustomBackground = false;
      this.metroLabel4.LabelMode = MetroFramework.Controls.MetroLabelMode.Default;
      this.metroLabel4.Location = new System.Drawing.Point(46, 52);
      this.metroLabel4.MetroFont = new System.Drawing.Font("Segoe UI Light", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
      this.metroLabel4.Name = "metroLabel4";
      this.metroLabel4.Size = new System.Drawing.Size(60, 35);
      this.metroLabel4.Style = MetroFramework.MetroColorStyle.Blue;
      this.metroLabel4.StyleManager = null;
      this.metroLabel4.TabIndex = 8;
      this.metroLabel4.Text = "Yhi :";
      this.metroLabel4.Theme = MetroFramework.MetroThemeStyle.Light;
      this.Tooltip.SetToolTip(this.metroLabel4, "Set the maximum value of the Y axis.");
      this.metroLabel4.UseStyleColors = false;
      // 
      // metroLabel2
      // 
      this.metroLabel2.Anchor = System.Windows.Forms.AnchorStyles.Top;
      this.metroLabel2.AutoSize = true;
      this.metroLabel2.CustomBackground = false;
      this.metroLabel2.LabelMode = MetroFramework.Controls.MetroLabelMode.Default;
      this.metroLabel2.Location = new System.Drawing.Point(718, 474);
      this.metroLabel2.MetroFont = new System.Drawing.Font("Segoe UI Light", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
      this.metroLabel2.Name = "metroLabel2";
      this.metroLabel2.Size = new System.Drawing.Size(60, 35);
      this.metroLabel2.Style = MetroFramework.MetroColorStyle.Blue;
      this.metroLabel2.StyleManager = null;
      this.metroLabel2.TabIndex = 6;
      this.metroLabel2.Text = "Xhi :";
      this.metroLabel2.Theme = MetroFramework.MetroThemeStyle.Light;
      this.Tooltip.SetToolTip(this.metroLabel2, "Set the maximum value of the X axis.");
      this.metroLabel2.UseStyleColors = false;
      // 
      // metroLabel1
      // 
      this.metroLabel1.Anchor = System.Windows.Forms.AnchorStyles.Top;
      this.metroLabel1.AutoSize = true;
      this.metroLabel1.CustomBackground = false;
      this.metroLabel1.LabelMode = MetroFramework.Controls.MetroLabelMode.Default;
      this.metroLabel1.Location = new System.Drawing.Point(203, 474);
      this.metroLabel1.MetroFont = new System.Drawing.Font("Segoe UI Light", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
      this.metroLabel1.Name = "metroLabel1";
      this.metroLabel1.Size = new System.Drawing.Size(61, 35);
      this.metroLabel1.Style = MetroFramework.MetroColorStyle.Blue;
      this.metroLabel1.StyleManager = null;
      this.metroLabel1.TabIndex = 4;
      this.metroLabel1.Text = "Xlo :";
      this.metroLabel1.Theme = MetroFramework.MetroThemeStyle.Light;
      this.Tooltip.SetToolTip(this.metroLabel1, "Set the minimum value of the X axis.");
      this.metroLabel1.UseStyleColors = false;
      // 
      // PictureBoxGetAxis
      // 
      this.PictureBoxGetAxis.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.PictureBoxGetAxis.Location = new System.Drawing.Point(64, 70);
      this.PictureBoxGetAxis.Name = "PictureBoxGetAxis";
      this.PictureBoxGetAxis.Size = new System.Drawing.Size(855, 480);
      this.PictureBoxGetAxis.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.PictureBoxGetAxis.TabIndex = 3;
      this.PictureBoxGetAxis.TabStop = false;
      this.Tooltip.SetToolTip(this.PictureBoxGetAxis, "Scroll the sliders to adjust the axis.");
      // 
      // PictureBoxFilter
      // 
      this.PictureBoxFilter.Location = new System.Drawing.Point(3, 17);
      this.PictureBoxFilter.Name = "PictureBoxFilter";
      this.PictureBoxFilter.Size = new System.Drawing.Size(677, 539);
      this.PictureBoxFilter.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.PictureBoxFilter.TabIndex = 6;
      this.PictureBoxFilter.TabStop = false;
      this.Tooltip.SetToolTip(this.PictureBoxFilter, "Scroll the range sliders to filter the image.");
      // 
      // PictureBoxOutput
      // 
      this.PictureBoxOutput.Location = new System.Drawing.Point(3, 17);
      this.PictureBoxOutput.Name = "PictureBoxOutput";
      this.PictureBoxOutput.Size = new System.Drawing.Size(709, 470);
      this.PictureBoxOutput.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.PictureBoxOutput.TabIndex = 6;
      this.PictureBoxOutput.TabStop = false;
      this.Tooltip.SetToolTip(this.PictureBoxOutput, "The captured data.");
      // 
      // SliderAxisOffset
      // 
      this.SliderAxisOffset.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this.SliderAxisOffset.Color = System.Drawing.Color.DeepSkyBlue;
      this.SliderAxisOffset.CustomBackground = true;
      this.SliderAxisOffset.Location = new System.Drawing.Point(785, 510);
      this.SliderAxisOffset.Maximum = 100;
      this.SliderAxisOffset.Minimum = 0;
      this.SliderAxisOffset.Name = "SliderAxisOffset";
      this.SliderAxisOffset.Orientation = MyLibrary.Classes.UDLROrientation.Up;
      this.SliderAxisOffset.Reverse = true;
      this.SliderAxisOffset.Size = new System.Drawing.Size(152, 58);
      this.SliderAxisOffset.TabIndex = 3;
      this.SliderAxisOffset.Text = "metroSlider2";
      this.Tooltip.SetToolTip(this.SliderAxisOffset, "Adjust the offset of the image.");
      this.SliderAxisOffset.Value = 0;
      this.SliderAxisOffset.Scroll += new System.Windows.Forms.ScrollEventHandler(this.SliderAxisOffset_Scroll);
      // 
      // SliderAxLengthY
      // 
      this.SliderAxLengthY.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this.SliderAxLengthY.Color = System.Drawing.SystemColors.MenuHighlight;
      this.SliderAxLengthY.CustomBackground = true;
      this.SliderAxLengthY.Location = new System.Drawing.Point(3, 70);
      this.SliderAxLengthY.Maximum = 100;
      this.SliderAxLengthY.Minimum = 0;
      this.SliderAxLengthY.Name = "SliderAxLengthY";
      this.SliderAxLengthY.Orientation = MyLibrary.Classes.UDLROrientation.Right;
      this.SliderAxLengthY.Reverse = false;
      this.SliderAxLengthY.Size = new System.Drawing.Size(60, 480);
      this.SliderAxLengthY.TabIndex = 2;
      this.SliderAxLengthY.Text = "metroSlider1";
      this.Tooltip.SetToolTip(this.SliderAxLengthY, "Adjust the length of Y axis.");
      this.SliderAxLengthY.Value = 25;
      this.SliderAxLengthY.Scroll += new System.Windows.Forms.ScrollEventHandler(this.SliderAxLengthY_Scroll);
      // 
      // SliderAxLengthX
      // 
      this.SliderAxLengthX.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this.SliderAxLengthX.Color = System.Drawing.SystemColors.MenuHighlight;
      this.SliderAxLengthX.CustomBackground = true;
      this.SliderAxLengthX.Location = new System.Drawing.Point(64, 12);
      this.SliderAxLengthX.Maximum = 100;
      this.SliderAxLengthX.Minimum = 0;
      this.SliderAxLengthX.Name = "SliderAxLengthX";
      this.SliderAxLengthX.Orientation = MyLibrary.Classes.UDLROrientation.Down;
      this.SliderAxLengthX.Reverse = false;
      this.SliderAxLengthX.Size = new System.Drawing.Size(855, 60);
      this.SliderAxLengthX.TabIndex = 1;
      this.SliderAxLengthX.Text = "metroSlider1";
      this.Tooltip.SetToolTip(this.SliderAxLengthX, "Adjust the length of X axis.");
      this.SliderAxLengthX.Value = 25;
      this.SliderAxLengthX.Scroll += new System.Windows.Forms.ScrollEventHandler(this.SliderAxLengthX_Scroll);
      // 
      // RangeSliderBlue
      // 
      this.RangeSliderBlue.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this.RangeSliderBlue.BarMax = 255;
      this.RangeSliderBlue.BarMin = 0;
      this.RangeSliderBlue.Color = MyLibrary.Classes.EMyColors.Blue;
      this.RangeSliderBlue.CustomBackground = true;
      this.RangeSliderBlue.Location = new System.Drawing.Point(177, 25);
      this.RangeSliderBlue.Name = "RangeSliderBlue";
      this.RangeSliderBlue.Orientation = MyLibrary.Classes.HVOrientation.Vertical;
      this.RangeSliderBlue.RangeMax = 255;
      this.RangeSliderBlue.RangeMin = 0;
      this.RangeSliderBlue.Reverse = true;
      this.RangeSliderBlue.Size = new System.Drawing.Size(69, 500);
      this.RangeSliderBlue.TabIndex = 3;
      this.RangeSliderBlue.Text = "metroRangeSlider3";
      this.Tooltip.SetToolTip(this.RangeSliderBlue, "Image filter of blue color.");
      this.RangeSliderBlue.Scroll += new System.EventHandler(this.RangeSliderBlue_Scroll);
      // 
      // RangeSliderGreen
      // 
      this.RangeSliderGreen.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this.RangeSliderGreen.BarMax = 255;
      this.RangeSliderGreen.BarMin = 0;
      this.RangeSliderGreen.Color = MyLibrary.Classes.EMyColors.Green;
      this.RangeSliderGreen.Cursor = System.Windows.Forms.Cursors.Default;
      this.RangeSliderGreen.CustomBackground = true;
      this.RangeSliderGreen.Location = new System.Drawing.Point(102, 25);
      this.RangeSliderGreen.Name = "RangeSliderGreen";
      this.RangeSliderGreen.Orientation = MyLibrary.Classes.HVOrientation.Vertical;
      this.RangeSliderGreen.RangeMax = 255;
      this.RangeSliderGreen.RangeMin = 0;
      this.RangeSliderGreen.Reverse = true;
      this.RangeSliderGreen.Size = new System.Drawing.Size(69, 500);
      this.RangeSliderGreen.TabIndex = 2;
      this.RangeSliderGreen.Text = "metroRangeSlider2";
      this.Tooltip.SetToolTip(this.RangeSliderGreen, "Image filter of green color.");
      this.RangeSliderGreen.Scroll += new System.EventHandler(this.RangeSliderGreen_Scroll);
      // 
      // RangeSliderRed
      // 
      this.RangeSliderRed.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this.RangeSliderRed.BarMax = 255;
      this.RangeSliderRed.BarMin = 0;
      this.RangeSliderRed.Color = MyLibrary.Classes.EMyColors.Red;
      this.RangeSliderRed.CustomBackground = true;
      this.RangeSliderRed.Location = new System.Drawing.Point(27, 25);
      this.RangeSliderRed.Name = "RangeSliderRed";
      this.RangeSliderRed.Orientation = MyLibrary.Classes.HVOrientation.Vertical;
      this.RangeSliderRed.RangeMax = 255;
      this.RangeSliderRed.RangeMin = 0;
      this.RangeSliderRed.Reverse = true;
      this.RangeSliderRed.Size = new System.Drawing.Size(69, 500);
      this.RangeSliderRed.TabIndex = 1;
      this.RangeSliderRed.Text = "metroRangeSlider1";
      this.Tooltip.SetToolTip(this.RangeSliderRed, "Image filter of red color.");
      this.RangeSliderRed.Scroll += new System.EventHandler(this.RangeSliderRed_Scroll);
      // 
      // PictureBoxInput
      // 
      this.PictureBoxInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.PictureBoxInput.Location = new System.Drawing.Point(50, 50);
      this.PictureBoxInput.Name = "PictureBoxInput";
      this.PictureBoxInput.Size = new System.Drawing.Size(850, 440);
      this.PictureBoxInput.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.PictureBoxInput.TabIndex = 2;
      this.PictureBoxInput.TabStop = false;
      this.Tooltip.SetToolTip(this.PictureBoxInput, "Browse the image to capture its data.");
      // 
      // ImageViewerSetAxLim
      // 
      this.ImageViewerSetAxLim.Image = null;
      this.ImageViewerSetAxLim.Location = new System.Drawing.Point(270, 44);
      this.ImageViewerSetAxLim.Name = "ImageViewerSetAxLim";
      this.ImageViewerSetAxLim.Size = new System.Drawing.Size(641, 395);
      this.ImageViewerSetAxLim.TabIndex = 21;
      this.Tooltip.SetToolTip(this.ImageViewerSetAxLim, "Input the axis limits into the textboxes.\r\n");
      // 
      // RedoButton
      // 
      this.RedoButton.Location = new System.Drawing.Point(15, 354);
      this.RedoButton.Name = "RedoButton";
      this.RedoButton.Size = new System.Drawing.Size(60, 60);
      this.RedoButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.RedoButton.TabIndex = 7;
      this.RedoButton.TabStop = false;
      this.Tooltip.SetToolTip(this.RedoButton, "Redo");
      this.RedoButton.EnabledChanged += new System.EventHandler(this.RedoButton_EnabledChanged);
      this.RedoButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RedoButton_MouseDown);
      this.RedoButton.MouseEnter += new System.EventHandler(this.RedoButton_MouseEnter);
      this.RedoButton.MouseLeave += new System.EventHandler(this.RedoButton_MouseLeave);
      this.RedoButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RedoButton_MouseUp);
      // 
      // UndoButton
      // 
      this.UndoButton.Location = new System.Drawing.Point(15, 157);
      this.UndoButton.Name = "UndoButton";
      this.UndoButton.Size = new System.Drawing.Size(60, 60);
      this.UndoButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.UndoButton.TabIndex = 6;
      this.UndoButton.TabStop = false;
      this.Tooltip.SetToolTip(this.UndoButton, "Undo");
      this.UndoButton.EnabledChanged += new System.EventHandler(this.UndoButton_EnabledChanged);
      this.UndoButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UndoButton_MouseDown);
      this.UndoButton.MouseEnter += new System.EventHandler(this.UndoButton_MouseEnter);
      this.UndoButton.MouseLeave += new System.EventHandler(this.UndoButton_MouseLeave);
      this.UndoButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.UndoButton_MouseUp);
      // 
      // ButtonNext
      // 
      this.ButtonNext.Highlight = true;
      this.ButtonNext.Location = new System.Drawing.Point(875, 704);
      this.ButtonNext.MetroFont = new System.Drawing.Font("Segoe UI Light", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ButtonNext.Name = "ButtonNext";
      this.ButtonNext.Size = new System.Drawing.Size(129, 61);
      this.ButtonNext.Style = MetroFramework.MetroColorStyle.Blue;
      this.ButtonNext.StyleManager = null;
      this.ButtonNext.TabIndex = 1;
      this.ButtonNext.Text = "Next";
      this.ButtonNext.Theme = MetroFramework.MetroThemeStyle.Light;
      this.ButtonNext.Click += new System.EventHandler(this.ButtonNext_Click);
      // 
      // ButtonBack
      // 
      this.ButtonBack.Highlight = true;
      this.ButtonBack.Location = new System.Drawing.Point(13, 704);
      this.ButtonBack.MetroFont = new System.Drawing.Font("Segoe UI Light", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
      this.ButtonBack.Name = "ButtonBack";
      this.ButtonBack.Size = new System.Drawing.Size(129, 61);
      this.ButtonBack.Style = MetroFramework.MetroColorStyle.Blue;
      this.ButtonBack.StyleManager = null;
      this.ButtonBack.TabIndex = 2;
      this.ButtonBack.Text = "Back";
      this.ButtonBack.Theme = MetroFramework.MetroThemeStyle.Light;
      this.ButtonBack.Visible = false;
      this.ButtonBack.Click += new System.EventHandler(this.ButtonBack_Click);
      // 
      // TabPage6
      // 
      this.TabPage6.Controls.Add(this.DataGridView);
      this.TabPage6.Controls.Add(this.ButtonSave);
      this.TabPage6.Controls.Add(this.PictureBoxOutput);
      this.TabPage6.CustomBackground = false;
      this.TabPage6.HorizontalScrollbar = false;
      this.TabPage6.HorizontalScrollbarBarColor = true;
      this.TabPage6.HorizontalScrollbarHighlightOnWheel = false;
      this.TabPage6.HorizontalScrollbarSize = 10;
      this.TabPage6.Location = new System.Drawing.Point(4, 40);
      this.TabPage6.Name = "TabPage6";
      this.TabPage6.Size = new System.Drawing.Size(961, 571);
      this.TabPage6.Style = MetroFramework.MetroColorStyle.Blue;
      this.TabPage6.StyleManager = null;
      this.TabPage6.TabIndex = 4;
      this.TabPage6.Text = "6. Save the Data  ";
      this.TabPage6.Theme = MetroFramework.MetroThemeStyle.Light;
      this.TabPage6.VerticalScrollbar = false;
      this.TabPage6.VerticalScrollbarBarColor = true;
      this.TabPage6.VerticalScrollbarHighlightOnWheel = false;
      this.TabPage6.VerticalScrollbarSize = 10;
      // 
      // DataGridView
      // 
      this.DataGridView.AllowUserToAddRows = false;
      this.DataGridView.AllowUserToDeleteRows = false;
      this.DataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.DataGridView.Location = new System.Drawing.Point(718, 17);
      this.DataGridView.Name = "DataGridView";
      this.DataGridView.ReadOnly = true;
      this.DataGridView.RowTemplate.Height = 31;
      this.DataGridView.Size = new System.Drawing.Size(240, 470);
      this.DataGridView.TabIndex = 2;
      // 
      // TabPage5
      // 
      this.TabPage5.Controls.Add(this.ImageViewerErase);
      this.TabPage5.Controls.Add(this.RedoButton);
      this.TabPage5.Controls.Add(this.UndoButton);
      this.TabPage5.CustomBackground = false;
      this.TabPage5.HorizontalScrollbar = false;
      this.TabPage5.HorizontalScrollbarBarColor = true;
      this.TabPage5.HorizontalScrollbarHighlightOnWheel = false;
      this.TabPage5.HorizontalScrollbarSize = 10;
      this.TabPage5.Location = new System.Drawing.Point(4, 40);
      this.TabPage5.Name = "TabPage5";
      this.TabPage5.Size = new System.Drawing.Size(961, 571);
      this.TabPage5.Style = MetroFramework.MetroColorStyle.Blue;
      this.TabPage5.StyleManager = null;
      this.TabPage5.TabIndex = 3;
      this.TabPage5.Text = "5. Erase the Stain  ";
      this.TabPage5.Theme = MetroFramework.MetroThemeStyle.Light;
      this.TabPage5.VerticalScrollbar = false;
      this.TabPage5.VerticalScrollbarBarColor = true;
      this.TabPage5.VerticalScrollbarHighlightOnWheel = false;
      this.TabPage5.VerticalScrollbarSize = 10;
      // 
      // ImageViewerErase
      // 
      this.ImageViewerErase.Image = null;
      this.ImageViewerErase.Location = new System.Drawing.Point(130, 36);
      this.ImageViewerErase.Name = "ImageViewerErase";
      this.ImageViewerErase.Size = new System.Drawing.Size(757, 475);
      this.ImageViewerErase.TabIndex = 9;
      this.ImageViewerErase.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ImageViewerErase_MouseDown);
      this.ImageViewerErase.MouseEnter += new System.EventHandler(this.ImageViewerErase_MouseEnter);
      this.ImageViewerErase.MouseLeave += new System.EventHandler(this.ImageViewerErase_MouseLeave);
      this.ImageViewerErase.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ImageViewerErase_MouseMove);
      this.ImageViewerErase.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ImageViewerErase_MouseUp);
      // 
      // TabPage4
      // 
      this.TabPage4.Controls.Add(this.PictureBoxFilter);
      this.TabPage4.Controls.Add(this.metroPanel1);
      this.TabPage4.CustomBackground = false;
      this.TabPage4.HorizontalScrollbar = false;
      this.TabPage4.HorizontalScrollbarBarColor = true;
      this.TabPage4.HorizontalScrollbarHighlightOnWheel = false;
      this.TabPage4.HorizontalScrollbarSize = 10;
      this.TabPage4.Location = new System.Drawing.Point(4, 40);
      this.TabPage4.Name = "TabPage4";
      this.TabPage4.Size = new System.Drawing.Size(961, 571);
      this.TabPage4.Style = MetroFramework.MetroColorStyle.Blue;
      this.TabPage4.StyleManager = null;
      this.TabPage4.TabIndex = 2;
      this.TabPage4.Text = "4. Filter the Image  ";
      this.TabPage4.Theme = MetroFramework.MetroThemeStyle.Light;
      this.TabPage4.VerticalScrollbar = false;
      this.TabPage4.VerticalScrollbarBarColor = true;
      this.TabPage4.VerticalScrollbarHighlightOnWheel = false;
      this.TabPage4.VerticalScrollbarSize = 10;
      // 
      // metroPanel1
      // 
      this.metroPanel1.Controls.Add(this.RangeSliderBlue);
      this.metroPanel1.Controls.Add(this.RangeSliderGreen);
      this.metroPanel1.Controls.Add(this.RangeSliderRed);
      this.metroPanel1.CustomBackground = false;
      this.metroPanel1.HorizontalScrollbar = false;
      this.metroPanel1.HorizontalScrollbarBarColor = true;
      this.metroPanel1.HorizontalScrollbarHighlightOnWheel = false;
      this.metroPanel1.HorizontalScrollbarSize = 10;
      this.metroPanel1.Location = new System.Drawing.Point(686, 17);
      this.metroPanel1.Name = "metroPanel1";
      this.metroPanel1.Size = new System.Drawing.Size(272, 539);
      this.metroPanel1.Style = MetroFramework.MetroColorStyle.Blue;
      this.metroPanel1.StyleManager = null;
      this.metroPanel1.TabIndex = 5;
      this.metroPanel1.Theme = MetroFramework.MetroThemeStyle.Light;
      this.metroPanel1.VerticalScrollbar = false;
      this.metroPanel1.VerticalScrollbarBarColor = true;
      this.metroPanel1.VerticalScrollbarHighlightOnWheel = false;
      this.metroPanel1.VerticalScrollbarSize = 10;
      // 
      // TabPage3
      // 
      this.TabPage3.Controls.Add(this.SliderAxisOffset);
      this.TabPage3.Controls.Add(this.PictureBoxGetAxis);
      this.TabPage3.Controls.Add(this.SliderAxLengthY);
      this.TabPage3.Controls.Add(this.SliderAxLengthX);
      this.TabPage3.CustomBackground = false;
      this.TabPage3.HorizontalScrollbar = false;
      this.TabPage3.HorizontalScrollbarBarColor = true;
      this.TabPage3.HorizontalScrollbarHighlightOnWheel = false;
      this.TabPage3.HorizontalScrollbarSize = 10;
      this.TabPage3.Location = new System.Drawing.Point(4, 40);
      this.TabPage3.Name = "TabPage3";
      this.TabPage3.Size = new System.Drawing.Size(961, 571);
      this.TabPage3.Style = MetroFramework.MetroColorStyle.Blue;
      this.TabPage3.StyleManager = null;
      this.TabPage3.TabIndex = 1;
      this.TabPage3.Text = "3. Adjust the Axis  ";
      this.TabPage3.Theme = MetroFramework.MetroThemeStyle.Light;
      this.TabPage3.VerticalScrollbar = false;
      this.TabPage3.VerticalScrollbarBarColor = true;
      this.TabPage3.VerticalScrollbarHighlightOnWheel = false;
      this.TabPage3.VerticalScrollbarSize = 10;
      // 
      // TabPage1
      // 
      this.TabPage1.Controls.Add(this.ButtonBrowse);
      this.TabPage1.Controls.Add(this.PictureBoxInput);
      this.TabPage1.CustomBackground = false;
      this.TabPage1.HorizontalScrollbar = false;
      this.TabPage1.HorizontalScrollbarBarColor = true;
      this.TabPage1.HorizontalScrollbarHighlightOnWheel = false;
      this.TabPage1.HorizontalScrollbarSize = 10;
      this.TabPage1.Location = new System.Drawing.Point(4, 40);
      this.TabPage1.Name = "TabPage1";
      this.TabPage1.Size = new System.Drawing.Size(961, 571);
      this.TabPage1.Style = MetroFramework.MetroColorStyle.Blue;
      this.TabPage1.StyleManager = null;
      this.TabPage1.TabIndex = 0;
      this.TabPage1.Text = "1. Select an Image  ";
      this.TabPage1.Theme = MetroFramework.MetroThemeStyle.Light;
      this.TabPage1.VerticalScrollbar = false;
      this.TabPage1.VerticalScrollbarBarColor = true;
      this.TabPage1.VerticalScrollbarHighlightOnWheel = false;
      this.TabPage1.VerticalScrollbarSize = 10;
      this.TabPage1.Click += new System.EventHandler(this.TabPage1_Click);
      // 
      // TabControlMain
      // 
      this.TabControlMain.Controls.Add(this.TabPage1);
      this.TabControlMain.Controls.Add(this.TabPage2);
      this.TabControlMain.Controls.Add(this.TabPage3);
      this.TabControlMain.Controls.Add(this.TabPage4);
      this.TabControlMain.Controls.Add(this.TabPage5);
      this.TabControlMain.Controls.Add(this.TabPage6);
      this.TabControlMain.CustomBackground = false;
      this.TabControlMain.Location = new System.Drawing.Point(23, 83);
      this.TabControlMain.MetroFont = new System.Drawing.Font("Segoe UI Light", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
      this.TabControlMain.Name = "TabControlMain";
      this.TabControlMain.SelectedIndex = 4;
      this.TabControlMain.Size = new System.Drawing.Size(969, 615);
      this.TabControlMain.Style = MetroFramework.MetroColorStyle.Blue;
      this.TabControlMain.StyleManager = null;
      this.TabControlMain.TabIndex = 0;
      this.TabControlMain.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.TabControlMain.Theme = MetroFramework.MetroThemeStyle.Light;
      this.TabControlMain.UseStyleColors = false;
      this.TabControlMain.SelectedIndexChanged += new System.EventHandler(this.TabControl_SelectedIndexChanged);
      // 
      // TabPage2
      // 
      this.TabPage2.Controls.Add(this.ImageViewerSetAxLim);
      this.TabPage2.Controls.Add(this.CheckBoxYLog);
      this.TabPage2.Controls.Add(this.TextBoxYBase);
      this.TabPage2.Controls.Add(this.metroLabel7);
      this.TabPage2.Controls.Add(this.CheckBoxXLog);
      this.TabPage2.Controls.Add(this.TextBoxXBase);
      this.TabPage2.Controls.Add(this.metroLabel6);
      this.TabPage2.Controls.Add(this.TextBoxYlo);
      this.TabPage2.Controls.Add(this.metroLabel5);
      this.TabPage2.Controls.Add(this.TextBoxYhi);
      this.TabPage2.Controls.Add(this.metroLabel4);
      this.TabPage2.Controls.Add(this.TextBoxXhi);
      this.TabPage2.Controls.Add(this.metroLabel2);
      this.TabPage2.Controls.Add(this.TextBoxXlo);
      this.TabPage2.Controls.Add(this.metroLabel1);
      this.TabPage2.CustomBackground = false;
      this.TabPage2.HorizontalScrollbar = false;
      this.TabPage2.HorizontalScrollbarBarColor = true;
      this.TabPage2.HorizontalScrollbarHighlightOnWheel = false;
      this.TabPage2.HorizontalScrollbarSize = 10;
      this.TabPage2.Location = new System.Drawing.Point(4, 40);
      this.TabPage2.Name = "TabPage2";
      this.TabPage2.Size = new System.Drawing.Size(961, 571);
      this.TabPage2.Style = MetroFramework.MetroColorStyle.Blue;
      this.TabPage2.StyleManager = null;
      this.TabPage2.TabIndex = 5;
      this.TabPage2.Text = "2. Set Axis Limits  ";
      this.TabPage2.Theme = MetroFramework.MetroThemeStyle.Light;
      this.TabPage2.VerticalScrollbar = false;
      this.TabPage2.VerticalScrollbarBarColor = true;
      this.TabPage2.VerticalScrollbarHighlightOnWheel = false;
      this.TabPage2.VerticalScrollbarSize = 10;
      // 
      // openFileDialog
      // 
      this.openFileDialog.Filter = "Images (*.jpg; *.png; *.bmp)|*.jpg;*.png;*.bmp";
      // 
      // BackgroundWorker
      // 
      this.BackgroundWorker.WorkerReportsProgress = true;
      this.BackgroundWorker.WorkerSupportsCancellation = true;
      this.BackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker_DoWork);
      this.BackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorker_RunWorkerCompleted);
      // 
      // pictureBox1
      // 
      this.pictureBox1.Image = global::DataCapturer.Properties.Resources.icon81;
      this.pictureBox1.Location = new System.Drawing.Point(6, 18);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(50, 50);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.pictureBox1.TabIndex = 3;
      this.pictureBox1.TabStop = false;
      // 
      // metroLabel3
      // 
      this.metroLabel3.AutoSize = true;
      this.metroLabel3.CustomBackground = false;
      this.metroLabel3.LabelMode = MetroFramework.Controls.MetroLabelMode.Default;
      this.metroLabel3.Location = new System.Drawing.Point(62, 18);
      this.metroLabel3.MetroFont = new System.Drawing.Font("Segoe UI Light", 35F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
      this.metroLabel3.Name = "metroLabel3";
      this.metroLabel3.Size = new System.Drawing.Size(217, 47);
      this.metroLabel3.Style = MetroFramework.MetroColorStyle.Blue;
      this.metroLabel3.StyleManager = null;
      this.metroLabel3.TabIndex = 5;
      this.metroLabel3.Text = "DataCapturer";
      this.metroLabel3.Theme = MetroFramework.MetroThemeStyle.Light;
      this.metroLabel3.UseStyleColors = false;
      // 
      // DataCapturer
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
      this.ClientSize = new System.Drawing.Size(1015, 776);
      this.Controls.Add(this.metroLabel3);
      this.Controls.Add(this.pictureBox1);
      this.Controls.Add(this.ButtonBack);
      this.Controls.Add(this.ButtonNext);
      this.Controls.Add(this.TabControlMain);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Location = new System.Drawing.Point(0, 0);
      this.MetroFont = new System.Drawing.Font("Segoe UI Light", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Name = "DataCapturer";
      this.Load += new System.EventHandler(this.DataCapturer_Load);
      this.Resize += new System.EventHandler(this.DataCapturer_Resize);
      ((System.ComponentModel.ISupportInitialize)(this.PictureBoxGetAxis)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.PictureBoxFilter)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.PictureBoxOutput)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.PictureBoxInput)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.RedoButton)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.UndoButton)).EndInit();
      this.TabPage6.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.DataGridView)).EndInit();
      this.TabPage5.ResumeLayout(false);
      this.TabPage4.ResumeLayout(false);
      this.metroPanel1.ResumeLayout(false);
      this.TabPage3.ResumeLayout(false);
      this.TabPage1.ResumeLayout(false);
      this.TabControlMain.ResumeLayout(false);
      this.TabPage2.ResumeLayout(false);
      this.TabPage2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

		}

		#endregion

		#region Useless
		private MetroFramework.Controls.MetroPanel metroPanel1;
		private MetroFramework.Controls.MetroLabel metroLabel1;
		private MetroFramework.Controls.MetroLabel metroLabel2;
		private MetroFramework.Controls.MetroLabel metroLabel4;
		private MetroFramework.Controls.MetroLabel metroLabel5;
		private MetroFramework.Controls.MetroLabel metroLabel6;
		private MetroFramework.Controls.MetroLabel metroLabel7;
		private System.Windows.Forms.DataGridView DataGridView;
		#endregion

		private MetroFramework.Components.MetroToolTip Tooltip;
		private MetroFramework.Controls.MetroButton ButtonNext;
		private MetroFramework.Controls.MetroButton ButtonBack;
		private MetroFramework.Controls.MetroButton ButtonBrowse;
		private MetroFramework.Controls.MetroButton ButtonSave;
		private MetroFramework.Controls.MetroTabPage TabPage6;
		private MetroFramework.Controls.MetroTabPage TabPage5;
		private MetroFramework.Controls.MetroTabPage TabPage4;
		private MetroFramework.Controls.MetroTabPage TabPage3;
		private MetroFramework.Controls.MetroTabPage TabPage2;
		private MetroFramework.Controls.MetroTabPage TabPage1;
		private MetroFramework.Controls.MetroTabControl TabControlMain;
		private MyLibrary.Controls.Slider SliderAxisOffset;
		private MyLibrary.Controls.Slider SliderAxLengthX;
		private MyLibrary.Controls.RangeSlider RangeSliderBlue;
		private MyLibrary.Controls.RangeSlider RangeSliderGreen;
		private MyLibrary.Controls.RangeSlider RangeSliderRed;
		private System.Windows.Forms.PictureBox PictureBoxOutput;
		private System.Windows.Forms.PictureBox PictureBoxGetAxis;
		private System.Windows.Forms.PictureBox PictureBoxInput;
		private System.Windows.Forms.PictureBox PictureBoxFilter;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.TextBox  TextBoxYlo;
		private System.Windows.Forms.TextBox  TextBoxYhi;
		private System.Windows.Forms.TextBox  TextBoxXhi;
		private System.Windows.Forms.TextBox  TextBoxXlo;
		private System.Windows.Forms.TextBox  TextBoxXBase;
		private System.Windows.Forms.TextBox  TextBoxYBase;
		private System.Windows.Forms.CheckBox CheckBoxXLog;
		private System.Windows.Forms.CheckBox CheckBoxYLog;
		private MyLibrary.Controls.Slider SliderAxLengthY;
		private System.Windows.Forms.PictureBox RedoButton;
		private System.Windows.Forms.PictureBox UndoButton;
		private System.ComponentModel.BackgroundWorker BackgroundWorker;
    private System.Windows.Forms.SaveFileDialog saveFileDialog;
    private System.Windows.Forms.PictureBox pictureBox1;
    private MyLibrary.Controls.ImageViewer ImageViewerSetAxLim;
    private MyLibrary.Controls.ImageViewer ImageViewerErase;
    private MetroFramework.Controls.MetroLabel metroLabel3;
  }
}

