using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace pEditorGrafico
{
    class Retangulo : Figura
    {
        private Ponto[] vertices;

        public Ponto[] Vertices
        {                           
            get { return vertices; }
            //set { vertices = value; }
        }

        public Linha[] GetLados()
        {
            var lados = new Linha[4];
            for (int counter = 0; counter < 4; counter++)
                lados[counter] = new Linha(this.Vertices[counter], this.Vertices[(counter + 1) % 4]);
            return lados;
        }

        public new Point Posicao    // Sobreescreve a propriedade herdada de figura
        {
            get { return posicao;  }
            set
            {
                var alt = Altura;
                var larg = Largura;
                var p = new Ponto(value.X, value.Y);
                vertices[0] = p;
                vertices[1] = p + new Ponto(larg, 0);
                vertices[2] = p + new Ponto(larg, alt);
                vertices[3] = p + new Ponto(0, alt);
            }
        }

        public int Largura
        {
            get { return Convert.ToInt32(vertices[0].GetDistancia(vertices[1])); }
            set
            {
                vertices[1] = vertices[0] + new Ponto(value, 0);
                vertices[2].X = vertices[1].X;
                vertices[2].Y = vertices[3].Y;
            }
        }

        public int Altura
        {
            get { return Convert.ToInt32(vertices[1].GetDistancia(vertices[2])); }
            set
            {
                vertices[3] = vertices[0] + new Ponto(0, value);
                vertices[2].Y = vertices[3].Y;
                vertices[2].X = vertices[3].X;
            }
        }

        public Retangulo(Ponto pos, int largura, int altura)
        {
            posicao.X = pos.Posicao.X;
            posicao.Y = pos.Posicao.Y;
            vertices = new Ponto[4];
            vertices[0] = pos;
            vertices[1] = pos + new Ponto(largura, 0);
            vertices[2] = pos + new Ponto(largura, altura);
            vertices[3] = pos + new Ponto(0, altura);
            base.corContorno = Color.Black;
        }

        public override void desenhar(Graphics g)
        {
            var verticePoint = new Point[4];
            for (int counter = 0; counter < vertices.Length; counter++)
                verticePoint[counter] = vertices[counter].ToPoint();
            Pen pen = new Pen(CorContorno, espessura);
            SolidBrush brush = new SolidBrush(CorPreenchimento);
            //g.DrawPolygon(pen, verticePoint);
            if (preencher)
                g.FillPolygon(brush, verticePoint);
            if(contornar)
                g.DrawPolygon(pen, verticePoint);
        }

        public void Girar(double graus)
        {
            Girar(graus, this.GetCentro());
        }

        public void Girar(double graus, Ponto centroDeRotacao)
        {
            throw new NotImplementedException();
        }

        public Ponto GetCentro()
        {
            throw new NotImplementedException();
        }

        public override bool Contem(Ponto p) //Verifica se o ponto passado por parâmetro está contido na figura
        {
            var lados = GetLados();
            for (int counter = 0; counter < 4; counter++)
                if (lados[counter].GetSemiplano(p) == -1)
                    return false;
            return true;
        }
    }
}
