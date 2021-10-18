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
using System.Windows.Shapes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Element = Autodesk.Revit.DB.Element;

namespace ProjetoFinal
{
    /// <summary>
    /// Lógica interna para Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public List<Reference> SelElements { get; set; }

        public Document Document { get; set; }       

        public Window1()
        {

            InitializeComponent();
            // Gráfico apenas representativo para quando iniciar a para inicilizar a janela
            PlotModel model = new PlotModel() { Title = "Gráfico resistência Momento x Normal" }; ;
            model.Axes.Add(new LinearAxis { Title = "Normal (kN)", Position = AxisPosition.Bottom });
            model.Axes.Add(new LinearAxis { Title = "Momento (kN.m)", Position = AxisPosition.Left});
            var grafpontos = new ScatterSeries();
            grafpontos.Points.Add(new ScatterPoint(40, 40, 0)); // ponto com tamanho 0, para conter ao menos um dado e plotar o grafico representativo.
            model.Series.Add(grafpontos);
            Grafica.Model = model;
        }

        public Window1(Document doc, List<Reference> selElements)
        {
            Document = doc;
            SelElements = selElements;
            DataContext = this;
            InitializeComponent();

    
            PlotModel model = new PlotModel();

            model.Axes.Add(new LinearAxis { Title = "Normal (kN)", Position = AxisPosition.Bottom /*, Minimum = -20, Maximum = 80 */});
            model.Axes.Add(new LinearAxis { Title = "Momento (kN.m)", Position = AxisPosition.Left/*, Minimum = -20, Maximum = 80*/});
            model = new PlotModel { Title = "Gráfico resistência Momento x Normal" };

            var grafpontos = new ScatterSeries();
            grafpontos.Points.Add(new ScatterPoint(40, 40, 0));

            model.Series.Add(grafpontos);
            Grafica.Model = model;
            
        }

        private void checkBox1_Click_1(object sender, RoutedEventArgs e)
        {
            checkBox2.IsChecked = false;
            checkBox3.IsChecked = false;
            checkBox4.IsChecked = false;

        }

        private void checkBox2_Click(object sender, RoutedEventArgs e)
        {
            checkBox1.IsChecked = false;
            checkBox3.IsChecked = false;
            checkBox4.IsChecked = false;
        }

        private void checkBox4_Click(object sender, RoutedEventArgs e)
        {
            checkBox1.IsChecked = false;
            checkBox3.IsChecked = false;
            checkBox2.IsChecked = false;
        }

        private void checkBox3_Click(object sender, RoutedEventArgs e)
        {
            checkBox1.IsChecked = false;
            checkBox4.IsChecked = false;
            checkBox2.IsChecked = false;
        }

