using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using ZedGraph;

namespace LightSourceTestAnalysisSystem
{
    class CIEChart
    {
        /// <summary>
        /// 颜色数组
        /// </summary>
        public static Color[]LineColors=new Color[15]
        {
            Color.DarkViolet,Color.Chartreuse,Color.Brown,Color.CadetBlue, Color.BurlyWood, Color.DarkGoldenrod, 
            Color.DarkSlateGray, Color.Gray, Color.MediumSlateBlue, Color.Olive, Color.DeepPink, Color.Transparent, 
            Color.Tomato, Color.Crimson, Color.MidnightBlue
        };
        /// <summary>
        /// 初始化CIE图
        /// </summary>
        /// <param name="CIEType">CIE类型</param>
        /// <param name="graph">控件</param>
        public static void IntallCIEChar(int CIEType, ref ZedGraph.ZedGraphControl graph)
        {
            //初始化基本属性
            graph.GraphPane.CurveList.Clear();
            graph.GraphPane.Chart.Fill = new ZedGraph.Fill(System.Drawing.Color.White);
            graph.IsShowContextMenu = true; //设置右键功能选项
            graph.IsShowPointValues = true;
            graph.PointValueFormat = "#0.0000";
            //初始化打点线
            ZedGraph.PointPairList CIEPoint = new ZedGraph.PointPairList();
            ZedGraph.LineItem CIELine = graph.GraphPane.AddCurve("", CIEPoint, System.Drawing.Color.SaddleBrown, ZedGraph.SymbolType.Circle); //曲线标题为空
            CIELine.Line.IsVisible = false;
            CIELine.Symbol.Fill = new Fill(Color.SaddleBrown);
            CIELine.Symbol.Size = 10;
            //初始化黑体辐射线
            ZedGraph.PointPairList darkline = new ZedGraph.PointPairList();//画黑体轨迹
            if (CIEType == 0)//CIE1931
            {
                //设置属性
                graph.GraphPane.Title.Text = "CIE-1931色品图"; //设置标题
                graph.GraphPane.XAxis.Title.Text = "x";
                graph.GraphPane.XAxis.Scale.Min = 0.0f;
                graph.GraphPane.XAxis.Scale.Max = 0.8f;
                graph.GraphPane.YAxis.Title.Text = "y";
                graph.GraphPane.YAxis.Scale.Max = 0.90f;
                graph.GraphPane.YAxis.Scale.Min = 0.0f;
                //黑体辐射
                for (int r = 1; r < CIEConstant.x_CCT.Length && r < CIEConstant.y_CCT.Length; r++)
                {
                    darkline.Add(CIEConstant.x_CCT[r], CIEConstant.y_CCT[r]);
                }
                ZedGraph.LineItem curve = graph.GraphPane.AddCurve("", darkline, System.Drawing.Color.Black, ZedGraph.SymbolType.None); //曲线标题为空
                curve.Line.IsSmooth = true;
                //CIE外圈
                for (int i = 0; i < CIEConstant.x_CIE.Length - 1 && i < CIEConstant.y_CIE.Length; i++)
                {
                    ZedGraph.PointPairList CIEline = new ZedGraph.PointPairList(); //
                    CIEline.Add(CIEConstant.x_CIE[i], CIEConstant.y_CIE[i]);
                    CIEline.Add(CIEConstant.x_CIE[i + 1], CIEConstant.y_CIE[i + 1]);
                    ZedGraph.LineItem CIEcurve = graph.GraphPane.AddCurve("", CIEline, CIEConstant.xy2Color(CIEConstant.x_CIE[i], CIEConstant.y_CIE[i]),
                        ZedGraph.SymbolType.None); //曲线标题为空
                }
                //CIE背景
                Assembly a = Assembly.GetExecutingAssembly(); 
                 Stream imgStream = a.GetManifestResourceStream("LightSourceTestAnalysisSystem.Resources.CIE1931.png");
                Image image = Bitmap.FromStream(imgStream) as Bitmap;
                TextureBrush brush = new TextureBrush(image);
                ZedGraph.PointPairList list2 = new ZedGraph.PointPairList();
                for (int i = 0; i < 101; i++)
                {
                    list2.Add(0, 0.01f * i);
                }
                for (int i = 0; i < 101; i++)
                {
                    list2.Add(0.01f * i, 1);
                }
                for (int i = 0; i < 100; i++)
                {
                    list2.Add(1, 0.01f * (100 - i));
                }
                for (int i = 0; i < 100; i++)
                {
                    list2.Add(0.01f * (100 - i), 0);
                }
                ZedGraph.LineItem myCurve2 = graph.GraphPane.AddCurve("", list2, Color.White, ZedGraph.SymbolType.None);
                myCurve2.Line.Fill = new ZedGraph.Fill(brush);
                myCurve2.Line.Width = 0.00001f;
            }
            else if (CIEType == 1)//CIE1960
            {
                //基本属性
                graph.GraphPane.Title.Text = "CIE-1960色品图"; //设置标题
                graph.GraphPane.XAxis.Title.Text = "u";
                graph.GraphPane.XAxis.Scale.Min = 0.0f;
                graph.GraphPane.XAxis.Scale.Max = 0.65f;
                graph.GraphPane.YAxis.Title.Text = "v";
                graph.GraphPane.YAxis.Scale.Max = 0.45f;
                graph.GraphPane.YAxis.Scale.Min = 0.0f;
                //黑体辐射
                for (int r = 1; r < CIEConstant.x_CCT.Length && r < CIEConstant.y_CCT.Length; r++)
                {
                    double u = CIEConstant.CIExyToCIEu(CIEConstant.x_CCT[r], CIEConstant.y_CCT[r]);
                    double v = CIEConstant.CIExyToCIEv(CIEConstant.x_CCT[r], CIEConstant.y_CCT[r]);
                    darkline.Add(u, v);
                }
                ZedGraph.LineItem curve = graph.GraphPane.AddCurve("", darkline, System.Drawing.Color.Black, ZedGraph.SymbolType.None); //曲线标题为空
                curve.Line.IsSmooth = true;
                //CIE外圈
                for (int i = 0; i < CIEConstant.x_CIE.Length - 1 && i < CIEConstant.y_CIE.Length; i++)
                {
                    ZedGraph.PointPairList CIEline = new ZedGraph.PointPairList(); //
                    double u = CIEConstant.CIExyToCIEu(CIEConstant.x_CIE[i], CIEConstant.y_CIE[i]);
                    double v = CIEConstant.CIExyToCIEv(CIEConstant.x_CIE[i], CIEConstant.y_CIE[i]);
                    CIEline.Add(u, v);
                    u = CIEConstant.CIExyToCIEu(CIEConstant.x_CIE[i + 1], CIEConstant.y_CIE[i + 1]);
                    v = CIEConstant.CIExyToCIEv(CIEConstant.x_CIE[i + 1], CIEConstant.y_CIE[i + 1]);
                    CIEline.Add(u, v);
                    ZedGraph.LineItem CIEcurve = graph.GraphPane.AddCurve("", CIEline,
                        CIEConstant.xy2Color(CIEConstant.x_CIE[i], CIEConstant.y_CIE[i]),
                        ZedGraph.SymbolType.None); //曲线标题为空

                }
                //CIE背景
                Assembly a = Assembly.GetExecutingAssembly();
                Stream imgStream = a.GetManifestResourceStream("LightSourceTestAnalysisSystem.Resources.CIE1960.png");
                Image image = Bitmap.FromStream(imgStream) as Bitmap;
                TextureBrush brush = new TextureBrush(image);
                ZedGraph.PointPairList list2 = new ZedGraph.PointPairList();
                for (int i = 0; i < 101; i++)
                {
                    list2.Add(0, 0.01f * i);
                }
                for (int i = 0; i < 101; i++)
                {
                    list2.Add(0.01f * i, 1);
                }
                for (int i = 0; i < 100; i++)
                {
                    list2.Add(1, 0.01f * (100 - i));
                }
                for (int i = 0; i < 100; i++)
                {
                    list2.Add(0.01f * (100 - i), 0);
                }
                ZedGraph.LineItem myCurve2 = graph.GraphPane.AddCurve("", list2, Color.White, ZedGraph.SymbolType.None);
                myCurve2.Line.Fill = new ZedGraph.Fill(brush);
                myCurve2.Line.Width = 0.00001f;
            }
            else if (CIEType ==2)//CIE1976
            {
                //基本属性
                graph.GraphPane.Title.Text = "CIE-1976色品图"; //设置标题
                graph.GraphPane.XAxis.Title.Text = "u'";
                graph.GraphPane.XAxis.Scale.Min = 0.0f;
                graph.GraphPane.XAxis.Scale.Max = 0.65f;
                graph.GraphPane.YAxis.Title.Text = "v'";
                graph.GraphPane.YAxis.Scale.Max = 0.65f;
                graph.GraphPane.YAxis.Scale.Min = 0.0f;
                //黑体辐射
                for (int r = 1; r < CIEConstant.x_CCT.Length && r < CIEConstant.y_CCT.Length; r++)
                {
                    double u1 = CIEConstant.CIExyToCIEuPrime(CIEConstant.x_CCT[r], CIEConstant.y_CCT[r]);
                    double v1 = CIEConstant.CIExyToCIEvPrime(CIEConstant.x_CCT[r], CIEConstant.y_CCT[r]);
                    darkline.Add(u1, v1);
                }
                ZedGraph.LineItem curve = graph.GraphPane.AddCurve("", darkline, System.Drawing.Color.Black, ZedGraph.SymbolType.None); //曲线标题为空
                curve.Line.IsSmooth = true;
                //CIE外圈
                for (int i = 0; i < CIEConstant.x_CIE.Length - 1 && i < CIEConstant.y_CIE.Length; i++)
                {
                    ZedGraph.PointPairList CIEline = new ZedGraph.PointPairList(); //
                    double u1 = CIEConstant.CIExyToCIEuPrime(CIEConstant.x_CIE[i], CIEConstant.y_CIE[i]);
                    double v1 = CIEConstant.CIExyToCIEvPrime(CIEConstant.x_CIE[i], CIEConstant.y_CIE[i]);
                    CIEline.Add(u1, v1);
                    u1 = CIEConstant.CIExyToCIEuPrime(CIEConstant.x_CIE[i + 1], CIEConstant.y_CIE[i + 1]);
                    v1 = CIEConstant.CIExyToCIEvPrime(CIEConstant.x_CIE[i + 1], CIEConstant.y_CIE[i + 1]);
                    CIEline.Add(u1, v1);
                    ZedGraph.LineItem CIEcurve = graph.GraphPane.AddCurve("", CIEline,
                        CIEConstant.xy2Color(CIEConstant.x_CIE[i], CIEConstant.y_CIE[i]),
                        ZedGraph.SymbolType.None); //曲线标题为空
                }
                //CIE背景
                Assembly a = Assembly.GetExecutingAssembly();
                Stream imgStream = a.GetManifestResourceStream("LightSourceTestAnalysisSystem.Resources.CIE1976.png");
                Image image = Bitmap.FromStream(imgStream) as Bitmap;
                TextureBrush brush = new TextureBrush(image);
                ZedGraph.PointPairList list2 = new ZedGraph.PointPairList();
                for (int i = 0; i < 101; i++)
                {
                    list2.Add(0, 0.01f * i);
                }
                for (int i = 0; i < 101; i++)
                {
                    list2.Add(0.01f * i, 1);
                }
                for (int i = 0; i < 100; i++)
                {
                    list2.Add(1, 0.01f * (100 - i));
                }
                for (int i = 0; i < 100; i++)
                {
                    list2.Add(0.01f * (100 - i), 0);
                }
                ZedGraph.LineItem myCurve2 = graph.GraphPane.AddCurve("", list2, Color.White, ZedGraph.SymbolType.None);
                myCurve2.Line.Fill = new ZedGraph.Fill(brush);
                myCurve2.Line.Width = 0.00001f;
            }
            graph.ContextMenuBuilder +=
                new ZedGraph.ZedGraphControl.ContextMenuBuilderEventHandler(zedGraph_ContextMenuBuilder);
            graph.AxisChange();
            graph.Refresh();

        }
        /// <summary>
        /// 初始化SDCM
        /// </summary>
        /// <param name="CIEType">CIE类型</param>
        /// <param name="CIECount">画图数量</param>
        /// <param name="graph">控件</param>
        public static void IntallSDCMCIEChar(int CIEType,int CIECount, ref ZedGraph.ZedGraphControl graph)
        {
            //基本属性
            graph.GraphPane.CurveList.Clear();
            graph.GraphPane.Chart.Fill = new ZedGraph.Fill(System.Drawing.Color.White);
            graph.IsShowContextMenu = true; //设置右键功能选项
            graph.IsShowPointValues = true;
            graph.PointValueFormat = "#0.0000";
            graph.GraphPane.XAxis.MajorGrid.IsVisible = true;
            graph.GraphPane.XAxis.MajorGrid.PenWidth = 1f;
            graph.GraphPane.YAxis.MajorGrid.IsVisible = true;
            graph.GraphPane.YAxis.MajorGrid.PenWidth = 1f;
            //CIE打点
            ZedGraph.PointPairList CIEPoint = new ZedGraph.PointPairList();
            ZedGraph.LineItem CIELine = graph.GraphPane.AddCurve("", CIEPoint, System.Drawing.Color.SaddleBrown, ZedGraph.SymbolType.Circle); //曲线标题为空
            CIELine.Line.IsVisible = false;
            CIELine.Symbol.Fill = new Fill(Color.SaddleBrown);
            CIELine.Symbol.Size = 10;
            //画图数量
            for (int i = 0; i < CIECount; i++)
            {
                ZedGraph.PointPairList CIEPoint1 = new ZedGraph.PointPairList();
                ZedGraph.LineItem CIELine1 = graph.GraphPane.AddCurve("", CIEPoint1, LineColors[i % 15], ZedGraph.SymbolType.None); //曲线标题为空
                CIELine1.Line.IsVisible = true;
                CIELine1.Symbol.Size = 1;
            }
           
            ZedGraph.PointPairList darkline = new ZedGraph.PointPairList();//画黑体轨迹
            if (CIEType == 0)//CIE1931
            {  
                //基本属性
                graph.GraphPane.Title.Text = "CIE-1931色品图"; //设置标题
                graph.GraphPane.XAxis.Title.Text = "x";
                graph.GraphPane.XAxis.Scale.Min = 0.0f;
                graph.GraphPane.XAxis.Scale.Max = 0.8f;
                graph.GraphPane.YAxis.Title.Text = "y";
                graph.GraphPane.YAxis.Scale.Max = 0.90f;
                graph.GraphPane.YAxis.Scale.Min = 0.0f;
                //黑体辐射
                for (int r = 1; r < CIEConstant.x_CCT.Length && r < CIEConstant.y_CCT.Length; r++)
                {
                    darkline.Add(CIEConstant.x_CCT[r], CIEConstant.y_CCT[r]);
                }
                ZedGraph.LineItem curve = graph.GraphPane.AddCurve("", darkline, System.Drawing.Color.Black, ZedGraph.SymbolType.None); //曲线标题为空
                curve.Line.IsSmooth = true;
                //CIE外圈
                for (int i = 0; i < CIEConstant.x_CIE.Length - 1 && i < CIEConstant.y_CIE.Length; i++)
                {
                    ZedGraph.PointPairList CIEline = new ZedGraph.PointPairList(); //
                    CIEline.Add(CIEConstant.x_CIE[i], CIEConstant.y_CIE[i]);
                    CIEline.Add(CIEConstant.x_CIE[i + 1], CIEConstant.y_CIE[i + 1]);
                    ZedGraph.LineItem CIEcurve = graph.GraphPane.AddCurve("", CIEline, CIEConstant.xy2Color(CIEConstant.x_CIE[i], CIEConstant.y_CIE[i]),
                        ZedGraph.SymbolType.None); //曲线标题为空
                }
                //CIE背景
                Assembly a = Assembly.GetExecutingAssembly();
                Stream imgStream = a.GetManifestResourceStream("LightSourceTestAnalysisSystem.Resources.CIE1931.png");
                Image image = Bitmap.FromStream(imgStream) as Bitmap;
                TextureBrush brush = new TextureBrush(image);
                ZedGraph.PointPairList list2 = new ZedGraph.PointPairList();
                for (int i = 0; i < 101; i++)
                {
                    list2.Add(0, 0.01f * i);
                }
                for (int i = 0; i < 101; i++)
                {
                    list2.Add(0.01f * i, 1);
                }
                for (int i = 0; i < 100; i++)
                {
                    list2.Add(1, 0.01f * (100 - i));
                }
                for (int i = 0; i < 100; i++)
                {
                    list2.Add(0.01f * (100 - i), 0);
                }
                ZedGraph.LineItem myCurve2 = graph.GraphPane.AddCurve("", list2, Color.White, ZedGraph.SymbolType.None);
                myCurve2.Line.Fill = new ZedGraph.Fill(brush);
                myCurve2.Line.Width = 0.00001f;
            }
            else if (CIEType == 1)//CIE1960
            {
                //基本属性
                graph.GraphPane.Title.Text = "CIE-1960色品图"; //设置标题
                graph.GraphPane.XAxis.Title.Text = "u";
                graph.GraphPane.XAxis.Scale.Min = 0.0f;
                graph.GraphPane.XAxis.Scale.Max = 0.65f;
                graph.GraphPane.YAxis.Title.Text = "v";
                graph.GraphPane.YAxis.Scale.Max = 0.45f;
                graph.GraphPane.YAxis.Scale.Min = 0.0f;
                //黑体辐射
                for (int r = 1; r < CIEConstant.x_CCT.Length && r < CIEConstant.y_CCT.Length; r++)
                {
                    double u = CIEConstant.CIExyToCIEu(CIEConstant.x_CCT[r], CIEConstant.y_CCT[r]);
                    double v = CIEConstant.CIExyToCIEv(CIEConstant.x_CCT[r], CIEConstant.y_CCT[r]);
                    darkline.Add(u, v);
                }
                ZedGraph.LineItem curve = graph.GraphPane.AddCurve("", darkline, System.Drawing.Color.Black, ZedGraph.SymbolType.None); //曲线标题为空
                curve.Line.IsSmooth = true;
                //CIE外圈
                for (int i = 0; i < CIEConstant.x_CIE.Length - 1 && i < CIEConstant.y_CIE.Length; i++)
                {
                    ZedGraph.PointPairList CIEline = new ZedGraph.PointPairList(); //
                    double u = CIEConstant.CIExyToCIEu(CIEConstant.x_CIE[i], CIEConstant.y_CIE[i]);
                    double v = CIEConstant.CIExyToCIEv(CIEConstant.x_CIE[i], CIEConstant.y_CIE[i]);
                    CIEline.Add(u, v);
                    u = CIEConstant.CIExyToCIEu(CIEConstant.x_CIE[i + 1], CIEConstant.y_CIE[i + 1]);
                    v = CIEConstant.CIExyToCIEv(CIEConstant.x_CIE[i + 1], CIEConstant.y_CIE[i + 1]);
                    CIEline.Add(u, v);
                    ZedGraph.LineItem CIEcurve = graph.GraphPane.AddCurve("", CIEline,
                        CIEConstant.xy2Color(CIEConstant.x_CIE[i], CIEConstant.y_CIE[i]),
                        ZedGraph.SymbolType.None); //曲线标题为空

                }
                //CIE背景
                Assembly a = Assembly.GetExecutingAssembly();
                Stream imgStream = a.GetManifestResourceStream("LightSourceTestAnalysisSystem.Resources.CIE1960.png");
                Image image = Bitmap.FromStream(imgStream) as Bitmap;
                TextureBrush brush = new TextureBrush(image);
                ZedGraph.PointPairList list2 = new ZedGraph.PointPairList();
                for (int i = 0; i < 101; i++)
                {
                    list2.Add(0, 0.01f * i);
                }
                for (int i = 0; i < 101; i++)
                {
                    list2.Add(0.01f * i, 1);
                }
                for (int i = 0; i < 100; i++)
                {
                    list2.Add(1, 0.01f * (100 - i));
                }
                for (int i = 0; i < 100; i++)
                {
                    list2.Add(0.01f * (100 - i), 0);
                }
                ZedGraph.LineItem myCurve2 = graph.GraphPane.AddCurve("", list2, Color.White, ZedGraph.SymbolType.None);
                myCurve2.Line.Fill = new ZedGraph.Fill(brush);
                myCurve2.Line.Width = 0.00001f;
            }
            else if (CIEType == 2)//CIE1976
            {
                //基本属性
                graph.GraphPane.Title.Text = "CIE-1976色品图"; //设置标题
                graph.GraphPane.XAxis.Title.Text = "u'";
                graph.GraphPane.XAxis.Scale.Min = 0.0f;
                graph.GraphPane.XAxis.Scale.Max = 0.65f;
                graph.GraphPane.YAxis.Title.Text = "v'";
                graph.GraphPane.YAxis.Scale.Max = 0.65f;
                graph.GraphPane.YAxis.Scale.Min = 0.0f;
                //黑体辐射
                for (int r = 1; r < CIEConstant.x_CCT.Length && r < CIEConstant.y_CCT.Length; r++)
                {
                    double u1 = CIEConstant.CIExyToCIEuPrime(CIEConstant.x_CCT[r], CIEConstant.y_CCT[r]);
                    double v1 = CIEConstant.CIExyToCIEvPrime(CIEConstant.x_CCT[r], CIEConstant.y_CCT[r]);
                    darkline.Add(u1, v1);
                }
                ZedGraph.LineItem curve = graph.GraphPane.AddCurve("", darkline, System.Drawing.Color.Black, ZedGraph.SymbolType.None); //曲线标题为空
                curve.Line.IsSmooth = true;
                //CIE外圈
                for (int i = 0; i < CIEConstant.x_CIE.Length - 1 && i < CIEConstant.y_CIE.Length; i++)
                {
                    ZedGraph.PointPairList CIEline = new ZedGraph.PointPairList(); //
                    double u1 = CIEConstant.CIExyToCIEuPrime(CIEConstant.x_CIE[i], CIEConstant.y_CIE[i]);
                    double v1 = CIEConstant.CIExyToCIEvPrime(CIEConstant.x_CIE[i], CIEConstant.y_CIE[i]);
                    CIEline.Add(u1, v1);
                    u1 = CIEConstant.CIExyToCIEuPrime(CIEConstant.x_CIE[i + 1], CIEConstant.y_CIE[i + 1]);
                    v1 = CIEConstant.CIExyToCIEvPrime(CIEConstant.x_CIE[i + 1], CIEConstant.y_CIE[i + 1]);
                    CIEline.Add(u1, v1);
                    ZedGraph.LineItem CIEcurve = graph.GraphPane.AddCurve("", CIEline,
                        CIEConstant.xy2Color(CIEConstant.x_CIE[i], CIEConstant.y_CIE[i]),
                        ZedGraph.SymbolType.None); //曲线标题为空

                }
                //CIE背景
                Assembly a = Assembly.GetExecutingAssembly();
                Stream imgStream = a.GetManifestResourceStream("LightSourceTestAnalysisSystem.Resources.CIE1976.png");
                Image image = Bitmap.FromStream(imgStream) as Bitmap;
                TextureBrush brush = new TextureBrush(image);
                ZedGraph.PointPairList list2 = new ZedGraph.PointPairList();
                for (int i = 0; i < 101; i++)
                {
                    list2.Add(0, 0.01f * i);
                }
                for (int i = 0; i < 101; i++)
                {
                    list2.Add(0.01f * i, 1);
                }
                for (int i = 0; i < 100; i++)
                {
                    list2.Add(1, 0.01f * (100 - i));
                }
                for (int i = 0; i < 100; i++)
                {
                    list2.Add(0.01f * (100 - i), 0);
                }
                ZedGraph.LineItem myCurve2 = graph.GraphPane.AddCurve("", list2, Color.White, ZedGraph.SymbolType.None);
                myCurve2.Line.Fill = new ZedGraph.Fill(brush);
                myCurve2.Line.Width = 0.00001f;
            }
            graph.ContextMenuBuilder +=
                new ZedGraph.ZedGraphControl.ContextMenuBuilderEventHandler(zedGraph_ContextMenuBuilder);
            graph.AxisChange();
            graph.Refresh();

        }
        /// <summary>
        /// zedGraph右击菜单中文化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="menuStrip"></param>
        /// <param name="mousePt"></param>
        /// <param name="objState"></param>
        private static void zedGraph_ContextMenuBuilder(ZedGraph.ZedGraphControl sender, ContextMenuStrip menuStrip, Point mousePt, ZedGraph.ZedGraphControl.ContextMenuObjectState objState)
        {
            foreach (ToolStripMenuItem item in menuStrip.Items)
            {
                switch (item.Name)
                {
                    case "copied_to_clip":
                        item.Text = @"复制到剪贴板";
                        break;
                    case "copy":
                        item.Text = @"复制";
                        break;
                    case "page_setup":
                        item.Text = @"页面设置...";
                        break;
                    case "print":
                        item.Text = @"打印...";
                        break;
                    case "save_as":
                        item.Text = @"另存图表...";
                        break;
                    case "set_default":
                        item.Text = @"恢复默认大小";
                        break;
                    case "show_val":
                        item.Text = @"显示节点数值";
                        break;
                    case "title_def":
                        item.Text = @"标题";
                        break;
                    case "undo_all":
                        item.Text = @"还原缩放/移动";
                        break;

                    case "unpan":
                        item.Text = @"还原移动";
                        break;

                    case "unzoom":
                        item.Text = @"还原缩放";
                        break;

                    case "x_title_def":
                        item.Text = @"X 轴";
                        break;
                    case "y_title_def":
                        item.Text = @"Y 轴";
                        break;

                }
            }
        }
        /// <summary>
        /// 写CIE打点图
        /// </summary>
        /// <param name="CIEType">CIE类型</param>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        /// <param name="graph">图表控件</param>
        public static void WriteCIEChar(int CIEType, double x, double y, ref ZedGraph.ZedGraphControl graph)
        {
            graph.GraphPane.CurveList[0].Clear();
            if (CIEType == 0)
            {
                graph.GraphPane.CurveList[0].AddPoint(x, y);
            }
            else if (CIEType == 1)
            {
                double u = CIEConstant.CIExyToCIEu(x, y);
                double v = CIEConstant.CIExyToCIEv(x, y);
                graph.GraphPane.CurveList[0].AddPoint(u, v);
            }
            else if (CIEType == 2)
            {
                double u1 = CIEConstant.CIExyToCIEuPrime(x, y);
                double v1 = CIEConstant.CIExyToCIEvPrime(x, y);
                graph.GraphPane.CurveList[0].AddPoint(u1, v1);
            }
            graph.Refresh();
        }
        /// <summary>
        /// 写SDCM图表
        /// </summary>
        /// <param name="index">曲线索引</param>
        /// <param name="CIEType">CIE类型</param>
        /// <param name="x">x坐标组</param>
        /// <param name="y">y坐标组</param>
        /// <param name="graph">图表控件</param>
        public static void WriteSDCMCIEChar(int index, int CIEType, double[] x, double[] y, ref ZedGraph.ZedGraphControl graph)
        {
            graph.GraphPane.CurveList[index].Clear();
            if (CIEType == 0)
            {
                for (int i = 0; i < x.Length && i < y.Length; i++)
                    graph.GraphPane.CurveList[index].AddPoint(x[i], y[i]);
                graph.GraphPane.XAxis.Scale.Min = x.Min() * 0.99;
                graph.GraphPane.XAxis.Scale.Max = x.Max() * 1.01;
                graph.GraphPane.YAxis.Scale.Min = y.Min() * 0.99;
                graph.GraphPane.YAxis.Scale.Max = y.Max() * 1.01;
            }
            else if (CIEType == 1)
            {
                double Xmin=Double.MaxValue,Ymin=Double.MaxValue,Xmax=Double.MinValue,Ymax=Double.MinValue;
                for (int i = 0; i < x.Length && i < y.Length; i++)
                {
                    double u = CIEConstant.CIExyToCIEu(x[i], y[i]);
                    double v = CIEConstant.CIExyToCIEv(x[i], y[i]);
                    if (Xmax < u) Xmax = u;
                    if (Xmin > u) Xmin = u;
                    if (Ymax < v) Ymax = v;
                    if (Ymin > v) Ymin = v;
                    graph.GraphPane.CurveList[index].AddPoint(u, v);
                    graph.GraphPane.XAxis.Scale.Min = Xmin * 0.99;
                    graph.GraphPane.XAxis.Scale.Max = Xmax * 1.01;
                    graph.GraphPane.YAxis.Scale.Min = Ymin * 0.99;
                    graph.GraphPane.YAxis.Scale.Max = Ymax * 1.01;
                }
            }
            else if (CIEType == 2)
            {
                double Xmin = Double.MaxValue, Ymin = Double.MaxValue, Xmax = Double.MinValue, Ymax = Double.MinValue;
                for (int i = 0; i < x.Length && i < y.Length; i++)
                {
                    double u1 = CIEConstant.CIExyToCIEuPrime(x[i], y[i]);
                    double v1 = CIEConstant.CIExyToCIEvPrime(x[i], y[i]);
                    if (Xmax < u1) Xmax = u1;
                    if (Xmin > u1) Xmin = u1;
                    if (Ymax < v1) Ymax = v1;
                    if (Ymin > v1) Ymin = v1;
                    graph.GraphPane.CurveList[index].AddPoint(u1, v1);
                    graph.GraphPane.XAxis.Scale.Min = Xmin * 0.99;
                    graph.GraphPane.XAxis.Scale.Max = Xmax * 1.01;
                    graph.GraphPane.YAxis.Scale.Min = Ymin * 0.99;
                    graph.GraphPane.YAxis.Scale.Max = Ymax * 1.01;
                }
            }
            graph.Refresh();
        }
        /// <summary>
        /// 转换麦克亚当椭圆参数到椭圆边界
        /// </summary>
        /// <param name="degree">麦克亚当椭圆的角度</param>
        /// <param name="x0">麦克亚当椭圆的中心x坐标</param>
        /// <param name="y0">麦克亚当椭圆的中心y坐标</param>
        /// <param name="a">麦克亚当椭圆的长轴a</param>
        /// <param name="b">麦克亚当椭圆的短轴b</param>
        /// <returns>椭圆的边界点集合</returns>
        public static void  ConverSDCMValueToEllipsePoint(double degree, double x0, double y0,
            double a, double b, ref double[] xs, ref double[] ys)
        {
            double x, y, r;
            degree = (degree) / 180 * 3.14159f;
            List<double> xList = new List<double>();
            List<double> yList = new List<double>();
            for (int i = 0; i < 165; i++)
            {
                r = -3.14f + 0.04f * i;
                x = (a * System.Math.Sin(r) - b * System.Math.Cos(r) * System.Math.Tan(degree)) /
                    (System.Math.Cos(degree) + System.Math.Sin(degree) * System.Math.Tan(degree)) + x0;
                y = (a * System.Math.Sin(r) + b * System.Math.Cos(r) / System.Math.Tan(degree)) /
                    (System.Math.Cos(degree) / System.Math.Tan(degree) + System.Math.Sin(degree)) + y0;
                xList.Add(x);
                yList.Add(y);
            }

            xs = xList.ToArray();
            ys = yList.ToArray();


        }
    }

