
namespace Demo.WindowsForms
{
   partial class MainForm
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
         if(disposing && (components != null))
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_setEnd = new System.Windows.Forms.Button();
            this.btn_setStart = new System.Windows.Forms.Button();
            this.btnCursor = new System.Windows.Forms.Button();
            this.btnMarker = new System.Windows.Forms.Button();
            this.btnGoTo = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxGeo = new System.Windows.Forms.TextBox();
            this.textBoxLng = new System.Windows.Forms.TextBox();
            this.textBoxLat = new System.Windows.Forms.TextBox();
            this.btnReload = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox_Show = new System.Windows.Forms.GroupBox();
            this.ckb_show_scale = new System.Windows.Forms.CheckBox();
            this.checkBoxShowGeoGrid = new System.Windows.Forms.CheckBox();
            this.checkBoxShowCenterMarker = new System.Windows.Forms.CheckBox();
            this.checkBox1_Infobox = new System.Windows.Forms.CheckBox();
            this.checkBox_Routes = new System.Windows.Forms.CheckBox();
            this.checkBoxCurrentMarker = new System.Windows.Forms.CheckBox();
            this.checkBox_Markers = new System.Windows.Forms.CheckBox();
            this.checkBoxDebug = new System.Windows.Forms.CheckBox();
            this.groupBoxProgress = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txe_direction = new System.Windows.Forms.TextBox();
            this.btn_add_dfline = new System.Windows.Forms.Button();
            this.btn_clear_routes = new System.Windows.Forms.Button();
            this.btn_addline = new System.Windows.Forms.Button();
            this.btnAddRoute = new System.Windows.Forms.Button();
            this.btnSetEnd = new System.Windows.Forms.Button();
            this.btnSetStart = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnCenterCursor = new System.Windows.Forms.Button();
            this.btnAddPushpin = new System.Windows.Forms.Button();
            this.checkBoxPlacemarkInfo = new System.Windows.Forms.CheckBox();
            this.btnClearAll = new System.Windows.Forms.Button();
            this.btnZoomCenter = new System.Windows.Forms.Button();
            this.btnAddMarker = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button_centerMarker = new System.Windows.Forms.Button();
            this.checkBoxTileHost = new System.Windows.Forms.CheckBox();
            this.checkBoxCanDrag = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxMode = new System.Windows.Forms.ComboBox();
            this.comboBoxMapType = new System.Windows.Forms.ComboBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.cacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.routingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.geocodingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.prefetchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MainMap = new GMap.NET.WindowsForms.GMapControl();
            this.groupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox_Show.SuspendLayout();
            this.groupBoxProgress.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_setEnd);
            this.groupBox1.Controls.Add(this.btn_setStart);
            this.groupBox1.Controls.Add(this.btnCursor);
            this.groupBox1.Controls.Add(this.btnMarker);
            this.groupBox1.Controls.Add(this.btnGoTo);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBoxGeo);
            this.groupBox1.Controls.Add(this.textBoxLng);
            this.groupBox1.Controls.Add(this.textBoxLat);
            this.groupBox1.Location = new System.Drawing.Point(14, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(203, 149);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Coordinates";
            // 
            // btn_setEnd
            // 
            this.btn_setEnd.Location = new System.Drawing.Point(94, 121);
            this.btn_setEnd.Name = "btn_setEnd";
            this.btn_setEnd.Size = new System.Drawing.Size(67, 22);
            this.btn_setEnd.TabIndex = 10;
            this.btn_setEnd.Text = "Set End";
            this.btn_setEnd.UseVisualStyleBackColor = true;
            this.btn_setEnd.Click += new System.EventHandler(this.btnSetEnd_Click);
            // 
            // btn_setStart
            // 
            this.btn_setStart.Location = new System.Drawing.Point(9, 121);
            this.btn_setStart.Name = "btn_setStart";
            this.btn_setStart.Size = new System.Drawing.Size(67, 22);
            this.btn_setStart.TabIndex = 9;
            this.btn_setStart.Text = "SetStart";
            this.btn_setStart.UseVisualStyleBackColor = true;
            this.btn_setStart.Click += new System.EventHandler(this.btnSetStart_Click);
            // 
            // btnCursor
            // 
            this.btnCursor.Location = new System.Drawing.Point(113, 97);
            this.btnCursor.Name = "btnCursor";
            this.btnCursor.Size = new System.Drawing.Size(48, 22);
            this.btnCursor.TabIndex = 8;
            this.btnCursor.Text = "Cursor";
            this.btnCursor.UseVisualStyleBackColor = true;
            this.btnCursor.Click += new System.EventHandler(this.btnCursor_Click);
            // 
            // btnMarker
            // 
            this.btnMarker.Location = new System.Drawing.Point(60, 97);
            this.btnMarker.Name = "btnMarker";
            this.btnMarker.Size = new System.Drawing.Size(48, 22);
            this.btnMarker.TabIndex = 7;
            this.btnMarker.Text = "Marker";
            this.btnMarker.UseVisualStyleBackColor = true;
            this.btnMarker.Click += new System.EventHandler(this.btnMarker_Click);
            // 
            // btnGoTo
            // 
            this.btnGoTo.Location = new System.Drawing.Point(9, 97);
            this.btnGoTo.Name = "btnGoTo";
            this.btnGoTo.Size = new System.Drawing.Size(48, 22);
            this.btnGoTo.TabIndex = 6;
            this.btnGoTo.Text = "Go To";
            this.btnGoTo.UseVisualStyleBackColor = true;
            this.btnGoTo.Click += new System.EventHandler(this.btnGoTo_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(163, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Geo";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(163, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Lng";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(163, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(22, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Lat";
            // 
            // textBoxGeo
            // 
            this.textBoxGeo.Location = new System.Drawing.Point(9, 71);
            this.textBoxGeo.Name = "textBoxGeo";
            this.textBoxGeo.Size = new System.Drawing.Size(154, 20);
            this.textBoxGeo.TabIndex = 2;
            this.textBoxGeo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxGeo_KeyPress);
            // 
            // textBoxLng
            // 
            this.textBoxLng.Location = new System.Drawing.Point(9, 45);
            this.textBoxLng.Name = "textBoxLng";
            this.textBoxLng.Size = new System.Drawing.Size(154, 20);
            this.textBoxLng.TabIndex = 1;
            this.textBoxLng.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxLng_KeyPress);
            // 
            // textBoxLat
            // 
            this.textBoxLat.Location = new System.Drawing.Point(9, 19);
            this.textBoxLat.Name = "textBoxLat";
            this.textBoxLat.Size = new System.Drawing.Size(154, 20);
            this.textBoxLat.TabIndex = 0;
            this.textBoxLat.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxLat_KeyPress);
            // 
            // btnReload
            // 
            this.btnReload.Location = new System.Drawing.Point(9, 99);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(55, 22);
            this.btnReload.TabIndex = 7;
            this.btnReload.Text = "Reload";
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.groupBox_Show);
            this.panel2.Controls.Add(this.groupBoxProgress);
            this.panel2.Controls.Add(this.groupBox4);
            this.panel2.Controls.Add(this.groupBox3);
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(922, 24);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(233, 732);
            this.panel2.TabIndex = 3;
            // 
            // groupBox_Show
            // 
            this.groupBox_Show.Controls.Add(this.ckb_show_scale);
            this.groupBox_Show.Controls.Add(this.checkBoxShowGeoGrid);
            this.groupBox_Show.Controls.Add(this.checkBoxShowCenterMarker);
            this.groupBox_Show.Controls.Add(this.checkBox1_Infobox);
            this.groupBox_Show.Controls.Add(this.checkBox_Routes);
            this.groupBox_Show.Controls.Add(this.checkBoxCurrentMarker);
            this.groupBox_Show.Controls.Add(this.checkBox_Markers);
            this.groupBox_Show.Controls.Add(this.checkBoxDebug);
            this.groupBox_Show.Location = new System.Drawing.Point(14, 565);
            this.groupBox_Show.Name = "groupBox_Show";
            this.groupBox_Show.Size = new System.Drawing.Size(203, 83);
            this.groupBox_Show.TabIndex = 6;
            this.groupBox_Show.TabStop = false;
            this.groupBox_Show.Text = "Show";
            // 
            // ckb_show_scale
            // 
            this.ckb_show_scale.AutoSize = true;
            this.ckb_show_scale.Location = new System.Drawing.Point(75, 60);
            this.ckb_show_scale.Name = "ckb_show_scale";
            this.ckb_show_scale.Size = new System.Drawing.Size(53, 17);
            this.ckb_show_scale.TabIndex = 9;
            this.ckb_show_scale.Text = "Scale";
            this.ckb_show_scale.UseVisualStyleBackColor = true;
            this.ckb_show_scale.CheckedChanged += new System.EventHandler(this.ckb_show_scale_CheckedChanged);
            // 
            // checkBoxShowGeoGrid
            // 
            this.checkBoxShowGeoGrid.AutoSize = true;
            this.checkBoxShowGeoGrid.Location = new System.Drawing.Point(12, 38);
            this.checkBoxShowGeoGrid.Name = "checkBoxShowGeoGrid";
            this.checkBoxShowGeoGrid.Size = new System.Drawing.Size(63, 17);
            this.checkBoxShowGeoGrid.TabIndex = 8;
            this.checkBoxShowGeoGrid.Text = "Geogrid";
            this.checkBoxShowGeoGrid.UseVisualStyleBackColor = true;
            this.checkBoxShowGeoGrid.CheckedChanged += new System.EventHandler(this.checkBoxShowGeoGrid_CheckedChanged);
            // 
            // checkBoxShowCenterMarker
            // 
            this.checkBoxShowCenterMarker.AutoSize = true;
            this.checkBoxShowCenterMarker.Checked = true;
            this.checkBoxShowCenterMarker.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowCenterMarker.Location = new System.Drawing.Point(133, 38);
            this.checkBoxShowCenterMarker.Name = "checkBoxShowCenterMarker";
            this.checkBoxShowCenterMarker.Size = new System.Drawing.Size(92, 17);
            this.checkBoxShowCenterMarker.TabIndex = 7;
            this.checkBoxShowCenterMarker.Text = "Center marker";
            this.checkBoxShowCenterMarker.UseVisualStyleBackColor = true;
            this.checkBoxShowCenterMarker.CheckedChanged += new System.EventHandler(this.checkBoxShowCenterMarker_CheckedChanged);
            // 
            // checkBox1_Infobox
            // 
            this.checkBox1_Infobox.AutoSize = true;
            this.checkBox1_Infobox.Checked = true;
            this.checkBox1_Infobox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1_Infobox.Location = new System.Drawing.Point(133, 16);
            this.checkBox1_Infobox.Name = "checkBox1_Infobox";
            this.checkBox1_Infobox.Size = new System.Drawing.Size(61, 17);
            this.checkBox1_Infobox.TabIndex = 2;
            this.checkBox1_Infobox.Text = "Infobox";
            this.checkBox1_Infobox.UseVisualStyleBackColor = true;
            this.checkBox1_Infobox.CheckedChanged += new System.EventHandler(this.checkBox1_Infobox_CheckedChanged);
            // 
            // checkBox_Routes
            // 
            this.checkBox_Routes.AutoSize = true;
            this.checkBox_Routes.Checked = true;
            this.checkBox_Routes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_Routes.Location = new System.Drawing.Point(75, 16);
            this.checkBox_Routes.Name = "checkBox_Routes";
            this.checkBox_Routes.Size = new System.Drawing.Size(60, 17);
            this.checkBox_Routes.TabIndex = 1;
            this.checkBox_Routes.Text = "Routes";
            this.checkBox_Routes.UseVisualStyleBackColor = true;
            this.checkBox_Routes.CheckedChanged += new System.EventHandler(this.checkBox_Routes_CheckedChanged);
            // 
            // checkBoxCurrentMarker
            // 
            this.checkBoxCurrentMarker.AutoSize = true;
            this.checkBoxCurrentMarker.Checked = true;
            this.checkBoxCurrentMarker.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCurrentMarker.Location = new System.Drawing.Point(75, 38);
            this.checkBoxCurrentMarker.Name = "checkBoxCurrentMarker";
            this.checkBoxCurrentMarker.Size = new System.Drawing.Size(56, 17);
            this.checkBoxCurrentMarker.TabIndex = 6;
            this.checkBoxCurrentMarker.Text = "Cursor";
            this.checkBoxCurrentMarker.UseVisualStyleBackColor = true;
            this.checkBoxCurrentMarker.CheckedChanged += new System.EventHandler(this.checkBoxCurrentMarker_CheckedChanged);
            // 
            // checkBox_Markers
            // 
            this.checkBox_Markers.AutoSize = true;
            this.checkBox_Markers.Checked = true;
            this.checkBox_Markers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_Markers.Location = new System.Drawing.Point(12, 16);
            this.checkBox_Markers.Name = "checkBox_Markers";
            this.checkBox_Markers.Size = new System.Drawing.Size(64, 17);
            this.checkBox_Markers.TabIndex = 0;
            this.checkBox_Markers.Text = "Markers";
            this.checkBox_Markers.UseVisualStyleBackColor = true;
            this.checkBox_Markers.CheckedChanged += new System.EventHandler(this.checkBox_Markers_CheckedChanged);
            // 
            // checkBoxDebug
            // 
            this.checkBoxDebug.AutoSize = true;
            this.checkBoxDebug.Location = new System.Drawing.Point(12, 60);
            this.checkBoxDebug.Name = "checkBoxDebug";
            this.checkBoxDebug.Size = new System.Drawing.Size(45, 17);
            this.checkBoxDebug.TabIndex = 4;
            this.checkBoxDebug.Text = "Grid";
            this.checkBoxDebug.UseVisualStyleBackColor = true;
            this.checkBoxDebug.CheckedChanged += new System.EventHandler(this.checkBoxDebug_CheckedChanged);
            // 
            // groupBoxProgress
            // 
            this.groupBoxProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxProgress.Controls.Add(this.progressBar1);
            this.groupBoxProgress.Location = new System.Drawing.Point(14, 667);
            this.groupBoxProgress.Name = "groupBoxProgress";
            this.groupBoxProgress.Size = new System.Drawing.Size(203, 53);
            this.groupBoxProgress.TabIndex = 5;
            this.groupBoxProgress.TabStop = false;
            this.groupBoxProgress.Text = "Progress";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(10, 16);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(167, 23);
            this.progressBar1.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.txe_direction);
            this.groupBox4.Controls.Add(this.btn_add_dfline);
            this.groupBox4.Controls.Add(this.btn_clear_routes);
            this.groupBox4.Controls.Add(this.btn_addline);
            this.groupBox4.Controls.Add(this.btnAddRoute);
            this.groupBox4.Controls.Add(this.btnSetEnd);
            this.groupBox4.Controls.Add(this.btnSetStart);
            this.groupBox4.Location = new System.Drawing.Point(14, 412);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(203, 152);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Routes";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(150, 102);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "degree";
            // 
            // txe_direction
            // 
            this.txe_direction.Location = new System.Drawing.Point(96, 99);
            this.txe_direction.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.txe_direction.Name = "txe_direction";
            this.txe_direction.Size = new System.Drawing.Size(45, 20);
            this.txe_direction.TabIndex = 10;
            this.txe_direction.Text = "0";
            this.txe_direction.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btn_add_dfline
            // 
            this.btn_add_dfline.Location = new System.Drawing.Point(9, 99);
            this.btn_add_dfline.Name = "btn_add_dfline";
            this.btn_add_dfline.Size = new System.Drawing.Size(79, 22);
            this.btn_add_dfline.TabIndex = 9;
            this.btn_add_dfline.Text = "Add DF Line";
            this.btn_add_dfline.UseVisualStyleBackColor = true;
            this.btn_add_dfline.Click += new System.EventHandler(this.btn_add_dfline_Click);
            // 
            // btn_clear_routes
            // 
            this.btn_clear_routes.Location = new System.Drawing.Point(94, 125);
            this.btn_clear_routes.Name = "btn_clear_routes";
            this.btn_clear_routes.Size = new System.Drawing.Size(96, 22);
            this.btn_clear_routes.TabIndex = 8;
            this.btn_clear_routes.Text = "Clear Routes";
            this.btn_clear_routes.UseVisualStyleBackColor = true;
            this.btn_clear_routes.Click += new System.EventHandler(this.btn_clear_routes_Click);
            // 
            // btn_addline
            // 
            this.btn_addline.Location = new System.Drawing.Point(9, 72);
            this.btn_addline.Name = "btn_addline";
            this.btn_addline.Size = new System.Drawing.Size(164, 22);
            this.btn_addline.TabIndex = 7;
            this.btn_addline.Text = "Add Line";
            this.btn_addline.UseVisualStyleBackColor = true;
            this.btn_addline.Click += new System.EventHandler(this.btn_addline_Click);
            // 
            // btnAddRoute
            // 
            this.btnAddRoute.Location = new System.Drawing.Point(9, 46);
            this.btnAddRoute.Name = "btnAddRoute";
            this.btnAddRoute.Size = new System.Drawing.Size(164, 22);
            this.btnAddRoute.TabIndex = 6;
            this.btnAddRoute.Text = "Add Route";
            this.btnAddRoute.UseVisualStyleBackColor = true;
            this.btnAddRoute.Click += new System.EventHandler(this.btnAddRoute_Click);
            // 
            // btnSetEnd
            // 
            this.btnSetEnd.Location = new System.Drawing.Point(94, 19);
            this.btnSetEnd.Name = "btnSetEnd";
            this.btnSetEnd.Size = new System.Drawing.Size(79, 22);
            this.btnSetEnd.TabIndex = 5;
            this.btnSetEnd.Text = "Set End";
            this.btnSetEnd.UseVisualStyleBackColor = true;
            this.btnSetEnd.Click += new System.EventHandler(this.btnSetEnd_Click);
            // 
            // btnSetStart
            // 
            this.btnSetStart.Location = new System.Drawing.Point(9, 19);
            this.btnSetStart.Name = "btnSetStart";
            this.btnSetStart.Size = new System.Drawing.Size(79, 22);
            this.btnSetStart.TabIndex = 4;
            this.btnSetStart.Text = "SetStart";
            this.btnSetStart.UseVisualStyleBackColor = true;
            this.btnSetStart.Click += new System.EventHandler(this.btnSetStart_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnCenterCursor);
            this.groupBox3.Controls.Add(this.btnAddPushpin);
            this.groupBox3.Controls.Add(this.checkBoxPlacemarkInfo);
            this.groupBox3.Controls.Add(this.btnClearAll);
            this.groupBox3.Controls.Add(this.btnZoomCenter);
            this.groupBox3.Controls.Add(this.btnAddMarker);
            this.groupBox3.Location = new System.Drawing.Point(14, 302);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(203, 104);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Markers";
            // 
            // btnCenterCursor
            // 
            this.btnCenterCursor.Location = new System.Drawing.Point(12, 75);
            this.btnCenterCursor.Name = "btnCenterCursor";
            this.btnCenterCursor.Size = new System.Drawing.Size(79, 22);
            this.btnCenterCursor.TabIndex = 5;
            this.btnCenterCursor.Text = "Center Cursor";
            this.btnCenterCursor.UseVisualStyleBackColor = true;
            this.btnCenterCursor.Click += new System.EventHandler(this.btnCenterCursor_Click);
            // 
            // btnAddPushpin
            // 
            this.btnAddPushpin.Location = new System.Drawing.Point(94, 19);
            this.btnAddPushpin.Name = "btnAddPushpin";
            this.btnAddPushpin.Size = new System.Drawing.Size(79, 22);
            this.btnAddPushpin.TabIndex = 4;
            this.btnAddPushpin.Text = "Add Pushpin";
            this.btnAddPushpin.UseVisualStyleBackColor = true;
            this.btnAddPushpin.Click += new System.EventHandler(this.btnAddPushpin_Click);
            // 
            // checkBoxPlacemarkInfo
            // 
            this.checkBoxPlacemarkInfo.AutoSize = true;
            this.checkBoxPlacemarkInfo.Location = new System.Drawing.Point(94, 51);
            this.checkBoxPlacemarkInfo.Name = "checkBoxPlacemarkInfo";
            this.checkBoxPlacemarkInfo.Size = new System.Drawing.Size(74, 17);
            this.checkBoxPlacemarkInfo.TabIndex = 3;
            this.checkBoxPlacemarkInfo.Text = "Place Info";
            this.checkBoxPlacemarkInfo.UseVisualStyleBackColor = true;
            // 
            // btnClearAll
            // 
            this.btnClearAll.Location = new System.Drawing.Point(94, 76);
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size(91, 22);
            this.btnClearAll.TabIndex = 2;
            this.btnClearAll.Text = "Clear Markers";
            this.btnClearAll.UseVisualStyleBackColor = true;
            this.btnClearAll.Click += new System.EventHandler(this.btnClearAllMarkers_Click);
            // 
            // btnZoomCenter
            // 
            this.btnZoomCenter.Location = new System.Drawing.Point(9, 47);
            this.btnZoomCenter.Name = "btnZoomCenter";
            this.btnZoomCenter.Size = new System.Drawing.Size(79, 22);
            this.btnZoomCenter.TabIndex = 1;
            this.btnZoomCenter.Text = "Zoom Center";
            this.btnZoomCenter.UseVisualStyleBackColor = true;
            this.btnZoomCenter.Click += new System.EventHandler(this.btnZoomCenter_Click);
            // 
            // btnAddMarker
            // 
            this.btnAddMarker.Location = new System.Drawing.Point(9, 18);
            this.btnAddMarker.Name = "btnAddMarker";
            this.btnAddMarker.Size = new System.Drawing.Size(79, 22);
            this.btnAddMarker.TabIndex = 0;
            this.btnAddMarker.Text = "Add Marker";
            this.btnAddMarker.UseVisualStyleBackColor = true;
            this.btnAddMarker.Click += new System.EventHandler(this.btnAddMarker_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnReload);
            this.groupBox2.Controls.Add(this.button_centerMarker);
            this.groupBox2.Controls.Add(this.checkBoxTileHost);
            this.groupBox2.Controls.Add(this.checkBoxCanDrag);
            this.groupBox2.Controls.Add(this.btnSave);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.comboBoxMode);
            this.groupBox2.Controls.Add(this.comboBoxMapType);
            this.groupBox2.Location = new System.Drawing.Point(14, 167);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(203, 129);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Gmap";
            // 
            // button_centerMarker
            // 
            this.button_centerMarker.Location = new System.Drawing.Point(72, 99);
            this.button_centerMarker.Name = "button_centerMarker";
            this.button_centerMarker.Size = new System.Drawing.Size(55, 22);
            this.button_centerMarker.TabIndex = 8;
            this.button_centerMarker.Text = "Center marker";
            this.button_centerMarker.UseVisualStyleBackColor = true;
            this.button_centerMarker.Click += new System.EventHandler(this.button_centerMarker_Click);
            // 
            // checkBoxTileHost
            // 
            this.checkBoxTileHost.AutoSize = true;
            this.checkBoxTileHost.Location = new System.Drawing.Point(95, 73);
            this.checkBoxTileHost.Name = "checkBoxTileHost";
            this.checkBoxTileHost.Size = new System.Drawing.Size(68, 17);
            this.checkBoxTileHost.TabIndex = 7;
            this.checkBoxTileHost.Text = "Tile Host";
            this.checkBoxTileHost.UseVisualStyleBackColor = true;
            this.checkBoxTileHost.CheckedChanged += new System.EventHandler(this.checkBoxTileHost_CheckedChanged);
            // 
            // checkBoxCanDrag
            // 
            this.checkBoxCanDrag.AutoSize = true;
            this.checkBoxCanDrag.Checked = true;
            this.checkBoxCanDrag.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCanDrag.Location = new System.Drawing.Point(9, 76);
            this.checkBoxCanDrag.Name = "checkBoxCanDrag";
            this.checkBoxCanDrag.Size = new System.Drawing.Size(73, 17);
            this.checkBoxCanDrag.TabIndex = 5;
            this.checkBoxCanDrag.Text = "Drag Map";
            this.checkBoxCanDrag.UseVisualStyleBackColor = true;
            this.checkBoxCanDrag.CheckedChanged += new System.EventHandler(this.checkBoxCanDrag_CheckedChanged);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(135, 99);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(55, 22);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(162, 49);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Mode";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(160, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Type";
            // 
            // comboBoxMode
            // 
            this.comboBoxMode.FormattingEnabled = true;
            this.comboBoxMode.Location = new System.Drawing.Point(9, 46);
            this.comboBoxMode.Name = "comboBoxMode";
            this.comboBoxMode.Size = new System.Drawing.Size(151, 21);
            this.comboBoxMode.TabIndex = 1;
            this.comboBoxMode.DropDownClosed += new System.EventHandler(this.comboBoxMode_DropDownClosed);
            // 
            // comboBoxMapType
            // 
            this.comboBoxMapType.FormattingEnabled = true;
            this.comboBoxMapType.Location = new System.Drawing.Point(9, 19);
            this.comboBoxMapType.Name = "comboBoxMapType";
            this.comboBoxMapType.Size = new System.Drawing.Size(151, 21);
            this.comboBoxMapType.TabIndex = 0;
            this.comboBoxMapType.DropDownClosed += new System.EventHandler(this.comboBoxMapType_DropDownClosed);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 756);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1155, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(42, 17);
            this.toolStripStatusLabel2.Text = "Zoom:";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.cacheToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1155, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.newToolStripMenuItem.Text = "&New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveAsToolStripMenuItem.Text = "Save &as";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyPageToolStripMenuItem,
            this.saveToolStripMenuItem1});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // copyPageToolStripMenuItem
            // 
            this.copyPageToolStripMenuItem.Name = "copyPageToolStripMenuItem";
            this.copyPageToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.copyPageToolStripMenuItem.Text = "&Copy page";
            this.copyPageToolStripMenuItem.Click += new System.EventHandler(this.copyPageToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem1
            // 
            this.saveToolStripMenuItem1.Name = "saveToolStripMenuItem1";
            this.saveToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.saveToolStripMenuItem1.Text = "&Save page";
            this.saveToolStripMenuItem1.Click += new System.EventHandler(this.saveToolStripMenuItem1_Click);
            // 
            // cacheToolStripMenuItem
            // 
            this.cacheToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.routingToolStripMenuItem,
            this.geocodingToolStripMenuItem,
            this.toolStripSeparator2,
            this.exportToolStripMenuItem,
            this.importToolStripMenuItem,
            this.toolStripSeparator3,
            this.clearToolStripMenuItem,
            this.prefetchToolStripMenuItem});
            this.cacheToolStripMenuItem.Name = "cacheToolStripMenuItem";
            this.cacheToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.cacheToolStripMenuItem.Text = "&Cache";
            // 
            // routingToolStripMenuItem
            // 
            this.routingToolStripMenuItem.Name = "routingToolStripMenuItem";
            this.routingToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.routingToolStripMenuItem.Text = "&Routing";
            this.routingToolStripMenuItem.Click += new System.EventHandler(this.routingToolStripMenuItem_Click);
            // 
            // geocodingToolStripMenuItem
            // 
            this.geocodingToolStripMenuItem.Name = "geocodingToolStripMenuItem";
            this.geocodingToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.geocodingToolStripMenuItem.Text = "&Geocoding";
            this.geocodingToolStripMenuItem.Click += new System.EventHandler(this.geocodingToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(129, 6);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.exportToolStripMenuItem.Text = "&Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.importToolStripMenuItem.Text = "&Import";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(129, 6);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.clearToolStripMenuItem.Text = "&Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.btnCacheClear_Click);
            // 
            // prefetchToolStripMenuItem
            // 
            this.prefetchToolStripMenuItem.Name = "prefetchToolStripMenuItem";
            this.prefetchToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.prefetchToolStripMenuItem.Text = "&Prefetch";
            this.prefetchToolStripMenuItem.Click += new System.EventHandler(this.btnCachePrefetch_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // MainMap
            // 
            this.MainMap.Bearing = 0F;
            this.MainMap.CanDragMap = true;
            this.MainMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainMap.EmptyTileColor = System.Drawing.Color.Navy;
            this.MainMap.GrayScaleMode = false;
            this.MainMap.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.MainMap.LabelEnabled = true;
            this.MainMap.LevelsKeepInMemory = 5;
            this.MainMap.LinesEnabled = true;
            this.MainMap.Location = new System.Drawing.Point(0, 24);
            this.MainMap.MarkersEnabled = true;
            this.MainMap.MaxZoom = 2;
            this.MainMap.MinZoom = 2;
            this.MainMap.MouseWheelZoomEnabled = true;
            this.MainMap.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.MainMap.Name = "MainMap";
            this.MainMap.NegativeMode = false;
            this.MainMap.PolygonsEnabled = true;
            this.MainMap.RetryLoadTile = 0;
            this.MainMap.RoutesEnabled = true;
            this.MainMap.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.MainMap.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.MainMap.ShowCenter = true;
            this.MainMap.ShowTileGridLines = false;
            this.MainMap.Size = new System.Drawing.Size(922, 732);
            this.MainMap.TabIndex = 4;
            this.MainMap.Zoom = 0D;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1155, 778);
            this.Controls.Add(this.MainMap);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(554, 106);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GMap.NET - Great Maps for Windows Forms";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainForm_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.groupBox_Show.ResumeLayout(false);
            this.groupBox_Show.PerformLayout();
            this.groupBoxProgress.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

      }

      #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textBoxLat;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBoxGeo;
        private System.Windows.Forms.TextBox textBoxLng;
        public GMap.NET.WindowsForms.GMapControl MainMap;
        private System.Windows.Forms.ComboBox comboBoxMapType;
        private System.Windows.Forms.ComboBox comboBoxMode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBoxDebug;
        private System.Windows.Forms.Button btnAddMarker;
        private System.Windows.Forms.Button btnClearAll;
        private System.Windows.Forms.Button btnZoomCenter;
        private System.Windows.Forms.CheckBox checkBoxPlacemarkInfo;
        private System.Windows.Forms.Button btnAddRoute;
        private System.Windows.Forms.Button btnSetEnd;
        private System.Windows.Forms.Button btnSetStart;
        private System.Windows.Forms.GroupBox groupBoxProgress;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnGoTo;
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox checkBoxCanDrag;
        private System.Windows.Forms.CheckBox checkBoxCurrentMarker;
        private System.Windows.Forms.CheckBox checkBoxTileHost;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Button btn_addline;
        private System.Windows.Forms.Button btn_clear_routes;
        private System.Windows.Forms.Button btnAddPushpin;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.GroupBox groupBox_Show;
        private System.Windows.Forms.CheckBox checkBox_Markers;
        private System.Windows.Forms.CheckBox checkBox1_Infobox;
        private System.Windows.Forms.CheckBox checkBox_Routes;
        private System.Windows.Forms.Button button_centerMarker;
        private System.Windows.Forms.Button btnMarker;
        private System.Windows.Forms.Button btnCenterCursor;
        private System.Windows.Forms.Button btnCursor;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyPageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cacheToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem routingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem geocodingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem prefetchToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBoxShowCenterMarker;
        private System.Windows.Forms.CheckBox checkBoxShowGeoGrid;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.CheckBox ckb_show_scale;
        private System.Windows.Forms.Button btn_setEnd;
        private System.Windows.Forms.Button btn_setStart;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txe_direction;
        private System.Windows.Forms.Button btn_add_dfline;
    }
}

