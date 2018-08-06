using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pEditorGrafico
{
    class Polilinha : Figura
    {
        protected ListaSimples<Ponto> vertices;

        public ListaSimples<Ponto> Vertices
        {
            get { return vertices; }
            set { vertices = value; }
        }

        public Polilinha(params Ponto[] pontos)
        {
            vertices = new ListaSimples<Ponto>();
            foreach (Ponto ponto in pontos)
                vertices.insereAposFim(new NoLista<Ponto>(ponto, null));
            base.posicao.X = pontos[0].X;
            base.posicao.Y = pontos[0].Y;
            CorContorno = Color.Black;
        }
        public override void desenhar(System.Drawing.Graphics g)
        {
            //g.DrawRectangle(new Pen(Color.Black), posicao.X, posicao.Y, 50, 50);
            NoLista<Ponto> inicio, fim;
            if (vertices.QtosNos == 0)
                return;
            if (vertices.QtosNos == 1)
            {
                vertices.primeiro.info.desenhar(g);
                return;
            }
            inicio = vertices.primeiro;
            fim = inicio.prox;
            while (fim != null)
            {
                g.DrawLine(new Pen(corContorno), inicio.info.X, inicio.info.Y, fim.info.X, fim.info.Y);
                inicio = fim;
                fim = fim.prox;
            }
        }

        public override bool Contem(Ponto p)
        {
            NoLista<Ponto> inicio, fim;
            if(vertices.QtosNos == 0)
                return false;
            if(vertices.QtosNos == 1)
                return vertices.primeiro.info.Contem(p);
            inicio = vertices.primeiro;
            fim = inicio.prox;
            while(fim != null)
            {
                var l = new Linha(inicio.info, fim.info);
                if (l.Contem(p))
                    return true;
                inicio = fim;
                fim = fim.prox;
            }
            return false;
        }
    }
}