    public static class CIEConstant
    {
        /// <summary>
        /// 范围限制
        /// </summary>
        /// <param name="value">输入值</param>
        /// <param name="low">最小值</param>
        /// <param name="high">最大值</param>
        /// <returns>合格值</returns>
        private static double Clamp(double value, double low, double high)
        {
            if (value < low) return low;
            if (value > high) return high;
            else return value;
        }

        /// <summary>
        /// Convert CIE-x,y to color(RGB)
        /// </summary>
        /// <param name="x">CIE-x</param>
        /// <param name="y">CIE-y</param>
        /// <returns>rgb（0-1）</returns>
        private static double[] xy2rgb(double x, double y)
        {
            double xRed = 0.7355;
            double yRed = 0.2645;
            double xGreen = 0.2658;
            double yGreen = 0.7243;
            double xBlue = 0.1669;
            double yBlue = 0.0085;
            double xWhite = 0.33333333;
            double yWhite = 0.33333333;
            /* Gets data from the specified color system structure. */
            double xr = xRed, yr = yRed, zr = 1 - (xr + yr);
            double xg = xGreen, yg = yGreen, zg = 1 - (xg + yg);
            double xb = xBlue, yb = yBlue, zb = 1 - (xb + yb);
            double xw = xWhite, yw = yWhite, zw = 1 - (xw + yw);

            /* xyz -> rgb matrix, before scaling to white. */
            double rx = (yg * zb) - (yb * zg), ry = (xb * zg) - (xg * zb), rz = (xg * yb) - (xb * yg);
            double gx = (yb * zr) - (yr * zb), gy = (xr * zb) - (xb * zr), gz = (xb * yr) - (xr * yb);
            double bx = (yr * zg) - (yg * zr), by = (xg * zr) - (xr * zg), bz = (xr * yg) - (xg * yr);

            /* White scaling factors.
               Dividing by yw scales the white luminance to unity, as conventional. */
            double rw = ((rx * xw) + (ry * yw) + (rz * zw)) / yw,
                gw = ((gx * xw) + (gy * yw) + (gz * zw)) / yw,
                bw = ((bx * xw) + (by * yw) + (bz * zw)) / yw;

            /* xyz -> rgb matrix, correctly scaled to white. */
            rx = rx / rw;
            ry = ry / rw;
            rz = rz / rw;
            gx = gx / gw;
            gy = gy / gw;
            gz = gz / gw;
            bx = bx / bw;
            by = by / bw;
            bz = bz / bw;

            /* rgb of the desired point */
            double r = (rx * x) + (ry * y) + (rz * (1.0 - x - y));
            double g = (gx * x) + (gy * y) + (gz * (1.0 - x - y));
            double b = (bx * x) + (by * y) + (bz * (1.0 - x - y));
            return new double[3] { r, g, b };
        }

