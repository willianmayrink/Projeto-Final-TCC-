using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoFinal
{
    public static class TabRes
    {
        private static List<ResistenciaBloco> resistenciaBlocos = new List<ResistenciaBloco>()
        {
            new ResistenciaBloco(3.0,2.4, 4.8),
            new ResistenciaBloco(4.0,3.2, 6.4),
            new ResistenciaBloco(6.0,4.5, 7.9),
            new ResistenciaBloco(8.0,6.0, 10.5),
            new ResistenciaBloco(10.0,7.0, 12.3),
            new ResistenciaBloco(12.0,8.4, 13.4),
            new ResistenciaBloco(14.0,9.8, 15.7),
            new ResistenciaBloco(16.0,10.4, 16.6),
            new ResistenciaBloco(18.0,11.7, 18.7),
            new ResistenciaBloco(20.0,12.0, 19.2),
            new ResistenciaBloco(22.0,12.1, 19.4),
            new ResistenciaBloco(24.0,13.2,  21.1),
        };


        public static ResistenciaBloco GetResistencia(double fbk)
        {
            return resistenciaBlocos.First(resistencia => resistencia.Fbk == fbk);
        }

    }
}
