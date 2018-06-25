namespace DataCapturer
{
  partial class StartMovie
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartMovie));
      this.axShockwaveFlash1 = new AxShockwaveFlashObjects.AxShockwaveFlash();
      this.timer1 = new System.Windows.Forms.Timer(this.components);
      this.progressBar1 = new System.Windows.Forms.ProgressBar();
      ((System.ComponentModel.ISupportInitialize)(this.axShockwaveFlash1)).BeginInit();
      this.SuspendLayout();
      // 
      // axShockwaveFlash1
      // 
      this.axShockwaveFlash1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.axShockwaveFlash1.Enabled = true;
      this.axShockwaveFlash1.Location = new System.Drawing.Point(0, 0);
      this.axShockwaveFlash1.Name = "axShockwaveFlash1";
      this.axShockwaveFlash1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axShockwaveFlash1.OcxState")));
      this.axShockwaveFlash1.Size = new System.Drawing.Size(412, 340);
      this.axShockwaveFlash1.TabIndex = 0;
      // 
      // timer1
      // 
      this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
      // 
      // progressBar1
      // 
      this.progressBar1.Location = new System.Drawing.Point(112, 282);
      this.progressBar1.Name = "progressBar1";
      this.progressBar1.Size = new System.Drawing.Size(172, 26);
      this.progressBar1.TabIndex = 1;
      // 
      // StartMovie
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(412, 340);
      this.Controls.Add(this.progressBar1);
      this.Controls.Add(this.axShockwaveFlash1);
      this.Name = "StartMovie";
      this.Text = "StartMovie";
      this.Load += new System.EventHandler(this.StartMovie_Load);
      ((System.ComponentModel.ISupportInitialize)(this.axShockwaveFlash1)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private AxShockwaveFlashObjects.AxShockwaveFlash axShockwaveFlash1;
    private System.Windows.Forms.Timer timer1;
    private System.Windows.Forms.ProgressBar progressBar1;
  }
}