        /// <summary>
        /// Convert CIE-x,y to color(RGB).
        /// </summary>
        /// <param name="x">CIE-x</param>
        /// <param name="y">CIE-y</param>
        /// <returns>Represents an ARGB (alpha, red, green, blue) color.</returns>
        public static Color xy2Color(double x, double y)
        {

            double[] rgb = xy2rgb(x, y);
            bool InsideGamut = true;
            for (int i = 0; i < rgb.Length; i++)
            {
                if (rgb[i] < 0)
                {
                    InsideGamut = false;
                    break;
                }
            }

            double r = Clamp(rgb[0], 0, 1.0);
            double g = Clamp(rgb[1], 0, 1.0);
            double b = Clamp(rgb[2], 0, 1.0);

            #region Gamma correction

            double rgb_max = Math.Max(Math.Max(r, g), b);
            int RGB_r = 0, RGB_g = 0, RGB_b = 0;
            double Gamma = 1;

            if (Math.Abs(Gamma - 1.00) < 0.0001)
            {
                RGB_r = (int)Math.Round(255 * r / rgb_max);
                RGB_g = (int)Math.Round(255 * g / rgb_max);
                RGB_b = (int)Math.Round(255 * b / rgb_max);
            }
            else
            {
                RGB_r = (int)Math.Round(255 * Math.Pow(r / rgb_max, Gamma));
                RGB_g = (int)Math.Round(255 * Math.Pow(g / rgb_max, Gamma));
                RGB_b = (int)Math.Round(255 * Math.Pow(b / rgb_max, Gamma));
            }

            #endregion

            return Color.FromArgb(RGB_r, RGB_g, RGB_b);
        }

