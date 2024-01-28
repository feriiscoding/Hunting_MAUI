using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunting.View
{
    public class AutoSizeContentView : ContentView
    {
        public AutoSizeContentView()
        {
            ChildAdded += new EventHandler<ElementEventArgs>(AutoSizeContentView_ChildAdded);
        }

        private void AutoSizeContentView_ChildAdded(object? sender, ElementEventArgs e)
        {
            if (Content != null)
                Content.SizeChanged += new EventHandler(Content_SizeChanged);
        }

        private void Content_SizeChanged(object? sender, EventArgs e)
        {
            Double heightScale = Height / Content.Height;
            Double widthScale = Width / Content.Width;

            if (widthScale > 0 && heightScale > 0)
                Content.Scale = Math.Min(heightScale, widthScale);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            Double heightScale = height / Content.Height;
            Double widthScale = width / Content.Width;

            if (widthScale > 0 && heightScale > 0)
                Content.Scale = Math.Min(heightScale, widthScale);
        }
    }
}
