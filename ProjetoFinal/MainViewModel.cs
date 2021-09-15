using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoFinal
{
    using System;

    using OxyPlot;
    using OxyPlot.Series;
    using OxyPlot.Axes;
    public class MainViewModel
    {
        public MainViewModel() // entrar com uma lista do tipo coordenada
        {
            //receber uma lista de pontos flutuantes

            this.MyModel = new PlotModel { Title = "Gráfico resistência Momento x Normal" };


            /*List<Coordenada> pontos = new List<Coordenada>();
            List<Coordenada> pontosentrada = new List<Coordenada>();
            //pontos = refpontos;
            //pontos=pontos.OrderBy(x => x.y).ToList(); // descobrir pq nao ta ordenando

            pontos.Add(new Coordenada(0.5, 7));
            pontos.Add(new Coordenada(6, 98));
            pontos.Add(new Coordenada(600, 225));
            pontos.Add(new Coordenada(50,50));

            pontos = pontos.OrderBy(x => x.x).ToList(); // descobrir pq nao ta ordenando

            pontosentrada.Add(new Coordenada(65, 50));
            pontosentrada.Add(new Coordenada(40, 400));
            pontosentrada.Add(new Coordenada(500, 40));


            var graflinha = new LineSeries();
            var grafpontos = new ScatterSeries();

            foreach (Coordenada ponto in pontos)
            {
                graflinha.Points.Add(new DataPoint(ponto.x, ponto.y));
                //grafpontos.Points.Add(new ScatterPoint(ponto.x, ponto.y, 4));
            }
            foreach (Coordenada ponto in pontosentrada)
            {
                //graflinha.Points.Add(new DataPoint(ponto.x, ponto.y));
                grafpontos.Points.Add(new ScatterPoint(ponto.x, ponto.y, 4));

            }




            grafpontos.Points.OrderBy(x => x.X);
            grafpontos.MarkerType = MarkerType.Circle;
            grafpontos.MarkerFill = OxyColors.Red;
            graflinha.Color = OxyColors.Blue;

            /*lineSeries1.Points.Add(new DataPoint(pontos[0].x, pontos[0].y));
            lineSeries1.Points.Add(new DataPoint(100, 40));*/

            this.MyModel.Axes.Add(new LinearAxis { Title = "Normal (kN)", Position = AxisPosition.Bottom /*, Minimum = -20, Maximum = 80 */});
            this.MyModel.Axes.Add(new LinearAxis { Title = "Momento (kN.m)", Position = AxisPosition.Left/*, Minimum = -20, Maximum = 80*/});

            this.MyModel = new PlotModel { Title = "Gráfico resistência Momento x Normal" };

            var grafpontos = new ScatterSeries();
            grafpontos.Points.Add(new ScatterPoint(40, 40, 4));
            
            this.MyModel.Series.Add(grafpontos);
            //this.MyModel.Series.Add(graflinha);
            //this.MyModel.Series.Add(grafpontos);*/

        }

        public MainViewModel(List<Coordenada> a)
        {
            this.MyModel = new PlotModel { Title = "Gráfico resistência Momento x Normal" };
            this.MyModel.Axes.Add(new LinearAxis { Title = "Normal (kN)", Position = AxisPosition.Bottom /*, Minimum = -20, Maximum = 80 */});
            this.MyModel.Axes.Add(new LinearAxis { Title = "Momento (kN.m)", Position = AxisPosition.Left/*, Minimum = -20, Maximum = 80*/});
            var grafpontos = new ScatterSeries();
            grafpontos.Points.Add(new ScatterPoint(40, 40, 4));
            grafpontos.Points.Add(new ScatterPoint(40, 40, 4));
            this.MyModel.Series.Add(grafpontos);
        }
        public PlotModel MyModel { get; private set; }

    }
}