        /// <summary>
        /// CIE-xy转换CIE-u
        /// </summary>
        /// <param name="x">CIE-x</param>
        /// <param name="y">CIE-y</param>
        /// <returns>CIE-u</returns>
        public static double CIExyToCIEu(double x, double y)
        {
            double denom = -2D * x + 12D * y + 3D;
            if (denom != 0.0D)
                return (4D * x) / denom;
            else
                return -1;
        }

        /// <summary>
        /// CIE-xy转换CIE-v
        /// </summary>
        /// <param name="x">CIE-x</param>
        /// <param name="y">CIE-y</param>
        /// <returns>CIE-v</returns>
        public static double CIExyToCIEv(double x, double y)
        {
            double denom = -2D * x + 12D * y + 3D;
            if (denom != 0.0D)
                return (6D * y) / denom;
            else
                return -1;
        }

        /// <summary>
        /// CIE-xy转换CIE-u'
        /// </summary>
        /// <param name="x">CIE-x</param>
        /// <param name="y">CIE-y</param>
        /// <returns>CIE-u'</returns>
        public static double CIExyToCIEuPrime(double x, double y)
        {
            double denom = -2D * x + 12D * y + 3D;
            if (denom != 0.0D)
                return (4D * x) / denom;
            else
                return -1;
        }

