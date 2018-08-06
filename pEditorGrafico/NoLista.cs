using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pEditorGrafico
{
    class NoLista<Tipo> where Tipo : IComparable<Tipo>
    {
        public Tipo info;
        public NoLista<Tipo> prox;

        public NoLista(Tipo novaInfo,
                       NoLista<Tipo> proximo)
        {
            info = novaInfo;
            prox = proximo;
        }
    }

}
