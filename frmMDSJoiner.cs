using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using CsvHelper;
using Microsoft.VisualBasic.FileIO;

namespace HACC_MDS_Joiner
{
    public partial class frmMain : Form
    {
        List<string> FilesProcessed = new List<string>();
        List<string> HeaderRow = new List<string>(10);
        List<List<string>> ClientData = new List<List<string>>();
        string OriginalFilename = "";

        const string VERSION_IDENTIFIER = "201";

        public frmMain()
        {
            InitializeComponent();
            resetForm();
        }
        private void resetForm()
        {
            btnSrc.Enabled = true;
            lblReset.Visible = false;
            btnSubsequent.Enabled = false;
            btnDownload.Enabled = false;
            HeaderRow = new List<string>();
            ClientData = new List<List<string>>();
            FilesProcessed = new List<string>();
            rtbOutput.Text = "";
            OriginalFilename = "";
        }
        private void startSubsequent()
        {
            btnSrc.Enabled = false;
            lblReset.Visible = true;
            btnSubsequent.Enabled = true;
            btnDownload.Enabled = true;
        }

        private void btnSrc_Click(object sender, EventArgs e)
        {
            try
            {
                openCSVFileDialog.Title = "Open Primary HACC MDS CSV File";
                DialogResult result = openCSVFileDialog.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    readCSVIntoArray(openCSVFileDialog.FileName, true);
                    OriginalFilename = Path.GetFileName(openCSVFileDialog.FileName);
                    startSubsequent();
                }
            }
            catch (Exception ex)
            {
                appendOutputError(ex.Message);
            }
            appendOutputPlain("");
        }    
        