        /// <summary>
        /// CIE-xy转换CIE-v'
        /// </summary>
        /// <param name="x">CIE-x</param>
        /// <param name="y">CIE-y</param>
        /// <returns>CIE-v'</returns>
        public static double CIExyToCIEvPrime(double x, double y)
        {
            double denom = -2D * x + 12D * y + 3D;
            if (denom != 0.0D)
                return (9D * y) / denom;
            else
                return -1;
        }

        #region 数据源

        public static double[] Wavelength_CIE = new double[]
        {
            380, 385, 390, 395, 400, 405, 410, 415, 420, 425,
            430, 435, 440, 445, 450, 455, 460, 465, 470, 475,
            480, 485, 490, 495, 500, 505, 510, 513, 515, 518,
            519, 520, 522, 525, 530, 535, 540, 545, 550, 555,
            560, 565, 570, 575, 580, 585, 590, 595, 600, 605,
            610, 615, 620, 625, 630, 635, 640, 645, 650, 655,
            660, 665, 670, 675, 680, 685, 690, 695, 700, 705,
            710, 715, 720, 725, 730, 735, 740, 745, 750, 755,
            760, 765, 770, 775, 780, 380
        };

        public static double[] x_CIE = new double[]
        {
            0.1741, 0.1740, 0.1738, 0.1736, 0.1733, 0.1730, 0.1726, 0.1721, 0.1714, 0.1703,
            0.1689, 0.1669, 0.1644, 0.1611, 0.1566, 0.1510, 0.1440, 0.1355, 0.1241, 0.1096,
            0.0913, 0.0687, 0.0454, 0.0235, 0.0082, 0.0039, 0.0139, 0.0229, 0.0389, 0.0491,
            0.0591, 0.0743, 0.0926, 0.1142, 0.1547, 0.1929, 0.2296, 0.2658, 0.3016, 0.3373,
            0.3731, 0.4087, 0.4441, 0.4788, 0.5125, 0.5449, 0.5752, 0.6029, 0.6270, 0.6482,
            0.6658, 0.6801, 0.6915, 0.7001, 0.7079, 0.7140, 0.7190, 0.7230, 0.7260, 0.7283,
            0.7300, 0.7311, 0.7320, 0.7327, 0.7334, 0.7340, 0.7344, 0.7346, 0.7347, 0.7347,
            0.7347, 0.7347, 0.7347, 0.7347, 0.7347, 0.7347, 0.7347, 0.7347, 0.7347, 0.7347,
            0.7347, 0.7347, 0.7347, 0.7347, 0.7347, 0.1741
        };

