﻿namespace GlanceMain;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        // 释放托盘图标
        NotifyIconManager.Current.Dispose();
        
        _optionsReloadToken?.Dispose();

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            this.btnScreenshots = new Sunny.UI.UISymbolButton();
            this.btnTranslate = new Sunny.UI.UISymbolButton();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.txtOCR = new Sunny.UI.UIRichTextBox();
            this.txtTranslate = new Sunny.UI.UIRichTextBox();
            this.toolTip = new Sunny.UI.UIToolTip(this.components);
            this.styleManager = new Sunny.UI.UIStyleManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnScreenshots
            // 
            this.btnScreenshots.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(190)))), ((int)(((byte)(172)))));
            this.btnScreenshots.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(190)))), ((int)(((byte)(172)))));
            this.btnScreenshots.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(190)))), ((int)(((byte)(172)))));
            this.btnScreenshots.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(203)))), ((int)(((byte)(189)))));
            this.btnScreenshots.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(152)))), ((int)(((byte)(138)))));
            this.btnScreenshots.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(152)))), ((int)(((byte)(138)))));
            this.btnScreenshots.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnScreenshots.Location = new System.Drawing.Point(104, 4);
            this.btnScreenshots.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnScreenshots.Name = "btnScreenshots";
            this.btnScreenshots.RadiusSides = ((Sunny.UI.UICornerRadiusSides)((Sunny.UI.UICornerRadiusSides.LeftTop | Sunny.UI.UICornerRadiusSides.LeftBottom)));
            this.btnScreenshots.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(190)))), ((int)(((byte)(172)))));
            this.btnScreenshots.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(203)))), ((int)(((byte)(189)))));
            this.btnScreenshots.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(152)))), ((int)(((byte)(138)))));
            this.btnScreenshots.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(152)))), ((int)(((byte)(138)))));
            this.btnScreenshots.Size = new System.Drawing.Size(30, 28);
            this.btnScreenshots.Style = Sunny.UI.UIStyle.Colorful;
            this.btnScreenshots.StyleCustomMode = true;
            this.btnScreenshots.Symbol = 57350;
            this.btnScreenshots.TabIndex = 1;
            this.btnScreenshots.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.btnScreenshots.Click += new System.EventHandler(this.btnScreenshots_Click);
            // 
            // btnTranslate
            // 
            this.btnTranslate.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(190)))), ((int)(((byte)(172)))));
            this.btnTranslate.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(190)))), ((int)(((byte)(172)))));
            this.btnTranslate.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(203)))), ((int)(((byte)(189)))));
            this.btnTranslate.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(152)))), ((int)(((byte)(138)))));
            this.btnTranslate.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(152)))), ((int)(((byte)(138)))));
            this.btnTranslate.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnTranslate.Location = new System.Drawing.Point(134, 4);
            this.btnTranslate.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnTranslate.Name = "btnTranslate";
            this.btnTranslate.RadiusSides = Sunny.UI.UICornerRadiusSides.None;
            this.btnTranslate.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(190)))), ((int)(((byte)(172)))));
            this.btnTranslate.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(203)))), ((int)(((byte)(189)))));
            this.btnTranslate.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(152)))), ((int)(((byte)(138)))));
            this.btnTranslate.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(152)))), ((int)(((byte)(138)))));
            this.btnTranslate.Size = new System.Drawing.Size(30, 28);
            this.btnTranslate.Style = Sunny.UI.UIStyle.Colorful;
            this.btnTranslate.StyleCustomMode = true;
            this.btnTranslate.Symbol = 362845;
            this.btnTranslate.TabIndex = 2;
            this.btnTranslate.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.btnTranslate.Click += new System.EventHandler(this.btnTranslate_Click);
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 35);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.txtOCR);
            this.splitContainer.Panel1MinSize = 300;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.AutoScroll = true;
            this.splitContainer.Panel2.Controls.Add(this.txtTranslate);
            this.splitContainer.Panel2Collapsed = false;
            this.splitContainer.Panel2MinSize = 300;
            this.splitContainer.Size = new System.Drawing.Size(800, 415);
            this.splitContainer.SplitterDistance = 395;
            this.splitContainer.SplitterWidth = 1;
            this.splitContainer.TabIndex = 3;
            // 
            // txtOCR
            // 
            this.txtOCR.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOCR.FillColor = System.Drawing.Color.White;
            this.txtOCR.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
            this.txtOCR.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtOCR.Location = new System.Drawing.Point(0, 0);
            this.txtOCR.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtOCR.MinimumSize = new System.Drawing.Size(1, 1);
            this.txtOCR.Name = "txtOCR";
            this.txtOCR.Padding = new System.Windows.Forms.Padding(2);
            this.txtOCR.RadiusSides = Sunny.UI.UICornerRadiusSides.None;
            this.txtOCR.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.txtOCR.ScrollBarColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.txtOCR.ShowText = false;
            this.txtOCR.Size = new System.Drawing.Size(800, 415);
            this.txtOCR.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.txtOCR.TabIndex = 0;
            this.txtOCR.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.txtOCR.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // txtTranslate
            // 
            this.txtTranslate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTranslate.FillColor = System.Drawing.Color.White;
            this.txtTranslate.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
            this.txtTranslate.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtTranslate.Location = new System.Drawing.Point(0, 0);
            this.txtTranslate.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtTranslate.MinimumSize = new System.Drawing.Size(1, 1);
            this.txtTranslate.Name = "txtTranslate";
            this.txtTranslate.Padding = new System.Windows.Forms.Padding(2);
            this.txtTranslate.RadiusSides = Sunny.UI.UICornerRadiusSides.None;
            this.txtTranslate.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.txtTranslate.ScrollBarColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.txtTranslate.ShowText = false;
            this.txtTranslate.Size = new System.Drawing.Size(96, 100);
            this.txtTranslate.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.txtTranslate.TabIndex = 0;
            this.txtTranslate.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.txtTranslate.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // toolTip
            // 
            this.toolTip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.toolTip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.toolTip.OwnerDraw = true;
            // 
            // styleManager
            // 
            this.styleManager.Style = Sunny.UI.UIStyle.LayuiGreen;
            // 
            // MainForm
            // 
            this.AllowAddControlOnTitle = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.ControlBoxFillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.Controls.Add(this.btnTranslate);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.btnScreenshots);
            this.Name = "MainForm";
            this.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.Text = "一目十行";
            this.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 800, 450);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

    }

    #endregion

    private Sunny.UI.UISymbolButton btnScreenshots;
    private Sunny.UI.UISymbolButton btnTranslate;
    private SplitContainer splitContainer;
    private Sunny.UI.UIRichTextBox txtOCR;
    private Sunny.UI.UIRichTextBox txtTranslate;
    private Sunny.UI.UIToolTip toolTip;
    private Sunny.UI.UIStyleManager styleManager;
}