        private void comboBox1_Loaded(object sender, RoutedEventArgs e)
        {
            comboBox1.Items.Add("Inserir valores");
            comboBox1.Items.Add(3.0);
            comboBox1.Items.Add(4.0);
            comboBox1.Items.Add(6.0);
            comboBox1.Items.Add(8.0);
            comboBox1.Items.Add(10.0);
            comboBox1.Items.Add(12.0);
            comboBox1.Items.Add(14.0);
            comboBox1.Items.Add(16.0);
            comboBox1.Items.Add(18.0);
            comboBox1.Items.Add(20.0);
            comboBox1.Items.Add(22.0);
            comboBox1.Items.Add(24.0);

        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox1.SelectedValue.ToString() != "Inserir valores")
            {

                labelFpk.IsEnabled = false;
                labelFpkcheio.IsEnabled = false;


                ResistenciaBloco fpktab = TabRes.GetResistencia(Convert.ToDouble(comboBox1.SelectedValue.ToString()));
                labelFpk.Text = fpktab.Fpk.ToString();

                ResistenciaBloco fpkcheiotab = TabRes.GetResistencia(Convert.ToDouble(comboBox1.SelectedValue.ToString()));
                labelFpkcheio.Text = fpkcheiotab.FpkCheio.ToString();


            }
            else
            {
                labelFpk.IsEnabled = true;
                labelFpk.Clear();
                labelFpkcheio.IsEnabled = true;
                labelFpkcheio.Clear();
            }

        }

        private void btnCalcular_Click(object sender, RoutedEventArgs e)
        {
            //abrir janela para entrar com valores de input

            List<Blocos> BlocosAlma = new List<Blocos>();
            List<Blocos> BlocosFlange = new List<Blocos>();
            //List<Graute> ListaGraute = new List<Graute>();
            List<Ferros> ListaFerros = new List<Ferros>();


            int DirecaoVento = 0;
            switch (true)
            {
                case true when checkBox1.IsChecked == true:
                    DirecaoVento = 1;
                    break;
                case true when checkBox2.IsChecked == true:
                    DirecaoVento = 2;
                    break;
                case true when checkBox3.IsChecked == true:
                    DirecaoVento = 3;
                    break;
                case true when checkBox4.IsChecked == true:
                    DirecaoVento = 4;
                    break;
            }


            foreach (Reference refelem in SelElements) //Trabalhando com os dados de input, referenciando cada elemento na sua devida lista
            {
                Element auxelem = Document.GetElement(refelem);
                var elemento = auxelem as FamilyInstance;
                string tipoelemento = elemento.Symbol.GetParameters("TipoElemento")[0].AsString();
                if (tipoelemento == "Bloco")
                {
                    AddBloco(elemento, auxelem, DirecaoVento);
                }
                else if (tipoelemento == "Ferro")
                {
                    AddFerro(elemento, DirecaoVento);
                }
                /*var location1 = elemento.Location as LocationPoint;
                double m1 = location1.Point.X;
                teste1 = elemento.Symbol.GetParameters("Comprimento")[0].AsDouble();*/

            };

            void AddBloco(FamilyInstance familyelem, Element elem, int direcaovento)
            {
                var location = familyelem.Location as LocationPoint;
                double refcomprimento = familyelem.Symbol.GetParameters("Comprimento")[0].AsDouble() * 30.48;
                double refespessura = familyelem.Symbol.GetParameters("Espessura")[0].AsDouble() * 30.48;
                double refXcg = 0;
                double refYcg = 0;
                switch (direcaovento)
                {

                    case 1:
                        refXcg = location.Point.Y * 30.48;
                        refYcg = location.Point.X * 30.48;
                        break;

                    case 2:
                        refXcg = location.Point.X * 30.48;
                        refYcg = location.Point.Y * 30.48;
                        break;

                    case 3:
                        refXcg = -location.Point.X * 30.48;
                        refYcg = location.Point.Y * 30.48;
                        break;

                    case 4:
                        refXcg = -location.Point.Y * 30.48;
                        refYcg = location.Point.X * 30.48;
                        break;
                }
                /*double refXcg = location.Point.X * 30.48;
                double refYcg = location.Point.Y * 30.48;*/

                double reforientacao = 0;
                if (DirecaoVento == 1 || DirecaoVento == 4) {
                    reforientacao = Math.Round(location.Rotation / Math.PI, 6) + 0.5;
                } else
                {
                    reforientacao = Math.Round(location.Rotation / Math.PI, 6);
                }



                double refarea = refcomprimento * refespessura;
                string refgrauteado = elem.GetParameters("grauteado")[0].AsValueString();
                //string refgrauteado = elem


                if (reforientacao == 0.5 || reforientacao == 1.5 || reforientacao == 2.5)
                {
                    BlocosFlange.Add(new Blocos(refcomprimento, refespessura, refXcg, refYcg, reforientacao, refarea, refgrauteado));
                }
                else if (reforientacao == 0 || reforientacao == 1 || reforientacao == 2)
                {
                    BlocosAlma.Add(new Blocos(refcomprimento, refespessura, refXcg, refYcg, reforientacao, refarea, refgrauteado));
                }
            }

            void AddFerro(FamilyInstance elem, int direcaovento)
            {
                var location = elem.Location as LocationPoint;
                double refbitola = elem.Symbol.GetParameters("Bitola")[0].AsDouble() * 30.48;
                double refarea = Math.PI * refbitola * refbitola / 4 / 100;  //  PI*D²/4   cm²
                double refXcg = 0;
                double refYcg = 0;
                switch (direcaovento)
                {
                    case 1:
                        refXcg = location.Point.Y * 30.48;
                        refYcg = location.Point.X * 30.48;
                        break;

                    case 2:
                        refXcg = location.Point.X * 30.48;
                        refYcg = location.Point.Y * 30.48;
                        break;

                    case 3:
                        refXcg = -location.Point.X * 30.48;
                        refYcg = location.Point.Y * 30.48;
                        break;

                    case 4:
                        refXcg = -location.Point.Y * 30.48;
                        refYcg = location.Point.X * 30.48;
                        break;
                }

                ListaFerros.Add(new Ferros(refbitola, refXcg, refYcg, refarea));
            }

            //var plottest = new PlotModel { Title = "TESTE" };
            //plottest.Axes.Add(new LinearAxis { Title = "Normal (kN)", Position = AxisPosition.Bottom /*, Minimum = -20, Maximum = 80 */});
            //plottest.Axes.Add(new LinearAxis { Title = "Momento (kN.m)", Position = AxisPosition.Left/*, Minimum = -20, Maximum = 80*/});


            #region Algoritmo de Cálculo

            if (BlocosAlma.Count == 0)
            {
                TaskDialog.Show("ERRO", "Selecione os elementos");

            }

            BlocosAlma = BlocosAlma.OrderBy(m => m.Xcg).ToList();
            BlocosFlange = BlocosFlange.OrderBy(m => m.Xcg).ToList();

            //depois de ordenado tudo, começa esse passo do codigo
            double InicioLN; //encontrando o ponto de inicio e fim chute LN
            double fimLN;
            double EixoPrincipal = BlocosAlma[0].Ycg;
            double he = 220; //receber valor como input
            double te = BlocosAlma[0].espessura;
            double indesbeltez = he / te;
            double R = 0.939;

            List<TabelaResistencia> resistencias = new List<TabelaResistencia>(); // colunas fbk / fpk/ fpk*  //tabela resistencia referencia norma VALORES EM MPa
            resistencias.Add(new TabelaResistencia(3, 2.4, 4.8));

            // Definindo fpk


            double fpk = 0;
            double fpkcheio = 0;

            if (comboBox1.SelectedValue.ToString() != "Inserir valores")
            {

                ResistenciaBloco fpktab = TabRes.GetResistencia(Convert.ToDouble(comboBox1.SelectedValue.ToString()));
                fpk = fpktab.Fpk;

                ResistenciaBloco fpkcheiotab = TabRes.GetResistencia(Convert.ToDouble(comboBox1.SelectedValue.ToString()));
                fpkcheio = fpkcheiotab.FpkCheio;

            }
            else
            {
                fpk = Convert.ToDouble(labelFpk.Text);
                fpkcheio = Convert.ToDouble(labelFpkcheio.Text);
            }


            #region COMO PEGAR RESISTENCIA
            double inputusuario = 4;//valor que virá do combobox na UI
            ResistenciaBloco PEGUEIRESISTENCIA = TabRes.GetResistencia(inputusuario);//instancia do struct com os valores de acordo com o fbk escolhido pelo usuario
            var fbkEscolhido = PEGUEIRESISTENCIA.Fbk;

            //foi criado um struct "Resistencia Bloco" para armazenar os valores de resistencia.
            //A classe TabRes é estática pois as informações nunca mudam.
            //O método GetResistencia devolve o struct "Resistencia Bloco". Esse método será usado com o vlor que o usuario escolher de input
            #endregion


            // Encontrando Inicio e Fim da LN

            if (BlocosFlange.Count == 0)
            {
                InicioLN = (BlocosAlma[0].Xcg - (BlocosAlma[0].comprimento / 2));
                fimLN = (BlocosAlma.Last().Xcg + (BlocosAlma.Last().comprimento / 2));

            }
            else
            {
                double faceblocoalmainicio = (BlocosAlma[0].Xcg - (BlocosAlma[0].comprimento / 2));
                double faceblocoflangeinicio = (BlocosFlange[0].Xcg - (BlocosFlange[0].espessura / 2));
                double faceblocoalmafim = BlocosAlma.Last().Xcg + (BlocosAlma.Last().comprimento / 2);
                double faceblocoflangefim = BlocosFlange.Last().Xcg + (BlocosFlange.Last().espessura / 2);

                if (faceblocoalmainicio >= faceblocoflangeinicio && faceblocoalmainicio - faceblocoflangeinicio < 16)
                {
                    InicioLN = faceblocoflangeinicio;
                }
                else
                {
                    InicioLN = faceblocoalmainicio;
                }

                if (faceblocoalmafim <= faceblocoflangefim && faceblocoflangefim - faceblocoalmafim < 16)
                {
                    fimLN = faceblocoflangefim;
                }
                else
                {
                    fimLN = faceblocoalmafim;
                }
                //
            }


            #endregion
            PlotModel model = new PlotModel();
            model = new PlotModel { Title = "Gráfico resistência Momento x Normal" };
            var grafpontos = new ScatterSeries();
            // Calculo dos pontos de momento e normal percorrendo a seção
            double normal, momento;
            double Xcg = CalcularCentroide(BlocosAlma, BlocosFlange, InicioLN);
            double ChuteLN = fimLN;  // colocando como start da ln o fim da seção , considerando assim partindo da parte toda tracionada até começo da seção considerando parte toda comprimida
            List<Coordenada> pontosgraficoresistencia = new List<Coordenada>();
            while (ChuteLN >= InicioLN)
            {
                normal = Normal(ListaFerros, BlocosAlma, BlocosFlange, fimLN, InicioLN, ChuteLN, EixoPrincipal, fpkcheio, fpk, R);
                momento = Momento(ListaFerros, BlocosAlma, BlocosFlange, fimLN, InicioLN, ChuteLN, EixoPrincipal, fpkcheio, fpk, R, Xcg);
                ChuteLN = ChuteLN - 1;
                grafpontos.Points.Add(new ScatterPoint(normal, momento, 2));
            }
            List<Coordenada> teste = new List<Coordenada>();
            grafpontos.MarkerType = MarkerType.Circle;
            grafpontos.MarkerFill = OxyColors.Red;
            model.Axes.Add(new LinearAxis { Title = "Normal (kN)", Position = AxisPosition.Bottom /*, Minimum = -20, Maximum = 80 */});
            model.Axes.Add(new LinearAxis { Title = "Momento (kN.m)", Position = AxisPosition.Left/*, Minimum = -20, Maximum = 80*/});
            model.Series.Add(grafpontos);
            Grafica.Model = model;
            var lo = 0;

            //Verificar armadura minima
            if(text_normalatuante.Text.Length>0 || text_momentoatuante.Text.Length > 0)
            {
                VefificarArmaduraMinima();
            }

            void VefificarArmaduraMinima()
            {
                bool parede100 = true;
                double normalresistente = 0;

                double normalatuante = Convert.ToDouble(text_normalatuante.Text);
                double momentoatuante = Convert.ToDouble(text_momentoatuante.Text);

                foreach (Blocos bloco in BlocosAlma)
                {
                    if (!bloco.grauteado)
                    {
                        parede100 = false;
                    }
                }
                foreach (Blocos bloco in BlocosFlange)
                {
                    if (!bloco.grauteado)
                    {
                        parede100 = false;
                    }
                }


                if (parede100)
                {
                    normalresistente = fpkcheio * 0.7 / 2.0 * 14 * (Math.Abs(fimLN - InicioLN) - 2 * (momentoatuante / normalatuante));  // fd*B(h-2ex)
                }
                else
                {
                    normalresistente = fpk * 0.7 / 2.0 * 14 * (Math.Abs(fimLN - InicioLN) - 2 * (momentoatuante / normalatuante));
                }

                if (normalatuante < normalresistente)
                {
                text_armaduraminima.Text = "PASSA";}
                else { 
                text_armaduraminima.Text = "NÃO PASSA";}

            }

            double Normal(List<Ferros> ferros, List<Blocos> alma, List<Blocos> flange, double reffimLN, double refInicioLN, double Xln, double eixoprincipal, double reffpkcheio, double reffpk, double refR)
            {   // MPa para KN/cm2 dividir por 10
                double tracao = 0;
                double compressao = 0;
                double Xlc = Xln + Math.Abs(Xln - fimLN) * 0.2;
                double Es = 21000; // KN/cm²  210000MPa
                double fyk = 50; //KN/cm²     //500MPa
                double deformacaoescoamento = (fyk / 1.15) / Es; // fyd/Es                   
                foreach (Ferros ferro in ferros)
                {
                    if (ferro.Xcg < Xln && ((Math.Abs(ferro.Ycg - eixoprincipal)) - 7 < 84))
                    {
                        double ei = ((0.003) / Math.Abs(fimLN - Xln) * (Math.Abs(Xln - ferro.Xcg)));  // compatibilização  (0.003 / x) = Ei / (di - x) 

                        if (ei < deformacaoescoamento)
                        {   // compatibilização    (0.003 / x) = Ei / (di - x) 
                            tracao += ferro.area / 1.15 * ei * Es * 0.5;   //(As/Gamas) * Ei* ES    //KN  (0.5 da norma)
                        }
                        else if (ei >= deformacaoescoamento && ei < 0.01)
                        {
                            tracao += ferro.area * fyk / 1.15 * 0.5;   // (As/Gamas) * fyk * 0.5       (0.5 da norma) // KN/cm²
                        }
                        else if (ei > 0.01)
                        {
                            // condição que ferro ruptura e nao é encontrado um valor de momento para este caso de LN.
                        }
                    }
                }

                foreach (Blocos bloco in alma)
                {
                    if ((bloco.Xcg - bloco.comprimento / 2) > Xlc)
                    {
                        //Bloco inteiro entra na contribuição, ai só analisar % de graute, sua resistencia e multiplicar pelo comprimento
                        if (bloco.grauteado)
                        {
                            compressao += (fpkcheio * 0.7) / 2.0 * bloco.area * R;
                        }
                        else
                        {
                            compressao += (fpk * 0.7) / 2.0 * bloco.area * R;
                        }

                    }
                    else if (((bloco.Xcg - bloco.comprimento / 2) < Xlc) & (Xlc < (bloco.Xcg + bloco.comprimento / 2)))
                    {
                        if (bloco.grauteado)
                        {
                            compressao += (fpkcheio * 0.7) / 2.0 * (((bloco.Xcg + bloco.comprimento / 2) - Xlc) * bloco.espessura) * R;
                        }
                        else
                        {
                            compressao += (fpk * 0.7) / 2.0 * (((bloco.Xcg + bloco.comprimento / 2) - Xlc) * bloco.espessura) * R;
                        }
                    }

                }


                foreach (Blocos bloco in flange)
                {

                    if ((bloco.Xcg - bloco.espessura / 2) > Xlc) // entra toda espessura do bloco na contribuição
                    {
                        if (((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 + bloco.comprimento / 2) < 84) // soma todo comprimento do bloco, esta dentro da flange de 84=6.t 
                        {
                            if (bloco.grauteado)
                            {
                                compressao += (fpkcheio * 0.7) / 2.0 * bloco.area * R;
                            }
                            else
                            {
                                compressao += (fpk * 0.7) / 2.0 * bloco.area * R;
                            }

                        }
                        else if ((((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 + bloco.comprimento / 2) > 84) & (((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 - bloco.comprimento / 2) < 84))
                        {
                            //entra o comprimento parcial do bloco na conta 
                            if (bloco.grauteado)
                            {
                                compressao += (fpkcheio * 0.7) / 2.0 * ((84 - ((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 - bloco.comprimento / 2)) * 14) * R;
                            }
                            else
                            {
                                compressao += (fpk * 0.7) / 2.0 * ((84 - ((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 - bloco.comprimento / 2)) * 14) * R;
                            }
                        }

                    }
                    else if (((bloco.Xcg - bloco.espessura / 2) < Xlc) & (Xlc < (bloco.Xcg + bloco.espessura / 2))) // entra uma parte da espessura do bloco
                    {
                        if (((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 + bloco.comprimento / 2) < 84) // soma todo comprimento do bloco, esta dentro da flange de 84=6.t 
                        {

                            //entra contribuição do comprimento inteiro, mas só uma parte da espessura
                            if (bloco.grauteado)
                            {
                                compressao += (fpkcheio * 0.7) / 2.0 * (((bloco.Xcg + bloco.espessura / 2) - Xlc) * bloco.comprimento) * R;
                            }
                            else
                            {
                                compressao += (fpk * 0.7) / 2.0 * (((bloco.Xcg + bloco.espessura / 2) - Xlc) * bloco.comprimento) * R;
                            }
                        }
                        else if ((((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 + bloco.comprimento / 2) > 84) & (((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 - bloco.comprimento / 2) < 84))
                        {
                            //entra o comprimento parcial do bloco na conta e também uma espessura parcial
                            if (bloco.grauteado)
                            {
                                compressao += (fpkcheio * 0.7) / 2.0 * (((bloco.Xcg + bloco.espessura / 2) - Xlc) * (84 - (Math.Abs(bloco.Ycg - eixoprincipal) - 7 - bloco.comprimento / 2))) * R;
                            }
                            else
                            {
                                compressao += (fpk * 0.7) / 2.0 * (((bloco.Xcg + bloco.espessura / 2) - Xlc) * (84 - (Math.Abs(bloco.Ycg - eixoprincipal) - 7 - bloco.comprimento / 2))) * R;
                            }
                        }

                    }
                }

                return compressao - tracao;

            }
            double Momento(List<Ferros> ferros, List<Blocos> alma, List<Blocos> flange, double reffimLN, double refInicioLN, double Xln, double eixoprincipal, double reffpkcheio, double reffpk, double refR, double refXcg)
            {

                double Mrd = 0;
                double Xlc = Xln + Math.Abs(Xln - fimLN) * 0.2;
                double Es = 21000; // kN/cm²
                double fyk = 50; //kN/cm²
                double defescoamento = (fyk / 1.15) / Es; // fyd/Es

                foreach (Ferros ferro in ferros)
                {
                    if (ferro.Xcg < Xln)
                    {
                        double ei = ((0.003) / Math.Abs(fimLN - Xln) * (Math.Abs(Xln - ferro.Xcg)));
                        if (ei < defescoamento)
                        {   // compatibilização    (0.003 / x) = Ei / (di - x) 
                            Mrd += ferro.area / 1.15 * ei * Es * 0.5 * (Xcg - ferro.Xcg);   //(As/Gamas) * Ei* ES * di //Valor em KN*cm
                        }
                        else if (ei > defescoamento && ei < 0.01)
                        {
                            Mrd += ferro.area * fyk / 1.15 * 0.5 * (Xcg - ferro.Xcg);  //(As/Gamas) * fyk * di
                        }
                        else
                        {
                            // condição que ferro ruptura e nao é encontrado um valor de momento para este caso de LN.
                        }
                    }
                }
                foreach (Blocos bloco in alma)
                {
                    if ((bloco.Xcg - bloco.comprimento / 2) > Xlc)
                    {   //Bloco inteiro entra na contribuição, ai só analisar % de graute, sua resistencia e multiplicar pelo comprimento

                        if (bloco.grauteado)
                        {
                            Mrd += (fpkcheio * 0.7) / 2.0 * bloco.area * R * (bloco.Xcg - Xcg); // Fd= fpk* 0.7 / 2 * A * R * Di  //Valor em KN*cm
                        }
                        else
                        {
                            Mrd += (fpk * 0.7) / 2.0 * bloco.area * R * (bloco.Xcg - Xcg);
                        }

                    }
                    else if (((bloco.Xcg - bloco.comprimento / 2) < Xlc) & (Xlc < (bloco.Xcg + bloco.comprimento / 2)))
                    {
                        if (bloco.grauteado)
                        {
                            Mrd += (fpkcheio * 0.7) / 2.0 * (((bloco.Xcg + bloco.comprimento / 2) - Xlc) * bloco.espessura) * R * ((Xlc + ((bloco.Xcg + bloco.comprimento / 2) - Xlc) / 2) - Xcg);
                        }
                        else
                        {
                            Mrd += (fpk * 0.7) / 2.0 * (((bloco.Xcg + bloco.comprimento / 2) - Xlc) * bloco.espessura) * R * ((Xlc + ((bloco.Xcg + bloco.comprimento / 2) - Xlc) / 2) - Xcg);
                        }
                    }
                }
                foreach (Blocos bloco in flange)
                {

                    if ((bloco.Xcg - bloco.espessura / 2) > Xlc) // entra toda espessura do bloco na contribuição
                    {
                        if (((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 + bloco.comprimento / 2) < 84) // soma todo comprimento do bloco, esta dentro da flange de 84=6.t 
                        {
                            if (bloco.grauteado)
                            {
                                Mrd += (fpkcheio * 0.7) / 2.0 * bloco.area * R * (bloco.Xcg - Xcg);
                            }
                            else
                            {
                                Mrd += (fpk * 0.7) / 2.0 * bloco.area * R * (bloco.Xcg - Xcg);
                            }

                        }
                        else if ((((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 + bloco.comprimento / 2) > 84) & (((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 - bloco.comprimento / 2) < 84))
                        {
                            //entra o comprimento parcial do bloco na conta // Considerar parte nao grauteada
                            if (bloco.grauteado)
                            {
                                Mrd += (fpkcheio * 0.7) / 2.0 * ((84 - ((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 - bloco.comprimento / 2)) * 14) * R * (bloco.Xcg - Xcg);
                            }
                            else
                            {
                                Mrd += (fpk * 0.7) / 2.0 * ((84 - ((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 - bloco.comprimento / 2)) * 14) * R * (bloco.Xcg - Xcg);
                            }
                        }

                    }
                    else if (((bloco.Xcg - bloco.espessura / 2) < Xlc) & (Xlc < (bloco.Xcg + bloco.espessura / 2))) // entra uma parte da espessura do bloco
                    {
                        if (((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 + bloco.comprimento / 2) < 84) // soma todo comprimento do bloco, esta dentro da flange de 84=6.t 
                        {

                            //entra contribuição do comprimento inteiro, mas só uma parte da espessura, ver como calcular...
                            if (bloco.grauteado)
                            {
                                Mrd += (fpkcheio * 0.7) / 2.0 * (((bloco.Xcg + bloco.espessura / 2) - Xlc) * bloco.comprimento) * R * ((Xlc + ((bloco.Xcg + bloco.espessura / 2) - Xlc) / 2) - Xcg);
                            }
                            else
                            {
                                Mrd += (fpk * 0.7) / 2.0 * (((bloco.Xcg + bloco.espessura / 2) - Xlc) * bloco.comprimento) * R * ((Xlc + ((bloco.Xcg + bloco.espessura / 2) - Xlc) / 2) - Xcg);
                            }
                        }
                        else if ((((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 + bloco.comprimento / 2) > 84) & (((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 - bloco.comprimento / 2) < 84))
                        {
                            //entra o comprimento parcial do bloco na conta e também uma espessura parcial, ver como calcular...
                            // considerar como nao grauteado
                            if (bloco.grauteado)
                            {
                                Mrd += (fpkcheio * 0.7) / 2.0 * (((bloco.Xcg + bloco.espessura / 2) - Xlc) * (84 - (Math.Abs(bloco.Ycg - eixoprincipal) - 7 - bloco.comprimento / 2))) * R * ((Xlc + ((bloco.Xcg + bloco.espessura / 2) - Xlc) / 2) - Xcg);
                            }
                            else
                            {
                                Mrd += (fpk * 0.7) / 2.0 * (((bloco.Xcg + bloco.espessura / 2) - Xlc) * (84 - (Math.Abs(bloco.Ycg - eixoprincipal) - 7 - bloco.comprimento / 2))) * R * ((Xlc + ((bloco.Xcg + bloco.espessura / 2) - Xlc) / 2) - Xcg);
                            }
                        }
                    }
                }
                return Mrd / 100; //passando para KN*m
            }

            double CalcularCentroide(List<Blocos> alma, List<Blocos> flange, double refInicioLN) // entrar com lista de blocosflange e lista de bloco normal e inicio linha neutra
            {
                double refXcg, SomaXi = 0;
                double Area = 0;

                foreach (Blocos bloco in flange)
                {
                    Area += bloco.espessura * bloco.comprimento;
                    SomaXi += bloco.espessura * bloco.comprimento * Math.Abs((bloco.Xcg - InicioLN));
                }

                foreach (Blocos bloco in alma)
                {
                    Area += bloco.espessura * bloco.comprimento;
                    SomaXi += bloco.espessura * bloco.comprimento * Math.Abs((bloco.Xcg - InicioLN));
                }
                return (SomaXi / Area + InicioLN);
            }
        }

        private void labelFpk_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (labelFpk.Text.Contains("."))
            {
                System.Windows.MessageBox.Show("Utilizar vírgula ',' para números fracionados.");
                labelFpk.Clear();
            }
        }

        private void labelFpkcheio_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (labelFpkcheio.Text.Contains("."))
            {
                System.Windows.MessageBox.Show("Utilizar vírgula ',' para números fracionados.");
                labelFpkcheio.Clear();
            }

        }

   
    }
}
