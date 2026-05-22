using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DFinalPed
{
    public class MainForm : Form
    {
        private Grafo grafo;
        private TablaHash tablaHash;
        private ComboBox cmbOrigen;
        private ComboBox cmbDestino;
        private RichTextBox txtResultados;
        private MapaCampusPanel mapaPanel;

        public MainForm()
        {
            grafo = Grafo.CrearCampusPorDefecto();
            tablaHash = new TablaHash();

            InicializarVentana();
            InicializarControles();
            CargarEdificios();
            MostrarMensajeInicial();
        }

        private void InicializarVentana()
        {
            Text = "Sistema de Navegación del Campus Universitario";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(900, 620);
            Size = new Size(930, 650);
            BackColor = SystemColors.Control;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        }

        private void InicializarControles()
        {
            Label titulo = new Label();
            titulo.Text = "Sistema de Navegación del Campus Universitario";
            titulo.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            titulo.AutoSize = true;
            titulo.Location = new Point(12, 10);
            Controls.Add(titulo);

            GroupBox grpMapa = new GroupBox();
            grpMapa.Text = "Mapa del campus";
            grpMapa.Location = new Point(12, 42);
            grpMapa.Size = new Size(535, 540);
            grpMapa.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            Controls.Add(grpMapa);

            mapaPanel = new MapaCampusPanel(grafo);
            mapaPanel.Location = new Point(10, 20);
            mapaPanel.Size = new Size(515, 510);
            mapaPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            mapaPanel.BackColor = Color.White;
            grpMapa.Controls.Add(mapaPanel);

            GroupBox grpSeleccion = new GroupBox();
            grpSeleccion.Text = "Selección de edificios";
            grpSeleccion.Location = new Point(560, 42);
            grpSeleccion.Size = new Size(335, 105);
            grpSeleccion.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Controls.Add(grpSeleccion);

            Label lblOrigen = new Label();
            lblOrigen.Text = "Edificio Origen:";
            lblOrigen.AutoSize = true;
            lblOrigen.Location = new Point(14, 27);
            grpSeleccion.Controls.Add(lblOrigen);

            Label lblDestino = new Label();
            lblDestino.Text = "Edificio Destino (DFS):";
            lblDestino.AutoSize = true;
            lblDestino.Location = new Point(170, 27);
            grpSeleccion.Controls.Add(lblDestino);

            cmbOrigen = new ComboBox();
            cmbOrigen.Location = new Point(14, 48);
            cmbOrigen.Size = new Size(145, 23);
            cmbOrigen.DropDownStyle = ComboBoxStyle.DropDownList;
            grpSeleccion.Controls.Add(cmbOrigen);

            cmbDestino = new ComboBox();
            cmbDestino.Location = new Point(170, 48);
            cmbDestino.Size = new Size(145, 23);
            cmbDestino.DropDownStyle = ComboBoxStyle.DropDownList;
            grpSeleccion.Controls.Add(cmbDestino);

            Label nota = new Label();
            nota.Text = "Nota: el destino solo aplica para el recorrido DFS.";
            nota.ForeColor = Color.DimGray;
            nota.AutoSize = true;
            nota.Location = new Point(14, 78);
            grpSeleccion.Controls.Add(nota);

            GroupBox grpOperaciones = new GroupBox();
            grpOperaciones.Text = "Operaciones";
            grpOperaciones.Location = new Point(560, 158);
            grpOperaciones.Size = new Size(335, 145);
            grpOperaciones.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Controls.Add(grpOperaciones);

            Button btnGrafo = CrearBoton("Mostrar Grafo", new Point(14, 25));
            btnGrafo.Click += BtnGrafo_Click;
            grpOperaciones.Controls.Add(btnGrafo);

            Button btnBfs = CrearBoton("Recorrido BFS", new Point(172, 25));
            btnBfs.Click += BtnBfs_Click;
            grpOperaciones.Controls.Add(btnBfs);

            Button btnDfs = CrearBoton("Recorrido DFS", new Point(14, 62));
            btnDfs.Click += BtnDfs_Click;
            grpOperaciones.Controls.Add(btnDfs);

            Button btnHash = CrearBoton("Tabla Hash", new Point(172, 62));
            btnHash.Click += BtnHash_Click;
            grpOperaciones.Controls.Add(btnHash);

            Button btnHeap = CrearBoton("Min-Heap Rutas", new Point(14, 99));
            btnHeap.Click += BtnHeap_Click;
            grpOperaciones.Controls.Add(btnHeap);

            Button btnReiniciar = CrearBoton("Reiniciar Todo", new Point(172, 99));
            btnReiniciar.Click += BtnReiniciar_Click;
            grpOperaciones.Controls.Add(btnReiniciar);

            GroupBox grpResultados = new GroupBox();
            grpResultados.Text = "Resultados";
            grpResultados.Location = new Point(560, 315);
            grpResultados.Size = new Size(335, 267);
            grpResultados.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Controls.Add(grpResultados);

            txtResultados = new RichTextBox();
            txtResultados.Location = new Point(10, 20);
            txtResultados.Size = new Size(315, 235);
            txtResultados.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtResultados.ReadOnly = true;
            txtResultados.BackColor = Color.White;
            txtResultados.ForeColor = Color.Black;
            txtResultados.Font = new Font("Consolas", 10F, FontStyle.Regular, GraphicsUnit.Point);
            grpResultados.Controls.Add(txtResultados);
        }

        private Button CrearBoton(string texto, Point ubicacion)
        {
            Button boton = new Button();
            boton.Text = texto;
            boton.Location = ubicacion;
            boton.Size = new Size(145, 28);
            boton.UseVisualStyleBackColor = true;
            return boton;
        }

        private void CargarEdificios()
        {
            cmbOrigen.Items.Clear();
            cmbDestino.Items.Clear();

            foreach (string clave in grafo.ObtenerClavesOrdenadas())
            {
                EdificioItem item = new EdificioItem(clave, clave + " — " + grafo.NombreCorto(clave));
                cmbOrigen.Items.Add(item);
                cmbDestino.Items.Add(new EdificioItem(item.Clave, item.Texto));
            }

            SeleccionarCombo(cmbOrigen, "A");
            SeleccionarCombo(cmbDestino, "E");
        }

        private void SeleccionarCombo(ComboBox combo, string clave)
        {
            for (int i = 0; i < combo.Items.Count; i++)
            {
                EdificioItem item = (EdificioItem)combo.Items[i];

                if (item.Clave.ToUpper() == clave.ToUpper())
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }

            if (combo.Items.Count > 0)
            {
                combo.SelectedIndex = 0;
            }
        }

        private string ObtenerOrigenSeleccionado()
        {
            return ((EdificioItem)cmbOrigen.SelectedItem).Clave;
        }

        private string ObtenerDestinoSeleccionado()
        {
            return ((EdificioItem)cmbDestino.SelectedItem).Clave;
        }

        private void MostrarMensajeInicial()
        {
            txtResultados.Text = "Listo para ejecutar operaciones.\n\n" +
                                 "Sugerencia de prueba:\n" +
                                 "1. Mostrar Grafo\n" +
                                 "2. Recorrido BFS desde A\n" +
                                 "3. Recorrido DFS desde A hasta E\n" +
                                 "4. Tabla Hash\n" +
                                 "5. Min-Heap Rutas";
            mapaPanel.LimpiarEstado();
        }

        private void BtnGrafo_Click(object sender, EventArgs e)
        {
            string salida = grafo.MostrarGrafo();
            txtResultados.Text = salida;
            mapaPanel.LimpiarEstado();
        }

        private void BtnBfs_Click(object sender, EventArgs e)
        {
            string origen = ObtenerOrigenSeleccionado();
            string salida = grafo.RecorridoBFS(origen);
            tablaHash.Limpiar();

            foreach (string clave in grafo.UltimasVisitas)
            {
                tablaHash.RegistrarVisita(grafo.NombreCorto(clave));
            }

            txtResultados.Text = salida;
            mapaPanel.EstablecerEstado(grafo.UltimasVisitas, new List<string>(), origen, string.Empty);
        }

        private void BtnDfs_Click(object sender, EventArgs e)
        {
            string origen = ObtenerOrigenSeleccionado();
            string destino = ObtenerDestinoSeleccionado();
            string salida = grafo.RecorridoDFS(origen, destino);
            tablaHash.Limpiar();

            foreach (string clave in grafo.UltimasVisitas)
            {
                tablaHash.RegistrarVisita(grafo.NombreCorto(clave));
            }

            txtResultados.Text = salida;
            mapaPanel.EstablecerEstado(grafo.UltimasVisitas, grafo.UltimoCamino, origen, destino);
        }

        private void BtnHash_Click(object sender, EventArgs e)
        {
            txtResultados.Text = tablaHash.MostrarEstadisticas();
        }

        private void BtnHeap_Click(object sender, EventArgs e)
        {
            MinHeap heap = new MinHeap();
            List<Camino> rutas = grafo.ObtenerTodosLosCaminosUnicos();

            foreach (Camino ruta in rutas)
            {
                string descripcion = grafo.NombreCorto(ruta.Origen) + " -> " + grafo.NombreCorto(ruta.Destino);
                heap.Insertar(descripcion, ruta.Distancia);
            }

            StringBuilder salida = new StringBuilder();
            salida.AppendLine("=== RUTAS ORDENADAS POR DISTANCIA ===");
            salida.AppendLine(new string('-', 50));
            salida.Append(heap.MostrarRutasOrdenadas());
            salida.AppendLine(new string('-', 50));
            salida.AppendLine("Total de rutas: " + rutas.Count);
            txtResultados.Text = salida.ToString();
        }

        private void BtnReiniciar_Click(object sender, EventArgs e)
        {
            grafo = Grafo.CrearCampusPorDefecto();
            tablaHash = new TablaHash();
            mapaPanel.ActualizarGrafo(grafo);
            CargarEdificios();
            MostrarMensajeInicial();
        }

        private class EdificioItem
        {
            public string Clave { get; set; }
            public string Texto { get; set; }

            public EdificioItem(string clave, string texto)
            {
                Clave = clave;
                Texto = texto;
            }

            public override string ToString()
            {
                return Texto;
            }
        }
    }

    internal class MapaCampusPanel : Panel
    {
        private Grafo grafo;
        private HashSet<string> visitados;
        private List<string> camino;
        private string origenSeleccionado;
        private string destinoSeleccionado;
        private readonly Dictionary<string, PointF> posiciones;

        public MapaCampusPanel(Grafo grafoInicial)
        {
            grafo = grafoInicial;
            visitados = new HashSet<string>();
            camino = new List<string>();
            origenSeleccionado = string.Empty;
            destinoSeleccionado = string.Empty;

            posiciones = new Dictionary<string, PointF>()
            {
                { "A", new PointF(0.23F, 0.55F) },
                { "B", new PointF(0.43F, 0.55F) },
                { "C", new PointF(0.23F, 0.25F) },
                { "D", new PointF(0.63F, 0.55F) },
                { "E", new PointF(0.43F, 0.80F) },
                { "F", new PointF(0.63F, 0.25F) },
                { "G", new PointF(0.83F, 0.55F) }
            };

            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        public void ActualizarGrafo(Grafo nuevoGrafo)
        {
            grafo = nuevoGrafo;
            LimpiarEstado();
        }

        public void LimpiarEstado()
        {
            visitados.Clear();
            camino.Clear();
            origenSeleccionado = string.Empty;
            destinoSeleccionado = string.Empty;
            Invalidate();
        }

        public void EstablecerEstado(IEnumerable<string> nuevosVisitados, IList<string> nuevoCamino, string origen, string destino)
        {
            if (nuevosVisitados == null)
            {
                visitados = new HashSet<string>();
            }
            else
            {
                visitados = new HashSet<string>(nuevosVisitados);
            }

            if (nuevoCamino == null)
            {
                camino = new List<string>();
            }
            else
            {
                camino = new List<string>(nuevoCamino);
            }

            origenSeleccionado = origen;
            destinoSeleccionado = destino;

            if (origenSeleccionado == null)
            {
                origenSeleccionado = string.Empty;
            }

            if (destinoSeleccionado == null)
            {
                destinoSeleccionado = string.Empty;
            }
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.Clear(Color.WhiteSmoke);

            Rectangle area = new Rectangle(16, 52, Width - 32, Height - 112);
            DibujarTitulo(e.Graphics);
            DibujarCaminos(e.Graphics, area);
            DibujarEdificios(e.Graphics, area);
            DibujarLeyenda(e.Graphics);
        }

        private void DibujarTitulo(Graphics g)
        {
            using (Font fuente = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point))
            using (Brush brocha = new SolidBrush(Color.FromArgb(28, 84, 124)))
            {
                g.DrawString("MAPA DEL CAMPUS UNIVERSITARIO", fuente, brocha, 16, 18);
            }
        }

        private void DibujarCaminos(Graphics g, Rectangle area)
        {
            using (Pen plumaNormal = new Pen(Color.LightGray, 4F))
            using (Pen plumaCamino = new Pen(Color.FromArgb(39, 174, 96), 6F))
            using (Font fuenteDistancia = new Font("Segoe UI", 8F, FontStyle.Bold, GraphicsUnit.Point))
            using (Brush brochaDistancia = new SolidBrush(Color.DimGray))
            {
                foreach (Camino tramo in grafo.ObtenerTodosLosCaminosUnicos())
                {
                    PointF puntoA = PuntoEnArea(tramo.Origen, area);
                    PointF puntoB = PuntoEnArea(tramo.Destino, area);
                    bool esCamino = ContieneTramoDelCamino(tramo.Origen, tramo.Destino);

                    if (esCamino)
                    {
                        g.DrawLine(plumaCamino, puntoA, puntoB);
                    }
                    else
                    {
                        g.DrawLine(plumaNormal, puntoA, puntoB);
                    }

                    float xTexto = (puntoA.X + puntoB.X) / 2F;
                    float yTexto = (puntoA.Y + puntoB.Y) / 2F;
                    string texto = tramo.Distancia + "m";
                    SizeF tamano = g.MeasureString(texto, fuenteDistancia);

                    using (Brush fondo = new SolidBrush(Color.FromArgb(235, 245, 250)))
                    {
                        g.FillRectangle(fondo, xTexto - tamano.Width / 2F - 4F, yTexto - tamano.Height / 2F - 2F, tamano.Width + 8F, tamano.Height + 4F);
                    }

                    g.DrawString(texto, fuenteDistancia, brochaDistancia, xTexto - tamano.Width / 2F, yTexto - tamano.Height / 2F);
                }
            }
        }

        private bool ContieneTramoDelCamino(string a, string b)
        {
            for (int i = 0; i < camino.Count - 1; i++)
            {
                string origen = camino[i];
                string destino = camino[i + 1];

                if ((origen.ToUpper() == a.ToUpper() && destino.ToUpper() == b.ToUpper()) ||
                    (origen.ToUpper() == b.ToUpper() && destino.ToUpper() == a.ToUpper()))
                {
                    return true;
                }
            }

            return false;
        }

        private void DibujarEdificios(Graphics g, Rectangle area)
        {
            foreach (string clave in grafo.ObtenerClavesOrdenadas())
            {
                PointF centro = PuntoEnArea(clave, area);
                Color color = ObtenerColorNodo(clave);
                RectangleF circulo = new RectangleF(centro.X - 24F, centro.Y - 24F, 48F, 48F);

                using (Brush brocha = new SolidBrush(color))
                using (Pen borde = new Pen(Color.White, 3F))
                {
                    g.FillEllipse(brocha, circulo);
                    g.DrawEllipse(borde, circulo);
                }

                using (Font fuenteClave = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point))
                using (Brush brochaClave = new SolidBrush(Color.White))
                {
                    SizeF tamanoClave = g.MeasureString(clave, fuenteClave);
                    g.DrawString(clave, fuenteClave, brochaClave, centro.X - tamanoClave.Width / 2F, centro.Y - tamanoClave.Height / 2F);
                }

                using (Font fuenteNombre = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point))
                using (Brush brochaNombre = new SolidBrush(Color.FromArgb(55, 65, 75)))
                {
                    string nombre = grafo.NombreCorto(clave);
                    SizeF tamanoNombre = g.MeasureString(nombre, fuenteNombre);
                    g.DrawString(nombre, fuenteNombre, brochaNombre, centro.X - tamanoNombre.Width / 2F, centro.Y + 28F);
                }
            }
        }

        private Color ObtenerColorNodo(string clave)
        {
            if (clave.ToUpper() == origenSeleccionado.ToUpper())
            {
                return Color.FromArgb(155, 89, 182);
            }

            if (clave.ToUpper() == destinoSeleccionado.ToUpper())
            {
                return Color.FromArgb(231, 76, 60);
            }

            if (camino.Contains(clave))
            {
                return Color.FromArgb(39, 174, 96);
            }

            if (visitados.Contains(clave))
            {
                return Color.FromArgb(243, 156, 18);
            }

            return Color.FromArgb(52, 152, 219);
        }

        private void DibujarLeyenda(Graphics g)
        {
            int y = Height - 44;
            DibujarItemLeyenda(g, 18, y, Color.FromArgb(52, 152, 219), "Sin visitar");
            DibujarItemLeyenda(g, 118, y, Color.FromArgb(155, 89, 182), "Origen");
            DibujarItemLeyenda(g, 198, y, Color.FromArgb(231, 76, 60), "Destino");
            DibujarItemLeyenda(g, 286, y, Color.FromArgb(243, 156, 18), "Visitado");
            DibujarItemLeyenda(g, 386, y, Color.FromArgb(39, 174, 96), "Camino");
        }

        private void DibujarItemLeyenda(Graphics g, int x, int y, Color color, string texto)
        {
            using (Brush brocha = new SolidBrush(color))
            using (Brush brochaTexto = new SolidBrush(Color.DimGray))
            using (Font fuente = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point))
            {
                g.FillEllipse(brocha, x, y + 4, 10, 10);
                g.DrawString(texto, fuente, brochaTexto, x + 15, y);
            }
        }

        private PointF PuntoEnArea(string clave, Rectangle area)
        {
            PointF relativo = posiciones[clave];
            return new PointF(area.Left + relativo.X * area.Width, area.Top + relativo.Y * area.Height);
        }
    }
}