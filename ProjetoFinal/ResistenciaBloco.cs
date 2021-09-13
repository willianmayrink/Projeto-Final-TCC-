using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoFinal
{
    public struct ResistenciaBloco
    {
        public double Fbk { get; set; }
        public double Fpk { get; set; }
        public double FpkCheio { get; set; }

        public ResistenciaBloco(double fbk, double fpk, double fpkCheio)
        {
            Fbk = fbk;
            Fpk = fpk;
            FpkCheio = fpkCheio;
        }
    }
}
