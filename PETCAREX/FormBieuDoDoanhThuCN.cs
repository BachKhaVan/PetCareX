using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PETCAREX
{
    public partial class FormBieuDoDoanhThuCN : Form
    {
        string _connStr;
        string _proc;
        DateTime _from, _to;

        public FormBieuDoDoanhThuCN(
            string connStr,
            string procName,
            DateTime from,
            DateTime to,
            string title
        )
        {
            InitializeComponent();

            _connStr = connStr;
            _proc = procName;
            _from = from;
            _to = to;

            Text = title;
            Width = 900;
            Height = 500;
            StartPosition = FormStartPosition.CenterParent;

            LoadChart();
        }

        void LoadChart()
        {
            BackColor = Color.FromArgb(255, 235, 235); // nền hồng

            // ===== NÚT QUAY LẠI =====
            Button btnBack = new Button()
            {
                Text = "← Quay lại",
                Size = new Size(110, 36),
                Location = new Point(20, 15),
                BackColor = Color.FromArgb(220, 70, 70),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.Click += (s, e) => this.Close();
            Controls.Add(btnBack);

            // ===== CHART =====
            Chart chart = new Chart()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(255, 235, 235)
            };
            Controls.Add(chart);
            chart.BringToFront();
            btnBack.BringToFront();

            // ===== CHART AREA =====
            ChartArea area = new ChartArea("MainArea");
            area.BackColor = Color.Transparent;

            area.AxisX.Interval = 1;
            area.AxisX.LabelStyle.Angle = -30;
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            area.AxisX.MajorGrid.Enabled = false;

            area.AxisY.LabelStyle.Font = new Font("Segoe UI", 9);
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(230, 200, 200);
            area.AxisY.LabelStyle.Format = "#,##0";
            area.AxisY.Title = "Doanh thu (triệu VND)";
            area.AxisY.TitleFont = new Font("Segoe UI", 10, FontStyle.Bold);
            //area.AxisY.Interval = 200;
            chart.ChartAreas.Add(area);

            // ===== SERIES =====
            Series series = new Series("DoanhThu")
            {
                ChartType = SeriesChartType.Column,
                IsValueShownAsLabel = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Color = Color.FromArgb(220, 70, 70),
                ChartArea = "MainArea"
            };
            chart.Series.Add(series);
            //series.SmartLabelStyle.Enabled = true;
            series.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.Yes;
            series.SmartLabelStyle.MovingDirection = LabelAlignmentStyles.Top;
            // ===== LOAD DATA =====
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(_proc, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FROMDATE", _from);
                cmd.Parameters.AddWithValue("@TODATE", _to);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            double maxMillion = 0;

            foreach (DataRow r in dt.Rows)
            {
                double v = Convert.ToDouble(r["DOANHTHU"]) / 1_000_000;
                if (v > maxMillion) maxMillion = v;
            }
            maxMillion = Math.Ceiling(maxMillion / 200) * 200; // làm tròn cho đẹp

            area.AxisY.Minimum = 0;
            area.AxisY.Maximum = maxMillion;
            area.AxisY.Interval = maxMillion / 5; // luôn 5 vạch
            // ===== ADD DATA TO CHART =====
            foreach (DataRow r in dt.Rows)
            {
                string macn = r["MACN"].ToString();
                string tencn = r["TENCN"].ToString();
                double value = Convert.ToDouble(r["DOANHTHU"]);
                double million = value / 1_000_000;

                // 👉 NHÃN: CN01 – Chi nhánh TP.HCM
                string labelX = $"{macn} – {tencn}";

                DataPoint p = new DataPoint();
                p.AxisLabel = labelX;
                p.YValues = new double[] { million };   // ✅ TRIỆU
                p.Label = $"{million:N1} M";// số trên cột

                series.Points.Add(p);
            }

            // ===== TITLE =====
            chart.Titles.Add(new Title(
                Text,
                Docking.Top,
                new Font("Segoe UI", 16, FontStyle.Bold),
                Color.FromArgb(220, 70, 70)
            ));
        }
    }
}
