using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading;//.Tasks;

namespace NewFMS
{
    public class BTLV
    {
        List<BTLV> children = new List<BTLV>();
        public String Tag = "";
        public String Value = "";
        String Data = "";
        public void AddEntry(String Tag, String Value)
        {
            if (Value == null)
                return;
            Data += Tag.PadLeft(2, ' ') + Value.Length.ToString().PadLeft(3, '0') + Value;
            children.Add(new BTLV() { Tag = Tag, Value = Value });
        }
        public BTLV Open(String d)
        {
            while (d.Length != 0)
            {
                BTLV tlv = new BTLV();
                tlv.Tag = d.Substring(0, 2);
                int l = int.Parse(d.Substring(2, 3));
                if (l > 0)
                    tlv.Value = d.Substring(5, l);
                d = d.Substring(5 + l, d.Length - 5 - l);
                children.Add(tlv);
            }
            return this;
        }
        public String Print(String indent)
        {
            String ss = "";
            if (children.Count == 0)
                return indent + "TAG: [" + Tag + "] Value: [" + Value + "]";
            foreach (BTLV t in children)
            {
                ss += t.Print(indent + "--") + "\r\n";
            }
            return ss;
        }
        public override string ToString()
        {
            return Data;
        }

        /////////////////////////////////////////
        public string ParsPosResp(byte[] bytesToRead, int len, ref bool isSuccecc)
        {
            isSuccecc = false;
            string resp = Encoding.ASCII.GetString(bytesToRead);
            if (len == 22)
                return (new BTLV().OpenFailed(resp));
            else if (len > 22)
            {
                isSuccecc = true;
                return (new BTLV().OpenSuccess(resp));
            }
            else
                return "Parser Error ";
        }

        private string OpenFailed(string respFailed)
        {
            string ParsedFailed = "";
            ParsedFailed = respFailed.Substring(9, 2) + "->" + respFailed.Substring(14, 2) + "\r\n";
            ParsedFailed += respFailed.Substring(16, 2) + "->" + respFailed.Substring(21, 1) + "\r\n";
            return ParsedFailed;
        }

        private string OpenSuccess(string respSuccess)
        {
            string ParsedSuccess = "";
            ParsedSuccess = " پاسخ تراکنش : " + GetTrxnResp(respSuccess) + "\r\n";
            ParsedSuccess += " شماره پیگیری : " + GetTraceNo(respSuccess) + "\r\n";
            ParsedSuccess += "شماره کارت : " + GetPANID(respSuccess) + "\r\n";
            ParsedSuccess += "شماره ترمینال : " + GetTerminalID(respSuccess) + "\r\n";
            ParsedSuccess += "مبلغ : " + GetAmount(respSuccess) + "\r\n";
            ParsedSuccess += "شماره مرجع : " + GetTrxnRRN(respSuccess) + "\r\n";
            ParsedSuccess += "تاریخ و زمان تراکنش : " + GetTrxnDateTime(respSuccess) + "\r\n";
            ParsedSuccess += "سریال تراکنش : " + GetTrxnSerial(respSuccess) + "\r\n";
            ParsedSuccess += "\r\n" + "------------------------------" + "\r\n";
            ParsedSuccess += "--TAG [RS]: value: " + GetTrxnResp(respSuccess) + "\r\n";
            ParsedSuccess += "--TAG [TR]: value: " + GetTraceNo(respSuccess) + "\r\n";
            ParsedSuccess += "--TAG [PN]: value: " + GetPANID(respSuccess) + "\r\n";
            ParsedSuccess += "--TAG [TM]: value: " + GetTerminalID(respSuccess) + "\r\n";
            ParsedSuccess += "--TAG [AM]: value: " + GetAmount(respSuccess) + "\r\n";
            ParsedSuccess += "--TAG [RN]: value: " + GetTrxnRRN(respSuccess) + "\r\n";
            ParsedSuccess += "--TAG [TI]: value: " + GetTrxnDateTime(respSuccess) + "\r\n";
            ParsedSuccess += "--TAG [SR]: value: " + GetTrxnSerial(respSuccess) + "\r\n";


            return ParsedSuccess;
        }


        public string GetTrxnResp(string respSuccess)
        {
            string ParsedSuccess = "";
            ParsedSuccess = respSuccess.Substring(14, 2);
            return ParsedSuccess;
        }
        public string GetTraceNo(string respSuccess)
        {
            string ParsedSuccess = "";
            ParsedSuccess = respSuccess.Substring((respSuccess.IndexOf("TR") + 5), Convert.ToInt32(respSuccess.Substring(respSuccess.IndexOf("TR") + 2, 3)));
            return ParsedSuccess;
        }

        public string GetPANID(string respSuccess)
        {
            string ParsedSuccess = "";
            ParsedSuccess = respSuccess.Substring((respSuccess.IndexOf("PN") + 5), Convert.ToInt32(respSuccess.Substring(respSuccess.IndexOf("PN") + 2, 3)));
            return ParsedSuccess;
        }

