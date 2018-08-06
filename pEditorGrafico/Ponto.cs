using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pEditorGrafico
{
    class Ponto : Figura, IComparable<Ponto>
    {
        public Ponto(int posX, int posY, Color corPonto)
        {
            base.posicao.X = posX;
            base.posicao.Y = posY;
            base.corContorno = corPonto;
        }
        public Ponto(int posX, int posY)
        {
            base.posicao.X = posX;
            base.posicao.Y = posY;
            corContorno = Color.Black; // Default color
        }

        public int X    //Acesso simplificado à coordenada X do ponto
        {
            get { return base.posicao.X; }
            set { base.posicao.X = value; }
        }

        public int Y    //Acesso simplificado à coordenada X do ponto
        {
            get { return base.posicao.Y; }
            set { base.posicao.Y = value; }
        }

        public double GetDistancia(Object outro)
        {                                      
            if(outro.GetType() == this.GetType())
            {
                double deltaX = base.posicao.X - ((Ponto)outro).posicao.X;
                double deltaY = base.posicao.Y - ((Ponto)outro).posicao.Y;
                return Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
            }
            if (outro.GetType().ToString() == "pEditorGrafico.Linha")
            {
                var other = outro as Linha;
                if (other.EhPerpendicular())
                    return Math.Abs(this.X - other.PontoInicio.X);
                var numerador = Math.Abs(other.GetCoefAngular() * this.X - this.Y + other.GetTermoIndependente());
                var denominador = Math.Sqrt(Math.Pow(other.GetCoefAngular(), 2) + 1);
                return numerador / denominador;
            }
            throw new NotImplementedException();
        }

        public override void desenhar(Graphics g)
        {
            //g.DrawRectangle(new Pen(Color.Black), posicao.X, posicao.Y, 50, 50);
            Pen pen = new Pen(Color.Black);
            if (contornar)
                g.DrawRectangle(pen, posicao.X, posicao.Y, 1, 1);
        }

        public void SomarCom(Ponto outro)
        {
            this.posicao.X += outro.posicao.X;
            this.posicao.Y += outro.posicao.Y;
        }

        public static Ponto operator + (Ponto p1, Ponto p2)
        {
            var p = new Ponto(p1.posicao.X, p1.posicao.Y);
            p.SomarCom(p2);
            return p;
        }

        public static Ponto operator -(Ponto p1, Ponto p2)
        {
            var p = new Ponto(p1.posicao.X, p1.posicao.Y);
            p.SomarCom(new Ponto(-p2.posicao.X, -p2.posicao.Y));
            return p;
        }

        public int CompareTo(Ponto other)
        {
            if (this.posicao.X.CompareTo(other.posicao.X) != 0)
                return this.posicao.X.CompareTo(other.posicao.X);
            return this.posicao.Y.CompareTo(other.posicao.Y);
        }

        public override bool Contem(Ponto p)
        {
            return CompareTo(p) == 0;
        }

        public Object Clone()
        {
            return new Ponto(X, Y, corContorno);
        }

        public Point ToPoint()
        {
            return new Point(X, Y);
        }
    }
}
