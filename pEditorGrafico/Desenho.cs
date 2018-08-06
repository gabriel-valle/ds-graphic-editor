using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace pEditorGrafico
{
    class Desenho
    {
        private Color corAtual;
        private ListaSimples<Figura> objetos;
        private Graphics context;

        public Color CorAtual
        {
            get { return corAtual;  }
            set { corAtual = value;  }
        }
        
        public Graphics Context
        {
            get { return context;  }
            set { context = value;  }
        }

        public Desenho(Graphics gr)
        {
            this.context = gr;
            objetos = new ListaSimples<Figura>();
            corAtual = Color.Black;
        }

        public ListaSimples<Figura> Objetos
        {
            get { return objetos; }
            set { objetos = value; }
        }

        public void AdicionarObjeto(Figura obj)
        {
            obj.CorPreenchimento = corAtual;
            objetos.insereAposFim(new NoLista<Figura>(obj, null));
        }

        public void Limpar(Color bgColor)
        {
            context.Clear(bgColor);
        }

        public void Salvar(string nomeArq)
        {
            StreamWriter arq = new StreamWriter(nomeArq);

            var atual = objetos.primeiro;
            while (atual != null)
            {
                if (atual.info.GetType().Equals(typeof(Circulo)))
                {
                    Circulo circ = atual.info as Circulo;
                    arq.WriteLine(circ.ToString());
                }
                else
                if (atual.info.GetType().Equals(typeof(Retangulo)))
                {
                    Retangulo ret = atual.info as Retangulo;
                    arq.WriteLine(ret.ToString());
                }
                else
                if (atual.info.GetType().Equals(typeof(Linha)))
                {
                    Linha lin = atual.info as Linha;
                    arq.WriteLine(lin.ToString());
                }
                else
                if (atual.info.GetType().Equals(typeof(Elipse)))
                {
                    Elipse el = atual.info as Elipse;
                    arq.WriteLine(el.ToString());
                }
                atual = atual.prox;
            }
            arq.Close();
        }

        public void Desenhar()
        {
            var atual = objetos.primeiro;
            while (atual != null)
            {
                atual.info.desenhar(context);
                atual = atual.prox;
            }
        }
    }
}
