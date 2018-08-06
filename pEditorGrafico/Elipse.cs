using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pEditorGrafico
{
    class Elipse : Figura
    {
        protected int largura, altura;

        public int Altura
        {
            get { return altura;  }
            set { altura = value;  }
        }

        public int Largura
        {
            get { return largura; }
            set { largura = value; }
        }

        public Ponto GetCentro()
        {
            return new Ponto(posicao.X + largura / 2, posicao.Y + altura / 2);
        }

        public Elipse(Ponto pos, int l, int a)
        {
            posicao.X = pos.X;
            posicao.Y = pos.Y;
            largura = l;
            altura = a;
            corContorno = Color.Black;
        }
        
        public Ponto[] GetFocos()
        {
            var focos = new Ponto[2];
            focos[0] =  new Ponto(posicao.X + largura / 2, posicao.Y + altura / 2);
            focos[1] = focos[0].Clone() as Ponto;
            double d;
            if(largura >= altura)
            {
                d = Math.Sqrt(Math.Pow(largura, 2) / 4 - Math.Pow(altura, 2) / 4);
                focos[0].X -= (int)d;
                focos[1].X += (int)d;
            }
            else
            {
                d = Math.Sqrt(Math.Pow(altura, 2) / 4 - Math.Pow(largura, 2) / 4);
                focos[0].Y -= (int)d;
                focos[1].Y += (int)d;
            }
            return focos;
        }
        
        public override bool Contem(Ponto p)
        {
            double distancia;
            distancia = this.GetFocos()[0].GetDistancia(p);
            distancia += this.GetFocos()[1].GetDistancia(p);
            if (largura > altura)
                return distancia > largura;
            return distancia > altura;
        }

        public override void desenhar(Graphics g)
        {
            //g.DrawRectangle(new Pen(Color.Black), posicao.X, posicao.Y, 50, 50);
            Pen p = new Pen(corContorno);
            g.DrawEllipse(p, posicao.X, posicao.Y, largura, altura);
        }

        public string ToString()
        {
            string xStr = Convert.ToString(base.posicao.X);             //toString de Elípse, fiz isso para estar de modo igual ao pedido
            string yStr = Convert.ToString(base.posicao.Y);             //na proposta, onde pegarei os pontos X e Y e a largura e altura da figura(raio1 e raio2)
            string larguraStr = Convert.ToString(largura);
            string alturaStr = Convert.ToString(altura);
            string corR = Convert.ToString(base.corContorno.R);
            string corG = Convert.ToString(base.corContorno.G);
            string corB = Convert.ToString(base.corContorno.B);

            while (xStr.Length < 5)
                xStr += " ";
            while (yStr.Length < 5)
                yStr += " ";
            while (larguraStr.Length < 5)
                larguraStr += " ";
            while (alturaStr.Length < 5)
                alturaStr += " ";
            while (corR.Length < 5)
                corR += " ";
            while (corG.Length < 5)
                corG += " ";
            while (corB.Length < 5)
                corB += " ";
            return "e    " + xStr + yStr + corR + corG + corB + larguraStr + alturaStr;
        }

        public Ponto GetPonto(double graus)
        {
            //graus = AngleConverter.DegToRad(graus);
            //if(largura>altura)  //Podemos pensar na elipse como um círculo distorcido ou em altura ou em largura
            //{
            //
            //}
            // var deltaX = (int)(Math.Sin(graus) * this.raio);
            //var deltaY = (int)(-Math.Cos(graus) * this.raio);
            //var delta = new Ponto(deltaX, deltaY);
            //delta += GetCentro();
            //return delta;
            throw new NotImplementedException();
        }
    }
}
