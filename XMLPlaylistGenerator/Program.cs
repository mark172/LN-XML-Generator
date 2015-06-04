using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace XMLPlaylistGenerator
{
    class Program
    {
        public static void OrderFiles(int count, List<string> combinedFiles, List<string> irvFiles, List<string> grmFiles)
        {
            int irv = 0;
            int grm = 0;
            for (int i = 0; i < (count); i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (irv == irvFiles.Count)
                    {
                        irv = 0;
                    }
                    int playlistnum = 8 * i + j + 1;
                    combinedFiles.Add("<slot slotId='" + playlistnum + "'><popId>X</popId><contentId>"  + irvFiles[irv]  + "</contentId><duration>15</duration></slot>");
                    //combinedFiles.Add(irvFiles[irv]);
                    irv++;
                    
                }
                for (int k = 0; k < 2; k++)
                {
                    if (grm == grmFiles.Count)
                    {
                        grm = 0;
                    }
                    int playlistnum = 8 * i + k + 7;
                    combinedFiles.Add("<slot slotId='" + playlistnum + "'><popId>X</popId><contentId>" + grmFiles[grm] + "</contentId><duration>15</duration></slot>");
                    //combinedFiles.Add(grmFiles[grm]);
                    grm++;
                }
            }
        }

        public static void PopulateFilenames(List<string> combinedFiles, List<string> grmFiles, List<string> irvFiles)
        {
            foreach (var g in grmFiles)
            {
                combinedFiles.Add("<content xmlns=''><id>" + g + "</id><filename>" + g + "</filename>" +
                    "<description>Test</description>" + 
                    "<sponsor><id>Test</id><name>Test</name></sponsor>" +
                    "<active>true</active>" +
                    "<modifyDate>" +
                    "2014-02-25T00:50:43" +
                    "</modifyDate></content>");
            }
            foreach (var v in irvFiles)
            {
                combinedFiles.Add("<content xmlns=''><id>" + v + "</id><filename>" + v + "</filename>" +
                    "<description>Test</description>" + 
                    "<sponsor><id>Test</id><name>Test</name></sponsor>" +
                    "<active>true</active>" +
                    "<modifyDate>" +
                    "2014-02-25T00:50:43" + 
                    "</modifyDate></content>");
            }
        }

        static void Main(string[] args)
        {

            string folderPathIrv = @"\\USHolwFs01\Shared3\DigitalSignage\StadiumVision\Irving Plaza Vestibule\IRV\";
            string folderPathGrm = @"\\USHolwFs01\Shared3\DigitalSignage\StadiumVision\Irving Plaza Vestibule\GRM\";
            string filePath = @"\\USHolwFs01\Shared3\DigitalSignage\StadiumVision\Irving Plaza Vestibule\Vestibule_playlist.xml";

            //List<string> rawFiles = new List<string>();
            List<string> irvFiles = new List<string>();
            List<string> grmFiles = new List<string>();
            List<string> combinedFiles = new List<string>();

            #region xmltext strings

            string xmlheadertext = @"<?xml version='1.0' encoding='utf-8'?>
<playlistbundle xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns='http://xmlns.cisco.com/stadiumvision/playlist/2011'>
  <mailNotification xmlns=''>arjendevries@livenation.com</mailNotification>
<contents>";
            //USE id 9999 for IRV_Entry_XML
            string xmlmiddletext = @"</contents>
<playlists>
    <playlist xmlns=''>
      <id>9999</id>
      <version>Test</version>
      <name>IRV_Entry_XML</name>
      <description>IRV Entry XML</description>
      <pop>false</pop>
      <resolution>
        <width>1920</width>
        <height>1080</height>
      </resolution>
      <modifyDate>" + 
                    "2014-02-25T00:50:43" + 
            "</modifyDate><slots>";

            string xmlendtext = @"</slots>
    </playlist>
  </playlists>
</playlistbundle>";

            #endregion

            combinedFiles.Add(xmlheadertext);

            string[] rawFilesIrv = Directory.GetFiles(folderPathIrv);
            string[] rawFilesGrm = Directory.GetFiles(folderPathGrm);
            int count = 0;

            Array.Sort<string>(rawFilesIrv);
            Array.Sort<string>(rawFilesGrm);

            foreach (string rfi in rawFilesIrv)
            {
                if (Path.GetFileName(rfi) == "Thumbs.db")
                {
                    continue;
                }
                else
                {
                    irvFiles.Add(Path.GetFileName(rfi));
                }
            }

            foreach (string rfg in rawFilesGrm)
            {
                if (Path.GetFileName(rfg) == "Thumbs.db")
                {
                    continue;
                }
                else
                {
                    grmFiles.Add(Path.GetFileName(rfg));
                }
            }

            PopulateFilenames(combinedFiles, grmFiles, irvFiles);

            combinedFiles.Add(xmlmiddletext);

            //make changes here
            if ((grmFiles.Count / 2) >= (irvFiles.Count / 8))
            {
                count = grmFiles.Count;
                if ((count % 2) == 1)
                {
                    count++;
                }
                count = count / 2;
            }
            else
            {
                count = irvFiles.Count;
                if ((count % 8) != 0)
                {
                    count = count + 8 - (count % 8);
                }
                count = count / 8;
            }

            OrderFiles(count, combinedFiles, irvFiles, grmFiles);

            // end changes

            combinedFiles.Add(xmlendtext);

            Console.WriteLine(count);
         
            File.WriteAllLines(filePath, combinedFiles);

            Console.Read();
        }
    }
}
