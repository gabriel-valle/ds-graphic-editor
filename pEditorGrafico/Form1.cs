using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace pEditorGrafico
{
    public partial class frmGrafico : Form
    {
        Desenho drawArea;
        string selTool;
        ListaSimples<Ponto> pontos;
        Bitmap salvo;
        ListaSimples<Figura> selecionados;
        string nomeArqAberto = "";

        public frmGrafico()
        {
            InitializeComponent();
            salvo = new Bitmap(pbAreaDesenho.Size.Width, pbAreaDesenho.Size.Height);
            drawArea = new Desenho(pbAreaDesenho.CreateGraphics());
            selTool = "";
            pontos = new ListaSimples<Ponto>();
            selecionados = new ListaSimples<Figura>();
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            if (dlgCor.ShowDialog() == DialogResult.OK)
                drawArea.CorAtual = dlgCor.Color;
        }

        private void pbAreaDesenho_MouseMove(object sender, MouseEventArgs e)
        {
            statusPosicao.Text = "X: " + e.X + ", Y: " + e.Y;

            var posCursor = new Ponto(e.X, e.Y);

            var temp = (Bitmap)salvo.Clone();   //Cria um bitmap temporário para exibir formas ainda não definitivas

            if (!pontos.estaVazia() && selTool!="")
            {
                switch (selTool)
                {
                    case "circulo":
                        var c = drawArea.Objetos.ultimo.info as Circulo;
                        c.SetRaio(Convert.ToInt32(new Ponto(c.Posicao.X, c.Posicao.Y).GetDistancia(posCursor)));
                        break;
                    case "retangulo":
                        //var r = (drawArea.Objetos.ultimo.info as Retangulo);
                        var pInicial = new Ponto(Math.Min(posCursor.Posicao.X, pontos.primeiro.info.Posicao.X), Math.Min(posCursor.Posicao.Y, pontos.primeiro.info.Posicao.Y));
                        var pFinal = new Ponto(Math.Max(posCursor.Posicao.X, pontos.primeiro.info.Posicao.X), Math.Max(posCursor.Posicao.Y, pontos.primeiro.info.Posicao.Y));
                        (drawArea.Objetos.ultimo.info as Retangulo).Posicao = pInicial.ToPoint();
                        (drawArea.Objetos.ultimo.info as Retangulo).Altura = pFinal.Y - pInicial.Y;
                        (drawArea.Objetos.ultimo.info as Retangulo).Largura = pFinal.X - pInicial.X;
                        statusDesenho.Text = "Retangulo(";
                        foreach (Ponto p in (drawArea.Objetos.ultimo.info as Retangulo).Vertices)
                            statusDesenho.Text += "(" + p.X + "," + p.Y + ")";
                        statusDesenho.Text += "), Largura: " + (drawArea.Objetos.ultimo.info as Retangulo).Largura + ", Altura: " + (drawArea.Objetos.ultimo.info as Retangulo).Altura;
                        break;
                    case "linha":
                        (drawArea.Objetos.ultimo.info as Linha).PontoFim = posCursor;
                        break;
                    case "polilinha":
                        (drawArea.Objetos.ultimo.info as Polilinha).Vertices.ultimo.info = posCursor;
                        break;
                    case "desenho livre":
                        (drawArea.Objetos.ultimo.info as Polilinha).Vertices.insereAposFim(new NoLista<Ponto>(posCursor, null));
                        break;
                    case "elipse":
                        pInicial = new Ponto(Math.Min(posCursor.Posicao.X, pontos.primeiro.info.Posicao.X), Math.Min(posCursor.Posicao.Y, pontos.primeiro.info.Posicao.Y));
                        pFinal = new Ponto(Math.Max(posCursor.Posicao.X, pontos.primeiro.info.Posicao.X), Math.Max(posCursor.Posicao.Y, pontos.primeiro.info.Posicao.Y));
                        (drawArea.Objetos.ultimo.info as Elipse).Posicao = pInicial.ToPoint();
                        (drawArea.Objetos.ultimo.info as Elipse).Altura = pFinal.Y - pInicial.Y;
                        (drawArea.Objetos.ultimo.info as Elipse).Largura = pFinal.X - pInicial.X;
                        break;
                    case "selecao":
                        selecionados = new ListaSimples<Figura>();
                        pInicial = new Ponto(Math.Min(posCursor.Posicao.X, pontos.primeiro.info.Posicao.X), Math.Min(posCursor.Posicao.Y, pontos.primeiro.info.Posicao.Y));
                        pFinal = new Ponto(Math.Max(posCursor.Posicao.X, pontos.primeiro.info.Posicao.X), Math.Max(posCursor.Posicao.Y, pontos.primeiro.info.Posicao.Y));
                        var rect = new Retangulo(pInicial, Math.Abs(pFinal.X - pInicial.X), Math.Abs(pFinal.Y - pInicial.Y));
                        rect.CorContorno = Color.Black;
                        rect.Preencher = true;
                        rect.Espessura = 3;
                        rect.CorPreenchimento = Color.FromArgb(60, 0, 0, 255);
                        rect.desenhar(Graphics.FromImage(temp));

                        if (pontos.QtosNos < 2)
                            pontos.insereAposFim(new NoLista<Ponto>(posCursor, null));
                        pontos.primeiro.prox.info = posCursor;
                        //pontos.insereAposFim(new NoLista<Ponto>(pInicial, null));
                        //pontos.insereAposFim(new NoLista<Ponto>(pFinal, null));
                        break;
                    case "deslocar":
                        //var deltaX = posCursor.X - pontos.primeiro.info.X;
                        //var deltaY = posCursor.Y - pontos.primeiro.info.Y;
                        //var atual = selecionados.primeiro;
                        break;
                }
            }

            if((selTool == "selecao"||selTool == "deslocar" || selTool=="ajustar" || selTool=="rotacionar") && !selecionados.estaVazia())
            {
                var forma = selecionados.primeiro;
                Retangulo rec;
                while (forma != null)
                {
                    switch (forma.info.GetType().ToString())    //Destaca 'pontos notáveis' nas figuras selecionadas
                    {
                        case "pEditorGrafico.Retangulo":
                            foreach (Ponto p in (forma.info as Retangulo).Vertices)
                            {
                                rec = new Retangulo(new Ponto(p.X - 2, p.Y - 2), 4, 4);
                                rec.Espessura = 2;
                                rec.Preencher = true;
                                rec.CorPreenchimento = Color.LightGray;
                                rec.desenhar(Graphics.FromImage(temp));
                            }
                            break;
                            case "pEditorGrafico.Circulo":
                                var pt = new Ponto(forma.info.Posicao.X-2, forma.info.Posicao.Y-2) + new Ponto((forma.info as Circulo).Raio, 0);
                                rec = new Retangulo(pt, 4, 4);
                                rec.Espessura = 2;
                                rec.Preencher = true;
                                rec.CorPreenchimento = Color.LightGray;
                                rec.desenhar(Graphics.FromImage(temp));
                            break;
                            case "pEditorGrafico.Linha":
                                foreach (Ponto p in (forma.info as Linha).Vertices)
                                {
                                    rec = new Retangulo(new Ponto(p.X - 2, p.Y - 2), 4, 4);
                                    rec.Espessura = 2;
                                    rec.Preencher = true;
                                    rec.CorPreenchimento = Color.LightGray;
                                    rec.desenhar(Graphics.FromImage(temp));
                                }
                                break;
                            case "pEditorGrafico.Ponto":
                                pt = (forma.info as Ponto);
                                rec = new Retangulo(new Ponto(pt.X - 2, pt.Y - 2), 4, 4);
                                rec.Espessura = 2;
                                rec.Preencher = true;
                                rec.CorPreenchimento = Color.LightGray;
                                rec.desenhar(Graphics.FromImage(temp));
                                break;
                            case "pEditorGrafico.Polilinha":
                                var poli = (forma.info as Polilinha);
                                var vertice = poli.Vertices.primeiro;
                                while (vertice != null)
                                {
                                    rec = new Retangulo(new Ponto(vertice.info.X - 2, vertice.info.Y - 2), 4, 4);
                                    rec.Espessura = 2;
                                    rec.Preencher = true;
                                    rec.CorPreenchimento = Color.LightGray;
                                    rec.desenhar(Graphics.FromImage(temp));
                                    vertice = vertice.prox;
                                }
                                break;
                            case "pEditorGrafico.Elipse":
                                Ponto[] ladosEl;
                                ladosEl = new Ponto[4];
                                var el = (forma.info as Elipse);
                                ladosEl[0] = new Ponto(el.Posicao.X + el.Largura / 2, el.Posicao.Y);
                                ladosEl[1] = new Ponto(el.Posicao.X + el.Largura, el.Posicao.Y + el.Altura / 2);
                                ladosEl[2] = new Ponto(el.Posicao.X + el.Largura / 2, el.Posicao.Y + el.Altura);
                                ladosEl[3] = new Ponto(el.Posicao.X, el.Posicao.Y + el.Altura / 2);
                                foreach (Ponto p in ladosEl)
                                {
                                    rec = new Retangulo(new Ponto(p.X - 2, p.Y - 2), 4, 4);
                                    rec.Espessura = 2;
                                    rec.Preencher = true;
                                    rec.CorPreenchimento = Color.LightGray;
                                    rec.desenhar(Graphics.FromImage(temp));
                                }
                                break;
                    }
                    forma = forma.prox;
                }
            }
            NoLista<Figura> atual;
             /*if(!selecionados.estaVazia() && selTool=="deslocar")
             {
                 bool achou = false;
                 //pbAreaDesenho.Cursor = Cursors.Default;
                 atual = selecionados.primeiro;
                 while(atual != null)
                 {
                     if(atual.info.Contem(posCursor))
                     {
                         achou = true;
                         break;
                     }
                     atual = atual.prox;
                 }
                 if (achou && pbAreaDesenho.Cursor!=Cursors.Hand)
                     pbAreaDesenho.Cursor = Cursors.Hand;
                 else 
                     if(pbAreaDesenho.Cursor != Cursors.Default)
                         pbAreaDesenho.Cursor = Cursors.Default;
             }*/

            if (!drawArea.Objetos.estaVazia())
                drawArea.Objetos.ultimo.info.desenhar(Graphics.FromImage(temp));

            atual = drawArea.Objetos.primeiro;
            while (atual != null)
            {
                switch (atual.info.GetType().ToString())
                {
                    case "pEditorGrafico.Retangulo":
                        foreach (Ponto p in (atual.info as Retangulo).Vertices)
                            if (p.GetDistancia(posCursor) <= 4)
                            {
                                new Retangulo(new Ponto(p.X - 2, p.Y - 2), 4, 4).desenhar(Graphics.FromImage(temp));
                                break;
                            }
                        break;
                    case "pEditorGrafico.Circulo":
                        var l = new Linha((atual.info as Circulo).GetCentro(), posCursor, Color.Black);
                        //statusDesenho.Text = "Inclinação: " + l.GetInclinacao();
                        var pCirculo = (atual.info as Circulo).GetPonto(l.GetInclinacao());
                        if (pCirculo.GetDistancia(posCursor) <= 6)
                        {
                            new Retangulo(new Ponto(pCirculo.X - 2, pCirculo.Y - 2), 4, 4).desenhar(Graphics.FromImage(temp));
                            break;
                        }
                        break;
                    case "pEditorGrafico.Linha":
                        foreach (Ponto p in (atual.info as Linha).Vertices)
                            if (p.GetDistancia(posCursor) <= 4)
                            {
                                new Retangulo(new Ponto(p.X - 2, p.Y - 2), 4, 4).desenhar(Graphics.FromImage(temp));
                                break;
                            }
                        break;
                    case "pEditorGrafico.Ponto":
                        var pt = (atual.info as Ponto);
                        if ((atual.info as Ponto).GetDistancia(posCursor) <= 4)
                        {
                            new Retangulo(new Ponto(pt.X - 2, pt.Y - 2), 4, 4).desenhar(Graphics.FromImage(temp));
                            break;
                        }
                        break;
                    case "pEditorGrafico.Polilinha":
                        var poli = (atual.info as Polilinha);
                        var vertice = poli.Vertices.primeiro;
                        while (vertice != null)
                        {
                            if (vertice.info.GetDistancia(posCursor) <= 4)
                            {
                                new Retangulo(new Ponto(vertice.info.X - 2, vertice.info.Y - 2), 4, 4).desenhar(Graphics.FromImage(temp));
                                break;
                            }
                            vertice = vertice.prox;
                        }
                        break;
                    case "pEditorGrafico.Elipse":
                        Ponto[] ladosEl;
                        ladosEl = new Ponto[4];
                        var el = (atual.info as Elipse);
                        ladosEl[0] = new Ponto(el.Posicao.X + el.Largura / 2, el.Posicao.Y);
                        ladosEl[1] = new Ponto(el.Posicao.X + el.Largura, el.Posicao.Y + el.Altura/2);
                        ladosEl[2] = new Ponto(el.Posicao.X + el.Largura/2, el.Posicao.Y + el.Altura);
                        ladosEl[3] = new Ponto(el.Posicao.X, el.Posicao.Y + el.Altura/2);
                        foreach (Ponto p in ladosEl)
                            if (p.GetDistancia(posCursor) <= 4)
                            {
                                new Retangulo(new Ponto(p.X - 2, p.Y - 2), 4, 4).desenhar(Graphics.FromImage(temp));
                                break;
                            }
                        break;
                }
                atual = atual.prox;
            }
            pbAreaDesenho.Image = temp;
        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void toolStripButton5_ButtonClick(object sender, EventArgs e)
        {
            selTool = "circulo";
            pontos = new ListaSimples<Ponto>();
        }

        private void toolStripButton8_ButtonClick(object sender, EventArgs e)
        {
            selTool = "elipse";
            pontos = new ListaSimples<Ponto>();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            selTool = "linha";
            pontos = new ListaSimples<Ponto>();
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            selTool = "ponto";
            pontos = new ListaSimples<Ponto>();
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            selTool = "retangulo";
            pontos = new ListaSimples<Ponto>();
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            selTool = "polilinha";
            pontos = new ListaSimples<Ponto>();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            selTool = "poligono";
            pontos = new ListaSimples<Ponto>();
        }

        private void pbAreaDesenho_MouseDown(object sender, MouseEventArgs e)
        {
            var posCursor = new Ponto(e.X, e.Y);

            switch (selTool)
            {
                case "circulo":
                    if (pontos.estaVazia())
                    {
                        drawArea.AdicionarObjeto(new Circulo(posCursor, posCursor, drawArea.CorAtual));
                        pontos.insereAposFim(new NoLista<Ponto>(posCursor, null));
                    }
                    break;
                case "retangulo":
                    if (pontos.estaVazia())
                    {
                        var rect = new Retangulo(posCursor, 0, 0);
                        rect.CorContorno = drawArea.CorAtual;
                        drawArea.AdicionarObjeto(rect);
                        pontos.insereAposFim(new NoLista<Ponto>(posCursor, null));
                    }
                    break;
                case "linha":
                    if (pontos.estaVazia())
                    {
                        drawArea.AdicionarObjeto(new Linha(posCursor, posCursor, drawArea.CorAtual));
                        pontos.insereAposFim(new NoLista<Ponto>(posCursor, null));
                    }
                    break;
                case "polilinha":
                    if(pontos.estaVazia())
                    {
                        var pol = new Polilinha(posCursor, posCursor);
                        pol.CorContorno = drawArea.CorAtual;
                        drawArea.AdicionarObjeto(pol);
                        pontos.insereAposFim(new NoLista<Ponto>(posCursor, null));
                    }
                    else
                    {
                        if (e.Button == System.Windows.Forms.MouseButtons.Right)
                        {
                            pontos = new ListaSimples<Ponto>();
                            drawArea.Objetos.ultimo.info.desenhar(Graphics.FromImage(salvo));
                            pbAreaDesenho.Image = salvo;
                        }
                        else
                        {
                            var pol = (Polilinha)drawArea.Objetos.ultimo.info;
                            pol.Vertices.insereAposFim(new NoLista<Ponto>(posCursor, null));
                            pontos.insereAposFim(new NoLista<Ponto>(posCursor, null));
                        }
                    }
                    break;
                case "desenho livre":
                    if(pontos.estaVazia())
                    {
                        var pol = new Polilinha(posCursor, posCursor);
                        pol.CorContorno = drawArea.CorAtual;
                        drawArea.AdicionarObjeto(pol);
                        pontos.insereAposFim(new NoLista<Ponto>(posCursor, null));
                    }
                    break;
                case "ponto":
                    drawArea.AdicionarObjeto(new Ponto(posCursor.X, posCursor.Y, drawArea.CorAtual));
                    drawArea.Objetos.ultimo.info.desenhar(Graphics.FromImage(salvo));
                    pbAreaDesenho.Image = salvo;
                    break;
                case "elipse":
                    if (pontos.estaVazia())
                    {
                        var el = new Elipse(posCursor, 0, 0);
                        el.CorContorno = drawArea.CorAtual;
                        drawArea.AdicionarObjeto(el);
                        pontos.insereAposFim(new NoLista<Ponto>(posCursor, null));
                    }
                break;
                case "selecao":
                    if (pontos.estaVazia())
                        pontos.insereAposFim(new NoLista<Ponto>(posCursor, null));
                    break;
                case "deslocar":
                    if (pontos.estaVazia())
                        pontos.insereAposFim(new NoLista<Ponto>(posCursor, null));
                    break;
            }
        }

        private void pbAreaDesenho_MouseUp(object sender, MouseEventArgs e)
        {
            var posCursor = new Ponto(e.X, e.Y);
            switch (selTool)
            {
                case "circulo":
                    if (!pontos.estaVazia())
                    {
                        pontos = new ListaSimples<Ponto>();
                        drawArea.Objetos.ultimo.info.desenhar(Graphics.FromImage(salvo));
                        //salvo.Save("imagem.bmp");
                        pbAreaDesenho.Image = salvo;
                        //salvo = drawArea.Context.Save();
                    }
                    break;
                case "retangulo":
                    if (!pontos.estaVazia())
                    {
                        pontos = new ListaSimples<Ponto>();
                        drawArea.Objetos.ultimo.info.desenhar(Graphics.FromImage(salvo));
                        pbAreaDesenho.Image = salvo;
                    }
                    break;
                case "linha":
                    if (!pontos.estaVazia())
                    {
                        pontos = new ListaSimples<Ponto>();
                        drawArea.Objetos.ultimo.info.desenhar(Graphics.FromImage(salvo));
                        pbAreaDesenho.Image = salvo;
                    }
                    break;
                case "desenho livre":
                    if (!pontos.estaVazia())
                    {
                        pontos = new ListaSimples<Ponto>();
                        drawArea.Objetos.ultimo.info.desenhar(Graphics.FromImage(salvo));
                        pbAreaDesenho.Image = salvo;
                    }
                    break;
                case "elipse":
                    if (!pontos.estaVazia())
                    {
                        pontos = new ListaSimples<Ponto>();
                        drawArea.Objetos.ultimo.info.desenhar(Graphics.FromImage(salvo));
                        pbAreaDesenho.Image = salvo;
                    }
                    break;
                case "selecao":
                    if (!pontos.estaVazia())
                    {
                        selecionados = new ListaSimples<Figura>();
                        var pInicial = new Ponto(Math.Min(pontos.ultimo.info.Posicao.X, pontos.primeiro.info.Posicao.X), Math.Min(pontos.ultimo.info.Posicao.Y, pontos.primeiro.info.Posicao.Y));
                        var pFinal = new Ponto(Math.Max(pontos.ultimo.info.Posicao.X, pontos.primeiro.info.Posicao.X), Math.Max(pontos.ultimo.info.Posicao.Y, pontos.primeiro.info.Posicao.Y));
                        var r = new Retangulo(pInicial, Math.Abs(pFinal.X - pInicial.X), Math.Abs(pFinal.Y - pInicial.Y));
                        var atual = drawArea.Objetos.primeiro;
                        bool contem;
                        while(atual != null)
                        {
                            contem = true;
                            switch (atual.info.GetType().ToString())
                            {
                                case "pEditorGrafico.Retangulo":
                                    foreach (Ponto p in (atual.info as Retangulo).Vertices)
                                        if (!r.Contem(p))
                                        {
                                            contem = false;
                                            break;
                                        }
                                    if (contem)
                                        selecionados.insereAposFim(new NoLista<Figura>(atual.info, null));
                                   statusDesenho.Text = "Contem:" + contem;
                                   break;
                                   case "pEditorGrafico.Circulo":
                                        var lados = r.GetLados();
                                        foreach(Linha l in lados)
                                        {
                                            var dist = (atual.info as Circulo).GetCentro().GetDistancia(l);
                                            if ((atual.info as Circulo).Raio > dist)
                                                {
                                                    contem = false;
                                                    break;
                                                }
                                        }
                                    contem = contem && r.Contem((atual.info as Circulo).GetCentro());
                                    if (contem)
                                        selecionados.insereAposFim(new NoLista<Figura>(atual.info, null));
                                    statusDesenho.Text = "Contem:" + contem;
                                    break;
                                    case "pEditorGrafico.Linha":
                                        contem = (r.Contem((atual.info as Linha).PontoInicio) && r.Contem((atual.info as Linha).PontoFim));
                                        if (contem)
                                            selecionados.insereAposFim(new NoLista<Figura>(atual.info, null));
                                        statusDesenho.Text = "Contem:" + contem;
                                    break;
                                    case "pEditorGrafico.Ponto":
                                        contem = (r.Contem(atual.info as Ponto));
                                        if (contem)
                                            selecionados.insereAposFim(new NoLista<Figura>(atual.info, null));
                                        statusDesenho.Text = "Contem:" + contem;
                                    break;
                                    case "pEditorGrafico.Polilinha":
                                        var poli = (atual.info as Polilinha);
                                        var vertice = poli.Vertices.primeiro;
                                        while (vertice != null)
                                        {
                                            if (!r.Contem(vertice.info))
                                            {
                                                contem = false;
                                                break;
                                            }
                                            vertice = vertice.prox;
                                        }
                                        if (contem)
                                            selecionados.insereAposFim(new NoLista<Figura>(atual.info, null));
                                        statusDesenho.Text = "Contem:" + contem;
                                    break;
                                    case "pEditorGrafico.Elipse":
                                        Ponto[] ladosEl;
                                        ladosEl = new Ponto[4];
                                        var el = (atual.info as Elipse);
                                        ladosEl[0] = new Ponto(el.Posicao.X + el.Largura / 2, el.Posicao.Y);
                                        ladosEl[1] = new Ponto(el.Posicao.X + el.Largura, el.Posicao.Y + el.Altura / 2);
                                        ladosEl[2] = new Ponto(el.Posicao.X + el.Largura / 2, el.Posicao.Y + el.Altura);
                                        ladosEl[3] = new Ponto(el.Posicao.X, el.Posicao.Y + el.Altura / 2);
                                        foreach (Ponto p in ladosEl)
                                            if (!r.Contem(p))
                                            {
                                                contem = false;
                                                break;
                                            }
                                        if (contem)
                                            selecionados.insereAposFim(new NoLista<Figura>(atual.info, null));
                                        statusDesenho.Text = "Contem:" + contem;
                                    break;
                            }
                            atual = atual.prox;
                        }
                        pontos = new ListaSimples<Ponto>();
                        if (!selecionados.estaVazia())
                            toolStripButton2.Enabled = true;
                        else
                            toolStripButton2.Enabled = false;
                    }
                    break;
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            //drawArea.Limpar(this.BackColor);
            pbAreaDesenho.BackgroundImage = salvo;
            //drawArea.Context.DrawPie(new Pen(new SolidBrush(Color.Black)), new Rectangle(0, 0, 100, 100), 0, 30);
            MessageBox.Show("Contexto carregado");
        }

        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            salvo = new Bitmap(pbAreaDesenho.Size.Width, pbAreaDesenho.Size.Height);
            Graphics g = Graphics.FromImage(salvo);
            drawArea.Context = g;   //O picturebox "empresta" o contexto para a imagem bitmap
            drawArea.Desenhar();
            drawArea.Context = pbAreaDesenho.CreateGraphics();
            salvo.Save("imagem.bmp");
            MessageBox.Show("Contexto atual salvo");
        }

        private void frmGrafico_ResizeEnd(object sender, EventArgs e)
        {
            salvo = new Bitmap(pbAreaDesenho.Width, pbAreaDesenho.Height);
            drawArea.Context = Graphics.FromImage(salvo);   //O picturebox 'empresta' o contexto para o arquivo bitmap
            drawArea.Desenhar();
            drawArea.Context = pbAreaDesenho.CreateGraphics();
            pbAreaDesenho.Image = salvo;
        }

        private void toolStripButton5_Click_1(object sender, EventArgs e)
        {
            selTool = "desenho livre";
            pontos = new ListaSimples<Ponto>();
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem != toolStripButton2)
            {
                selecionados = new ListaSimples<Figura>();
                toolStripButton2.Enabled = false;
            }
            if (!pontos.estaVazia())
            {
                pontos = new ListaSimples<Ponto>();
                drawArea.Objetos.ultimo.info.desenhar(Graphics.FromImage(salvo));
                pbAreaDesenho.Image = salvo;
            }
        }

        private void toolStripButton13_Click_1(object sender, EventArgs e)
        {
            salvo = new Bitmap(pbAreaDesenho.Width, pbAreaDesenho.Height);
            drawArea.Objetos.limparLista();
            drawArea.Limpar(this.BackColor);
            selecionados.limparLista();
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton14_Click(object sender, EventArgs e)
        {
            selTool = "selecao";
            pontos = new ListaSimples<Ponto>();
        }

        private void removerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            while(!selecionados.estaVazia())
            {
                var atual = selecionados.primeiro;
                drawArea.Objetos.existeDado(atual.info); // Chamamos existe dado para posicionar atual e anterior no nó a ser removido
                drawArea.Objetos.removerNo();
                selecionados.primeiro = selecionados.primeiro.prox; //Removemos o nó deletado dos selecionados
                selecionados.QtosNos--;
            }
            Graphics.FromImage(salvo).Clear(this.BackColor);
            drawArea.Context = Graphics.FromImage(salvo);
            drawArea.Desenhar();
            drawArea.Context = pbAreaDesenho.CreateGraphics();
            pbAreaDesenho.Image = salvo;
        }

        private void deslocarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selTool = "deslocar";
            pbAreaDesenho.Cursor = Cursors.NoMove2D;
            pontos = new ListaSimples<Ponto>();
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dlgAbrir.ShowDialog() == DialogResult.OK)     // abertura do arquivo, sua leitura e armazenamento de figuras na lista ligada
            {
                StreamReader arq = new StreamReader(dlgAbrir.FileName);

                drawArea.Objetos = new ListaSimples<Figura>();
                Color c = new Color();
                Ponto pi; // ponto inicial/origem/centro
                string linha = null;
                nomeArqAberto = dlgAbrir.FileName;
                while ((linha = arq.ReadLine()) != null)
                {
                    string fig = linha.Substring(0, 5);
                    fig = fig.Trim();
                    if (fig.Equals("p"))                                                    //assim como na documentação das instruções, foi feito o abrir
                    {                                                                       //verificando, sempre, a primeira letra para identificar a figura
                                                                                            //a ser criada
                        c = Color.FromArgb(Convert.ToInt16(linha.Substring(15, 5)),
                                Convert.ToInt16(linha.Substring(20, 5)), Convert.ToInt16(linha.Substring(25, 5)));

                        pi = new Ponto(Convert.ToInt16(linha.Substring(5, 5)), Convert.ToInt16(linha.Substring(10, 5)), c);
                        drawArea.AdicionarObjeto(pi);
                    }
                    else
                        if (fig.Equals("c"))
                    {

                        c = Color.FromArgb(Convert.ToInt16(linha.Substring(15, 5)),
                            Convert.ToInt16(linha.Substring(20, 5)), Convert.ToInt16(linha.Substring(25, 5)));

                        pi = new Ponto(Convert.ToInt16(linha.Substring(5, 5)), Convert.ToInt16(linha.Substring(10, 5)));
                        Circulo circ = new Circulo(pi, Convert.ToInt16(linha.Substring(30, 5)), c);
                        drawArea.AdicionarObjeto(circ);
                    }
                    else
                            if (fig.Equals("l"))
                    {

                        c = Color.FromArgb(Convert.ToInt16(linha.Substring(15, 5)),
                        Convert.ToInt16(linha.Substring(20, 5)), Convert.ToInt16(linha.Substring(25, 5)));

                        pi = new Ponto(Convert.ToInt16(linha.Substring(5, 5)), Convert.ToInt16(linha.Substring(10, 5)));
                        Ponto Pf = new Ponto(Convert.ToInt16(linha.Substring(30, 5)), Convert.ToInt16(linha.Substring(35, 5)));


                        Linha l = new Linha(pi, Pf, c);
                        drawArea.AdicionarObjeto(l);
                    }
                    else
                                if (fig.Equals("e"))
                    {
                        pi = new Ponto(Convert.ToInt16(linha.Substring(5, 5)), Convert.ToInt16(linha.Substring(10, 5)));

                        c = Color.FromArgb(Convert.ToInt16(linha.Substring(15, 5)),
                            Convert.ToInt16(linha.Substring(20, 5)), Convert.ToInt16(linha.Substring(25, 5)));
                        int raio1 = Convert.ToInt16(linha.Substring(30, 5));
                        int raio2 = Convert.ToInt16(linha.Substring(35, 5));
                        Elipse el = new Elipse(pi, raio1, raio2);
                        el.CorContorno = c;
                        drawArea.AdicionarObjeto(el);
                    }

                }
                arq.Close();
                Graphics.FromImage(salvo).Clear(BackColor);
                drawArea.Context = Graphics.FromImage(salvo);   //O picturebox 'empresta' o contexto para o arquivo bitmap
                drawArea.Desenhar();
                drawArea.Context = pbAreaDesenho.CreateGraphics();
                pbAreaDesenho.Image = salvo;
            }
        }

        private void salvarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!nomeArqAberto.Equals(""))
            {
                drawArea.Salvar(nomeArqAberto);
            }
            else {
                if (dlgAbrir.ShowDialog() == DialogResult.OK)     
                {
                    nomeArqAberto = dlgAbrir.FileName;
                    drawArea.Salvar(dlgAbrir.FileName);
                }
            }
        }

        private void salvarComoToolStripMenuItem_Click(object sender, EventArgs e)
        {
                if (dlgAbrir.ShowDialog() == DialogResult.OK)     
                {
                    drawArea.Salvar(dlgAbrir.FileName);
                }
        }

        private void frmGrafico_FormClosed(object sender, FormClosedEventArgs e)
        {
            DialogResult result = MessageBox.Show("Você deseja salvar???", "Salvar", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
                if (dlgAbrir.ShowDialog() == DialogResult.OK)     // abertura do arquivo, sua leitura e armazenamento de figuras na lista ligada
                {
                    drawArea.Salvar(dlgAbrir.FileName);
                }
        }
    }
}
