namespace SgxMap
{
    partial class SgxMapControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(SgxMapControl));
            TopPanel = new Panel();
            btn_printMap = new Button();
            btn_exprtMap = new Button();
            btn_copyMap = new Button();
            btn_saveMap = new Button();
            btn_openMap = new Button();
            btn_newMap = new Button();
            BottomPanel = new Panel();
            label_status = new Label();
            SearchPanel = new Panel();
            comboBoxMapType = new ComboBox();
            btn_zoomMap = new Button();
            trackBar_Zoom = new TrackBar();
            btn_shrinkMap = new Button();
            btn_findAddress = new Button();
            controlBox_address = new ComboBox();
            DrawPanel = new Panel();
            btn_draw_circle = new Button();
            btn_draw_ruler = new Button();
            btn_draw_ellipse = new Button();
            btn_draw_textbox = new Button();
            btn_draw_radius = new Button();
            btn_draw_rectangle = new Button();
            btn_draw_freeform = new Button();
            btn_draw_arrow = new Button();
            btn_draw_line = new Button();
            btn_drwa_scribble = new Button();
            btn_draw_pushpin = new Button();
            btn_draw = new Button();
            MainMap = new SgxMapPanel();
            toolTip_draw = new ToolTip(components);
            textBox_editor = new TextBox();
            TopPanel.SuspendLayout();
            BottomPanel.SuspendLayout();
            SearchPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar_Zoom).BeginInit();
            DrawPanel.SuspendLayout();
            SuspendLayout();
            // 
            // TopPanel
            // 
            TopPanel.BackColor = SystemColors.ControlDarkDark;
            TopPanel.BorderStyle = BorderStyle.Fixed3D;
            TopPanel.Controls.Add(btn_printMap);
            TopPanel.Controls.Add(btn_exprtMap);
            TopPanel.Controls.Add(btn_copyMap);
            TopPanel.Controls.Add(btn_saveMap);
            TopPanel.Controls.Add(btn_openMap);
            TopPanel.Controls.Add(btn_newMap);
            TopPanel.Dock = DockStyle.Top;
            TopPanel.ForeColor = SystemColors.ControlDarkDark;
            TopPanel.Location = new Point(0, 0);
            TopPanel.Name = "TopPanel";
            TopPanel.Size = new Size(669, 33);
            TopPanel.TabIndex = 0;
            // 
            // btn_printMap
            // 
            btn_printMap.Image = (Image)resources.GetObject("btn_printMap.Image");
            btn_printMap.Location = new Point(94, 4);
            btn_printMap.Name = "btn_printMap";
            btn_printMap.Size = new Size(28, 23);
            btn_printMap.TabIndex = 5;
            toolTip_draw.SetToolTip(btn_printMap, "Print map");
            btn_printMap.UseVisualStyleBackColor = true;
            // 
            // btn_exprtMap
            // 
            btn_exprtMap.Image = (Image)resources.GetObject("btn_exprtMap.Image");
            btn_exprtMap.Location = new Point(187, 4);
            btn_exprtMap.Name = "btn_exprtMap";
            btn_exprtMap.Size = new Size(28, 23);
            btn_exprtMap.TabIndex = 4;
            toolTip_draw.SetToolTip(btn_exprtMap, "Export map in image file");
            btn_exprtMap.UseVisualStyleBackColor = true;
            // 
            // btn_copyMap
            // 
            btn_copyMap.Image = (Image)resources.GetObject("btn_copyMap.Image");
            btn_copyMap.Location = new Point(154, 4);
            btn_copyMap.Name = "btn_copyMap";
            btn_copyMap.Size = new Size(28, 23);
            btn_copyMap.TabIndex = 3;
            toolTip_draw.SetToolTip(btn_copyMap, "Copy map to clipboard");
            btn_copyMap.UseVisualStyleBackColor = true;
            // 
            // btn_saveMap
            // 
            btn_saveMap.Image = (Image)resources.GetObject("btn_saveMap.Image");
            btn_saveMap.Location = new Point(64, 4);
            btn_saveMap.Name = "btn_saveMap";
            btn_saveMap.Size = new Size(28, 23);
            btn_saveMap.TabIndex = 2;
            toolTip_draw.SetToolTip(btn_saveMap, "Save map in file");
            btn_saveMap.UseVisualStyleBackColor = true;
            // 
            // btn_openMap
            // 
            btn_openMap.Image = (Image)resources.GetObject("btn_openMap.Image");
            btn_openMap.Location = new Point(34, 4);
            btn_openMap.Name = "btn_openMap";
            btn_openMap.Size = new Size(28, 23);
            btn_openMap.TabIndex = 1;
            toolTip_draw.SetToolTip(btn_openMap, "Open a new map");
            btn_openMap.UseVisualStyleBackColor = true;
            // 
            // btn_newMap
            // 
            btn_newMap.Image = Properties.Resources.NEW;
            btn_newMap.Location = new Point(4, 4);
            btn_newMap.Name = "btn_newMap";
            btn_newMap.Size = new Size(28, 23);
            btn_newMap.TabIndex = 0;
            toolTip_draw.SetToolTip(btn_newMap, "Create a new map");
            btn_newMap.UseVisualStyleBackColor = true;
            btn_newMap.Click += btn_newMap_Click;
            // 
            // BottomPanel
            // 
            BottomPanel.BackColor = SystemColors.ControlDarkDark;
            BottomPanel.BorderStyle = BorderStyle.Fixed3D;
            BottomPanel.Controls.Add(label_status);
            BottomPanel.Dock = DockStyle.Bottom;
            BottomPanel.Location = new Point(0, 310);
            BottomPanel.Name = "BottomPanel";
            BottomPanel.Size = new Size(669, 29);
            BottomPanel.TabIndex = 2;
            // 
            // label_status
            // 
            label_status.AutoSize = true;
            label_status.ForeColor = SystemColors.Control;
            label_status.Location = new Point(10, 5);
            label_status.Name = "label_status";
            label_status.Size = new Size(39, 15);
            label_status.TabIndex = 0;
            label_status.Text = "Status";
            // 
            // SearchPanel
            // 
            SearchPanel.BackColor = SystemColors.ControlDarkDark;
            SearchPanel.BorderStyle = BorderStyle.Fixed3D;
            SearchPanel.Controls.Add(comboBoxMapType);
            SearchPanel.Controls.Add(btn_zoomMap);
            SearchPanel.Controls.Add(trackBar_Zoom);
            SearchPanel.Controls.Add(btn_shrinkMap);
            SearchPanel.Controls.Add(btn_findAddress);
            SearchPanel.Controls.Add(controlBox_address);
            SearchPanel.Dock = DockStyle.Top;
            SearchPanel.Location = new Point(0, 33);
            SearchPanel.Name = "SearchPanel";
            SearchPanel.Size = new Size(669, 35);
            SearchPanel.TabIndex = 3;
            // 
            // comboBoxMapType
            // 
            comboBoxMapType.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxMapType.FormattingEnabled = true;
            comboBoxMapType.Location = new Point(431, 3);
            comboBoxMapType.Name = "comboBoxMapType";
            comboBoxMapType.Size = new Size(213, 23);
            comboBoxMapType.Sorted = true;
            comboBoxMapType.TabIndex = 9;
            toolTip_draw.SetToolTip(comboBoxMapType, "Select map provider");
            comboBoxMapType.SelectedValueChanged += comboBoxMapType_SelectedValueChanged;
            // 
            // btn_zoomMap
            // 
            btn_zoomMap.Image = (Image)resources.GetObject("btn_zoomMap.Image");
            btn_zoomMap.Location = new Point(397, 4);
            btn_zoomMap.Name = "btn_zoomMap";
            btn_zoomMap.Size = new Size(28, 23);
            btn_zoomMap.TabIndex = 8;
            toolTip_draw.SetToolTip(btn_zoomMap, "Zoom in");
            btn_zoomMap.UseVisualStyleBackColor = true;
            btn_zoomMap.Click += btn_zoomMap_Click;
            // 
            // trackBar_Zoom
            // 
            trackBar_Zoom.Location = new Point(286, 3);
            trackBar_Zoom.Name = "trackBar_Zoom";
            trackBar_Zoom.Size = new Size(116, 45);
            trackBar_Zoom.TabIndex = 7;
            toolTip_draw.SetToolTip(trackBar_Zoom, "Zoom");
            trackBar_Zoom.Scroll += trackBar_Zoom_Scroll;
            // 
            // btn_shrinkMap
            // 
            btn_shrinkMap.Image = (Image)resources.GetObject("btn_shrinkMap.Image");
            btn_shrinkMap.Location = new Point(261, 5);
            btn_shrinkMap.Name = "btn_shrinkMap";
            btn_shrinkMap.Size = new Size(28, 23);
            btn_shrinkMap.TabIndex = 6;
            toolTip_draw.SetToolTip(btn_shrinkMap, "Zoom out");
            btn_shrinkMap.UseVisualStyleBackColor = true;
            btn_shrinkMap.Click += btn_shrinkMap_Click;
            // 
            // btn_findAddress
            // 
            btn_findAddress.Image = (Image)resources.GetObject("btn_findAddress.Image");
            btn_findAddress.Location = new Point(216, 5);
            btn_findAddress.Name = "btn_findAddress";
            btn_findAddress.Size = new Size(28, 23);
            btn_findAddress.TabIndex = 5;
            toolTip_draw.SetToolTip(btn_findAddress, "Find place");
            btn_findAddress.UseVisualStyleBackColor = true;
            btn_findAddress.Click += btn_findAddress_Click;
            // 
            // controlBox_address
            // 
            controlBox_address.FormattingEnabled = true;
            controlBox_address.Location = new Point(9, 6);
            controlBox_address.Name = "controlBox_address";
            controlBox_address.Size = new Size(206, 23);
            controlBox_address.TabIndex = 0;
            controlBox_address.Text = "Type place or address";
            // 
            // DrawPanel
            // 
            DrawPanel.BackColor = SystemColors.ControlDarkDark;
            DrawPanel.BorderStyle = BorderStyle.Fixed3D;
            DrawPanel.Controls.Add(btn_draw_circle);
            DrawPanel.Controls.Add(btn_draw_ruler);
            DrawPanel.Controls.Add(btn_draw_ellipse);
            DrawPanel.Controls.Add(btn_draw_textbox);
            DrawPanel.Controls.Add(btn_draw_radius);
            DrawPanel.Controls.Add(btn_draw_rectangle);
            DrawPanel.Controls.Add(btn_draw_freeform);
            DrawPanel.Controls.Add(btn_draw_arrow);
            DrawPanel.Controls.Add(btn_draw_line);
            DrawPanel.Controls.Add(btn_drwa_scribble);
            DrawPanel.Controls.Add(btn_draw_pushpin);
            DrawPanel.Controls.Add(btn_draw);
            DrawPanel.Dock = DockStyle.Top;
            DrawPanel.Location = new Point(0, 68);
            DrawPanel.Name = "DrawPanel";
            DrawPanel.Size = new Size(669, 49);
            DrawPanel.TabIndex = 4;
            // 
            // btn_draw_circle
            // 
            btn_draw_circle.Image = (Image)resources.GetObject("btn_draw_circle.Image");
            btn_draw_circle.Location = new Point(486, 5);
            btn_draw_circle.Name = "btn_draw_circle";
            btn_draw_circle.Size = new Size(37, 37);
            btn_draw_circle.TabIndex = 11;
            toolTip_draw.SetToolTip(btn_draw_circle, "Circle");
            btn_draw_circle.UseVisualStyleBackColor = true;
            btn_draw_circle.Click += btn_draw_circle_Click;
            // 
            // btn_draw_ruler
            // 
            btn_draw_ruler.Image = (Image)resources.GetObject("btn_draw_ruler.Image");
            btn_draw_ruler.Location = new Point(443, 5);
            btn_draw_ruler.Name = "btn_draw_ruler";
            btn_draw_ruler.Size = new Size(37, 37);
            btn_draw_ruler.TabIndex = 10;
            toolTip_draw.SetToolTip(btn_draw_ruler, "Ruler");
            btn_draw_ruler.UseVisualStyleBackColor = true;
            btn_draw_ruler.Click += btn_draw_ruler_Click;
            // 
            // btn_draw_ellipse
            // 
            btn_draw_ellipse.Image = (Image)resources.GetObject("btn_draw_ellipse.Image");
            btn_draw_ellipse.Location = new Point(318, 4);
            btn_draw_ellipse.Name = "btn_draw_ellipse";
            btn_draw_ellipse.Size = new Size(37, 37);
            btn_draw_ellipse.TabIndex = 9;
            toolTip_draw.SetToolTip(btn_draw_ellipse, "Oval");
            btn_draw_ellipse.UseVisualStyleBackColor = true;
            btn_draw_ellipse.Click += btn_draw_ellipse_Click;
            // 
            // btn_draw_textbox
            // 
            btn_draw_textbox.Image = (Image)resources.GetObject("btn_draw_textbox.Image");
            btn_draw_textbox.Location = new Point(400, 5);
            btn_draw_textbox.Name = "btn_draw_textbox";
            btn_draw_textbox.Size = new Size(37, 37);
            btn_draw_textbox.TabIndex = 8;
            toolTip_draw.SetToolTip(btn_draw_textbox, "Text box");
            btn_draw_textbox.UseVisualStyleBackColor = true;
            btn_draw_textbox.Click += btn_draw_textbox_Click;
            // 
            // btn_draw_radius
            // 
            btn_draw_radius.Image = (Image)resources.GetObject("btn_draw_radius.Image");
            btn_draw_radius.Location = new Point(359, 3);
            btn_draw_radius.Name = "btn_draw_radius";
            btn_draw_radius.Size = new Size(37, 37);
            btn_draw_radius.TabIndex = 7;
            toolTip_draw.SetToolTip(btn_draw_radius, "Radius");
            btn_draw_radius.UseVisualStyleBackColor = true;
            btn_draw_radius.Click += btn_draw_radius_Click;
            // 
            // btn_draw_rectangle
            // 
            btn_draw_rectangle.Image = (Image)resources.GetObject("btn_draw_rectangle.Image");
            btn_draw_rectangle.Location = new Point(277, 4);
            btn_draw_rectangle.Name = "btn_draw_rectangle";
            btn_draw_rectangle.Size = new Size(37, 37);
            btn_draw_rectangle.TabIndex = 6;
            toolTip_draw.SetToolTip(btn_draw_rectangle, "Rectangle");
            btn_draw_rectangle.UseVisualStyleBackColor = true;
            btn_draw_rectangle.Click += btn_draw_rectangle_Click;
            // 
            // btn_draw_freeform
            // 
            btn_draw_freeform.Image = (Image)resources.GetObject("btn_draw_freeform.Image");
            btn_draw_freeform.Location = new Point(235, 4);
            btn_draw_freeform.Name = "btn_draw_freeform";
            btn_draw_freeform.Size = new Size(37, 37);
            btn_draw_freeform.TabIndex = 5;
            toolTip_draw.SetToolTip(btn_draw_freeform, "Freeform");
            btn_draw_freeform.UseVisualStyleBackColor = true;
            // 
            // btn_draw_arrow
            // 
            btn_draw_arrow.Image = (Image)resources.GetObject("btn_draw_arrow.Image");
            btn_draw_arrow.Location = new Point(193, 4);
            btn_draw_arrow.Name = "btn_draw_arrow";
            btn_draw_arrow.Size = new Size(37, 37);
            btn_draw_arrow.TabIndex = 4;
            toolTip_draw.SetToolTip(btn_draw_arrow, "Arrow");
            btn_draw_arrow.UseVisualStyleBackColor = true;
            btn_draw_arrow.Click += btn_draw_arrow_Click;
            // 
            // btn_draw_line
            // 
            btn_draw_line.Image = (Image)resources.GetObject("btn_draw_line.Image");
            btn_draw_line.Location = new Point(151, 4);
            btn_draw_line.Name = "btn_draw_line";
            btn_draw_line.Size = new Size(37, 37);
            btn_draw_line.TabIndex = 3;
            toolTip_draw.SetToolTip(btn_draw_line, "Line");
            btn_draw_line.UseVisualStyleBackColor = true;
            btn_draw_line.Click += btn_drtaw_line_Click;
            // 
            // btn_drwa_scribble
            // 
            btn_drwa_scribble.Image = (Image)resources.GetObject("btn_drwa_scribble.Image");
            btn_drwa_scribble.Location = new Point(109, 4);
            btn_drwa_scribble.Name = "btn_drwa_scribble";
            btn_drwa_scribble.Size = new Size(37, 37);
            btn_drwa_scribble.TabIndex = 2;
            toolTip_draw.SetToolTip(btn_drwa_scribble, "Create pushpin");
            btn_drwa_scribble.UseVisualStyleBackColor = true;
            // 
            // btn_draw_pushpin
            // 
            btn_draw_pushpin.Image = (Image)resources.GetObject("btn_draw_pushpin.Image");
            btn_draw_pushpin.Location = new Point(67, 4);
            btn_draw_pushpin.Name = "btn_draw_pushpin";
            btn_draw_pushpin.Size = new Size(37, 37);
            btn_draw_pushpin.TabIndex = 1;
            toolTip_draw.SetToolTip(btn_draw_pushpin, "Create pushpin");
            btn_draw_pushpin.UseVisualStyleBackColor = true;
            // 
            // btn_draw
            // 
            btn_draw.Font = new Font("Segoe UI", 12F);
            btn_draw.Location = new Point(4, 3);
            btn_draw.Margin = new Padding(0);
            btn_draw.Name = "btn_draw";
            btn_draw.Size = new Size(58, 38);
            btn_draw.TabIndex = 0;
            btn_draw.Text = "Draw";
            toolTip_draw.SetToolTip(btn_draw, "Draw items");
            btn_draw.UseVisualStyleBackColor = true;
            btn_draw.Click += btn_draw_Click;
            // 
            // MainMap
            // 
            MainMap.Bearing = 0F;
            MainMap.CanDragMap = true;
            MainMap.Dock = DockStyle.Fill;
            MainMap.EllipsesEnabled = true;
            MainMap.EmptyTileColor = Color.White;
            MainMap.HelperLineOption = HelperLineOptions.ShowOnModifierKey;
            MainMap.LabelEnabled = true;
            MainMap.LevelsKeepInMemory = 5;
            MainMap.LinesEnabled = true;
            MainMap.Location = new Point(0, 117);
            MainMap.MapScaleInfoPosition = MapScaleInfoPositions.Top;
            MainMap.MarkersEnabled = true;
            MainMap.MaxZoom = 2;
            MainMap.MinZoom = 2;
            MainMap.MouseWheelZoomEnabled = true;
            MainMap.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            MainMap.Name = "MainMap";
            MainMap.PolygonsEnabled = true;
            MainMap.RectanglesEnabled = true;
            MainMap.RetryLoadTile = 0;
            MainMap.RoutesEnabled = true;
            MainMap.ScaleMode = ScaleModes.Integer;
            MainMap.SelectedAreaFillColor = Color.FromArgb(33, 65, 105, 225);
            MainMap.ShowCenter = true;
            MainMap.ShowDistance = false;
            MainMap.ShowGeoGridLines = false;
            MainMap.ShowMapScaleInfo = false;
            MainMap.ShowTileGridLines = false;
            MainMap.Size = new Size(669, 193);
            MainMap.TabIndex = 5;
            MainMap.Zoom = 0D;
            MainMap.MouseClick += MainMap_MouseClick;
            // 
            // textBox_editor
            // 
            textBox_editor.Location = new Point(288, 168);
            textBox_editor.Name = "textBox_editor";
            textBox_editor.Size = new Size(100, 23);
            textBox_editor.TabIndex = 6;
            textBox_editor.Visible = false;
            textBox_editor.VisibleChanged += textBox_editor_VisibleChanged;
            textBox_editor.Leave += textBox_editor_Leave;
            // 
            // SgxMapControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(textBox_editor);
            Controls.Add(MainMap);
            Controls.Add(DrawPanel);
            Controls.Add(SearchPanel);
            Controls.Add(BottomPanel);
            Controls.Add(TopPanel);
            Name = "SgxMapControl";
            Size = new Size(669, 339);
            TopPanel.ResumeLayout(false);
            BottomPanel.ResumeLayout(false);
            BottomPanel.PerformLayout();
            SearchPanel.ResumeLayout(false);
            SearchPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar_Zoom).EndInit();
            DrawPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel TopPanel;
        private Button btn_newMap;
        private Button btn_copyMap;
        private Button btn_saveMap;
        private Button btn_openMap;
        private Button btn_exprtMap;
        private Panel BottomPanel;
        private Label label_status;
        private Button btn_printMap;
        private Panel SearchPanel;
        private Panel DrawPanel;
        private Button btn_draw;
        private Button btn_findAddress;
        private ComboBox controlBox_address;
        private Button btn_zoomMap;
        private TrackBar trackBar_Zoom;
        private Button btn_shrinkMap;
        private ComboBox comboBoxMapType;
        public SgxMapPanel MainMap;
        private ToolTip toolTip_draw;
        private Button btn_draw_pushpin;
        private Button btn_draw_line;
        private Button btn_drwa_scribble;
        private Button btn_draw_freeform;
        private Button btn_draw_arrow;
        private Button btn_draw_radius;
        private Button btn_draw_rectangle;
        private Button btn_draw_textbox;
        private Button btn_draw_ellipse;
        private TextBox textBox_editor;
        private Button btn_draw_ruler;
        private Button btn_draw_circle;
    }
}