        private void btnSubsequent_Click(object sender, EventArgs e)
        {
            try
            {
                openCSVFileDialog.Title = "Open Secondary HACC MDS CSV File";
                DialogResult result = openCSVFileDialog.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    readCSVIntoArray(openCSVFileDialog.FileName, false);
                }
            }
            catch (Exception ex)
            {
                appendOutputError(ex.Message);
            }
            appendOutputPlain("");
        }

        private void readCSVIntoArray(string filepath, bool isfirst)
        {
            appendOutputPlain("Loading file: " + filepath);

            var md5 = MD5.Create();
            var stream = File.OpenRead(filepath);
            var hash = md5.ComputeHash(stream);
            string md5string = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            stream.Close();

            if (FilesProcessed.Contains(md5string))
            {
                throw new Exception("File already loaded");
            }
            else
            {
                FilesProcessed.Add(md5string);
            }


            TextFieldParser csvParser = new TextFieldParser(filepath);
            csvParser.SetDelimiters(new string[] { "," });
            csvParser.HasFieldsEnclosedInQuotes = true;

            //read first row
            string[] fields = csvParser.ReadFields();

            //check for errors
            if(fields.Length < 10)
                throw new Exception("Header row is malformed: expected 10 columns got " + fields.Length.ToString());
            if (fields[1].Trim() != VERSION_IDENTIFIER)
                throw new Exception("Unknown version identifier: expected " + VERSION_IDENTIFIER + " got " + fields[1].Trim());

            if (isfirst)
            {
                //load into header row
                HeaderRow.Insert(0, "HACC");            //COLLECTION IDENTIFIER
                HeaderRow.Insert(1, VERSION_IDENTIFIER);//VERSION IDENTIFIER	
                HeaderRow.Insert(2, "HEADER");          //RECORD TYPE	
                HeaderRow.Insert(3, fields[3]);         //AGENCY IDENTIFIER	
                HeaderRow.Insert(4, fields[4]);         //DATA COLLECTION IDENTIFIER	
                HeaderRow.Insert(5, fields[5]);         //TRANSMISSION NUMBER	
                HeaderRow.Insert(6, fields[6]);         //EXPORT FILE PORTION	
                HeaderRow.Insert(7, fields[7]);         //NUMBER OF CLIENT RECORDS FOLLOWING	
                HeaderRow.Insert(8, fields[8]);         //NAME OF SOFTWARE	
                HeaderRow.Insert(9, "ENDHEADER");       //END HEADER MARKER

                //output details
                appendOutputPlain("Agency Identifier: " + HeaderRow[3]);
                appendOutputPlain("Data Collection Identifier: " + HeaderRow[4]);
                appendOutputPlain("Name of Software: " + HeaderRow[8]);
            }
            else
            {
                //show warnings
                if (fields[4] != HeaderRow[4])
                    appendOutputWarning("Report date mismatch, primary: " + HeaderRow[4] + ", secondary " + fields[4]);
            }
            
            //read client lines
            int clientrow = 0;
            int merged = 0;
            int added = 0;
            while (!csvParser.EndOfData){
                clientrow++;
                List<string> newRow = loadCSVClientRow(clientrow,csvParser.ReadFields());

                int existingrowindex = existingClientRowIndex(newRow);
                if(existingrowindex >= 0) {
                    //merge
                    merged++;
                    mergeClientRows(newRow, existingrowindex);                    
                }
                else
                {
                    //add
                    added++;
                    ClientData.Add(newRow);
                }                
            }

            csvParser.Close();

            appendOutputPlain("Found " + clientrow + " Client Rows");
            appendOutputPlain("Added " + added + " Client Rows");
            if (merged > 0)
            {
                appendOutputPlain("Merged " + merged + " Client Rows (Duplicate SLK)");
            }
            appendOutputPlain("Total " + ClientData.Count + " Client Rows");            
        }

        private void mergeClientRows(List<string> newrow, int existingindex)
        {
            //leave, 0 //COLLECTION IDENTIFER
            //leave, 1 //VERSION IDENTIFIER
            //leave, 2 //RECORD TYPE
            //leave, 3 //LETTERS OF NAME
            //leave, 4 //DATE OF BIRTH
            //leave, 5 //DATE OF BIRTH ESTIMATE FLAG
            //leave, 6 //SEX
            mergeClientValueCode(newrow, 7, existingindex, "9999"); //COUNTRY OF BIRTH
            mergeClientValueCode(newrow, 8, existingindex, "9999"); //PREFERRED LANGUAGE
            mergeClientValueCode(newrow, 9, existingindex, "9");    //NEED FOR INTERPRETER
            mergeClientValueCode(newrow, 10, existingindex, "9");   //INDIGENOUS STATUS
            mergeClientValueCode(newrow, 11, existingindex);        //STATE / TERRITORY IDENTIFIER
            mergeClientValueCode(newrow, 12, existingindex);        //RESIDENTIAL LOCALITY
            mergeClientValueCode(newrow, 13, existingindex);        //RESIDENTIAL POSTCODE
            mergeClientValueCode(newrow, 14, existingindex);        //SLK MISSING FLAG
            mergeClientValueCode(newrow, 15, existingindex, "9");   //LIVING ARRANGEMENT
            mergeClientValueCode(newrow, 16, existingindex, "9");   //GOVT.PENSION / BENEFIT STATUS
            mergeClientValueCode(newrow, 17, existingindex, "9");   //DVA ENTITLEMENT
            mergeClientValueCode(newrow, 18, existingindex, "99");  //ACCOMMODATION

            mergeClientValueCode(newrow, 19, existingindex, "9");   //CARER AVAILABILITY
            mergeClientValueCode(newrow, 20, existingindex);        //CARER - LETTERS OF NAME
            mergeClientValueCode(newrow, 21, existingindex);        //CARER - DATE OF BIRTH
            mergeClientValueCode(newrow, 22, existingindex);        //CARER - DATE OF BIRTH ESTIMATE FLAG
            mergeClientValueCode(newrow, 23, existingindex);        //CARER - SEX
            mergeClientValueCode(newrow, 24, existingindex);        //CARER - COUNTRY OF BIRTH
            mergeClientValueCode(newrow, 25, existingindex);        //CARER - PREFERRED LANGUAGE
            mergeClientValueCode(newrow, 26, existingindex);        //CARER - INDIGENOUS STATUS
            mergeClientValueCode(newrow, 27, existingindex);        //CARER - STATE / TERRITORY IDENFIER
            mergeClientValueCode(newrow, 28, existingindex);        //CARER - RESIDENTIAL LOCALITY
            mergeClientValueCode(newrow, 29, existingindex);        //CARER - POSTCODE
            mergeClientValueCode(newrow, 30, existingindex);        //CARER RESIDENCY STATUS
            mergeClientValueCode(newrow, 31, existingindex);        //RELATIONSHIP OF CARER TO CARE RECIPIENT
            mergeClientValueCode(newrow, 32, existingindex);        //CARER FOR MORE THAN ONE PERSON

            mergeClientValueDate(newrow, 33, existingindex, false); //DATE OF LAST UPDATE
            mergeClientValueCode(newrow, 34, existingindex, "99");  //SOURCE OF REFERRAL
            mergeClientValueDate(newrow, 35, existingindex, true);  //DATE OF ENTRY INTO HACC SERVICE EPISODE
            mergeClientValueDate(newrow, 36, existingindex, false); //DATE OF EXIT FROM HACC SERVICE EPISODE
            mergeClientValueCode(newrow, 37, existingindex);        //MAIN REASON FOR CESSATION OF SERVICES

            mergeClientValueIntAddDefZero(newrow, 37, existingindex);  //DOMESTIC ASSISTANCE(hours)
            mergeClientValueIntAddDefZero(newrow, 38, existingindex);  //VOL.SOCIAL SUPPORT(hours)
            mergeClientValueIntAddDefZero(newrow, 39, existingindex);  //NURSING RECEIVED AT HOME(hours)
            mergeClientValueIntAddDefZero(newrow, 40, existingindex);  //NURSING RECEIVED AT CENTRE(hours)

            mergeClientValueIntAddDefZero(newrow, 41, existingindex);  //PODIATRY AT HOME(hours)
            mergeClientValueIntAddDefZero(newrow, 42, existingindex);  //OCCUPATIONAL THERAPY AT HOME(hours)
            mergeClientValueIntAddDefZero(newrow, 43, existingindex);  //SPEECH PATHOLOGY AT HOME(hours)
            mergeClientValueIntAddDefZero(newrow, 44, existingindex);  //DIETETICS AT HOME(hours)
            mergeClientValueIntAddDefZero(newrow, 45, existingindex);  //PHYSIOTHERAPY AT HOME(hours)
            mergeClientValueIntAddDefZero(newrow, 46, existingindex);  //AUDIOLOGY AT HOME(hours)
            mergeClientValueIntAddDefZero(newrow, 47, existingindex);  //COUNSELLING AT HOME(hours)

            mergeClientValueIntAddDefZero(newrow, 49, existingindex);  //ALLIED HEALTH CARE RECEIVED AT HOME -TOTAL TIME(Hours)
            mergeClientValueIntAddDefZero(newrow, 50, existingindex);  //PODIATRY AT CENTRE(hours)
            mergeClientValueIntAddDefZero(newrow, 51, existingindex);  //OCCUPATIONAL THERAPY AT CENTRE(hours)
            mergeClientValueIntAddDefZero(newrow, 52, existingindex);  //SPEECH PATHOLOGY AT CENTRE(hours)
            mergeClientValueIntAddDefZero(newrow, 53, existingindex);  //DIETETICS AT CENTRE(hours)
            mergeClientValueIntAddDefZero(newrow, 54, existingindex);  //PHYSIOTHERAPY AT CENTRE(hours)
            mergeClientValueIntAddDefZero(newrow, 55, existingindex);  //AUDIOLOGY AT CENTRE(hours)
            mergeClientValueIntAddDefZero(newrow, 56, existingindex);  //COUNSELLING AT CENTRE(hours)
            mergeClientValueIntAddDefZero(newrow, 57, existingindex);  //ALLIED HEALTH CARE RECEIVED AT CENTRE(hours)

            mergeClientValueIntAddDefZero(newrow, 58, existingindex);  //PERSONAL CARE(hours)
            mergeClientValueIntAddDefZero(newrow, 59, existingindex);  //PLANNED ACTIVITY GROUP, CORE(hours)
            mergeClientValueIntAddDefZero(newrow, 60, existingindex);  //PLANNED ACTIVITY GROUP, HIGH(hours)
            mergeClientValueIntAddDefZero(newrow, 61, existingindex);  //MEALS RECEIVED AT HOME(no.of meals)
            mergeClientValueIntAddDefZero(newrow, 62, existingindex);  //MEALS RECEIVED AT CENTRE(no.of meals)
            mergeClientValueIntAddDefZero(newrow, 63, existingindex);  //RESPITE(hours)
            mergeClientValueIntAddDefZero(newrow, 64, existingindex);  //ASSESSMENT(hours)
            mergeClientValueIntAddDefZero(newrow, 65, existingindex);  //CASE MANAGEMENT(hours)
            mergeClientValueIntAddDefZero(newrow, 66, existingindex);  //CLIENT CARE COORDINATION(hours)
            mergeClientValueIntAddDefZero(newrow, 67, existingindex);  //PROPERTY MAINTENANCE(hours)

            mergeClientValueIntAddDefZero(newrow, 68, existingindex);  //PROVISION OF GOODS AND EQUIPMENT - Self Care Aids
            mergeClientValueIntAddDefZero(newrow, 69, existingindex);  //PROVISION OF GOODS AND EQUIPMENT - Supporting and Mobility Aids
            mergeClientValueIntAddDefZero(newrow, 70, existingindex);  //PROVISION OF GOODS AND EQUIPMENT - Communication Aids
            mergeClientValueIntAddDefZero(newrow, 71, existingindex);  //PROVISION OF GOODS AND EQUIPMENT - Aids for reading
            mergeClientValueIntAddDefZero(newrow, 72, existingindex);  //PROVISION OF GOODS AND EQUIPMENT - Medical Care Aids
            mergeClientValueIntAddDefZero(newrow, 73, existingindex);  //PROVISION OF GOODS AND EQUIPMENT - Car modifications
            mergeClientValueIntAddDefZero(newrow, 74, existingindex);  //PROVISION OF GOODS AND EQUIPMENT - Other goods / equipment

            mergeClientValueIntAddDefZero(newrow, 75, existingindex);  //COUNSELLING / SUPPORT, INFORMATION AND ADVOCACY - CARE RECIPIENT(hours)
            mergeClientValueIntAddDefZero(newrow, 76, existingindex);  //COUNSELLING / SUPPORT, INFORMATION AND ADVOCACY - CARER(hours)

            mergeClientValueCode(newrow, 77, existingindex, "9");   //FUNCTIONAL STATUS - Housework
            mergeClientValueCode(newrow, 78, existingindex, "9");   //FUNCTIONAL STATUS - Transport
            mergeClientValueCode(newrow, 79, existingindex, "9");   //FUNCTIONAL STATUS - Shopping
            mergeClientValueCode(newrow, 80, existingindex, "9");   //FUNCTIONAL STATUS - Medication
            mergeClientValueCode(newrow, 81, existingindex, "9");   //FUNCTIONAL STATUS - Money
            mergeClientValueCode(newrow, 82, existingindex, "9");   //FUNCTIONAL STATUS - Walking
            mergeClientValueCode(newrow, 83, existingindex, "9");   //FUNCTIONAL STATUS - Mobility
            mergeClientValueCode(newrow, 84, existingindex, "9");   //FUNCTIONAL STATUS - Self - care screen
            mergeClientValueCode(newrow, 85, existingindex, "9");   //FUNCTIONAL STATUS - Bathing
            mergeClientValueCode(newrow, 86, existingindex, "9");   //FUNCTIONAL STATUS - Dressing
            mergeClientValueCode(newrow, 87, existingindex, "9");   //FUNCTIONAL STATUS - Eating
            mergeClientValueCode(newrow, 88, existingindex, "9");   //FUNCTIONAL STATUS - Toilet
            mergeClientValueCode(newrow, 89, existingindex, "9");   //FUNCTIONAL STATUS - Communication
            mergeClientValueCode(newrow, 90, existingindex, "9");   //FUNCTIONAL STATUS - Memory
            mergeClientValueCode(newrow, 91, existingindex, "9");   //FUNCTIONAL STATUS - Behaviour

            mergeClientValueCode(newrow, 92, existingindex,"0");    //HRS Registered Client
            mergeClientValueCode(newrow, 93, existingindex, "0");   //HRS Confirmation Call
            mergeClientValueIntAddDefZero(newrow, 94, existingindex);  //HRS Call -out in Time 1
            mergeClientValueIntAddDefZero(newrow, 95, existingindex);  //HRS Call -out in Time 2
            mergeClientValueIntAddDefZero(newrow, 96, existingindex);  //HRS Call -out in Time 3
            mergeClientValueIntAddDefZero(newrow, 97, existingindex);  //HRS Call -out in Time 4

            mergeClientValueIntAddDefZero(newrow, 98, existingindex);   //SCP Respite daytime in home
            mergeClientValueIntAddDefZero(newrow, 99, existingindex);   //SCP Respite overnight in home non - active
            mergeClientValueIntAddDefZero(newrow, 100, existingindex);  //SCP Respite overnight in home active
            mergeClientValueIntAddDefZero(newrow, 101, existingindex);  //SCP Respite residential
            mergeClientValueIntAddDefZero(newrow, 102, existingindex);  //SCP Counselling and support
            mergeClientValueIntAddDefZero(newrow, 103, existingindex);  //SCP Goods and equipment cost(whole $)

            mergeClientValueIntAddPrefHigher(newrow, 104, existingindex);  //CCP Dependent Children
            mergeClientValueCode(newrow, 105, existingindex, "99");        //CCP Disability Type
            mergeClientValueIntAddDefZero(newrow, 106, existingindex);  //CCP Assertive Outreach(hours)
            mergeClientValueIntAddDefZero(newrow, 107, existingindex);  //CCP Care Coordination(hours)
            mergeClientValueIntAddDefZero(newrow, 108, existingindex);  //CCP Flexible Care Funds(whole $)
            mergeClientValueIntAddDefZero(newrow, 109, existingindex);  //CCP Housing Assistance(hours)
            mergeClientValueIntAddDefZero(newrow, 110, existingindex);  //CCP Group Social Support(hours)
            
            mergeClientValueIntAddDefZero(newrow, 111, existingindex);  //HSAP Assertive Outreach(hours)
            mergeClientValueIntAddDefZero(newrow, 112, existingindex);  //HSAP Care Coordination(hours)
            mergeClientValueIntAddDefZero(newrow, 113, existingindex);  //HSAP Flexible Care Funds(whole $)
            mergeClientValueIntAddDefZero(newrow, 114, existingindex);  //HSAP Housing Assistance(hours)
            
            mergeClientValueIntAddDefZero(newrow, 115, existingindex);  //OPHR Assertive Outreach(hours)
            mergeClientValueIntAddDefZero(newrow, 116, existingindex);  //OPHR Care Coordination(hours)
            mergeClientValueIntAddDefZero(newrow, 117, existingindex);  //OPHR Flexible Care Funds(whole $)
            mergeClientValueIntAddDefZero(newrow, 118, existingindex);  //OPHR Housing Assistance(hours)
            mergeClientValueIntAddDefZero(newrow, 119, existingindex);  //OPHR Group Social Support(hours)
            
            mergeClientValueIntAddDefZero(newrow, 120, existingindex);  //SRS Care Coordination(hours)
            mergeClientValueIntAddDefZero(newrow, 121, existingindex);  //SRS Housing Assistance(hours)
            mergeClientValueIntAddDefZero(newrow, 122, existingindex);  //SRS Group Social Support(hours)
            
            //leave, 123    //END CLIENT MARKER
        }

        private void mergeClientValueIntAddDefZero(List<string> newrow, int fieldindex, int existingindex)
        {
            int newint, existingint;           
            int.TryParse(newrow[fieldindex], out newint);
            int.TryParse(ClientData[existingindex][fieldindex], out existingint);
            if (newint < 0) 
                newint = 0;
            if (existingint < 0)
                existingint = 0;
            ClientData[existingindex][fieldindex] = (newint + existingint).ToString();            
        }

        private void mergeClientValueIntAddPrefHigher(List<string> newrow, int fieldindex, int existingindex)
        {
            int newint, existingint;            
            int.TryParse(newrow[fieldindex], out newint);
            int.TryParse(ClientData[existingindex][fieldindex], out existingint);
            if (newint < 0)
                newint = 0;
            if (existingint < 0)
                existingint = 0;
            if(newint < existingint)
                ClientData[existingindex][fieldindex] = (newint).ToString();            
        }

        private void mergeClientValueDate(List<string> newrow, int fieldindex, int existingindex, bool preferenceearlierdate)
        {
            //update if existing is empty and new is not
            if(newrow[fieldindex] != "")
            {
                //existing is empty, just update with date string
                if (ClientData[existingindex][fieldindex] == "")
                {
                    ClientData[existingindex][fieldindex] = newrow[fieldindex];
                }
                else
                {
                    DateTime newdate;
                    DateTime existingdate;
                    if(DateTime.TryParseExact(newrow[fieldindex], "dd/MM/yyyy", null, DateTimeStyles.None, out newdate)
                        && DateTime.TryParseExact(ClientData[existingindex][fieldindex], "dd/MM/yyyy", null, DateTimeStyles.None, out existingdate))
                    {
                        if(newdate != existingdate)
                        {
                            if (preferenceearlierdate)
                            {
                                if(newdate < existingdate)
                                {
                                    ClientData[existingindex][fieldindex] = newrow[fieldindex];
                                }
                            }
                            else
                            {
                                if (newdate > existingdate)
                                {
                                    ClientData[existingindex][fieldindex] = newrow[fieldindex];
                                }
                            }
                        }                        
                    }
                }
            }            
        }

        private void mergeClientValueCode(List<string> newrow, int fieldindex, int existingindex,string notstatedcode = null)
        {
            if (newrow[fieldindex] != "")
            {
                //update the original only if the existing code is the not-stated code
                if (notstatedcode != null)
                {
                    if (ClientData[existingindex][fieldindex] == "" || ClientData[existingindex][fieldindex] == notstatedcode)
                    {
                        ClientData[existingindex][fieldindex] = newrow[fieldindex];
                    }
                }
                //update only if the original is empty, i.e. preference the original
                else
                {
                    if (ClientData[existingindex][fieldindex] == "")
                    {
                        ClientData[existingindex][fieldindex] = newrow[fieldindex];
                    }
                }
            }
        }

        private int existingClientRowIndex(List<string> newclientrow)
        {
            string newclientslk = getSLK581(newclientrow);           
            foreach (var existingrow in ClientData.Select((value, i) => new { i, value }))
            {
                var value = existingrow.value;
                var index = existingrow.i;
                if (newclientslk == getSLK581(existingrow.value))
                    return existingrow.i;                
            }            
            return -1;
        }

        private string getSLK581(List<string> clientrow)
        {
            return (clientrow[3] + clientrow[4].Replace("/", "") + clientrow[5]).ToUpper();
        }

        private List<string> loadCSVClientRow(int rownum,string[] csvfields)
        {
            List<string> result = new List<string>(124);            
            if (csvfields.Length < 124)
                throw new Exception("Client row ("+rownum+") is malformed: expected 124 columns got " + csvfields.Length.ToString());
            
            result.Insert(0, "HACC");                  //COLLECTION IDENTIFER
            result.Insert(1, VERSION_IDENTIFIER);      //VERSION IDENTIFIER
            result.Insert(2, "CLIENT");                //RECORD TYPE

            result.Insert(3, csvfields[3].Trim());     //LETTERS OF NAME
            result.Insert(4, csvfields[4].Trim());     //DATE OF BIRTH
            result.Insert(5, csvfields[5].Trim());     //DATE OF BIRTH ESTIMATE FLAG
            result.Insert(6, csvfields[6].Trim());     //SEX

            result.Insert(7, csvfields[7].Trim());     //COUNTRY OF BIRTH
            result.Insert(8, csvfields[8].Trim());     //PREFERRED LANGUAGE
            result.Insert(9, csvfields[9].Trim());     //NEED FOR INTERPRETER
            result.Insert(10, csvfields[10].Trim());   //INDIGENOUS STATUS
            result.Insert(11, csvfields[11].Trim());   //STATE / TERRITORY IDENTIFIER
            result.Insert(12, csvfields[12].Trim());   //RESIDENTIAL LOCALITY
            result.Insert(13, csvfields[13].Trim());   //RESIDENTIAL POSTCODE
            result.Insert(14, csvfields[14].Trim());   //SLK MISSING FLAG
            result.Insert(15, csvfields[15].Trim());   //LIVING ARRANGEMENT
            result.Insert(16, csvfields[16].Trim());   //GOVT.PENSION / BENEFIT STATUS
            result.Insert(17, csvfields[17].Trim());   //DVA ENTITLEMENT
            result.Insert(18, csvfields[18].Trim());   //ACCOMMODATION

            result.Insert(19, csvfields[19].Trim());   //CARER AVAILABILITY
            result.Insert(20, csvfields[20].Trim());   //CARER - LETTERS OF NAME
            result.Insert(21, csvfields[21].Trim());   //CARER - DATE OF BIRTH
            result.Insert(22, csvfields[22].Trim());   //CARER - DATE OF BIRTH ESTIMATE FLAG
            result.Insert(23, csvfields[23].Trim());   //CARER - SEX
            result.Insert(24, csvfields[24].Trim());   //CARER - COUNTRY OF BIRTH
            result.Insert(25, csvfields[25].Trim());   //CARER - PREFERRED LANGUAGE
            result.Insert(26, csvfields[26].Trim());   //CARER - INDIGENOUS STATUS
            result.Insert(27, csvfields[27].Trim());   //CARER - STATE / TERRITORY IDENFIER
            result.Insert(28, csvfields[28].Trim());   //CARER - RESIDENTIAL LOCALITY
            result.Insert(29, csvfields[29].Trim());   //CARER - POSTCODE
            result.Insert(30, csvfields[30].Trim());   //CARER RESIDENCY STATUS
            result.Insert(31, csvfields[31].Trim());   //RELATIONSHIP OF CARER TO CARE RECIPIENT
            result.Insert(32, csvfields[32].Trim());   //CARER FOR MORE THAN ONE PERSON

            result.Insert(33, csvfields[33].Trim());   //DATE OF LAST UPDATE
            result.Insert(34, csvfields[34].Trim());   //SOURCE OF REFERRAL
            result.Insert(35, csvfields[35].Trim());   //DATE OF ENTRY INTO HACC SERVICE EPISODE
            result.Insert(36, csvfields[36].Trim());   //DATE OF EXIT FROM HACC SERVICE EPISODE
            result.Insert(37, csvfields[37].Trim());   //MAIN REASON FOR CESSATION OF SERVICES

            result.Insert(38, csvfields[38].Trim());   //DOMESTIC ASSISTANCE(hours)
            result.Insert(39, csvfields[39].Trim());   //VOL.SOCIAL SUPPORT(hours)
            result.Insert(40, csvfields[40].Trim());   //NURSING RECEIVED AT HOME(hours)
            result.Insert(41, csvfields[41].Trim());   //NURSING RECEIVED AT CENTRE(hours)

            result.Insert(42, csvfields[42].Trim());   //PODIATRY AT HOME(hours)
            result.Insert(43, csvfields[43].Trim());   //OCCUPATIONAL THERAPY AT HOME(hours)
            result.Insert(44, csvfields[44].Trim());   //SPEECH PATHOLOGY AT HOME(hours)
            result.Insert(45, csvfields[45].Trim());   //DIETETICS AT HOME(hours)
            result.Insert(46, csvfields[46].Trim());   //PHYSIOTHERAPY AT HOME(hours)
            result.Insert(47, csvfields[47].Trim());   //AUDIOLOGY AT HOME(hours)
            result.Insert(48, csvfields[48].Trim());   //COUNSELLING AT HOME(hours)

            result.Insert(49, csvfields[49].Trim());   //ALLIED HEALTH CARE RECEIVED AT HOME -TOTAL TIME(Hours)
            result.Insert(50, csvfields[50].Trim());   //PODIATRY AT CENTRE(hours)
            result.Insert(51, csvfields[51].Trim());   //OCCUPATIONAL THERAPY AT CENTRE(hours)
            result.Insert(52, csvfields[52].Trim());   //SPEECH PATHOLOGY AT CENTRE(hours)
            result.Insert(53, csvfields[53].Trim());   //DIETETICS AT CENTRE(hours)
            result.Insert(54, csvfields[54].Trim());   //PHYSIOTHERAPY AT CENTRE(hours)
            result.Insert(55, csvfields[55].Trim());   //AUDIOLOGY AT CENTRE(hours)
            result.Insert(56, csvfields[56].Trim());   //COUNSELLING AT CENTRE(hours)
            result.Insert(57, csvfields[57].Trim());   //ALLIED HEALTH CARE RECEIVED AT CENTRE(hours)

            result.Insert(58, csvfields[58].Trim());   //PERSONAL CARE(hours)
            result.Insert(59, csvfields[59].Trim());   //PLANNED ACTIVITY GROUP, CORE(hours)
            result.Insert(60, csvfields[60].Trim());   //PLANNED ACTIVITY GROUP, HIGH(hours)
            result.Insert(61, csvfields[61].Trim());   //MEALS RECEIVED AT HOME(no.of meals)
            result.Insert(62, csvfields[62].Trim());   //MEALS RECEIVED AT CENTRE(no.of meals)
            result.Insert(63, csvfields[63].Trim());   //RESPITE(hours)
            result.Insert(64, csvfields[64].Trim());   //ASSESSMENT(hours)
            result.Insert(65, csvfields[65].Trim());   //CASE MANAGEMENT(hours)
            result.Insert(66, csvfields[66].Trim());   //CLIENT CARE COORDINATION(hours)
            result.Insert(67, csvfields[67].Trim());   //PROPERTY MAINTENANCE(hours)

            result.Insert(68, csvfields[68].Trim());   //PROVISION OF GOODS AND EQUIPMENT - Self Care Aids
            result.Insert(69, csvfields[69].Trim());   //PROVISION OF GOODS AND EQUIPMENT - Supporting and Mobility Aids
            result.Insert(70, csvfields[70].Trim());   //PROVISION OF GOODS AND EQUIPMENT - Communication Aids
            result.Insert(71, csvfields[71].Trim());   //PROVISION OF GOODS AND EQUIPMENT - Aids for reading
            result.Insert(72, csvfields[72].Trim());   //PROVISION OF GOODS AND EQUIPMENT - Medical Care Aids
            result.Insert(73, csvfields[73].Trim());   //PROVISION OF GOODS AND EQUIPMENT - Car modifications
            result.Insert(74, csvfields[74].Trim());   //PROVISION OF GOODS AND EQUIPMENT - Other goods / equipment

            result.Insert(75, csvfields[75].Trim());   //COUNSELLING / SUPPORT, INFORMATION AND ADVOCACY - CARE RECIPIENT(hours)
            result.Insert(76, csvfields[76].Trim());   //COUNSELLING / SUPPORT, INFORMATION AND ADVOCACY - CARER(hours)

            result.Insert(77, csvfields[77].Trim());   //FUNCTIONAL STATUS - Housework
            result.Insert(78, csvfields[78].Trim());   //FUNCTIONAL STATUS - Transport
            result.Insert(79, csvfields[79].Trim());   //FUNCTIONAL STATUS - Shopping
            result.Insert(80, csvfields[80].Trim());   //FUNCTIONAL STATUS - Medication
            result.Insert(81, csvfields[81].Trim());   //FUNCTIONAL STATUS - Money
            result.Insert(82, csvfields[82].Trim());   //FUNCTIONAL STATUS - Walking
            result.Insert(83, csvfields[83].Trim());   //FUNCTIONAL STATUS - Mobility
            result.Insert(84, csvfields[84].Trim());   //FUNCTIONAL STATUS - Self - care screen
            result.Insert(85, csvfields[85].Trim());   //FUNCTIONAL STATUS - Bathing
            result.Insert(86, csvfields[86].Trim());   //FUNCTIONAL STATUS - Dressing
            result.Insert(87, csvfields[87].Trim());   //FUNCTIONAL STATUS - Eating
            result.Insert(88, csvfields[88].Trim());   //FUNCTIONAL STATUS - Toilet
            result.Insert(89, csvfields[89].Trim());   //FUNCTIONAL STATUS - Communication
            result.Insert(90, csvfields[90].Trim());   //FUNCTIONAL STATUS - Memory
            result.Insert(91, csvfields[91].Trim());   //FUNCTIONAL STATUS - Behaviour

            result.Insert(92, csvfields[92].Trim());   //HRS Registered Client
            result.Insert(93, csvfields[93].Trim());   //HRS Confirmation Call
            result.Insert(94, csvfields[94].Trim());   //HRS Call -out in Time 1
            result.Insert(95, csvfields[95].Trim());   //HRS Call -out in Time 2
            result.Insert(96, csvfields[96].Trim());   //HRS Call -out in Time 3
            result.Insert(97, csvfields[97].Trim());   //HRS Call -out in Time 4

            result.Insert(98, csvfields[98].Trim());   //SCP Respite daytime in home
            result.Insert(99, csvfields[99].Trim());   //SCP Respite overnight in home non - active
            result.Insert(100, csvfields[100].Trim()); //SCP Respite overnight in home active
            result.Insert(101, csvfields[101].Trim()); //SCP Respite residential
            result.Insert(102, csvfields[102].Trim()); //SCP Counselling and support
            result.Insert(103, csvfields[103].Trim()); //SCP Goods and equipment cost(whole $)

            result.Insert(104, csvfields[104].Trim()); //CCP Dependent Children
            result.Insert(105, csvfields[105].Trim()); //CCP Disability Type
            result.Insert(106, csvfields[106].Trim()); //CCP Assertive Outreach(hours)
            result.Insert(107, csvfields[107].Trim()); //CCP Care Coordination(hours)
            result.Insert(108, csvfields[108].Trim()); //CCP Flexible Care Funds(whole $)
            result.Insert(109, csvfields[109].Trim()); //CCP Housing Assistance(hours)
            result.Insert(110, csvfields[110].Trim()); //CCP Group Social Support(hours)

            result.Insert(111, csvfields[111].Trim()); //HSAP Assertive Outreach(hours)
            result.Insert(112, csvfields[112].Trim()); //HSAP Care Coordination(hours)
            result.Insert(113, csvfields[113].Trim()); //HSAP Flexible Care Funds(whole $)
            result.Insert(114, csvfields[114].Trim()); //HSAP Housing Assistance(hours)

            result.Insert(115, csvfields[115].Trim()); //OPHR Assertive Outreach(hours)
            result.Insert(116, csvfields[116].Trim()); //OPHR Care Coordination(hours)
            result.Insert(117, csvfields[117].Trim()); //OPHR Flexible Care Funds(whole $)
            result.Insert(118, csvfields[118].Trim()); //OPHR Housing Assistance(hours)
            result.Insert(119, csvfields[119].Trim()); //OPHR Group Social Support(hours)

            result.Insert(120, csvfields[120].Trim()); //SRS Care Coordination(hours)
            result.Insert(121, csvfields[121].Trim()); //SRS Housing Assistance(hours)
            result.Insert(122, csvfields[122].Trim()); //SRS Group Social Support(hours)

            result.Insert(123, "ENDCLIENT");           //END CLIENT MARKER

            return result;
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                saveCSVFileDialog.FileName = OriginalFilename;
                DialogResult result = saveCSVFileDialog.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    saveCSVFile(saveCSVFileDialog.FileName);                    
                }
            }
            catch (Exception ex)
            {
                appendOutputError(ex.Message);
            }
            appendOutputPlain("");
        }

        private void saveCSVFile(string path)
        {
            appendOutputPlain("Saving file to " + path);
            var writer = new StreamWriter(path);
            var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            //write header
            foreach (string headervalue in HeaderRow)
            {
                csv.WriteField(headervalue);                
            }
            csv.NextRecord();
            //write client data
            foreach (List<string> ClientDataRow in ClientData)
            {
                foreach (string rowvalue in ClientDataRow)
                {
                    csv.WriteField(rowvalue);
                }
                csv.NextRecord();
            }
            writer.Flush();
            writer.Close();
            appendOutputPlain("Done");
        }

        private void lblReset_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            resetForm();
        }

        private void appendOutput(string text, Color? color = null)
        {
            if (color == null) 
                color = Color.Black;
            rtbOutput.SelectionStart = rtbOutput.TextLength;
            rtbOutput.SelectionLength = 0;
            rtbOutput.SelectionColor = (Color)color;
            rtbOutput.AppendText(text);
            rtbOutput.SelectionColor = rtbOutput.ForeColor;
            rtbOutput.SelectionStart = rtbOutput.Text.Length;            
            rtbOutput.ScrollToCaret();
        }

        private void appendOutputWarning(string text)
        {
            appendOutput("Warning: ", Color.FromArgb(245, 191, 66));
            appendOutput(text + "\n");
        }

        private void appendOutputError(string text)
        {
            appendOutput("Error: ", Color.Red);
            appendOutput(text + "\n");
        }

        private void appendOutputPlain(string text)
        {
            appendOutput(text + "\n");
        }
    }
}
