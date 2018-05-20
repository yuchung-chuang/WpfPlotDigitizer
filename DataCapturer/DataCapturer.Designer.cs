using MetroFramework;

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
			this.Tooltip = new MetroFramework.Components.MetroToolTip();
			this.ButtonSave = new MetroFramework.Controls.MetroButton();
			this.RangeSliderBlue = new MetroFramework.Controls.RangeSlider();
			this.RangeSliderGreen = new MetroFramework.Controls.RangeSlider();
			this.RangeSliderRed = new MetroFramework.Controls.RangeSlider();
			this.SliderAxisOffset = new MetroFramework.Controls.Slider();
			this.SliderAxLengthY = new MetroFramework.Controls.Slider();
			this.SliderAxLengthX = new MetroFramework.Controls.Slider();
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
			this.PictureBoxWarnGetAxis = new System.Windows.Forms.PictureBox();
			this.PictureBoxWarnSetAxLim = new System.Windows.Forms.PictureBox();
			this.PictureBoxGetAxis = new System.Windows.Forms.PictureBox();
			this.PictureBoxFilter = new System.Windows.Forms.PictureBox();
			this.PictureBoxEraser = new System.Windows.Forms.PictureBox();
			this.PictureBoxOutput = new System.Windows.Forms.PictureBox();
			this.ImageViewerSetAxLim = new MetroFramework.Controls.ImageViewer();
			this.ButtonNext = new MetroFramework.Controls.MetroButton();
			this.ButtonBack = new MetroFramework.Controls.MetroButton();
			this.TabPage6 = new MetroFramework.Controls.MetroTabPage();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.XData = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.YData = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.TabPage5 = new MetroFramework.Controls.MetroTabPage();
			this.RedoButton = new System.Windows.Forms.PictureBox();
			this.UndoButton = new System.Windows.Forms.PictureBox();
			this.TabPage4 = new MetroFramework.Controls.MetroTabPage();
			this.metroPanel1 = new MetroFramework.Controls.MetroPanel();
			this.TabPage3 = new MetroFramework.Controls.MetroTabPage();
			this.TabPage1 = new MetroFramework.Controls.MetroTabPage();
			this.PictureBoxInput = new System.Windows.Forms.PictureBox();
			this.TabControlMain = new MetroFramework.Controls.MetroTabControl();
			this.TabPage2 = new MetroFramework.Controls.MetroTabPage();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.PictureBoxWarnGetAxis)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.PictureBoxWarnSetAxLim)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.PictureBoxGetAxis)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.PictureBoxFilter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.PictureBoxEraser)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.PictureBoxOutput)).BeginInit();
			this.TabPage6.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.TabPage5.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.RedoButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.UndoButton)).BeginInit();
			this.TabPage4.SuspendLayout();
			this.metroPanel1.SuspendLayout();
			this.TabPage3.SuspendLayout();
			this.TabPage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.PictureBoxInput)).BeginInit();
			this.TabControlMain.SuspendLayout();
			this.TabPage2.SuspendLayout();
			this.SuspendLayout();
			// 
			// Tooltip
			// 
			this.Tooltip.AutoPopDelay = 5000;
			this.Tooltip.InitialDelay = 300;
			this.Tooltip.MetroFont = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Tooltip.ReshowDelay = 200;
			this.Tooltip.Style = MetroFramework.MetroColorStyle.Blue;
			this.Tooltip.StyleManager = null;
			this.Tooltip.Theme = MetroFramework.MetroThemeStyle.Light;
			this.Tooltip.Popup += new System.Windows.Forms.PopupEventHandler(this.Tooltip_Popup);
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
			this.ButtonSave.TabIndex = 5;
			this.ButtonSave.Text = "Save";
			this.ButtonSave.Theme = MetroFramework.MetroThemeStyle.Light;
			this.Tooltip.SetToolTip(this.ButtonSave, "Save your data.");
			this.ButtonSave.Click += new System.EventHandler(this.ButtonSave_Click);
			// 
			// RangeSliderBlue
			// 
			this.RangeSliderBlue.ArrowChange = ((uint)(1u));
			this.RangeSliderBlue.BackColor = System.Drawing.Color.Transparent;
			this.RangeSliderBlue.BarMax = 255;
			this.RangeSliderBlue.BarMin = 0;
			this.RangeSliderBlue.CustomBackground = false;
			this.RangeSliderBlue.Location = new System.Drawing.Point(177, 25);
			this.RangeSliderBlue.Name = "RangeSliderBlue";
			this.RangeSliderBlue.Orientation = MetroFramework.SliderOrientation.Vertical;
			this.RangeSliderBlue.PageChange = ((uint)(5u));
			this.RangeSliderBlue.RangeMax = 255;
			this.RangeSliderBlue.RangeMin = 0;
			this.RangeSliderBlue.Reverse = true;
			this.RangeSliderBlue.ScrollChange = ((uint)(10u));
			this.RangeSliderBlue.Size = new System.Drawing.Size(69, 500);
			this.RangeSliderBlue.Style = MetroFramework.MetroColorStyle.Blue;
			this.RangeSliderBlue.StyleManager = null;
			this.RangeSliderBlue.TabIndex = 5;
			this.RangeSliderBlue.Text = "metroRangeSlider3";
			this.RangeSliderBlue.Theme = MetroFramework.MetroThemeStyle.Light;
			this.Tooltip.SetToolTip(this.RangeSliderBlue, "Image filter of blue color.");
			this.RangeSliderBlue.Scroll += new System.EventHandler(this.RangeSliderBlue_Scroll);
			// 
			// RangeSliderGreen
			// 
			this.RangeSliderGreen.ArrowChange = ((uint)(1u));
			this.RangeSliderGreen.BackColor = System.Drawing.Color.Transparent;
			this.RangeSliderGreen.BarMax = 255;
			this.RangeSliderGreen.BarMin = 0;
			this.RangeSliderGreen.CustomBackground = false;
			this.RangeSliderGreen.Location = new System.Drawing.Point(102, 25);
			this.RangeSliderGreen.Name = "RangeSliderGreen";
			this.RangeSliderGreen.Orientation = MetroFramework.SliderOrientation.Vertical;
			this.RangeSliderGreen.PageChange = ((uint)(5u));
			this.RangeSliderGreen.RangeMax = 255;
			this.RangeSliderGreen.RangeMin = 0;
			this.RangeSliderGreen.Reverse = true;
			this.RangeSliderGreen.ScrollChange = ((uint)(10u));
			this.RangeSliderGreen.Size = new System.Drawing.Size(69, 500);
			this.RangeSliderGreen.Style = MetroFramework.MetroColorStyle.Green;
			this.RangeSliderGreen.StyleManager = null;
			this.RangeSliderGreen.TabIndex = 4;
			this.RangeSliderGreen.Text = "metroRangeSlider2";
			this.RangeSliderGreen.Theme = MetroFramework.MetroThemeStyle.Light;
			this.Tooltip.SetToolTip(this.RangeSliderGreen, "Image filter of green color.");
			this.RangeSliderGreen.Scroll += new System.EventHandler(this.RangeSliderGreen_Scroll);
			// 
			// RangeSliderRed
			// 
			this.RangeSliderRed.ArrowChange = ((uint)(1u));
			this.RangeSliderRed.BackColor = System.Drawing.Color.Transparent;
			this.RangeSliderRed.BarMax = 255;
			this.RangeSliderRed.BarMin = 0;
			this.RangeSliderRed.CustomBackground = false;
			this.RangeSliderRed.Location = new System.Drawing.Point(27, 25);
			this.RangeSliderRed.Name = "RangeSliderRed";
			this.RangeSliderRed.Orientation = MetroFramework.SliderOrientation.Vertical;
			this.RangeSliderRed.PageChange = ((uint)(5u));
			this.RangeSliderRed.RangeMax = 255;
			this.RangeSliderRed.RangeMin = 0;
			this.RangeSliderRed.Reverse = true;
			this.RangeSliderRed.ScrollChange = ((uint)(10u));
			this.RangeSliderRed.Size = new System.Drawing.Size(69, 500);
			this.RangeSliderRed.Style = MetroFramework.MetroColorStyle.Red;
			this.RangeSliderRed.StyleManager = null;
			this.RangeSliderRed.TabIndex = 3;
			this.RangeSliderRed.Text = "metroRangeSlider1";
			this.RangeSliderRed.Theme = MetroFramework.MetroThemeStyle.Light;
			this.Tooltip.SetToolTip(this.RangeSliderRed, "Image filter of red color.");
			this.RangeSliderRed.Scroll += new System.EventHandler(this.RangeSliderRed_Scroll);
			// 
			// SliderAxisOffset
			// 
			this.SliderAxisOffset.ArrowChange = ((uint)(1u));
			this.SliderAxisOffset.BackColor = System.Drawing.Color.Transparent;
			this.SliderAxisOffset.BarMax = 100;
			this.SliderAxisOffset.BarMin = 0;
			this.SliderAxisOffset.CustomBackground = false;
			this.SliderAxisOffset.Location = new System.Drawing.Point(785, 510);
			this.SliderAxisOffset.Name = "SliderAxisOffset";
			this.SliderAxisOffset.Orientation = MetroFramework.SliderOrientation.Up;
			this.SliderAxisOffset.PageChange = ((uint)(5u));
			this.SliderAxisOffset.Reverse = true;
			this.SliderAxisOffset.ScrollChange = ((uint)(10u));
			this.SliderAxisOffset.Size = new System.Drawing.Size(152, 58);
			this.SliderAxisOffset.Style = MetroFramework.MetroColorStyle.Yellow;
			this.SliderAxisOffset.StyleManager = null;
			this.SliderAxisOffset.TabIndex = 5;
			this.SliderAxisOffset.Text = "metroSlider2";
			this.SliderAxisOffset.Theme = MetroFramework.MetroThemeStyle.Light;
			this.Tooltip.SetToolTip(this.SliderAxisOffset, "Adjust the offset of the image.");
			this.SliderAxisOffset.Value = 0;
			this.SliderAxisOffset.Scroll += new System.Windows.Forms.ScrollEventHandler(this.SliderAxisOffset_Scroll);
			// 
			// SliderAxLengthY
			// 
			this.SliderAxLengthY.ArrowChange = ((uint)(1u));
			this.SliderAxLengthY.BackColor = System.Drawing.Color.Transparent;
			this.SliderAxLengthY.BarMax = 100;
			this.SliderAxLengthY.BarMin = 0;
			this.SliderAxLengthY.CustomBackground = false;
			this.SliderAxLengthY.Location = new System.Drawing.Point(3, 70);
			this.SliderAxLengthY.Name = "SliderAxLengthY";
			this.SliderAxLengthY.Orientation = MetroFramework.SliderOrientation.Right;
			this.SliderAxLengthY.PageChange = ((uint)(5u));
			this.SliderAxLengthY.Reverse = false;
			this.SliderAxLengthY.ScrollChange = ((uint)(10u));
			this.SliderAxLengthY.Size = new System.Drawing.Size(60, 480);
			this.SliderAxLengthY.Style = MetroFramework.MetroColorStyle.Teal;
			this.SliderAxLengthY.StyleManager = null;
			this.SliderAxLengthY.TabIndex = 6;
			this.SliderAxLengthY.Text = "metroSlider1";
			this.SliderAxLengthY.Theme = MetroFramework.MetroThemeStyle.Light;
			this.Tooltip.SetToolTip(this.SliderAxLengthY, "Adjust the length of Y axis.");
			this.SliderAxLengthY.Value = 25;
			this.SliderAxLengthY.Scroll += new System.Windows.Forms.ScrollEventHandler(this.SliderAxLengthY_Scroll);
			// 
			// SliderAxLengthX
			// 
			this.SliderAxLengthX.ArrowChange = ((uint)(1u));
			this.SliderAxLengthX.BackColor = System.Drawing.Color.Transparent;
			this.SliderAxLengthX.BarMax = 100;
			this.SliderAxLengthX.BarMin = 0;
			this.SliderAxLengthX.CustomBackground = false;
			this.SliderAxLengthX.Location = new System.Drawing.Point(64, 12);
			this.SliderAxLengthX.Name = "SliderAxLengthX";
			this.SliderAxLengthX.Orientation = MetroFramework.SliderOrientation.Down;
			this.SliderAxLengthX.PageChange = ((uint)(5u));
			this.SliderAxLengthX.Reverse = false;
			this.SliderAxLengthX.ScrollChange = ((uint)(10u));
			this.SliderAxLengthX.Size = new System.Drawing.Size(855, 60);
			this.SliderAxLengthX.Style = MetroFramework.MetroColorStyle.Teal;
			this.SliderAxLengthX.StyleManager = null;
			this.SliderAxLengthX.TabIndex = 4;
			this.SliderAxLengthX.Text = "metroSlider1";
			this.SliderAxLengthX.Theme = MetroFramework.MetroThemeStyle.Light;
			this.Tooltip.SetToolTip(this.SliderAxLengthX, "Adjust the length of X axis.");
			this.SliderAxLengthX.Value = 25;
			this.SliderAxLengthX.Scroll += new System.Windows.Forms.ScrollEventHandler(this.SliderAxLengthX_Scroll);
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
			this.ButtonBrowse.TabIndex = 5;
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
			this.CheckBoxYLog.Location = new System.Drawing.Point(48, 229);
			this.CheckBoxYLog.Name = "CheckBoxYLog";
			this.CheckBoxYLog.Size = new System.Drawing.Size(190, 36);
			this.CheckBoxYLog.TabIndex = 19;
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
			this.TextBoxYBase.Location = new System.Drawing.Point(112, 273);
			this.TextBoxYBase.Name = "TextBoxYBase";
			this.TextBoxYBase.Size = new System.Drawing.Size(127, 39);
			this.TextBoxYBase.TabIndex = 18;
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
			this.CheckBoxXLog.Location = new System.Drawing.Point(493, 472);
			this.CheckBoxXLog.Name = "CheckBoxXLog";
			this.CheckBoxXLog.Size = new System.Drawing.Size(191, 36);
			this.CheckBoxXLog.TabIndex = 16;
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
			this.TextBoxXBase.Location = new System.Drawing.Point(557, 517);
			this.TextBoxXBase.Name = "TextBoxXBase";
			this.TextBoxXBase.Size = new System.Drawing.Size(127, 39);
			this.TextBoxXBase.TabIndex = 15;
			this.TextBoxXBase.Text = "10";
			this.TextBoxXBase.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.Tooltip.SetToolTip(this.TextBoxXBase, "Set the base number of X axis.");
			this.TextBoxXBase.TextChanged += new System.EventHandler(this.TextBoxXBase_TextChanged);
			// 
			// TextBoxYlo
			// 
			this.TextBoxYlo.BackColor = System.Drawing.SystemColors.Control;
			this.TextBoxYlo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TextBoxYlo.Location = new System.Drawing.Point(112, 427);
			this.TextBoxYlo.Name = "TextBoxYlo";
			this.TextBoxYlo.Size = new System.Drawing.Size(127, 39);
			this.TextBoxYlo.TabIndex = 11;
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
			this.TextBoxYhi.TabIndex = 9;
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
			this.TextBoxXhi.TabIndex = 7;
			this.TextBoxXhi.Text = "1";
			this.TextBoxXhi.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.Tooltip.SetToolTip(this.TextBoxXhi, "Set the maximum value of the X axis.");
			this.TextBoxXhi.TextChanged += new System.EventHandler(this.TextBoxXhi_TextChanged);
			// 
			// TextBoxXlo
			// 
			this.TextBoxXlo.BackColor = System.Drawing.SystemColors.Control;
			this.TextBoxXlo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TextBoxXlo.Location = new System.Drawing.Point(312, 472);
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
			this.metroLabel7.Location = new System.Drawing.Point(15, 275);
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
			this.metroLabel6.Location = new System.Drawing.Point(460, 519);
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
			this.metroLabel5.Location = new System.Drawing.Point(45, 429);
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
			this.metroLabel1.Location = new System.Drawing.Point(245, 474);
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
			// PictureBoxWarnGetAxis
			// 
			this.PictureBoxWarnGetAxis.BackColor = System.Drawing.Color.Transparent;
			this.PictureBoxWarnGetAxis.Image = global::DataCapturer.Properties.Resources._01300543896251147062126622831_s;
			this.PictureBoxWarnGetAxis.Location = new System.Drawing.Point(365, 53);
			this.PictureBoxWarnGetAxis.Name = "PictureBoxWarnGetAxis";
			this.PictureBoxWarnGetAxis.Size = new System.Drawing.Size(40, 40);
			this.PictureBoxWarnGetAxis.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.PictureBoxWarnGetAxis.TabIndex = 7;
			this.PictureBoxWarnGetAxis.TabStop = false;
			this.Tooltip.SetToolTip(this.PictureBoxWarnGetAxis, "Can\'t capture appropriate axis!");
			this.PictureBoxWarnGetAxis.Visible = false;
			// 
			// PictureBoxWarnSetAxLim
			// 
			this.PictureBoxWarnSetAxLim.BackColor = System.Drawing.Color.Transparent;
			this.PictureBoxWarnSetAxLim.Image = global::DataCapturer.Properties.Resources._01300543896251147062126622831_s;
			this.PictureBoxWarnSetAxLim.Location = new System.Drawing.Point(213, 53);
			this.PictureBoxWarnSetAxLim.Name = "PictureBoxWarnSetAxLim";
			this.PictureBoxWarnSetAxLim.Size = new System.Drawing.Size(40, 40);
			this.PictureBoxWarnSetAxLim.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.PictureBoxWarnSetAxLim.TabIndex = 6;
			this.PictureBoxWarnSetAxLim.TabStop = false;
			this.Tooltip.SetToolTip(this.PictureBoxWarnSetAxLim, "Not all axis limits have been set!");
			this.PictureBoxWarnSetAxLim.Visible = false;
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
			// PictureBoxEraser
			// 
			this.PictureBoxEraser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.PictureBoxEraser.Cursor = System.Windows.Forms.Cursors.Default;
			this.PictureBoxEraser.Location = new System.Drawing.Point(97, 18);
			this.PictureBoxEraser.Name = "PictureBoxEraser";
			this.PictureBoxEraser.Size = new System.Drawing.Size(828, 550);
			this.PictureBoxEraser.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.PictureBoxEraser.TabIndex = 5;
			this.PictureBoxEraser.TabStop = false;
			this.Tooltip.SetToolTip(this.PictureBoxEraser, "Click on the image to erase the stain.");
			this.PictureBoxEraser.LoadCompleted += new System.ComponentModel.AsyncCompletedEventHandler(this.PictureBoxEraser_LoadCompleted);
			this.PictureBoxEraser.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PictureBoxEraser_MouseDown);
			this.PictureBoxEraser.MouseEnter += new System.EventHandler(this.PictureBoxEraser_MouseEnter);
			this.PictureBoxEraser.MouseLeave += new System.EventHandler(this.PictureBoxEraser_MouseLeave);
			this.PictureBoxEraser.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PictureBoxEraser_MouseMove);
			this.PictureBoxEraser.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PictureBoxEraser_MouseUp);
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
			// ImageViewerSetAxLim
			// 
			this.ImageViewerSetAxLim.Image = null;
			this.ImageViewerSetAxLim.Location = new System.Drawing.Point(266, 50);
			this.ImageViewerSetAxLim.Name = "ImageViewerSetAxLim";
			this.ImageViewerSetAxLim.Size = new System.Drawing.Size(645, 395);
			this.ImageViewerSetAxLim.TabIndex = 20;
			this.Tooltip.SetToolTip(this.ImageViewerSetAxLim, "Input the axis limits into the textboxes.\r\n");
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
			this.ButtonNext.TabIndex = 3;
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
			this.ButtonBack.TabIndex = 4;
			this.ButtonBack.Text = "Back";
			this.ButtonBack.Theme = MetroFramework.MetroThemeStyle.Light;
			this.ButtonBack.Visible = false;
			this.ButtonBack.Click += new System.EventHandler(this.ButtonBack_Click);
			// 
			// TabPage6
			// 
			this.TabPage6.Controls.Add(this.dataGridView1);
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
			// dataGridView1
			// 
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.AllowUserToDeleteRows = false;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.XData,
            this.YData});
			this.dataGridView1.Location = new System.Drawing.Point(718, 17);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.ReadOnly = true;
			this.dataGridView1.RowTemplate.Height = 31;
			this.dataGridView1.Size = new System.Drawing.Size(240, 470);
			this.dataGridView1.TabIndex = 13;
			// 
			// XData
			// 
			this.XData.HeaderText = "X Data";
			this.XData.Name = "XData";
			this.XData.ReadOnly = true;
			// 
			// YData
			// 
			this.YData.HeaderText = "Y Data";
			this.YData.Name = "YData";
			this.YData.ReadOnly = true;
			// 
			// TabPage5
			// 
			this.TabPage5.Controls.Add(this.RedoButton);
			this.TabPage5.Controls.Add(this.UndoButton);
			this.TabPage5.Controls.Add(this.PictureBoxEraser);
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
			// RedoButton
			// 
			this.RedoButton.Image = global::DataCapturer.Properties.Resources.Redo_icon;
			this.RedoButton.Location = new System.Drawing.Point(15, 354);
			this.RedoButton.Name = "RedoButton";
			this.RedoButton.Size = new System.Drawing.Size(60, 60);
			this.RedoButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.RedoButton.TabIndex = 7;
			this.RedoButton.TabStop = false;
			this.RedoButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RedoButton_MouseDown);
			this.RedoButton.MouseEnter += new System.EventHandler(this.RedoButton_MouseEnter);
			this.RedoButton.MouseLeave += new System.EventHandler(this.RedoButton_MouseLeave);
			this.RedoButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RedoButton_MouseUp);
			// 
			// UndoButton
			// 
			this.UndoButton.Image = global::DataCapturer.Properties.Resources.Undo_icon;
			this.UndoButton.Location = new System.Drawing.Point(15, 157);
			this.UndoButton.Name = "UndoButton";
			this.UndoButton.Size = new System.Drawing.Size(60, 60);
			this.UndoButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.UndoButton.TabIndex = 6;
			this.UndoButton.TabStop = false;
			this.UndoButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UndoButton_MouseDown);
			this.UndoButton.MouseEnter += new System.EventHandler(this.UndoButton_MouseEnter);
			this.UndoButton.MouseLeave += new System.EventHandler(this.UndoButton_MouseLeave);
			this.UndoButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.UndoButton_MouseUp);
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
			this.TabControlMain.SelectedIndex = 1;
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
			// DataCapturer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(1015, 776);
			this.Controls.Add(this.PictureBoxWarnGetAxis);
			this.Controls.Add(this.PictureBoxWarnSetAxLim);
			this.Controls.Add(this.ButtonBack);
			this.Controls.Add(this.ButtonNext);
			this.Controls.Add(this.TabControlMain);
			this.Location = new System.Drawing.Point(0, 0);
			this.MetroFont = new System.Drawing.Font("Segoe UI Light", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "DataCapturer";
			this.Text = "Data Capturer";
			this.Load += new System.EventHandler(this.DataCapturer_Load);
			this.Resize += new System.EventHandler(this.DataCapturer_Resize);
			((System.ComponentModel.ISupportInitialize)(this.PictureBoxWarnGetAxis)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.PictureBoxWarnSetAxLim)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.PictureBoxGetAxis)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.PictureBoxFilter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.PictureBoxEraser)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.PictureBoxOutput)).EndInit();
			this.TabPage6.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.TabPage5.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.RedoButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.UndoButton)).EndInit();
			this.TabPage4.ResumeLayout(false);
			this.metroPanel1.ResumeLayout(false);
			this.TabPage3.ResumeLayout(false);
			this.TabPage1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.PictureBoxInput)).EndInit();
			this.TabControlMain.ResumeLayout(false);
			this.TabPage2.ResumeLayout(false);
			this.TabPage2.PerformLayout();
			this.ResumeLayout(false);

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
		private System.Windows.Forms.DataGridView dataGridView1;
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
		private MetroFramework.Controls.Slider SliderAxisOffset;
		private MetroFramework.Controls.Slider SliderAxLengthX;
		private MetroFramework.Controls.RangeSlider RangeSliderBlue;
		private MetroFramework.Controls.RangeSlider RangeSliderGreen;
		private MetroFramework.Controls.RangeSlider RangeSliderRed;
		private System.Windows.Forms.PictureBox PictureBoxOutput;
		private System.Windows.Forms.PictureBox PictureBoxEraser;
		private System.Windows.Forms.PictureBox PictureBoxGetAxis;
		private System.Windows.Forms.PictureBox PictureBoxInput;
		private System.Windows.Forms.PictureBox PictureBoxFilter;
		private System.Windows.Forms.PictureBox PictureBoxWarnSetAxLim;
		private System.Windows.Forms.PictureBox PictureBoxWarnGetAxis;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.DataGridViewTextBoxColumn XData;
		private System.Windows.Forms.DataGridViewTextBoxColumn YData;
		private System.Windows.Forms.TextBox  TextBoxYlo;
		private System.Windows.Forms.TextBox  TextBoxYhi;
		private System.Windows.Forms.TextBox  TextBoxXhi;
		private System.Windows.Forms.TextBox  TextBoxXlo;
		private System.Windows.Forms.TextBox  TextBoxXBase;
		private System.Windows.Forms.TextBox  TextBoxYBase;
		private System.Windows.Forms.CheckBox CheckBoxXLog;
		private System.Windows.Forms.CheckBox CheckBoxYLog;
		private MetroFramework.Controls.Slider SliderAxLengthY;
		private System.Windows.Forms.PictureBox RedoButton;
		private System.Windows.Forms.PictureBox UndoButton;
		private MetroFramework.Controls.ImageViewer ImageViewerSetAxLim;
	}
}

