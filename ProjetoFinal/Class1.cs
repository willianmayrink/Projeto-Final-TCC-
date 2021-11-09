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
    public class Class1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                

                UIApplication uiapp = commandData.Application;
                Document doc = uiapp.ActiveUIDocument.Document;


                
                Autodesk.Revit.UI.Selection.Selection sel = uiapp.ActiveUIDocument.Selection;
                List<Reference> selElements = sel.PickObjects(ObjectType.Element).ToList();  // Inicia o plugin e pede os valores de entrada, no caso os elementos da subestrutrura.

                var modelo = new Window1(doc, selElements);
                modelo.ShowDialog();

                return Result.Succeeded;
            }
            catch (Exception)
            {
                var a = new OxyPlot.Wpf.LineAnnotation();
                throw;
            }


        }

        #region CALCULAR Ix
        // codigo calcular Ix  ( NUNCA UTILIZADO, SOMENTE SE UM DIA PRECISAR)
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


