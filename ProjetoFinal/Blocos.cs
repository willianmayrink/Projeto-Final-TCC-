using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoFinal
{
    public class Blocos
    {
        public double comprimento;
        public double espessura;
        public double Xcg;
        public double Ycg;
        public double orientacao;
        public double area;
        public bool grauteado;
        public Blocos(double refcomprimento, double refespessura, double refXcg, double refYcg, double reforientacao, double refarea, string refgrauteado)
        {
            comprimento = refcomprimento;
            espessura = refespessura;
            Xcg = refXcg;
            Ycg = refYcg;
            orientacao = reforientacao;
            area = refarea;

            if (refgrauteado == "Yes")
            {
                grauteado = true;

            }
            else if (refgrauteado == "No")
            {
                grauteado = false;
            }
        }
    }
    public class Ferros
    {
        public double bitola;
        public double Xcg;
        public double Ycg;
        public double area;
        public Ferros(double refbitola, double refXcg, double refYcg, double refarea)
        {
            bitola = refbitola;
            Xcg = refXcg;
            Ycg = refYcg;
            area = refarea;
        }
    }

    public class TabelaResistencia // criar um list , e cada objeto é uma linha da tabela de resistencia
    {
        public double fbk;
        public double fpk;
        public double fpkcheio;
        public TabelaResistencia(double a, double b, double c)
        {
            fbk = a;
            fpk = b;
            fpkcheio = c;
        }
    }
}
