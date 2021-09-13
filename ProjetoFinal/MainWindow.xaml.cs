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
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;


namespace ProjetoFinal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
  
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
           InitializeComponent();
           btnCalcular.Click += BtnCalcular_Click;
        }
        private void BtnCalcular_Click(object sender, RoutedEventArgs e)
        {


            PlotModel model = new PlotModel();
            LinearAxis ejeX = new LinearAxis();
            LinearAxis ejeY = new LinearAxis();

            model.Axes.Add(ejeX);
            model.Axes.Add(ejeY);
            LineSeries linea = new LineSeries();
            linea.Title = "Valores generados";
            linea.Points.Add(new DataPoint(5, 10));
            linea.Points.Add(new DataPoint(20, 132));
            model.Series.Add(linea);
            Grafica.Model = model;
        }

    }
} 