        public static double[] y_CIE = new double[]
        {
            0.0050, 0.0050, 0.0049, 0.0049, 0.0048, 0.0048, 0.0048, 0.0048, 0.0051, 0.0058,
            0.0069, 0.0086, 0.0109, 0.0138, 0.0177, 0.0227, 0.0297, 0.0399, 0.0578, 0.0868,
            0.1327, 0.2007, 0.2950, 0.4127, 0.5384, 0.6548, 0.7502, 0.7831, 0.8120, 0.8245,
            0.8305, 0.8338, 0.8321, 0.8262, 0.8059, 0.7816, 0.7543, 0.7243, 0.6923, 0.6589,
            0.6245, 0.5896, 0.5547, 0.5202, 0.4866, 0.4544, 0.4242, 0.3965, 0.3725, 0.3514,
            0.3340, 0.3197, 0.3083, 0.2993, 0.2920, 0.2859, 0.2809, 0.2770, 0.2740, 0.2717,
            0.2700, 0.2689, 0.2680, 0.2673, 0.2666, 0.2660, 0.2656, 0.2654, 0.2653, 0.2653,
            0.2653, 0.2653, 0.2653, 0.2653, 0.2653, 0.2653, 0.2653, 0.2653, 0.2653, 0.2653,
            0.2653, 0.2653, 0.2653, 0.2653, 0.2653, 0.0050
        };

