using System.Drawing;

namespace GrayBMP {
   class PolyFill {
      public void AddLine (int x0, int y0, int x1, int y1) => mVertices.Add (new (x0, y0, x1, y1));
      public void Fill (GrayBMP bmp, int color) {
         var width = bmp.Width - 1;
         for (int i = 0; i < width; i++) {
            var pts = new List<int> ();
            foreach (var vertex in mVertices) {
               if (vertex.ScanIntersection (new Point (1, i), new Point (width, i), out double resX))
                  pts.Add ((int)resX);
            }
            pts = pts.Order ().ToList ();
            for (int j = 0; j < pts.Count; j += 2)
               bmp.DrawHorizontalLine (pts[j], pts[j + 1], i, color);
         }
      }

      /// <summary>Creates a vertex for a given points.</summary>
      class PVertex {
         public PVertex (int x0, int y0, int x1, int y1) { Point1 = new Point (x0, y0); Point2 = new Point (x1, y1); }
         public Point Point1 { get; private set; }
         public Point Point2 { get; private set; }

         public bool ScanIntersection (Point pt1, Point pt2, out double intptX) {
            var startPt = Point1; var endPt = Point2;
            intptX = 0;
            double factor = .5;
            double a = endPt.Y - startPt.Y;
            double b = startPt.X - endPt.X;
            if (a == 0 && b == 0) return false;
            double c = a * startPt.X + b * startPt.Y;

            double a1 = pt2.Y + factor - (pt1.Y + factor);
            double b1 = pt1.X - pt2.X;
            if (a1 == 0 && b1 == 0) return false;
            double c1 = a1 * pt1.X + b1 * (pt1.Y + factor);

            var delta = a * b1 - a1 * b;
            if (delta == 0)
               return false;
            else {
               double x = (b1 * c - b * c1) / delta;
               double y = (a * c1 - a1 * c) / delta;
               double dx = endPt.X - startPt.X, dy = endPt.Y - startPt.Y;
               intptX = x;
               var val = Math.Abs (dx) > Math.Abs (dy) ? ((x - startPt.X) / dx) : (y - startPt.Y) / dy;
               return val >= 0 && val <= 1;
            }
         }
      }
      List<PVertex> mVertices = new ();
   }
}
