using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoFinal
{
    public class Coordenada
    {
        public double x;
        public double y;
        //bool talvez add um bool pra analsiar se o ponto é uma solução.
        public Coordenada(double refx, double refy)
        {
            x = refx;
            y = refy;
        }

    }
}
