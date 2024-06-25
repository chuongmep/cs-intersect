namespace deep_intersect
{
	public partial class Form1 : Form
	{
		private List<PointF> polygonA;
		private List<PointF> polygonB;
		private List<PointF> intersection;
		public Form1()
		{
			InitializeComponent();
			this.Text = "Polygon Intersection";
			this.Width = 800;
			this.Height = 600;
			this.BackColor = Color.White;

			// Define the coordinates for polygons A and B
			polygonA = new List<PointF>
			{
				new PointF(4, 1),
				new PointF(6, 1),
				new PointF(6, 6),
				new PointF(4, 6)
			};

			polygonB = new List<PointF>
			{
				new PointF(2, 5),
				new PointF(8, 5),
				new PointF(8, 8),
				new PointF(2, 8)
			};

			// Calculate the intersection of polygons A and B
			intersection = GetIntersection(polygonA, polygonB);

		}
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			var g = e.Graphics;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			// Draw Polygon A
			DrawPolygon(g, polygonA, Brushes.Blue, Pens.Blue);

			// Draw Polygon B
			DrawPolygon(g, polygonB, Brushes.Green, Pens.Green);

			// Draw Intersection Polygon
			if (intersection.Count > 0)
			{
				DrawPolygon(g, intersection, Brushes.Red, Pens.Red);
			}
		}

		private void DrawPolygon(Graphics g, List<PointF> polygon, Brush brush, Pen pen)
		{
			var points = polygon.Select(p => new PointF(p.X * 50, p.Y * 50)).ToArray();
			g.FillPolygon(brush, points);
			g.DrawPolygon(pen, points);
		}

		private List<PointF> GetIntersection(List<PointF> poly1, List<PointF> poly2)
		{
			List<PointF> outputList = poly1;

			foreach (var edge in GetEdges(poly2))
			{
				List<PointF> inputList = outputList;
				outputList = new List<PointF>();

				if (inputList.Count == 0)
					break;

				PointF S = inputList[inputList.Count - 1];

				foreach (PointF E in inputList)
				{
					if (IsInside(edge, E))
					{
						if (!IsInside(edge, S))
						{
							outputList.Add(ComputeIntersection(S, E, edge));
						}
						outputList.Add(E);
					}
					else if (IsInside(edge, S))
					{
						outputList.Add(ComputeIntersection(S, E, edge));
					}
					S = E;
				}
			}

			return outputList;
		}

		private List<(PointF, PointF)> GetEdges(List<PointF> polygon)
		{
			var edges = new List<(PointF, PointF)>();
			for (int i = 0; i < polygon.Count; i++)
			{
				PointF start = polygon[i];
				PointF end = polygon[(i + 1) % polygon.Count];
				edges.Add((start, end));
			}
			return edges;
		}

		private bool IsInside((PointF, PointF) edge, PointF p)
		{
			PointF a = edge.Item1;
			PointF b = edge.Item2;
			return (b.X - a.X) * (p.Y - a.Y) > (b.Y - a.Y) * (p.X - a.X);
		}

		private PointF ComputeIntersection(PointF S, PointF E, (PointF, PointF) edge)
		{
			PointF a = edge.Item1;
			PointF b = edge.Item2;
			float A1 = E.Y - S.Y;
			float B1 = S.X - E.X;
			float C1 = A1 * S.X + B1 * S.Y;

			float A2 = b.Y - a.Y;
			float B2 = a.X - b.X;
			float C2 = A2 * a.X + B2 * a.Y;

			float delta = A1 * B2 - A2 * B1;
			if (delta == 0)
				throw new InvalidOperationException("Lines are parallel");

			return new PointF(
				(B2 * C1 - B1 * C2) / delta,
				(A1 * C2 - A2 * C1) / delta
			);
		}
	}
}