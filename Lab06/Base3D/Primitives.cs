using System;

namespace Lab06.Base3D
{
    public static class Primitives
    {
        public static Polytope Cube(double size)
        {
            var cube = new Polytope();
            var halfSize = size / 2;
            cube.Add(new Point { X = halfSize, Y = halfSize, Z = halfSize });
            cube.Add(new Point { X = -halfSize, Y = halfSize, Z = halfSize });
            cube.Add(new Point { X = -halfSize, Y = -halfSize, Z = halfSize });
            cube.Add(new Point { X = halfSize, Y = -halfSize, Z = halfSize });
            cube.Add(new Point { X = halfSize, Y = halfSize, Z = -halfSize });
            cube.Add(new Point { X = -halfSize, Y = halfSize, Z = -halfSize });
            cube.Add(new Point { X = -halfSize, Y = -halfSize, Z = -halfSize });
            cube.Add(new Point { X = halfSize, Y = -halfSize, Z = -halfSize });

            cube.Add(new Polygon(new[] { 0, 1, 2, 3 }));
            cube.Add(new Polygon(new[] { 4, 0, 3, 7 }));
            cube.Add(new Polygon(new[] { 5, 4, 7, 6 }));
            cube.Add(new Polygon(new[] { 1, 5, 6, 2 }));
            cube.Add(new Polygon(new[] { 0, 4, 5, 1 }));
            cube.Add(new Polygon(new[] { 3, 2, 6, 7 }));

            return cube;
        }
        public static Polytope Tetrahedron(double size)
        {
            Polytope cube = Cube(size);
            Polytope tetra = new Polytope();

            tetra.Add(cube.points[0]);
            tetra.Add(cube.points[2]);
            tetra.Add(cube.points[5]);
            tetra.Add(cube.points[7]);

            tetra.Add(new Polygon(new[] { 0, 2, 1 }));
            tetra.Add(new Polygon(new[] { 0, 1, 3 }));
            tetra.Add(new Polygon(new[] { 0, 3, 2 }));
            tetra.Add(new Polygon(new[] { 1, 2, 3 }));

            return tetra;
        }

        public static Polytope Octahedron(double size)
        {
            Polytope cube = Cube(size);
            Polytope octa = new Polytope();

            foreach (var edge in cube.polygons)
            {
                var sump = new Point();
                foreach (var p in edge.Points(cube))
                    sump += p;
                sump /= 4;
                octa.Add(new Point { X = sump.X, Y = sump.Y, Z = sump.Z });
            }

            octa.Add(new Polygon(new int[] { 0, 5, 1 }));
            octa.Add(new Polygon(new int[] { 2, 1, 5 }));

            octa.Add(new Polygon(new int[] { 0, 3, 5 }));
            octa.Add(new Polygon(new int[] { 2, 5, 3 }));

            octa.Add(new Polygon(new int[] { 0, 4, 3 }));
            octa.Add(new Polygon(new int[] { 2, 3, 4 }));

            octa.Add(new Polygon(new int[] { 0, 1, 4 }));
            octa.Add(new Polygon(new int[] { 2, 4, 1 }));

            return octa;
        }
        private static Point nextCirclePoint(double z, double angle)
        {
            return new Point { X = Math.Sin(Math.PI * angle / 180), Y = Math.Cos(Math.PI * angle / 180), Z = z };
        }
        public static Polytope Icosahedron()
        {
            Polytope icosa = new Polytope();

            for (int angle = 0; angle < 360; angle += 72)
            {
                icosa.Add(nextCirclePoint(0.5, angle));
                icosa.Add(nextCirclePoint(-0.5, angle + 36));
            }

            icosa.Add(new Point { X = 0, Y = 0, Z = Math.Sqrt(5) / 2 });
            icosa.Add(new Point { X = 0, Y = 0, Z = -Math.Sqrt(5) / 2 });
            //конусы снизу и сверху и треугольнки внутри
            for (int i = 0; i < 10; i++)
            {
                if (i % 2 == 0)
                {
                    icosa.Add(new Polygon(new int[] { i, 10, (i + 2) % 10 }));
                    icosa.Add(new Polygon(new int[] { (i + 2) % 10, (i + 1) % 10, i }));
                }
                else
                {
                    icosa.Add(new Polygon(new int[] { (i + 2) % 10, 11, i }));
                    icosa.Add(new Polygon(new int[] { i, (i + 1) % 10, (i + 2) % 10 }));
                }
            }

            return icosa;
        }
    }
}