        public static double[] CCT_CCT = new double[]
        {
            1000, 1100, 1200, 1300, 1400, 1500, 1600, 1700, 1800, 1900,
            2000, 2100, 2200, 2300, 2400, 2500, 2600, 2700, 2800, 2900,
            3000, 3100, 3200, 3300, 3400, 3500, 3600, 3700, 3800, 3900,
            4000, 4100, 4200, 4300, 4400, 4500, 4600, 4700, 4800, 4900,
            5000, 5100, 5200, 5300, 5400, 5500, 5600, 5700, 5800, 5900,
            6000, 6100, 6200, 6300, 6400, 6500, 6600, 6700, 6800, 6900,
            7000, 7100, 7200, 7300, 7400, 7500, 7600, 7700, 7800, 7900,
            8000, 8100, 8200, 8300, 8400, 8500, 8600, 8700, 8800, 8900,
            9000, 9100, 9200, 9300, 9400, 9500, 9600, 9700, 9800, 9900,
            10000, 10100, 10200, 10300, 10400, 10500, 10600, 10700, 10800, 10900,
            11000, 11100, 11200, 11300, 11400, 11500, 11600, 11700, 11800, 11900,
            12000, 12100, 12200, 12300, 12400, 12500, 12600, 12700, 12800, 12900,
            13000, 13100, 13200, 13300, 13400, 13500, 13600, 13700, 13800, 13900,
            14000, 14100, 14200, 14300, 14400, 14500, 14600, 14700, 14800, 14900,
            15000, 15100, 15200, 15300, 15400, 15500, 15600, 15700, 15800, 15900,
            16000, 16100, 16200, 16300, 16400, 16500, 16600, 16700, 16800, 16900,
            17000, 17100, 17200, 17300, 17400, 17500, 17600, 17700, 17800, 17900,
            18000, 18100, 18200, 18300, 18400, 18500, 18600, 18700, 18800, 18900,
            19000, 19100, 19200, 19300, 19400, 19500, 19600, 19700, 19800, 19900,
            20000

        };

