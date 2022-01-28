using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        public Window1(Document doc, List<Reference> selElements)
        {
            Document = doc;
            SelElements = selElements;
            DataContext = this;
            InitializeComponent();

            // criando gráfico apenas representativo para quando abrir a janela.
            PlotModel model = new PlotModel();
            model.Axes.Add(new LinearAxis { Title = "Normal (kN)", Position = AxisPosition.Bottom /*, Minimum = -20, Maximum = 80 */});
            model.Axes.Add(new LinearAxis { Title = "Momento (kN.m)", Position = AxisPosition.Left/*, Minimum = -20, Maximum = 80*/});
            model = new PlotModel { Title = "Gráfico resistência Normal x Momento" };
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
            var timer = new Stopwatch();
            timer.Start();
            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            StringBuilder sb4 = new StringBuilder();
            StringBuilder testeareas = new StringBuilder();
            StringBuilder testeareas2 = new StringBuilder();
            StringBuilder dadosgerais = new StringBuilder();
            StringBuilder sbfinal = new StringBuilder();
            StringBuilder dadosNormal = new StringBuilder();


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

            #region TRATANDO OS DADOS DE ENTRADA

            // ordenando e agrupando os dados de entrada de acordo com o tipo de elemento e suas propriedades para cada instância de elemento do revit.
            List<Blocos> BlocosAlma = new List<Blocos>();
            List<Blocos> BlocosFlange = new List<Blocos>();
            List<Ferros> ListaFerros = new List<Ferros>();

            foreach (Reference refelem in SelElements) // Adicionando cada tipo de elemento em sua determinada lista
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
                        refXcg =  location.Point.Y * 30.48;
                        refYcg =  location.Point.X * 30.48;
                        break;

                    case 2:
                        refXcg =  location.Point.X * 30.48;
                        refYcg =  location.Point.Y * 30.48;
                        break;

                    case 3:
                        refXcg = -location.Point.X * 30.48;
                        refYcg =  location.Point.Y * 30.48;
                        break;

                    case 4:
                        refXcg = -location.Point.Y * 30.48;
                        refYcg =  location.Point.X * 30.48;
                        break;
                }

                double reforientacao = 0;
                if (DirecaoVento == 1 || DirecaoVento == 4) {
                    reforientacao = Math.Round(location.Rotation / Math.PI, 6) + 0.5;
                } else
                {
                    reforientacao = Math.Round(location.Rotation / Math.PI, 6);
                }

                double refarea = refcomprimento * refespessura;
                //string refgrauteado = elem.GetParameters("grauteado")[0].AsValueString();
                bool refgrauteado = Convert.ToBoolean(elem.GetParameters("grauteado")[0].AsInteger());

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
                double refarea = (Math.PI * refbitola * refbitola / 4) / 100;  //  PI*D²/4   cm²
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


            if (BlocosAlma.Count == 0)
            {
                TaskDialog.Show("ERRO", "Selecione os elementos");

            }

            BlocosAlma = BlocosAlma.OrderBy(m => m.Xcg).ToList();
            BlocosFlange = BlocosFlange.OrderBy(m => m.Xcg).ToList();
            double he = Convert.ToDouble(text_alturaparede.Text);
            double te = BlocosAlma[0].espessura;
            double indesbeltez = he / te;
            double R = 1 - Math.Pow(indesbeltez / 40, 3);
            

            #endregion

            #region DEFININDO FPK

            double fpk;
            double fpkcheio;

            if (comboBox1.SelectedValue.ToString() != "Inserir valores")
            {

                ResistenciaBloco fpktab = TabRes.GetResistencia(Convert.ToDouble(comboBox1.SelectedValue.ToString()));
                fpk = fpktab.Fpk/10; //divindo 10 passando Mpa Para kN/cm²
                    
                ResistenciaBloco fpkcheiotab = TabRes.GetResistencia(Convert.ToDouble(comboBox1.SelectedValue.ToString()));
                fpkcheio = fpkcheiotab.FpkCheio/10; //divindo 10 passando Mpa Para kN/cm²
                dadosgerais.Append("Fbk:").Append(comboBox1.SelectedValue.ToString()).Append(" Mpa\n");
            }
            else
            {
                fpk = Convert.ToDouble(labelFpk.Text)/10;
                fpkcheio = Convert.ToDouble(labelFpkcheio.Text)/10;
                dadosgerais.Append("Fbk: -").Append('\n');
            }
            dadosgerais.Append("Fpk: ").Append((fpk*10).ToString()).Append(" Mpa\n").Append("Fpk*: ").Append((fpkcheio*10).ToString()).Append(" Mpa\n");

            #endregion

            #region COMO PEGAR RESISTENCIA
            double inputusuario = 4;//valor que virá do combobox na UI
            ResistenciaBloco PEGUEIRESISTENCIA = TabRes.GetResistencia(inputusuario);//instancia do struct com os valores de acordo com o fbk escolhido pelo usuario
            var fbkEscolhido = PEGUEIRESISTENCIA.Fbk;

            //foi criado um struct "Resistencia Bloco" para armazenar os valores de resistencia.
            //A classe TabRes é estática pois as informações nunca mudam.
            //O método GetResistencia devolve o struct "Resistencia Bloco". Esse método será usado com o vlor que o usuario escolher de input
            #endregion

            #region INICIO E FIM DA SEÇÃO
            // Encontrando Inicio e Fim da Seção

            double InicioLN;
            double fimLN;
            double EixoPrincipal = BlocosAlma[0].Ycg;
            dadosgerais.Append("====================  DADOS GERAIS  ====================\n").Append('\n');
            dadosgerais.Append("Altura efetiva: ").Append(he.ToString()).Append(" cm\n").Append("R: ").Append(R.ToString()).Append('\n').Append("Indice Esbeltez (λ): ").Append(indesbeltez.ToString()).Append('\n');


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
            dadosgerais.Append("Comprimento Subestrutura: ").Append((fimLN-InicioLN).ToString()).Append('\n');
            #endregion



            PlotModel model = new PlotModel() { Title = "Gráfico resistência Normal x  Momento" };
            var grafpontos = new ScatterSeries();
            var grafpontosrompidos= new ScatterSeries();
            var grafpontos3= new ScatterSeries();
            var grafpontos4 = new ScatterSeries();
            var grafpontos4a = new ScatterSeries();
            var grafpontos5 = new ScatterSeries();
            var pontoentrada = new ScatterSeries();
            bool ruptura;
            bool escoamento;
            bool dominio4a;
            bool dominio4;

            double Xcg = CalcularCentroide(BlocosAlma, BlocosFlange, InicioLN);
            double ChuteLN = fimLN;  // colocando como start da ln o fim da seção , considerando assim partindo da parte toda tracionada até começo da seção considerando parte toda comprimida
            double normal,momento,FimChuteLN;
            FimChuteLN= fimLN - ((fimLN - InicioLN) / 0.8);
            dadosgerais.Append("Centroide: ").Append((fimLN - Xcg).ToString()).Append('\n');
   
            sb1.Append("Normal (kN)").Append(';').Append("Momento (kN.m)").Append(';').Append("Dist. LN (x)").Append(';').Append("Dist. Comp. (0.8 x)").Append(';').Append("σ (kN/cm²) ").Append(';').Append("ε").Append(';').Append("Contribuição (kN.m)").Append(';').Append('\n');
            double parcial;
            while (ChuteLN >= FimChuteLN)
            {
                normal = Normal(ListaFerros, BlocosAlma, BlocosFlange, fimLN, InicioLN, ChuteLN, EixoPrincipal, fpkcheio, fpk, R);
                momento = Momento(ListaFerros, BlocosAlma, BlocosFlange, fimLN, InicioLN, ChuteLN, EixoPrincipal, fpkcheio, fpk, R, Xcg);

                if (ruptura)
                {//
                    grafpontos.Points.Add(new ScatterPoint(normal, momento, 2, 100, 300));
                }
                else if (!ruptura && !escoamento)
                {
                    grafpontosrompidos.Points.Add(new ScatterPoint(normal, momento, 2, 350, 15));
                }
                else if(escoamento && !dominio4a)
                { 
                grafpontos3.Points.Add(new ScatterPoint(normal, momento, 2, 350, 15));
                    }

                else if (dominio4a && ChuteLN>InicioLN)
                {
                    grafpontos4a.Points.Add(new ScatterPoint(normal, momento, 2, 350, 15));
                }
                else
                {
                    grafpontos5.Points.Add(new ScatterPoint(normal, momento, 2, 350, 15));
                }

                sb1.Append(normal.ToString()).Append(';').Append(momento.ToString()).Append(';').Append(Math.Abs((ChuteLN - fimLN)).ToString()).Append(';').Append(sb2).Append('\n');//.Append(ruptura.ToString()).Append('\n');
                ChuteLN = ChuteLN - 1;
                dadosNormal.Append('\n');
            }


            #region VERIFICANDO ARMADURA MÍNIMA E COMPRESSÃO SIMPLES

            if (text_normalatuante.Text.Length > 0 || text_momentoatuante.Text.Length > 0)
            {
                VerificarArmaduraMinimaeCompressaosimples();
            }
            else
            {
                text_armaduraminima.Clear();
            }

            void VerificarArmaduraMinimaeCompressaosimples()
            {
                bool parede100 = true;
                double normalresistentesimplificado = 0;
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
                    normalresistentesimplificado = fpkcheio * 0.7 / 2.0 * 14 * (Math.Abs(fimLN - InicioLN) - (2 * (momentoatuante / normalatuante)*100));  // fd*B(h-2ex)
                    normalresistente = fpkcheio * 0.7 / 2.0 * 14 * (fimLN - InicioLN) * R;
                }
                else
                {
                    normalresistentesimplificado = fpk * 0.7 / 2.0 * 14 * (Math.Abs(fimLN - InicioLN) - ( 2 * (momentoatuante / normalatuante)*100));
                    normalresistente = fpk * 0.7 / 2.0 * 14 * (fimLN - InicioLN) * R;
                }
                StringBuilder sb3 = new StringBuilder();
                sb3.Append(Math.Abs(fimLN - InicioLN).ToString()).Append(';');
                sb3.Append((momentoatuante / normalatuante).ToString()).Append(';');
                sb3.Append(fpk.ToString()).Append(';');
                sb3.Append(normalresistentesimplificado.ToString()).Append(';');
                File.WriteAllText(@"C:\testes\normalresiste.txt", sb3.ToString());
                if (normalatuante < normalresistentesimplificado  && (momentoatuante / normalatuante) < (0.5 * Math.Abs(fimLN - InicioLN)) )  // Nd<Nrd   e  e<0.5h 
                {
                text_armaduraminima.Text = "PASSA";}
                else { 
                text_armaduraminima.Text = "NÃO PASSA";}

                if (normalresistente > normalatuante)
                {
                    text_compressaosimples.Text = "PASSA";
                }
                else
                {
                    text_compressaosimples.Text = "NÃO PASSA";
                }
                
                



                pontoentrada.Points.Add(new ScatterPoint(normalatuante, momentoatuante, 8));
                pontoentrada.MarkerType = MarkerType.Triangle;
                pontoentrada.MarkerFill = OxyColors.Green;
                model.Series.Add(pontoentrada);
            }

            #endregion

            #region CRIANDO GRÁFICOS E TXT'S
         
            grafpontos.MarkerType = MarkerType.Circle;
            grafpontosrompidos.MarkerType = MarkerType.Circle;
            grafpontos3.MarkerType = MarkerType.Circle;
            grafpontos.MarkerFill = OxyColors.Blue;
            grafpontosrompidos.MarkerFill = OxyColors.Red;
            grafpontos3.MarkerFill = OxyColors.Magenta;
            grafpontos4.MarkerFill = OxyColors.Black;
            grafpontos4a.MarkerFill = OxyColors.DarkOrange;
            grafpontos5.MarkerFill = OxyColors.Gray;

            model.Axes.Add(new LinearAxis { Title = "Normal (kN)", Position = AxisPosition.Bottom /*, Minimum = -20, Maximum = 80 */});
            model.Axes.Add(new LinearAxis { Title = "Momento (kN.m)", Position = AxisPosition.Left/*, Minimum = -20, Maximum = 80*/});
            model.Series.Add(grafpontos);
            model.Series.Add(grafpontosrompidos);
            model.Series.Add(grafpontos3);
            model.Series.Add(grafpontos4);
            model.Series.Add(grafpontos4a);
            model.Series.Add(grafpontos5);
            Grafica.Model = model;


            timer.Stop();
            TimeSpan timeTaken = timer.Elapsed;
            dadosgerais.Append('\n').Append(timeTaken.ToString());
            // criando os txt's
            File.WriteAllText(@"C:\testes\Dados Iterativos.txt", sb1.ToString());
            File.WriteAllText(@"C:\testes\Dados Gerais.txt", dadosgerais.ToString());
            File.WriteAllText(@"C:\testes\Dados normal.txt", dadosNormal.ToString());
            File.WriteAllText(@"C:\testes\Dados areas.txt", sb4.ToString());
            File.WriteAllText(@"C:\testes\teste areas.txt", testeareas.ToString());
            File.WriteAllText(@"C:\testes\teste areas2.txt", testeareas2.ToString());
            #endregion


            double Normal(List<Ferros> ferros, List<Blocos> alma, List<Blocos> flange, double reffimLN, double refInicioLN, double Xln, double eixoprincipal, double reffpkcheio, double reffpk, double refR)
            {   // MPa para KN/cm2 dividir por 10
                double tracao = 0;
                double compressao = 0;
                double Xlc = Xln + Math.Abs(Xln - fimLN) * 0.2;
                double Es = 21000; // KN/cm²  210000MPa
                double fyk = 50; //KN/cm²     //500MPa
                double deformacaoescoamento = (fyk / 1.15) / Es; // fyd/Es
                ruptura = false;
                escoamento = true;
                dominio4a = true;
                double aux=0;
                dadosNormal.Append((fimLN - Xln).ToString()).Append(';').Append( (fimLN-Xlc).ToString()).Append(';');
                foreach (Ferros ferro in ferros)
                {
                    if (ferro.Xcg < Xln)
                    {
                        dominio4a = false;

                        double ei = ((0.003) / Math.Abs(fimLN - Xln) * (Math.Abs(Xln - ferro.Xcg)));  // compatibilzação  (0.003 / x) = Ei / (di - x) 

                        if (ei < deformacaoescoamento)
                        {   // compatibilização    (0.003 / x) = Ei / (di - x) 
                            tracao += ferro.area/ 1.15 * ei * Es;   //(As/Gamas) * Ei* ES    //KN  (0.5 da norma)
                            aux = ferro.area / 1.15 * ei * Es;
                            
                        }
                        else if (ei >= deformacaoescoamento && ei < 0.01)
                        {
                            tracao += ferro.area * fyk / 1.15 ;   // (As/Gamas) * fyk * 0.5       (0.5 da norma) // KN/cm²
                            aux = ferro.area * fyk / 1.15;
                            escoamento = false;
                        }
                        else if (ei > 0.01)
                        {
                            tracao += ferro.area * fyk / 1.15 ;
                            aux = ferro.area * fyk / 1.15;
                            ruptura = true;
                            escoamento = false;
                        }
                        //dadosNormal.Append(aux.ToString()).Append(';') ;
                    }
                }

             
                foreach (Blocos bloco in alma)
                {
                    //dadosNormal.Append((bloco.Xcg - (bloco.comprimento / 2)).ToString()).Append(';').Append(Xlc.ToString()).Append(';').Append(bloco.Xcg.ToString()).Append(';').Append(bloco.comprimento.ToString()).Append(';');
                    
                    if ((bloco.Xcg - (bloco.comprimento / 2)) >= Xlc)
                    {
                        if (bloco.grauteado)
                        {
                            compressao += fpkcheio * 0.7 / 2.0 * bloco.area;
                            parcial= fpkcheio * 0.7 / 2.0 * bloco.area;
                            dadosNormal.Append(fpkcheio.ToString()).Append(';').Append(bloco.area.ToString()).Append(';').Append(';');
                            
                        }
                        else
                        {
                            compressao += fpk * 0.7 / 2.0 * bloco.area ;
                            parcial= fpk * 0.7 / 2.0 * bloco.area;
                            dadosNormal.Append(fpk.ToString()).Append(';').Append(bloco.area.ToString()).Append(';').Append(';');
                        }

                    }

                    else if (((bloco.Xcg - bloco.comprimento / 2) < Xlc) & (Xlc < (bloco.Xcg + bloco.comprimento / 2)))
                    {
                        

                        if (bloco.grauteado)
                        {
                            compressao += (fpkcheio * 0.7) / 2.0 * (((bloco.Xcg + bloco.comprimento / 2) - Xlc) * bloco.espessura) ;
                            parcial= (fpkcheio * 0.7) / 2.0 * (((bloco.Xcg + bloco.comprimento / 2) - Xlc) * bloco.espessura);
                            dadosNormal.Append(parcial.ToString()).Append(';').Append(bloco.grauteado.ToString()).Append(';');
                        }
                        else
                        {
                            compressao += (fpk * 0.7) / 2.0 * (((bloco.Xcg + bloco.comprimento / 2) - Xlc) * bloco.espessura) ;
                            parcial=(fpk * 0.7) / 2.0 * (((bloco.Xcg + bloco.comprimento / 2) - Xlc) * bloco.espessura);
                            dadosNormal.Append(fpk.ToString()).Append(';').Append(bloco.area.ToString()).Append(';').Append(';');
                        }
                    }
                   

                }


                foreach (Blocos bloco in flange)
                {

                    if ((bloco.Xcg - bloco.espessura / 2) >= Xlc) // entra toda espessura do bloco na contribuição
                    {
                        if (((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 + bloco.comprimento / 2) < 84) // soma todo comprimento do bloco, esta dentro da flange de 84=6.t 
                        {
                            if (bloco.grauteado)
                            {
                                compressao += (fpkcheio * 0.7) / 2.0 * bloco.area ;
                                dadosNormal.Append(fpkcheio.ToString()).Append(';').Append(bloco.area.ToString()).Append(';').Append(';');
                            }
                            else
                            {
                                compressao += (fpk * 0.7) / 2.0 * bloco.area ;
                                dadosNormal.Append(fpk.ToString()).Append(';').Append(bloco.area.ToString()).Append(';').Append(';');
                            }

                        }
                        else if ((((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 + bloco.comprimento / 2) > 84) & (((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 - bloco.comprimento / 2) < 84))
                        {
                            //entra o comprimento parcial do bloco na conta 
                            if (bloco.grauteado)
                            {
                                compressao += (fpkcheio * 0.7) / 2.0 * ((84 - ((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 - bloco.comprimento / 2)) * 14) ;
                                dadosNormal.Append(fpkcheio.ToString()).Append(';').Append(bloco.area.ToString()).Append(';').Append(';');
                            }
                            else
                            {
                                compressao += (fpk * 0.7) / 2.0 * ((84 - ((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 - bloco.comprimento / 2)) * 14) ;
                                dadosNormal.Append(fpk.ToString()).Append(';').Append(bloco.area.ToString()).Append(';').Append(';');
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
                                compressao += (fpkcheio * 0.7) / 2.0 * (((bloco.Xcg + bloco.espessura / 2) - Xlc) * bloco.comprimento) ;
                                dadosNormal.Append(fpkcheio.ToString()).Append(';').Append(bloco.area.ToString()).Append(';').Append(';');
                            }
                            else
                            {
                                compressao += (fpk * 0.7) / 2.0 * (((bloco.Xcg + bloco.espessura / 2) - Xlc) * bloco.comprimento) ;
                                dadosNormal.Append(fpk.ToString()).Append(';').Append(bloco.area.ToString()).Append(';').Append(';');
                            }
                        }
                        else if ((((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 + bloco.comprimento / 2) > 84) & (((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 - bloco.comprimento / 2) < 84))
                        {
                            //entra o comprimento parcial do bloco na conta e também uma espessura parcial
                            if (bloco.grauteado)
                            {
                                compressao += (fpkcheio * 0.7) / 2.0 * (((bloco.Xcg + bloco.espessura / 2) - Xlc) * (84 - (Math.Abs(bloco.Ycg - eixoprincipal) - 7 - bloco.comprimento / 2))) ;
                                dadosNormal.Append(fpkcheio.ToString()).Append(';').Append(bloco.area.ToString()).Append(';').Append(';');
                            }
                            else
                            {
                                compressao += (fpk * 0.7) / 2.0 * (((bloco.Xcg + bloco.espessura / 2) - Xlc) * (84 - (Math.Abs(bloco.Ycg - eixoprincipal) - 7 - bloco.comprimento / 2)));
                                dadosNormal.Append(fpk.ToString()).Append(';').Append(bloco.area.ToString()).Append(';').Append(';');
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
                double defescoamento = fyk / 1.15 / Es; // fyd/Es
                double areablocos=0;
                double areablocosgr = 0;
                sb2.Clear();
                sb2.Append((reffimLN - Xlc).ToString()).Append(";");
                
                foreach (Ferros ferro in ferros)
                {
                    
                    if (ferro.Xcg < Xln)
                    {
                        double ei = ((0.003) / Math.Abs(fimLN - Xln) * (Math.Abs(Xln - ferro.Xcg)));
                        if (ei < defescoamento)
                        {   // compatibilização    (0.003 / x) = Ei / (di - x) 
                            Mrd += ferro.area / 1.15 * ei * Es * (Xcg - ferro.Xcg);   //(As/Gamas) * Ei* ES * di //Valor em KN*cm
                            parcial = ferro.area / 1.15 * ei * Es  * (Xcg - ferro.Xcg);   //(As/Gamas) * fyk * di
                            sb2.Append((ei * Es).ToString()).Append(";").Append(ei.ToString()).Append(";").Append((parcial/100).ToString()).Append(";"); ;
                        }
                        else if (ei > defescoamento && ei < 0.01)
                        {
                            Mrd += ferro.area * fyk / 1.15  * (Xcg - ferro.Xcg);  //(As/Gamas) * fyk * di
                            parcial= ferro.area * fyk / 1.15  * (Xcg - ferro.Xcg);  //(As/Gamas) * fyk * di
                            sb2.Append((fyk/1.15).ToString()).Append(";").Append(ei.ToString()).Append(";").Append((parcial/100).ToString()).Append(";");
                        }
                        else
                        {
                            Mrd += ferro.area * fyk / 1.15  * (Xcg - ferro.Xcg);
                            parcial = ferro.area * fyk / 1.15  * (Xcg - ferro.Xcg);  //(As/Gamas) * fyk * di
                            
                            sb2.Append("RUPTURA").Append(";").Append(ei.ToString()).Append(";").Append((parcial/100).ToString()).Append(";");
                        }
                    }
                }
                
                foreach (Blocos bloco in alma)
                {

                    if ((bloco.Xcg - bloco.comprimento / 2) >= Xlc)
                    {   //Bloco inteiro entra na contribuição, ai só analisar % de graute, sua resistencia e multiplicar pelo comprimento

                        if (bloco.grauteado)
                        {
                            Mrd += (fpkcheio * 0.7) / 2.0 * bloco.area  * (bloco.Xcg - Xcg); // Fd= fpk* 0.7 / 2 * A * R * Di  //Valor em KN*cm
                            areablocosgr += bloco.area;
                        }
                        else
                        {
                            Mrd += (fpk * 0.7) / 2.0 * bloco.area  * (bloco.Xcg - Xcg);
                            areablocos += bloco.area;
                            testeareas.Append(bloco.area.ToString() +"; BLOCO INTEIRO ALMA");
                        }

                    }
                    else if (((bloco.Xcg - bloco.comprimento / 2) < Xlc) & (Xlc < (bloco.Xcg + bloco.comprimento / 2)))
                    {
                        if (bloco.grauteado)
                        {
                            Mrd += (fpkcheio * 0.7) / 2.0 * (((bloco.Xcg + bloco.comprimento / 2) - Xlc) * bloco.espessura)  * ((Xlc + ((bloco.Xcg + bloco.comprimento / 2) - Xlc) / 2) - Xcg);
                            areablocosgr += (((bloco.Xcg + bloco.comprimento / 2) - Xlc) * bloco.espessura);
                        }
                        else
                        {
                            Mrd += (fpk * 0.7) / 2.0 * (((bloco.Xcg + bloco.comprimento / 2) - Xlc) * bloco.espessura) * ((Xlc + ((bloco.Xcg + bloco.comprimento / 2) - Xlc) / 2) - Xcg);
                            areablocos += (((bloco.Xcg + bloco.comprimento / 2) - Xlc) * bloco.espessura);
                            testeareas.Append( (((bloco.Xcg + bloco.comprimento / 2) - Xlc) * bloco.espessura).ToString() + "; BLOCO PARCIAL ALMA" );
                            //testeareas2.Append(bloco.Xcg.ToString() + "+" + bloco.comprimento.ToString() + ";" + bloco.espessura.ToString() + ";" + Xlc.ToString() + "\n");
                        }
                    }
                }
               

                foreach (Blocos bloco in flange)
                {

                    if ((bloco.Xcg - bloco.espessura / 2) >= Xlc) // entra toda espessura do bloco na contribuição
                    {
                        if (((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 + bloco.comprimento / 2) < 84) // soma todo comprimento do bloco, esta dentro da flange de 84=6.t 
                        {
                            if (bloco.grauteado)
                            {
                                Mrd += (fpkcheio * 0.7) / 2.0 * bloco.area  * (bloco.Xcg - Xcg);
                                areablocosgr += bloco.area;
                            }
                            else
                            {
                                Mrd += (fpk * 0.7) / 2.0 * bloco.area  * (bloco.Xcg - Xcg);
                                areablocos += bloco.area;
                                testeareas.Append(bloco.area.ToString() + "; BLOCO INTEIRO FLANGE" );
                            }

                        }
                        else if ((((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 + bloco.comprimento / 2) > 84) & (((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 - bloco.comprimento / 2) < 84))
                        {
                            //entra o comprimento parcial do bloco na conta 
                            if (bloco.grauteado)
                            {
                                Mrd += (fpkcheio * 0.7) / 2.0 * ((84 - ((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 - bloco.comprimento / 2)) * 14)  * (bloco.Xcg - Xcg);
                            }
                            else
                            {
                                Mrd += (fpk * 0.7) / 2.0 * ((84 - ((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 - bloco.comprimento / 2)) * 14) * (bloco.Xcg - Xcg);
                                testeareas.Append(((84 - ((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 - bloco.comprimento / 2)) * 14).ToString() + "; BLOCO PARCIAL COMPRIMENTO FLANGE");
                                
                            }
                        }

                    }
                    else if (((bloco.Xcg - bloco.espessura / 2) <= Xlc) & (Xlc < (bloco.Xcg + bloco.espessura / 2))) // entra uma parte da espessura do bloco
                    {
                        if (((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 + bloco.comprimento / 2) < 84) // soma todo comprimento do bloco, esta dentro da flange de 84=6.t 
                        {

                            //entra contribuição do comprimento inteiro, mas só uma parte da espessura
                            if (bloco.grauteado)
                            {
                                Mrd += (fpkcheio * 0.7) / 2.0 * (((bloco.Xcg + bloco.espessura / 2) - Xlc) * bloco.comprimento) * ((Xlc + ((bloco.Xcg + bloco.espessura / 2) - Xlc) / 2) - Xcg);
                                areablocosgr += (((bloco.Xcg + bloco.espessura / 2) - Xlc) * bloco.comprimento);
                            }
                            else
                            {
                                Mrd += (fpk * 0.7) / 2.0 * (((bloco.Xcg + bloco.espessura / 2) - Xlc) * bloco.comprimento)  * ((Xlc + ((bloco.Xcg + bloco.espessura / 2) - Xlc) / 2) - Xcg);
                                areablocos += ((bloco.Xcg + bloco.espessura / 2) - Xlc) * bloco.comprimento;
                                testeareas.Append((((bloco.Xcg + bloco.espessura / 2) - Xlc) * bloco.comprimento).ToString() + "; BLOCO PARCIAL ESPESSURA FLANGE");
                                testeareas2.Append(bloco.Xcg.ToString() + ";" + bloco.espessura.ToString() + ";" + Xlc.ToString() + "\n" );
                            }
                        }
                        else if ((((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 + bloco.comprimento / 2) > 84) & (((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 - bloco.comprimento / 2) < 84))
                        {
                            //entra o comprimento parcial do bloco na conta e também uma espessura parcial, ver como calcular...
                            // considerar como nao grauteado
                            if (bloco.grauteado)
                            {
                                Mrd += (fpkcheio * 0.7) / 2.0 * (((bloco.Xcg + bloco.espessura / 2) - Xlc) * (84 - (Math.Abs(bloco.Ycg - eixoprincipal) - 7 - bloco.comprimento / 2))) * ((Xlc + ((bloco.Xcg + bloco.espessura / 2) - Xlc) / 2) - Xcg);
                            }
                            else
                            {
                                Mrd += (fpk * 0.7) / 2.0 * (((bloco.Xcg + bloco.espessura / 2) - Xlc) * (84 - (Math.Abs(bloco.Ycg - eixoprincipal) - 7 - bloco.comprimento / 2)))  * ((Xlc + ((bloco.Xcg + bloco.espessura / 2) - Xlc) / 2) - Xcg);
                                testeareas.Append((((bloco.Xcg + bloco.espessura / 2) - Xlc) * (84 - (Math.Abs(bloco.Ycg - eixoprincipal) - 7 - bloco.comprimento / 2))).ToString() + "; BLOCO PARCIAL ESPESSURA e COMPRIMENTO FLANGE");

                            }
                        }
                    }
                }
                sb4.Append(areablocos.ToString()).Append(";");
                sb4.Append(areablocosgr.ToString()).Append(";");
                sb4.Append("\n");
                testeareas.Append("\n");
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
