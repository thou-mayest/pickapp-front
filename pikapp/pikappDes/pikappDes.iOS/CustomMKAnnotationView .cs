using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using MapKit;

namespace pikappDes.iOS
{
    class CustomMKAnnotationView : MKAnnotationView
    {
		public string Name { get; set; }

		public string Url { get; set; }

		public CustomMKAnnotationView(IMKAnnotation annotation, string id)
			: base(annotation, id)
		{
		}

	}
}