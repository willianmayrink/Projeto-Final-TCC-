using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using OxyPlot;
using OxyPlot.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Element = Autodesk.Revit.DB.Element;

namespace ProjetoFinal
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public partial class Class1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                #region Input e tratamento dos dados de entrada

                UIApplication uiapp = commandData.Application;
                Document doc = uiapp.ActiveUIDocument.Document;



                Autodesk.Revit.UI.Selection.Selection sel = uiapp.ActiveUIDocument.Selection;
                List<Reference> selElements = sel.PickObjects(ObjectType.Element).ToList(); // Pegando os elementos e armazenando em uma lista



                var modelo = new Window1(doc, selElements);
                modelo.ShowDialog();



                ////abrir janela para entrar com valores de input


                //List<Blocos> BlocosAlma = new List<Blocos>();
                //List<Blocos> BlocosFlange = new List<Blocos>();
                ////List<Graute> ListaGraute = new List<Graute>();
                //List<Ferros> ListaFerros = new List<Ferros>();

                //foreach (Reference refelem in selElements) //Trabalhando com os dados de input, referenciando cada elemento na sua devida lista
                //{
                //    Element auxelem = doc.GetElement(refelem);
                //    var elemento = auxelem as FamilyInstance;
                //    string tipoelemento = elemento.Symbol.GetParameters("TipoElemento")[0].AsString();
                //    if (tipoelemento == "Bloco")
                //    {
                //        AddBloco(elemento, auxelem);
                //    }
                //    else if (tipoelemento == "Ferro")
                //    {
                //        AddFerro(elemento);
                //    }
                //    /*var location1 = elemento.Location as LocationPoint;
                //    double m1 = location1.Point.X;
                //    teste1 = elemento.Symbol.GetParameters("Comprimento")[0].AsDouble();*/


                //    //TaskDialog.Show("TESTEEEE", m1.ToString() + "/n" + teste1.ToString());
                //};



                //void AddBloco(FamilyInstance familyelem, Element elem)
                //{
                //    var location = familyelem.Location as LocationPoint;
                //    double refcomprimento = familyelem.Symbol.GetParameters("Comprimento")[0].AsDouble() * 30.48;
                //    double refespessura = familyelem.Symbol.GetParameters("Espessura")[0].AsDouble() * 30.48;
                //    double refXcg = location.Point.X * 30.48;
                //    double refYcg = location.Point.Y * 30.48;
                //    double reforientacao = Math.Round(location.Rotation / Math.PI, 6);
                //    double refarea = refcomprimento * refespessura;
                //    string refgrauteado = elem.GetParameters("grauteado")[0].AsValueString();
                //    //string refgrauteado = elem


                //    if (reforientacao == 0.5 || reforientacao == 1.5)
                //    {
                //        BlocosFlange.Add(new Blocos(refcomprimento, refespessura, refXcg, refYcg, reforientacao, refarea, refgrauteado));
                //    }
                //    else if (reforientacao == 0 || reforientacao == 1 || reforientacao == 2)
                //    {
                //        BlocosAlma.Add(new Blocos(refcomprimento, refespessura, refXcg, refYcg, reforientacao, refarea, refgrauteado));
                //    }
                //}

                //void AddFerro(FamilyInstance elem)
                //{
                //    var location = elem.Location as LocationPoint;
                //    double refbitola = elem.Symbol.GetParameters("Bitola")[0].AsDouble() * 30.48;
                //    double refarea = Math.PI * refbitola * refbitola / 4 / 100;  //  PI*D²/4   cm²
                //    double refXcg = location.Point.X * 30.48;
                //    double refYcg = location.Point.Y * 30.48;
                //    ListaFerros.Add(new Ferros(refbitola, refXcg, refYcg, refarea));
                //}

                //#endregion

                ////var plottest = new PlotModel { Title = "TESTE" };
                ////plottest.Axes.Add(new LinearAxis { Title = "Normal (kN)", Position = AxisPosition.Bottom /*, Minimum = -20, Maximum = 80 */});
                ////plottest.Axes.Add(new LinearAxis { Title = "Momento (kN.m)", Position = AxisPosition.Left/*, Minimum = -20, Maximum = 80*/});


                //#region Algoritmo de Cálculo

                //if (BlocosAlma.Count == 0)
                //{
                //    TaskDialog.Show("ERRO", "Selecione os elementos");
                //    return Result.Succeeded;
                //}

                //BlocosAlma = BlocosAlma.OrderBy(x => x.Xcg).ToList();
                //BlocosFlange = BlocosFlange.OrderBy(x => x.Xcg).ToList();

                ////depois de ordenado tudo, começa esse passo do codigo
                //double InicioLN; //encontrando o ponto de inicio e fim chute LN
                //double fimLN;
                //double EixoPrincipal = BlocosAlma[0].Ycg;
                //double he = 220; //receber valor como input
                //double te = 14; // espessura de um bloco qualquer
                //double R = 1 - Math.Pow((he - te) / 40, 3);

                //List<TabelaResistencia> resistencias = new List<TabelaResistencia>(); // colunas fbk / fpk/ fpk*  //tabela resistencia referencia norma VALORES EM MPa
                //resistencias.Add(new TabelaResistencia(3, 2.4, 4.8));

                ////definir fpk e fpkcheio / se usuario decidiu por usar referencia, procurar na tabela a partir do fbk se nao, pegar o valor inserido pelo usuario. dividindo por 10 passando para KN/cm²
                //double fpk = 9.8 / 10;
                //double fpkcheio = 15.7 / 10;

                //#region COMO PEGAR RESISTENCIA
                //double inputusuario = 4;//valor que virá do combobox na UI
                //ResistenciaBloco PEGUEIRESISTENCIA = TabRes.GetResistencia(inputusuario);//instancia do struct com os valores de acordo com o fbk escolhido pelo usuario
                //var fbkEscolhido = PEGUEIRESISTENCIA.Fbk;

                ////foi criado um struct "Resistencia Bloco" para armazenar os valores de resistencia.
                ////A classe TabRes é estática pois as informações nunca mudam.
                ////O método GetResistencia devolve o struct "Resistencia Bloco". Esse método será usado com o vlor que o usuario escolher de input
                //#endregion


                //// Encontrando Inicio e Fim da LN

                //if (BlocosFlange.Count == 0)
                //{
                //    InicioLN = (BlocosAlma[0].Xcg - (BlocosAlma[0].comprimento / 2));
                //    fimLN = (BlocosAlma.Last().Xcg + (BlocosAlma.Last().comprimento / 2));

                //}
                //else
                //{
                //    double faceblocoalmainicio = (BlocosAlma[0].Xcg - (BlocosAlma[0].comprimento / 2));
                //    double faceblocoflangeinicio = (BlocosFlange[0].Xcg - (BlocosFlange[0].espessura / 2));
                //    double faceblocoalmafim = BlocosAlma.Last().Xcg + (BlocosAlma.Last().comprimento / 2);
                //    double faceblocoflangefim = BlocosFlange.Last().Xcg + (BlocosFlange.Last().espessura / 2);

                //    if (faceblocoalmainicio >= faceblocoflangeinicio && faceblocoalmainicio - faceblocoflangeinicio < 16)
                //    {
                //        InicioLN = faceblocoflangeinicio;
                //    }
                //    else
                //    {
                //        InicioLN = faceblocoalmainicio;
                //    }

                //    if (faceblocoalmafim <= faceblocoflangefim && faceblocoflangefim - faceblocoalmafim < 16)
                //    {
                //        fimLN = faceblocoflangefim;
                //    }
                //    else
                //    {
                //        fimLN = faceblocoalmafim;
                //    }
                //    //
                //}



                //// Calculo dos pontos de momento e normal percorrendo a seção
                //double normal, momento;
                //double Xcg = CalcularCentroide(BlocosAlma, BlocosFlange, InicioLN);
                //double ChuteLN = InicioLN;  // colocando como start da ln o fim da seção , considerando assim partindo da parte toda tracionada até começo da seção considerando parte toda comprimida

                ///*while (ChuteLN >= InicioLN)
                //{*/
                //normal = Normal(ListaFerros, BlocosAlma, BlocosFlange, fimLN, InicioLN, ChuteLN, EixoPrincipal, fpkcheio, fpk, R);
                //momento = Momento(ListaFerros, BlocosAlma, BlocosFlange, fimLN, InicioLN, ChuteLN, EixoPrincipal, fpkcheio, fpk, R, Xcg);
                ////ChuteLN -= 1;
                ///*}*/


                return Result.Succeeded;
            }
            catch (Exception)
            {
                var a = new OxyPlot.Wpf.LineAnnotation();
                throw;
            }


        }

        
        
 
    
        public double Normal(List<Ferros> ferros, List<Blocos> alma, List<Blocos> flange, double fimLN, double InicioLN, double Xln, double eixoprincipal, double fpkcheio, double fpk, double R)
        {   // MPa para KN/cm2 dividir por 10
            double tracao = 0;
            double compressao = 0;
            double Xlc = Xln + Math.Abs(Xln - fimLN) * 0.2;
            double Es = 21000; // KN/cm²  210000MPa
            double fyk = 50; //KN/cm²     //500MPa
            double deformacaoescoamento = (fyk / 1.15) / Es; // fyd/Es                   
            foreach (Ferros ferro in ferros)
            {
                if (ferro.Xcg < Xln)
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
        public double Momento(List<Ferros> ferros, List<Blocos> alma, List<Blocos> flange, double fimLN, double InicioLN, double Xln, double eixoprincipal, double fpkcheio, double fpk, double R, double Xcg)
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

        public double CalcularCentroide(List<Blocos> alma, List<Blocos> flange, double InicioLN) // entrar com lista de blocosflange e lista de bloco normal e inicio linha neutra
        {
            double Xcg, SomaXi = 0;
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
        /*public double CalcularIx(List<Blocos> alma, List<Blocos> flange, double InicioLN, double eixoprincipal, double Xcg) //entrar com lista de blocos e Xcg //Fazer pros casos da flange ser menor que 6*t
        { // adicionar parametro de entrada Xcg que é o valaor calculado na função calcularcentroide
            double Ix = 0; //Iy =  Iycg + A* Xcg²
            foreach (Blocos bloco in flange)
            {
                if (((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 + bloco.comprimento / 2) < 84) // soma todo comprimento do bloco, esta dentro da flange de 84=6.t 
                {
                    Ix += Math.Pow(bloco.espessura, 3) * bloco.comprimento / 12 + bloco.espessura * bloco.comprimento * Math.Pow((bloco.Xcg - Xcg), 2);
                    //entra contribuição do bloco inteiro

                }
                else if ((((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 + bloco.comprimento / 2) > 84) & (((Math.Abs(bloco.Ycg - eixoprincipal)) - 7 - bloco.comprimento / 2) < 84))
                {
                    //entra o comprimento parcial do bloco na conta //ver porcentagem de graute
                    Ix += Math.Pow(bloco.espessura, 3) * (84 - (Math.Abs(bloco.Ycg - eixoprincipal) - 7 - bloco.comprimento / 2)) / 12 + bloco.espessura * (84 - (Math.Abs(bloco.Ycg - eixoprincipal) - 7 - bloco.comprimento / 2)) * Math.Pow((bloco.Xcg - Xcg), 2);
                }
            }

            foreach (Blocos bloco in flange)
            {
                Ix += Math.Pow(bloco.comprimento, 3) * bloco.espessura / 12 + bloco.espessura * bloco.comprimento * Math.Pow((bloco.Xcg - Xcg), 2);
            }
            return Ix;
        }*/
        #endregion

    }

}


