using FortniteZoneSimulator.Properties;
using MediaLib;
using System;
using System.Drawing;
using System.IO;

namespace FortniteZoneSimulator
{
    public class ZoneSimulator
    {
        private const double CONTRACT_RATIO = 2d / 3d;

        private Bitmap _fortniteMap;
        private int _size;
        private Random _rand;
        private Bitmap _currentZoneSet;
        private System.Windows.Point _currentCenter;
        private double _currentRadius;
        private double _nextRadius;
        private System.Windows.Point _nextCenter;

        public Bitmap FortniteMap
        {
            get { return _fortniteMap; }
        }

        public Bitmap InitialZoneSet { get; set; }

        public ZoneSimulator()
        {
            _fortniteMap = new Bitmap(Resources.FortniteMap);
            _size = _fortniteMap.Height;
            _rand = new Random();

            InitialZoneSet = GetInitialZone();
            _currentZoneSet = new Bitmap(InitialZoneSet);
        }

        public void Reset()
        {
            InitialZoneSet = GetInitialZone();
            _currentZoneSet = new Bitmap(InitialZoneSet);
        }

        public Bitmap GetNextZoneSet()
        {
            _nextCenter = GetRandCircle(_currentCenter, _currentRadius * (1 - CONTRACT_RATIO));
            _nextRadius = _currentRadius * CONTRACT_RATIO;

            var bitmapWithCirlce = ShapeHelper.AddCircle(new Bitmap(_fortniteMap), Brushes.Blue, _currentCenter, (float)_currentRadius);
            var bitmapCropToCircle = ShapeHelper.CropToCircle(bitmapWithCirlce, Color.MediumOrchid, _currentCenter, _currentRadius);

            _currentZoneSet = ShapeHelper.AddCircle(new Bitmap(_fortniteMap), Brushes.WhiteSmoke, _nextCenter, (float)_nextRadius);

            return ShapeHelper.AddCircle(new Bitmap(bitmapCropToCircle), Brushes.WhiteSmoke, _nextCenter, (float)_nextRadius); ;
        }

        public Bitmap DoContractSteps(int stepCount, int step)
        {
            // Abbildung von current -> next
            var stepRadius = _nextRadius + (stepCount - step) / (double)stepCount * (_currentRadius - _nextRadius);
            var stepCenter = _nextCenter + (stepCount - step) / (double)stepCount * (_currentCenter - _nextCenter);
            var bitmapWithCirlce = ShapeHelper.AddCircle(new Bitmap(_currentZoneSet), Brushes.Blue, stepCenter, (float)stepRadius);

            if(step == stepCount)
            {
                _currentRadius = stepRadius;
                _currentCenter = stepCenter;
            }

            return ShapeHelper.CropToCircle(bitmapWithCirlce, Color.MediumOrchid, stepCenter, stepRadius);
        }

        private Bitmap GetInitialZone()
        {
            _currentRadius = _size / 2 * CONTRACT_RATIO;
            _currentCenter = GetInitialRandomCenter();
            var bitmapWithCirlce = ShapeHelper.AddCircle(new Bitmap(_fortniteMap), Brushes.WhiteSmoke, _currentCenter, (float)_currentRadius);
            return ShapeHelper.CropToCircle(bitmapWithCirlce, Color.MediumOrchid, _currentCenter, _currentRadius);
        }

        private System.Windows.Point GetRandCircle(System.Windows.Point center, double radius)
        {
            double angle = 2 * Math.PI * _rand.NextDouble();
            double randRadius = _rand.NextDouble() * radius;
            double x = center.X + randRadius * Math.Cos(angle);
            double y = center.Y + randRadius * Math.Sin(angle);

            return new System.Windows.Point(x, y);
        }

        private System.Windows.Point GetInitialRandomCenter()
        {
            int minX = (int)(CONTRACT_RATIO * _size / 2d);
            int maxX = (int)(CONTRACT_RATIO * _size);
            var randX = _rand.Next(minX, maxX);

            int minY = (int)(CONTRACT_RATIO * _size / 2d);
            int maxY = (int)(CONTRACT_RATIO * _size);
            var randY = _rand.Next(minY, maxY);

            return new System.Windows.Point(randX, randY);
        }
    }
}
