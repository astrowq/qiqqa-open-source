﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Qiqqa.Common.Configuration;
using Qiqqa.Documents.PDF;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.BibTex;
using Utilities.Misc;

namespace Qiqqa.Exporting
{
    class Word2007Export
    {
        internal static void Export(List<PDFDocument> pdf_documents)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Library_Word2007Export);

            string initial_directory = null;
            if (null == initial_directory) initial_directory = Path.GetDirectoryName(ConfigurationManager.Instance.ConfigurationRecord.System_LastWord2007ExportFile);
            if (null == initial_directory) initial_directory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string initial_filename = null;
            if (null == initial_filename) initial_filename = Path.GetFileName(ConfigurationManager.Instance.ConfigurationRecord.System_LastWord2007ExportFile);
            if (null == initial_filename) initial_filename = "Qiqqa2Word2007.xml";

            SaveFileDialog dlg = new SaveFileDialog
            {
                CheckFileExists = false,
                CheckPathExists = false,
                Filter = "XML Files|*.xml|All Files|*.*",
                InitialDirectory = initial_directory,
                FileName = initial_filename,
                Title = "Where would you like to save your exported Word 2007 bibliography file?"
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // Remember the filename for next time
                string filename = dlg.FileName;
                ConfigurationManager.Instance.ConfigurationRecord.System_LastWord2007ExportFile = filename;
                ConfigurationManager.Instance.ConfigurationRecord_Bindable.NotifyPropertyChanged(() => ConfigurationManager.Instance.ConfigurationRecord.System_LastWord2007ExportFile);
                

                Logging.Info("Exporting entries to Word 2007 bibliography format");
                

                XmlDocument doc;
                XmlElement elem_sources;
                BibTexToWordConvertor.ConvertWrapperBibTexToXML(out doc, out elem_sources);                


                // Write out the entries
                for (int i = 0; i < pdf_documents.Count; ++i)
                {
                    PDFDocument pdf_document = pdf_documents[i];
                    StatusManager.Instance.UpdateStatus("Word2007Export", String.Format("Exporting entry {0} of {1}", i, pdf_documents.Count), i, pdf_documents.Count);

                    string bibtex = pdf_document.BibTex;
                    if (!String.IsNullOrEmpty(bibtex))
                    {
                        try
                        {
                            XmlNode node_source = BibTexToWordConvertor.ConvertBibTexToXML(doc, bibtex);
                            elem_sources.AppendChild(node_source);
                        }
                        catch (Exception ex)
                        {
                            Logging.Warn(ex, "There was a problem converting a piece of BibTeX to Word2007 format.  BibTeX is '{0}'", bibtex);
                        }
                    }
                }

                // Write out the header
                DateTime now = DateTime.Now;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<!--");
                sb.AppendLine("==============================================================================");
                sb.AppendLine("This Word 2007 bibliography file was generated by Qiqqa (http://www.qiqqa.com)");
                sb.AppendLine(String.Format("{0} {1}", now.ToLongDateString(), now.ToLongTimeString()));
                sb.AppendLine("Version 1");
                sb.AppendLine("==============================================================================");
                sb.AppendLine("-->");
                sb.AppendLine();
                sb.AppendLine(XMLTools.ToString(doc));


                // Write to disk
                File.WriteAllText(filename, sb.ToString());

                StatusManager.Instance.UpdateStatus("Word2007Export", String.Format("Exported your BibTex entries to {0}", filename));
            }
        }
    }
}
