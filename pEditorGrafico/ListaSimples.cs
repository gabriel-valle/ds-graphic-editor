using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pEditorGrafico
{
    class ListaSimples<Tipo>  where Tipo : IComparable<Tipo>
    {
        public NoLista<Tipo> primeiro, ultimo, anterior, atual;
        int quantosNos;

        public int QtosNos
        {
            get
            {
                return quantosNos;
            }
            set
            {
                quantosNos = value;
            }
        }

        private
            bool primeiroAcessoDoPercurso;
        public ListaSimples()
        {
            primeiro = ultimo = anterior = atual = null;
            quantosNos = 0;
            primeiroAcessoDoPercurso = false;
        }

        public bool estaVazia()
        {
          return (primeiro == null);
        }

        public bool existeEmOrdem(Tipo procurado)
        {
            bool achouMaior = false,
                 achouIgual = false;
            anterior = null;
            atual = primeiro;

            while (!(achouIgual || achouMaior) && (atual != null))
            {
                if (atual.info.CompareTo(procurado) == 0)
                    achouIgual = true;
                else
                    if (atual.info.CompareTo(procurado) > 0)
                        achouMaior = true;
                    else
                    {
                        anterior = atual;
                        atual = atual.prox;
                    }
            }
            return achouIgual;
        }

        public void iniciarPercursoSequencial()
        {
            if (!estaVazia())
            {
                anterior = null;
                atual                    = primeiro;
                primeiroAcessoDoPercurso = true;
            }
        }

        public void insereAntesDoInicio(NoLista<Tipo> novoNo)
        {
            if (estaVazia())
                ultimo = novoNo;

            novoNo.prox = primeiro;
            primeiro    = novoNo;
            quantosNos++;
        }

        public void insereAposFim(NoLista<Tipo> novoNo)
        { 
          if (estaVazia() )
             primeiro = novoNo;
          else
            ultimo.prox = novoNo;

          novoNo.prox = null;
          ultimo      = novoNo;
          ++quantosNos;
        }

        public void insereAposNo(NoLista<Tipo> qualNo, NoLista<Tipo> novoNo) 
        {
          if ( (qualNo == null) || estaVazia() )
             throw new Exception("Local inválido para inserção");

          novoNo.prox = qualNo.prox;
          qualNo.prox = novoNo;
          ++quantosNos;

          if (qualNo == ultimo )
             ultimo = novoNo;
        }

        public void insereEmOrdem(NoLista<Tipo> novoNo)
        {
          if (estaVazia() )
             insereAntesDoInicio(novoNo);
          else
            if (anterior == ultimo)
               insereAposFim(novoNo);
            else
              if (anterior == null )
                 insereAntesDoInicio(novoNo);
              else
              {
                novoNo.prox   = atual;
                anterior.prox = novoNo;
                ++quantosNos;
              }
        }

        public void limparLista()
        {
            while(!this.estaVazia())
            {
                atual = primeiro;
                removerNo();
            }
        }

        public bool existeDado(Tipo outroProcurado) //Busca dado em listas desordenadas
        {
            anterior = null;
            atual = primeiro;

            if (estaVazia())
                return false;
            while (atual != null)
            {
                if (atual.info.Equals(outroProcurado))
                    return true;
                anterior = atual;
                atual = atual.prox;
            }
            return false;
        }
            

        public void listar(ListBox listbox)
        {
          listbox.Items.Clear();
          atual = primeiro;
          while (atual != null)
          {
            listbox.Items.Add(atual.info.ToString());
            atual = atual.prox;
          }
        }

        public bool podePercorrer()
        {
          if (atual == null)
             return false;
          else
            if (primeiroAcessoDoPercurso)
            {
              primeiroAcessoDoPercurso = false;
              return true;
            }
            else
              {
                  anterior = atual;
                atual = atual.prox;
                return (atual != null);
              }
        }

        public void removerNo(NoLista<Tipo> qualNo, NoLista<Tipo> noAnterior)
        {
          if (! estaVazia() )
          {
            if (qualNo == primeiro )
            {
              primeiro = primeiro.prox;
              if (primeiro == null)  // só havia um único nó
                 ultimo = null;
            }
            else
              if (qualNo == ultimo)
              {
                noAnterior.prox = null;
                ultimo          = noAnterior;
              }
              else
              {
                noAnterior.prox = qualNo.prox;
                qualNo.prox     = null;
              }

            qualNo = null;
            --quantosNos;
          }
        }

        public void removerNo()
        {
          removerNo(atual, anterior);
        }


        // exercícios de 1 a 4:

        // exercício 1:
        public int contarNos()
        {
            int contador = 0;
            iniciarPercursoSequencial();
            while (podePercorrer())
                contador++;
            
            return contador;
        }

        // exercício 2 : na aplicação

        // exercício 3 : casamento de listas

        public ListaSimples<Tipo> casamento(ListaSimples<Tipo> outra)
        {
            ListaSimples<Tipo> nova = new ListaSimples<Tipo>();

            iniciarPercursoSequencial();  // lista this
            outra.iniciarPercursoSequencial();

            while (!estaVazia() && !outra.estaVazia())
            {
                switch (outra.atual.info.CompareTo(this.atual.info))
                {
                    case -1:
                        outra.primeiro = outra.primeiro.prox;
                        outra.atual.prox = null;
                        nova.insereAposFim(outra.atual);
                        outra.atual = outra.primeiro;
                        outra.QtosNos--;
                        break;

                    case 0:
                        outra.primeiro = outra.primeiro.prox;
                        outra.atual.prox = null;
                        nova.insereAposFim(outra.atual);
                        outra.atual = outra.primeiro;
                        outra.QtosNos--;

                        this.primeiro = this.primeiro.prox;
                        this.atual.prox = null;
                        this.atual = this.primeiro;
                        this.QtosNos--;
                        break;

                    case 1:
                        this.primeiro = this.primeiro.prox;
                        this.atual.prox = null;
                        nova.insereAposFim(this.atual);
                        this.atual = this.primeiro;
                        this.QtosNos--;
                        break;
                }
            }

            if (this.estaVazia())
            {
                nova.ultimo.prox = outra.primeiro;
                nova.ultimo = outra.ultimo;
                nova.QtosNos = nova.QtosNos + outra.QtosNos;
            }
            else
            if (outra.estaVazia())
            {
                nova.ultimo.prox = this.primeiro;
                nova.ultimo = this.ultimo;
                nova.QtosNos += this.QtosNos;
            }

            return nova;
        }

        // exercício 4 : inversão de lista
        public void inverter()
        {
            if (!estaVazia())
            {
                NoLista<Tipo>  um = null, 
                               dois = primeiro,
                               tres = primeiro.prox;
                while (um != ultimo)
                {
                    dois.prox = um;
                    um = dois;
                    dois = tres;
                    if (tres.prox != null)
                        tres = tres.prox;
                }

                tres = primeiro;
                primeiro = um;
                ultimo = tres;
                
            }
        }

        private void encontrarMenor(ref NoLista<Tipo> oAntMenor, 
                                    ref NoLista<Tipo> oMenor)
        {
            oMenor    = atual = primeiro;
            oAntMenor = anterior = null;

            while (atual != null)
            {
                if (atual.info.CompareTo(oMenor.info) < 0)
                {
                    oAntMenor = anterior;
                    oMenor = atual;
                }
                anterior = atual;
                atual = atual.prox;
            }
        }

        public void removerMenorNo( NoLista<Tipo> ant, 
                                    NoLista<Tipo> qual)
        {
            if (ant == null)  // ou atual == primeiro
            {
                primeiro = primeiro.prox;
                if (primeiro == null)
                    ultimo = null;  // estaziou a lista
            }
            else
                if (qual == ultimo)  // vamos remover o menor nó
                {
                    ultimo = ant;
                    ant.prox = null;
                }
                else
                {
                    ant.prox = qual.prox;
                    qual.prox = null;
                }
            quantosNos--;
        }

        public void ordenar()
        {
            NoLista<Tipo> menor = null, antMenor = null;
            ListaSimples<Tipo> ordenada = new ListaSimples<Tipo>();
            while (!estaVazia())
            {
                encontrarMenor(ref antMenor, ref menor);
                removerMenorNo(antMenor, menor);
                ordenada.insereAposFim(menor);
            }
            primeiro = ordenada.primeiro;
            ultimo = ordenada.ultimo;
            QtosNos = ordenada.QtosNos;
        }
    }
}
