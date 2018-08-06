using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pEditorGrafico
{
    class Linha : Figura
    {
        protected Ponto[] vertices;

        public Ponto[] Vertices
        {
            get { return vertices;  }
            set { vertices = value; }
        }

        public Ponto PontoInicio
        {
            get { return vertices[0];  }
            set { vertices[0] = value; }
        }

        public Ponto PontoFim
        {
            get { return vertices[1]; }
            set { vertices[1] = value; }
        }

        public double GetCoefAngular()  //Retorna o coeficiente angular da reta que passa pelos dois pontos
        {
            float deltaY = this.PontoFim.Y - this.PontoInicio.Y;
            float deltaX = this.PontoFim.X - this.PontoInicio.X;
            if (deltaX == 0)
                throw new DivideByZeroException();
            float r = deltaY / deltaX;
            return r;
        }

        public bool EhPerpendicular()
        {
            return this.PontoInicio.X == this.PontoFim.X;
        }

        public double GetInclinacao()   //Retorna o ângulo entre o eixo vertical e a reta em graus
        {
            if (this.EhPerpendicular())
            {
                if (this.PontoInicio.Y > this.PontoFim.Y)
                    return 0;
                else
                    return 180;
            }
            var rtrn = (AngleConverter.RadToDeg(Math.Atan(GetCoefAngular())) + 90) % 360;
            if (this.PontoFim.X < this.PontoInicio.X)
                rtrn += 180;
            return rtrn % 360;
        }

        public double GetTermoIndependente() // Retorna o termo independente da função da reta
        {
            return this.PontoInicio.Y - this.GetCoefAngular() * this.PontoInicio.X;
        }

        public Linha(Ponto pi, Ponto pf, Color novaCor)
        {
            vertices = new Ponto[2];
            posicao = new Point(pi.X, pi.Y);
            vertices[0] = pi;
            vertices[1] = pf;
            corContorno = novaCor;
        }

        public Linha(Ponto pi, Ponto pf)
        {
            vertices = new Ponto[2];
            posicao = new Point(pi.X, pi.Y);
            vertices[0] = pi;
            vertices[1] = pf;
        }

        public Linha(Ponto pi, double inclinacao, double comprimento)
        {
            vertices = new Ponto[2];
            posicao = pi.Posicao;
            vertices[0] = pi;
            var c = new Circulo(pi, (int)comprimento, Color.Black);
            PontoFim = c.GetPonto(inclinacao);
            corContorno = Color.Black;
        }

        public override void desenhar(Graphics g)
        {
            Pen pen = new Pen(corContorno);
            g.DrawLine(pen, base.posicao.X, base.posicao.Y,         // ponto inicial
                         vertices[1].Posicao.X, vertices[1].Posicao.Y);
        }

        public double Length
        {
            get { return vertices[0].GetDistancia(vertices[1]); }
        }
        public override bool Contem(Ponto p)
        {
            if (this.EhPerpendicular())
                return (this.PontoInicio.X == p.X) && (Math.Max(this.PontoInicio.Y, this.PontoFim.Y) > p.Y) && (Math.Min(this.PontoInicio.Y, this.PontoFim.Y) < p.Y);
            else
            {
                var y = this.GetCoefAngular()*p.X+this.GetTermoIndependente();
                return p.Y == y;
            }
        }
        public int GetSemiplano(Ponto ponto)  //Retorna -1 se o ponto estiver à 'esquerda' ou 1 se estiver à direita
        {
            int plano;
            if (this.PontoFim.X - this.PontoInicio.X == 0) //Nesse caso não há coeficiente angular, pois a reta é perpendicular
            {
                plano = 1;
                if (this.PontoFim.X < ponto.X)
                    plano = -1;
                if (this.PontoInicio.Y > this.PontoFim.Y)
                    plano *= -1;
                return plano;
            }
            var y = this.GetCoefAngular() * ponto.X + this.GetTermoIndependente();
            plano = 1;
            if (ponto.Y > y)
                plano = -1;
            if (y == ponto.Y)
                return 0;
            if (this.PontoFim.X > this.PontoInicio.X)
                plano *= -1;
            return plano;
        }
    }
    class AngleConverter    //Classe estática para conversões entre graus e radianos
    {
        public static double RadToDeg(double angulo)
        {
            return angulo * 180 / Math.PI;
        }
        public static double DegToRad(double angulo)
        {
            return Math.PI * angulo / 180;
        }
    }
}
