using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Serilog;

namespace client.graphics
{
    public class SplitscreenManager
    {
        private Viewport[] _viewports;
        private Rectangle _fullBounds;
        private Viewport _singular;
        private int _viewCount;
        
        public Viewport[] Viewports => _viewports;
        
        public SplitscreenManager(Rectangle fullBounds)
        {
            _viewports = Array.Empty<Viewport>();
            _fullBounds = fullBounds;
            _singular = new Viewport(fullBounds);
            Log.Debug("Created splitscreen manager with total bounds {Bounds}", fullBounds);
        }

        public void SetViewCount(int count)
        {
            _viewCount = count;
            UpdateViews();
        }
        public void IncrementViewCount()
        {
            ++_viewCount;
            UpdateViews();
        }
        public void DecrementViewCount()
        {
            --_viewCount;
            UpdateViews();
        }
        public Viewport GetView(int index)
        {
            return _viewports[index];
        }
        // Do not render with this viewport
        public Viewport GetSingular()
        {
            return _singular;
        }
        private void UpdateViews()
        {
            
            _viewports = new Viewport[_viewCount];
            
            // Square grid layout of views
            var viewGridDim = (int)Math.Ceiling(Math.Sqrt(_viewCount));
            Log.Debug("View Count {ViewCount} View Grid Dim {ViewGridDim}", _viewCount, viewGridDim );
            var newWidth = _fullBounds.Width / viewGridDim;
            var newHeight = _fullBounds.Height / viewGridDim;
            for (var i = 0;  i < _viewCount; ++i)
            {
                var mod = i % viewGridDim;
                var div = i / viewGridDim;
                _viewports[i] = new Viewport(
                    _fullBounds.X + newWidth * mod, _fullBounds.Y + newHeight * div, 
                    newWidth, newHeight);
                Log.Debug("Viewport {ViewPort}", _viewports[i]);
            }
        }
        
    }
}