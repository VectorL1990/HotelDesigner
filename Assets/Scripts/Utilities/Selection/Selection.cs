using RealtimeCSG;

namespace RealtimeCSG
{
    internal sealed class SelectedBrushSurface
    {
        public CSGBrush Brush;
        public int SurfaceIndex;
        public CSGPlane? SurfacePlane;

        public SelectedBrushSurface(CSGBrush brush, int surfaceIndex, CSGPlane surfacePlane)
        {
            Brush = brush;
            SurfaceIndex = surfaceIndex;
            SurfacePlane = surfacePlane;
        }
    }
}
