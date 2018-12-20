/* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */
using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Dogs.ViewModels.Data.Models
{
    [XmlRoot(ElementName = "link", Namespace = "http://www.topografix.com/GPX/1/1")]
    public class Link
    {
        [XmlElement(ElementName = "text", Namespace = "http://www.topografix.com/GPX/1/1")]
        public string Text { get; set; }
        [XmlAttribute(AttributeName = "href")]
        public string Href { get; set; }
    }

    [XmlRoot(ElementName = "bounds", Namespace = "http://www.topografix.com/GPX/1/1")]
    public class Bounds
    {
        [XmlAttribute(AttributeName = "maxlat")]
        public string Maxlat { get; set; }
        [XmlAttribute(AttributeName = "maxlon")]
        public string Maxlon { get; set; }
        [XmlAttribute(AttributeName = "minlat")]
        public string Minlat { get; set; }
        [XmlAttribute(AttributeName = "minlon")]
        public string Minlon { get; set; }
    }

    [XmlRoot(ElementName = "metadata", Namespace = "http://www.topografix.com/GPX/1/1")]
    public class Metadata
    {
        [XmlElement(ElementName = "link", Namespace = "http://www.topografix.com/GPX/1/1")]
        public Link Link { get; set; }
        [XmlElement(ElementName = "time", Namespace = "http://www.topografix.com/GPX/1/1")]
        public string Time { get; set; }
        [XmlElement(ElementName = "bounds", Namespace = "http://www.topografix.com/GPX/1/1")]
        public Bounds Bounds { get; set; }
    }

    [XmlRoot(ElementName = "TrackExtension", Namespace = "http://www.garmin.com/xmlschemas/GpxExtensions/v3")]
    public class TrackExtension
    {
        [XmlElement(ElementName = "DisplayColor", Namespace = "http://www.garmin.com/xmlschemas/GpxExtensions/v3")]
        public string DisplayColor { get; set; }
    }

    [XmlRoot(ElementName = "extensions", Namespace = "http://www.topografix.com/GPX/1/1")]
    public class Extensions
    {
        [XmlElement(ElementName = "TrackExtension", Namespace = "http://www.garmin.com/xmlschemas/GpxExtensions/v3")]
        public TrackExtension TrackExtension { get; set; }
        [XmlElement(ElementName = "TrackPointExtension", Namespace = "http://www.garmin.com/xmlschemas/TrackPointExtension/v1")]
        public TrackPointExtension TrackPointExtension { get; set; }
        [XmlElement(ElementName = "TrackPointExtension", Namespace = "http://www.garmin.com/xmlschemas/GpxExtensions/v3")]
        public TrackPointExtension2 TrackPointExtension2 { get; set; }
    }

    [XmlRoot(ElementName = "trkpt", Namespace = "http://www.topografix.com/GPX/1/1")]
    public class Trkpt
    {
        [XmlElement(ElementName = "ele", Namespace = "http://www.topografix.com/GPX/1/1")]
        public string Ele { get; set; }
        [XmlElement(ElementName = "time", Namespace = "http://www.topografix.com/GPX/1/1")]
        public string Time { get; set; }
        [XmlAttribute(AttributeName = "lat")]
        public string Lat { get; set; }
        [XmlAttribute(AttributeName = "lon")]
        public string Lon { get; set; }
        [XmlElement(ElementName = "extensions", Namespace = "http://www.topografix.com/GPX/1/1")]
        public Extensions Extensions { get; set; }
    }

    [XmlRoot(ElementName = "trkseg", Namespace = "http://www.topografix.com/GPX/1/1")]
    public class Trkseg
    {
        [XmlElement(ElementName = "trkpt", Namespace = "http://www.topografix.com/GPX/1/1")]
        public List<Trkpt> Trkpt { get; set; }
    }

    [XmlRoot(ElementName = "trk", Namespace = "http://www.topografix.com/GPX/1/1")]
    public class Trk
    {
        [XmlElement(ElementName = "name", Namespace = "http://www.topografix.com/GPX/1/1")]
        public string Name { get; set; }
        [XmlElement(ElementName = "extensions", Namespace = "http://www.topografix.com/GPX/1/1")]
        public Extensions Extensions { get; set; }
        [XmlElement(ElementName = "trkseg", Namespace = "http://www.topografix.com/GPX/1/1")]
        public Trkseg Trkseg { get; set; }
    }

    [XmlRoot(ElementName = "TrackPointExtension", Namespace = "http://www.garmin.com/xmlschemas/TrackPointExtension/v1")]
    public class TrackPointExtension
    {
        [XmlElement(ElementName = "atemp", Namespace = "http://www.garmin.com/xmlschemas/TrackPointExtension/v1")]
        public string Atemp { get; set; }
        [XmlElement(ElementName = "depth", Namespace = "http://www.garmin.com/xmlschemas/TrackPointExtension/v1")]
        public string Depth { get; set; }
        [XmlElement(ElementName = "hr", Namespace = "http://www.garmin.com/xmlschemas/TrackPointExtension/v1")]
        public string Hr { get; set; }
        [XmlElement(ElementName = "cad", Namespace = "http://www.garmin.com/xmlschemas/TrackPointExtension/v1")]
        public string Cad { get; set; }
    }

    [XmlRoot(ElementName = "TrackPointExtension", Namespace = "http://www.garmin.com/xmlschemas/GpxExtensions/v3")]
    public class TrackPointExtension2
    {
        [XmlElement(ElementName = "Temperature", Namespace = "http://www.garmin.com/xmlschemas/GpxExtensions/v3")]
        public string Temperature { get; set; }
        [XmlElement(ElementName = "Depth", Namespace = "http://www.garmin.com/xmlschemas/GpxExtensions/v3")]
        public string Depth { get; set; }
    }

    [XmlRoot(ElementName = "gpx", Namespace = "http://www.topografix.com/GPX/1/1")]
    public class Gpx
    {
        [XmlElement(ElementName = "metadata", Namespace = "http://www.topografix.com/GPX/1/1")]
        public Metadata Metadata { get; set; }
        [XmlElement(ElementName = "trk", Namespace = "http://www.topografix.com/GPX/1/1")]
        public Trk Trk { get; set; }
        [XmlAttribute(AttributeName = "creator")]
        public string Creator { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
        [XmlAttribute(AttributeName = "schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string SchemaLocation { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }
        [XmlAttribute(AttributeName = "wptx1", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Wptx1 { get; set; }
        [XmlAttribute(AttributeName = "gpxtrx", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Gpxtrx { get; set; }
        [XmlAttribute(AttributeName = "gpxtpx", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Gpxtpx { get; set; }
        [XmlAttribute(AttributeName = "gpxx", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Gpxx { get; set; }
        [XmlAttribute(AttributeName = "trp", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Trp { get; set; }
        [XmlAttribute(AttributeName = "adv", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Adv { get; set; }
        [XmlAttribute(AttributeName = "prs", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Prs { get; set; }
        [XmlAttribute(AttributeName = "tmd", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Tmd { get; set; }
        [XmlAttribute(AttributeName = "vptm", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Vptm { get; set; }
        [XmlAttribute(AttributeName = "ctx", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ctx { get; set; }
        [XmlAttribute(AttributeName = "gpxacc", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Gpxacc { get; set; }
        [XmlAttribute(AttributeName = "gpxpx", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Gpxpx { get; set; }
        [XmlAttribute(AttributeName = "vidx1", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Vidx1 { get; set; }
    }

}