        public static double[] x_CCT = new double[]
        {
            0.6499, 0.6361, 0.6226, 0.6095, 0.5966, 0.5841, 0.572, 0.5601, 0.5486, 0.5375,
            0.5267, 0.5162, 0.5062, 0.4965, 0.4872, 0.4782, 0.4696, 0.4614, 0.4535, 0.446,
            0.4388, 0.432, 0.4254, 0.4192, 0.4132, 0.4075, 0.4021, 0.3969, 0.3919, 0.3872,
            0.3827, 0.3784, 0.3743, 0.3704, 0.3666, 0.3631, 0.3596, 0.3563, 0.3532, 0.3502,
            0.3473, 0.3446, 0.3419, 0.3394, 0.3369, 0.3346, 0.3323, 0.3302, 0.3281, 0.3261,
            0.3242, 0.3223, 0.3205, 0.3188, 0.3171, 0.3155, 0.314, 0.3125, 0.311, 0.3097,
            0.3083, 0.307, 0.3058, 0.3045, 0.3034, 0.3022, 0.3011, 0.3, 0.299, 0.298,
            0.297, 0.2961, 0.2952, 0.2943, 0.2934, 0.2926, 0.2917, 0.291, 0.2902, 0.2894,
            0.2887, 0.288, 0.2873, 0.2866, 0.286, 0.2853, 0.2847, 0.2841, 0.2835, 0.2829,
            0.2824, 0.2818, 0.2813, 0.2807, 0.2802, 0.2797, 0.2792, 0.2788, 0.2783, 0.2778,
            0.2774, 0.277, 0.2765, 0.2761, 0.2757, 0.2753, 0.2749, 0.2745, 0.2742, 0.2738,
            0.2734, 0.2731, 0.2727, 0.2724, 0.2721, 0.2717, 0.2714, 0.2711, 0.2708, 0.2705,
            0.2702, 0.2699, 0.2696, 0.2694, 0.2691, 0.2688, 0.2686, 0.2683, 0.268, 0.2678,
            0.2675, 0.2673, 0.2671, 0.2668, 0.2666, 0.2664, 0.2662, 0.2659, 0.2657, 0.2655,
            0.2653, 0.2651, 0.2649, 0.2647, 0.2645, 0.2643, 0.2641, 0.2639, 0.2638, 0.2636,
            0.2634, 0.2632, 0.2631, 0.2629, 0.2627, 0.2626, 0.2624, 0.2622, 0.2621, 0.2619,
            0.2618, 0.2616, 0.2615, 0.2613, 0.2612, 0.261, 0.2609, 0.2608, 0.2606, 0.2605,
            0.2604, 0.2602, 0.2601, 0.26, 0.2598, 0.2597, 0.2596, 0.2595, 0.2593, 0.2592,
            0.2591, 0.259, 0.2589, 0.2588, 0.2587, 0.2586, 0.2584, 0.2583, 0.2582, 0.2581,
            0.258
        };

        public static double[] y_CCT = new double[]
        {
            0.3474,0.3594,0.3703,0.3801,0.3887,0.3962,0.4025,0.4076,0.4118,0.415,
            0.4173,0.4188,0.4196,0.4198,0.4194,0.4186,0.4173,0.4158,0.4139,0.4118,
            0.4095,0.407,0.4044,0.4018,0.399,0.3962,0.3934,0.3905,0.3877,0.3849,
            0.382,0.3793,0.3765,0.3738,0.3711,0.3685,0.3659,0.3634,0.3609,0.3585,
            0.3561,0.3538,0.3516,0.3494,0.3472,0.3451,0.3431,0.3411,0.3392,0.3373,
            0.3355,0.3337,0.3319,0.3302,0.3286,0.327,0.3254,0.3238,0.3224,0.3209,
            0.3195,0.3181,0.3168,0.3154,0.3142,0.3129,0.3117,0.3105,0.3094,0.3082,
            0.3071,0.3061,0.305,0.304,0.303,0.302,0.3011,0.3001,0.2992,0.2983,
            0.2975,0.2966,0.2958,0.295,0.2942,0.2934,0.2927,0.2919,0.2912,0.2905,
            0.2898,0.2891,0.2884,0.2878,0.2871,0.2865,0.2859,0.2853,0.2847,0.2841,
            0.2836,0.283,0.2825,0.2819,0.2814,0.2809,0.2804,0.2799,0.2794,0.2789,
            0.2785,0.278,0.2776,0.2771,0.2767,0.2763,0.2758,0.2754,0.275,0.2746,
            0.2742,0.2738,0.2735,0.2731,0.2727,0.2724,0.272,0.2717,0.2713,0.271,
            0.2707,0.2703,0.27,0.2697,0.2694,0.2691,0.2688,0.2685,0.2682,0.2679,
            0.2676,0.2673,0.2671,0.2668,0.2665,0.2663,0.266,0.2657,0.2655,0.2652,
            0.265,0.2648,0.2645,0.2643,0.2641,0.2638,0.2636,0.2634,0.2632,0.2629,
            0.2627,0.2625,0.2623,0.2621,0.2619,0.2617,0.2615,0.2613,0.2611,0.2609,
            0.2607,0.2606,0.2604,0.2602,0.26,0.2598,0.2597,0.2595,0.2593,0.2592,
            0.259,0.2588,0.2587,0.2585,0.2584,0.2582,0.258,0.2579,0.2577,0.2576,
            0.2574
        };

        #endregion
    }
}
