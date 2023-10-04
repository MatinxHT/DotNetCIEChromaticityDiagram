using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChromaticityDotNet;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using OxyPlot;
using static ChromaticityDotNet.StandardChromaticityClass;

namespace CIEChromaticityDiagram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CIEViewModel? _viewModel;

        //List<DataGridValue> ScatterPointValueStructList = new();

        public MainWindow()
        {
            InitializeComponent();

            PlotGraph();
        }

        private void PlotGraph()
        {
            _viewModel = new CIEViewModel();
            DataContext = _viewModel;
        }

        public class CIEViewModel
        {
            public PlotModel CIE1931Model { get; private set; }

            //public List<DataGridValue> ScatterPointValueStructList { get; set; }

            public CIEViewModel()
            {
                //ScatterPointValueStructList = new List<DataGridValue>(scatterPointValueStructList);
                CIE1931Model = new PlotModel
                {
                    Title = "CIE1931 Space",
                    TitleFontSize = 14, // 修改标题字体大小
                    TitleFontWeight = OxyPlot.FontWeights.Normal, // 修改标题字体粗细
                    TitleFont = "Arial", // 修改标题字体
                                         //Background = backgroundImage,
                                         //Subtitle = "单位: 反射率/% 波长/nm",
                                         //SubtitleFontSize = 10,
                    Axes = {
                    new LinearAxis {
                       Position = AxisPosition.Bottom, // 设置该坐标轴在图形的位置
                       Title = "CIE x", // 设置该坐标轴的标题
                       TitleFontSize = 10,
                       MajorGridlineStyle = LineStyle.Solid, // 设置主网格线的样式为实线
                       MinorGridlineStyle = LineStyle.Dot, // 设置次网格线的样式为点线
                       Minimum = 0,
                       Maximum = 0.8,
                    },
                    new LinearAxis {
                        Position = AxisPosition.Left,
                        Title = "CIE y",
                        TitleFontSize = 10,
                        MajorGridlineStyle = LineStyle.Solid,
                        MinorGridlineStyle = LineStyle.Dot,
                        Minimum = 0,
                        Maximum = 0.9,
                    }
                },
                };

                // 创建图例对象，设置样式
                Legend legend = new Legend
                {
                    // 图例的背景颜色为半透明白色
                    LegendBackground = OxyColor.FromAColor(100, OxyColors.White),
                    // 图例边框颜色为黑色
                    LegendBorder = OxyColors.Black,
                    // 图例边框厚度
                    LegendBorderThickness = 1,
                    // 图例放置在图像里外
                    LegendPlacement = LegendPlacement.Inside,
                    // 图例放置位置
                    LegendPosition = LegendPosition.RightTop,
                    // 图例条目之间的距离
                    LegendItemSpacing = 3,
                    // 图例符号和文字之间的距离
                    LegendSymbolMargin = 3,
                    // 图例符号长度
                    LegendSymbolLength = 18,
                    // 图例符号放置位置
                    LegendSymbolPlacement = LegendSymbolPlacement.Left,
                    // 图例文字大小
                    FontSize = 3
                };

                // 绘制黑体辐射线
                DrawBlackbodyRadiation(legend);

                // 绘制 CIE 外圈
                DrawCIEOuterRing();

                //打点
                //AddScatter();

            }
            //public void AddScatter()
            //{
            //    if (ScatterPointValueStructList == null) return;

            //    foreach (var item in ScatterPointValueStructList)
            //    {
            //        ScatterSeries scatterSeries = new ScatterSeries
            //        {
            //            Title = "Ch" + item.Channel.ToString() + $" At{item.TestTime: HH:mm:ss}", // 设置散点系列的图例标题
            //            MarkerType = MarkerType.Circle, // 设置散点标记的类型
            //            MarkerSize = 2, // 设置散点标记的大小
            //            MarkerFill = OxyColors.Blue, // 设置散点标记的颜色
            //        };

            //        scatterSeries.Points.Add(new ScatterPoint(item.ciex, item.ciey)); // 添加散点
            //        CIE1931Model.Series.Add(scatterSeries); // 将散点系列添加到图表中
            //    }
            //}

            private void DrawBlackbodyRadiation(Legend legend)
            {
                LineSeries blackbodySeries = new LineSeries
                {
                    Color = OxyColors.Black,
                    StrokeThickness = 2,
                    Title = "PlanckianLocus",
                    //Smooth = true // 使用平滑线
                };

                // 假设 CIEConstant.x_CCT 和 CIEConstant.y_CCT 分别为黑体辐射的 x 和 y 坐标数据
                for (int i = 1; i < CIEConstant.x_CCT.Length && i < CIEConstant.y_CCT.Length; i++)
                {
                    blackbodySeries.Points.Add(new DataPoint(CIEConstant.x_CCT[i], CIEConstant.y_CCT[i]));
                }

                CIE1931Model.Series.Add(blackbodySeries);
                CIE1931Model.Legends.Add(legend);
            }

            private void DrawCIEOuterRing()
            {
                LineSeries cieOuterSeries = new LineSeries
                {
                    Color = OxyColors.Gray,
                    StrokeThickness = 1,
                    Title = "CIE OuterRing"
                };

                // 假设 CIEConstant.x_CIE 和 CIEConstant.y_CIE 分别为外圈的 x 和 y 坐标数据
                for (int i = 0; i < CIEConstant.x_CIE.Length - 1 && i < CIEConstant.y_CIE.Length; i++)
                {
                    cieOuterSeries.Points.Add(new DataPoint(CIEConstant.x_CIE[i], CIEConstant.y_CIE[i]));
                    cieOuterSeries.Points.Add(new DataPoint(CIEConstant.x_CIE[i + 1], CIEConstant.y_CIE[i + 1]));
                }

                CIE1931Model.Series.Add(cieOuterSeries);
                //CIE1931Model.Legends.Add(legend);
            }
        }

    }
}
