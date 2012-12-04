using System;
using AlarmWorkflow.AlarmSource.Fax;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;

namespace AlarmWorkflow.Parser.ILSFFBParser
{
    /// <summary>
    /// Description of ILSStraubingParser.
    /// </summary>
    [Export("ILSStraubingParser", typeof(IFaxParser))]
    sealed class ILSStraubingParser : IFaxParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ILSStraubingParser class.
        /// </summary>
        public ILSStraubingParser()
        {
        }

        #endregion

        #region Methods

        private string GetMessageText(string line, string prefix)
        {
            if (prefix == null)
            {
                prefix = "";
            }

            if (prefix.Length > 0)
            {
                line = line.Remove(0, prefix.Length).Trim();
            }
            else
            {
                int colonIndex = line.IndexOf(':');
                if (colonIndex != -1)
                {
                    line = line.Remove(0, colonIndex + 1);
                }
            }

            if (line.StartsWith(":"))
            {
                line = line.Remove(0, 1).Trim();
            }

            return line;
        }

        #endregion

        #region IFaxParser Members


        Operation IFaxParser.Parse(string[] lines)
        {
            Operation operation = new Operation();

            try
            {

                //Definition der bool Variablen
                //bool nextIsOrt = false;
                bool ReplStreet = false;
                bool ReplCity = false;
                bool ReplComment = false;
                bool ReplPicture = false;
                bool Faxtime = false;
                bool nextIsOrt = false;
                //bool getEinsatzort = false;



                foreach (string line in lines)
                {

                    string msg;
                    string prefix;
                    int x = line.IndexOf(':');
                    if (x != -1)
                    {
                        prefix = line.Substring(0, x);
                        msg = line.Substring(x + 1).Trim();

                        prefix = prefix.Trim().ToUpperInvariant();
                        switch (prefix)
                        {

                            //F�llen der Standardinformatione Alarmfax Cases mit  ":"
                            case "EINSATZORT":
                                operation.Location = msg;
                                break;
                            case "STRA�E":
                            case "STRABE":
                                operation.Street = msg;
                                break;
                            case "OBJEKT":
                            case "9BJEKT":
                                operation.Property = msg;
                                break;
                            case "HINWEIS":
                                operation.Comment = msg;
                                break;
                            case "EINSATZPLAN":
                                operation.OperationPlan = msg;
                                break;
                        }
                    }

                    // Anzeige des Zeitpunkts des Faxeingangs
                    if (Faxtime == false)
                    {
                        DateTime uhrzeit = DateTime.Now;
                        operation.CustomData["Faxtime"] = "Faxeingang: " + uhrzeit.ToString("HH:mm:ss ");
                        Faxtime = true;
                    }

                    // Weitere Standardinfos auslesen
                    if (line.StartsWith("Einsatznummer"))
                    {
                        operation.OperationNumber = line.Substring(14);
                    }

                    if (line.StartsWith("Name"))
                    {
                        operation.Messenger = operation.Messenger + line.Substring(5);
                    }

                    operation.Messenger = operation.Messenger + " ";

                    if (operation.Messenger.Contains("Ausger�ckt") == true)
                    {
                        operation.Messenger = operation.Messenger.Replace(": Alarmiert : Ausger�ckt", "");
                        operation.Messenger = operation.Messenger.Trim();
                    }

                    if (line.StartsWith("Schlagw."))
                    {
                        operation.Picture = operation.Picture + line.Substring(11);
                    }

                    if (line.StartsWith("Stichw. B"))
                    {
                        operation.EmergencyKeyword = operation.EmergencyKeyword + line.Substring(10);
                        operation.EmergencyKeyword = operation.EmergencyKeyword.Trim();
                    }

                    if (line.StartsWith("Stichw. T"))
                    {
                        operation.EmergencyKeyword = operation.EmergencyKeyword + line.Substring(10);
                        operation.EmergencyKeyword = operation.EmergencyKeyword.Trim();
                    }

                    if (line.StartsWith("Stichw. S"))
                    {
                        operation.EmergencyKeyword = operation.EmergencyKeyword + line.Substring(10);
                        operation.EmergencyKeyword = operation.EmergencyKeyword.Trim();
                    }

                    if (line.StartsWith("Stichw. I"))
                    {
                        operation.EmergencyKeyword = operation.EmergencyKeyword + line.Substring(10);
                        operation.EmergencyKeyword = operation.EmergencyKeyword.Trim();
                    }

                    if (line.StartsWith("Stichw. R"))
                    {
                        operation.EmergencyKeyword = operation.EmergencyKeyword + line.Substring(10);
                        operation.EmergencyKeyword = operation.EmergencyKeyword.Trim();
                    }                   

                    //Ort Einlesen
                    if ((line.StartsWith("Ort")) && (nextIsOrt == false))
                    {
                        operation.City = operation.City + line.Substring(4);
                        nextIsOrt = true;
                    }

                    // Sonderzeichenersetzung im Meldebild

                    if (ReplPicture == false)
                    {
                        operation.Picture = operation.Picture + " ";
                        ReplPicture = true;
                    }

                    if (operation.Picture.Contains("�") == true)
                    {
                        operation.Picture = operation.Picture.Replace("�", "ss");
                    }

                    if (operation.Picture.Contains("�") == true)
                    {
                        operation.Picture = operation.Picture.Replace("�", "ae");
                    }

                    if (operation.Picture.Contains("�") == true)
                    {
                        operation.Picture = operation.Picture.Replace("�", "oe");
                    }

                    if (operation.Picture.Contains("�") == true)
                    {
                        operation.Picture = operation.Picture.Replace("�", "ue");
                    }

                    // Sonderzeichenersetzung im Ort

                    if (ReplCity == false)
                    {
                        operation.City = operation.City + " ";
                        ReplCity = true;
                    }

                    if (operation.City.Contains("�") == true)
                    {
                        operation.City = operation.City.Replace("�", "ss");
                    }

                    if (operation.City.Contains("�") == true)
                    {
                        operation.City = operation.City.Replace("�", "ae");
                    }

                    if (operation.City.Contains("�") == true)
                    {
                        operation.City = operation.City.Replace("�", "oe");
                    }

                    if (operation.City.Contains("�") == true)
                    {
                        operation.City = operation.City.Replace("�", "ue");
                    }

                    // Sonderzeichenersetzung in der Strasse

                    if (ReplStreet == false)
                    {
                        operation.Street = operation.Street + " ";
                        ReplStreet = true;
                    }

                    if (operation.Street.Contains("Haus-Nr.:") == true)
                    {
                        operation.Street = operation.Street.Replace("Haus-Nr.:", "");
                    }

                    if (operation.Street.Contains("�") == true)
                    {
                        operation.Street = operation.Street.Replace("�", "ss");
                    }

                    if (operation.Street.Contains("�") == true)
                    {
                        operation.Street = operation.Street.Replace("�", "ae");
                    }

                    if (operation.Street.Contains("�") == true)
                    {
                        operation.Street = operation.Street.Replace("�", "oe");
                    }

                    if (operation.Street.Contains("�") == true)
                    {
                        operation.Street = operation.Street.Replace("�", "ue");
                    }

                    // Sonderzeichenersetzung im Hinweis

                    if (ReplComment == false)
                    {
                        operation.Comment = operation.Comment + " ";
                        ReplComment = true;
                    }

                    if (operation.Comment.Contains("�") == true)
                    {
                        operation.Comment = operation.Comment.Replace("�", "ss");
                    }

                    if (operation.Comment.Contains("�") == true)
                    {
                        operation.Comment = operation.Comment.Replace("�", "ae");
                    }

                    if (operation.Comment.Contains("�") == true)
                    {
                        operation.Comment = operation.Comment.Replace("�", "oe");
                    }

                    if (operation.Comment.Contains("�") == true)
                    {
                        operation.Comment = operation.Comment.Replace("�", "ue");
                    }


                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(this, ex);
            }

            return operation;
        }

        #endregion

    }
}