        public string GetTerminalID(string respSuccess)
        {
            string ParsedSuccess = "";
            ParsedSuccess = respSuccess.Substring((respSuccess.IndexOf("TM") + 5), Convert.ToInt32(respSuccess.Substring(respSuccess.IndexOf("TM") + 2, 3)));
            return ParsedSuccess;
        }
        public string GetAmount(string respSuccess)
        {
            string ParsedSuccess = "";
            ParsedSuccess = respSuccess.Substring((respSuccess.IndexOf("AM") + 5), Convert.ToInt32(respSuccess.Substring(respSuccess.IndexOf("AM") + 2, 3)));
            return ParsedSuccess;
        }
        public string GetTrxnSerial(string respSuccess)
        {
            string ParsedSuccess = "";
            try
            {
                ParsedSuccess = respSuccess.Substring((respSuccess.IndexOf("SR") + 5), Convert.ToInt32(respSuccess.Substring(respSuccess.IndexOf("SR") + 2, 3)));
            }
            catch (Exception ex)
            {

            }
            return ParsedSuccess;
        }
        private string GetTrxnRRN(string respSuccess)
        {
            string ParsedSuccess = "";
            ParsedSuccess = respSuccess.Substring((respSuccess.IndexOf("RN") + 5), Convert.ToInt32(respSuccess.Substring(respSuccess.IndexOf("RN") + 2, 3)));
            return ParsedSuccess;
        }
        private string GetTrxnDateTime(string respSuccess)
        {
            string ParsedSuccess = "";
            ParsedSuccess = respSuccess.Substring((respSuccess.IndexOf("TI") + 5), Convert.ToInt32(respSuccess.Substring(respSuccess.IndexOf("TI") + 2, 3)));
            return ParsedSuccess;
        }
        private string GetTrxnDate(string respSuccess)
        {
            string ParsedSuccess = "";
            ParsedSuccess = respSuccess.Substring((respSuccess.IndexOf("TI") + 5), 10);// Convert.ToInt32(respSuccess.Substring(respSuccess.IndexOf("TI") + 2, 3)));
            return ParsedSuccess;
        }

        private string GetTrxnTime(string respSuccess)
        {
            string ParsedSuccess = "";
            ParsedSuccess = respSuccess.Substring((respSuccess.IndexOf("TI") + 5 + 10 + 1), 8);// Convert.ToInt32(respSuccess.Substring(respSuccess.IndexOf("TI") + 2, 3)));
            return ParsedSuccess;
        }

        public string GetBankName(string respSuccess)
        {
            string PAN = respSuccess.Substring((respSuccess.IndexOf("PN") + 5), Convert.ToInt32(respSuccess.Substring(respSuccess.IndexOf("PN") + 2, 3)));
            string BankName = "";
            if (PAN.Substring(0, 6) == "621986")
                return ("بانک سامان");
            if (PAN.Substring(0, 6) == "622106")
                return ("بانک پارسیان");
            if (PAN.Substring(0, 6) == "603799")
                return ("بانک ملی");
            if (PAN.Substring(0, 6) == "589210")
                return ("بانک سپه");
            if (PAN.Substring(0, 6) == "627648")
                return ("بانک توسعه صادرات");
            if (PAN.Substring(0, 6) == "628023")
                return ("بانک مسکن");
            if (PAN.Substring(0, 6) == "603770")
                return ("بانک کشاورزی");
            if (PAN.Substring(0, 6) == "627961")
                return ("بانک صنعت و معدن");
            if (PAN.Substring(0, 6) == "627760")
                return (" پست بانک");
            if (PAN.Substring(0, 6) == "502908")
                return ("توسعه تعاون");
            if (PAN.Substring(0, 6) == "627412")
                return ("اقتصاد نوین");
            if (PAN.Substring(0, 6) == "639347")
                return ("بانک پاسارگاد");
            if (PAN.Substring(0, 6) == "627488")
                return ("بانک کارآفرین");
            if (PAN.Substring(0, 6) == "639346")
                return ("بانک سینا");
            if (PAN.Substring(0, 6) == "639607")
                return ("بانک سرمایه");
            if (PAN.Substring(0, 6) == "502806")
                return ("بانک شهر");
            if (PAN.Substring(0, 6) == "603769")
                return ("بانک صادرات");
            if (PAN.Substring(0, 6) == "610433")
                return ("بانک ملت");
            if (PAN.Substring(0, 6) == "627353")
                return ("بانک تجارت");
            if (PAN.Substring(0, 6) == "589463")
                return ("بانک رفاه");
            if (PAN.Substring(0, 6) == "627381")
                return ("بانک انصار");
            if (PAN.Substring(0, 6) == "502938")
                return ("بانک دی");
            if (PAN.Substring(0, 6) == "505801")
                return ("بانک کوثر");

            return BankName;
        }

        private string GetBankBin(string respSuccess)
        {
            string PAN = respSuccess.Substring((respSuccess.IndexOf("PN") + 5), Convert.ToInt32(respSuccess.Substring(respSuccess.IndexOf("PN") + 2, 3)));
            string BankName = "";
            BankName = PAN.Substring(0, 6);
            return BankName;
        }
    }
}