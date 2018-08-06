using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pEditorGrafico
{
    class Circulo : Figura
    {
        private int raio;

        public int Raio
        {
            get { return raio; }
            set {
                if(raio>=0)
                    raio = value;
            }
        }

        public Circulo(Ponto centro, int novoRaio, Color novaCor)
        {
            if (novoRaio < 0)
                throw new Exception("Raio inválido");
            raio = novoRaio;
            base.posicao.X = centro.Posicao.X;
            base.posicao.Y = centro.Posicao.Y;
            base.corContorno = novaCor;
        }

        public Circulo(Ponto centro, Ponto pontoCircunferencia, Color novaCor)
        {
            posicao.X = centro.Posicao.X;
            posicao.Y = centro.Posicao.Y;
            raio = Convert.ToInt32(centro.GetDistancia(pontoCircunferencia));
            CorContorno = novaCor;
        }

        public void SetRaio(int novoRaio)
        {
            if (novoRaio >= 0)
                raio = novoRaio;
        }


        public override void desenhar(Graphics g)
        {
            SolidBrush brush = new SolidBrush(CorPreenchimento);
            Pen pen = new Pen(CorContorno);
            if(Preencher)
                g.FillEllipse(brush, base.posicao.X - raio, base.posicao.Y - raio, 2 * raio, 2 * raio);
            if(Contornar)
                g.DrawEllipse(pen, base.posicao.X - raio, base.posicao.Y - raio, 2 * raio, 2 * raio);
        }

        public Ponto GetCentro()
        {
            return new Ponto(base.posicao.X, base.posicao.Y);
        }

        public Ponto GetPonto(double graus) //Retorna a posição do ponto da circunferencia situado 
        {                                   // x graus do topo
            graus = Math.PI * 2 * graus / 360;
            var deltaX = (int)(Math.Sin(graus) * this.raio);
            var deltaY = (int)(-Math.Cos(graus) * this.raio);
            var delta = new Ponto(deltaX, deltaY);
            delta += GetCentro();
            return delta;
        }

        public string ToString()
        {
            string xStr = Convert.ToString(base.posicao.X);             //toString de Círculo, fiz isso para estar de modo igual ao pedido
            string yStr = Convert.ToString(base.posicao.Y);             //na proposta, onde todas as strings contem 5 de tamanho
            string raioStr = Convert.ToString(raio);
            string corR = Convert.ToString(base.corContorno.R);
            string corG = Convert.ToString(base.corContorno.G);
            string corB = Convert.ToString(base.corContorno.B);

            while (xStr.Length < 5)
                xStr += " ";
            while (yStr.Length < 5)
                yStr += " ";
            while (raioStr.Length < 5)
                raioStr += " ";
            while (corR.Length < 5)
                corR += " ";
            while (corG.Length < 5)
                corG += " ";
            while (corB.Length < 5)
                corB += " ";
            return "c    " + xStr + yStr + corR + corG + corB + raioStr;
        }

        public override bool Contem(Ponto p)
        {
            return GetCentro().GetDistancia(p) < raio;
        }

        /*public override bool Equals(object obj)
        {
            if(obj == null || this.GetType()!=obj.GetType())
                return false;
            if(this == obj)
                return true;
            var c = obj as Circulo;
            if (!this.posicao.Equals(c.posicao))
                return false;
            return true;
        }*/
    }
}
