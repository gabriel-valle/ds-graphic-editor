using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace pEditorGrafico
{
    abstract class Figura: IComparable<Figura>
    {
        protected Color corPreenchimento, corContorno;
        protected Point posicao;
        protected int espessura = 1;
        protected bool contornar = true;
        protected bool preencher;

        public int Espessura
        {
            get { return espessura; }
            set { espessura = value; }
        }

        public bool Preencher
        {
            get { return preencher;  }
            set { preencher = value; }
        }

        public bool Contornar
        {
            get { return contornar; }
            set { contornar = value; }
        }

        public Color CorPreenchimento
        {
            get { return corPreenchimento; }
            set { corPreenchimento = value; }
        }

        public Color CorContorno
        {
            get { return corContorno; }
            set { corContorno = value; }
        }

        public Point Posicao
        {
            get { return posicao; }
            set { posicao = value; }
        }

        public int CompareTo(Figura other)
        {
            if (this.posicao.X.CompareTo(other.posicao.X) != 0)
                return this.posicao.X.CompareTo(other.posicao.X);
            return this.posicao.Y.CompareTo(other.posicao.Y);
        }

        public abstract void desenhar(Graphics g);
        public abstract bool Contem(Ponto p);
    }
}
