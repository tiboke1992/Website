using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Online_BoekenApp
{
    class DatabaseFiller : IDatabaseFiller
    {
        private SqlConnection connectie;
        private SqlCommand command;

        public DatabaseFiller() {
            //Source: http://weblogs.asp.net/owscott/archive/2005/08/26/Using-connection-strings-from-web.config-in-ASP.NET-v2.0.aspx
            connectie = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["BOEKENConnectionString"].ConnectionString);
        }

        public bool fillUitgevers(string padNaarUitgeversFile)
        {
            bool success = false;
            string commando = "INSERT INTO Uitgever VALUES ";
            bool eerste = true;
            try
            {
                StreamReader r = new StreamReader(padNaarUitgeversFile);
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    //Read the file...
                    string[] array = line.Split('\t');
                    for (int i = 0; i < array.Length; i++) {
                        array[i] = array[i].Trim();
                    }
                    if (eerste)
                    {
                        commando += "('" + array[0].Replace("'", "''").ToUpper() + "', '" + array[1].Replace("'", "''") + "')"; eerste = false;
                    }
                    else {
                        commando += ", ('" + array[0].Replace("'", "''").ToUpper() + "', '" + array[1].Replace("'", "''") + "')";
                    }
                }
                r.Close();
                log(commando);
                connectie.Open();
                command = new SqlCommand(commando, connectie);
                command.ExecuteNonQuery();
                connectie.Close();
                success = true;
            }
            catch (Exception e) {
                log(e.Message);
                success = false;
                connectie.Close();
            }
            return success;
        }

        public void log(string text) {
            TextWriter w = new StreamWriter(@"C:\Users\Adriaan\Documents\Visual Studio 2010\Projects\Online_BoekenApp\Online_BoekenApp\bin\log.txt", true);
            w.WriteLine(text);
            w.Close();
        }

        public bool fillCategorieen(string padNaarCategorieenFile)
        {
            bool success = false;
            string commando = "INSERT INTO Categorie VALUES ";
            bool eerste = true;
            try
            {
                StreamReader r = new StreamReader(padNaarCategorieenFile);
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    //Read the file...
                    string[] array = line.Split('\t');
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i] = array[i].Trim();
                    }
                    if (eerste)
                    {
                        commando += "('" + array[0].Replace("'", "''".ToUpper()) + "', '" + array[1].Replace("'", "''") + "')"; eerste = false;
                    }
                    else
                    {
                        commando += ", ('" + array[0].Replace("'", "''").ToUpper() + "', '" + array[1].Replace("'", "''") + "')";
                    }
                }
                r.Close();
                log(commando);
                connectie.Open();
                command = new SqlCommand(commando, connectie);
                command.ExecuteNonQuery();
                connectie.Close();
                success = true;
            }
            catch (Exception e)
            {
                log(e.Message);
                success = false;
                connectie.Close();
            }
            return success;
        }

        public bool fillBoeken(string padNaarBoekenFile)
        {
            bool success = false;
            //Empty the table
            //Source: http://msdn.microsoft.com/en-us/library/aa260621(SQL.80).aspx
            /*command = new SqlCommand("TRUNCATE TABLE Boek", connectie);
            connectie.Open();
            command.ExecuteNonQuery();
            connectie.Close();*/
            
            //Fill Table
            //Source linecount: http://stackoverflow.com/questions/119559/determine-the-number-of-lines-within-a-text-file
            string[] commandos = new string[File.ReadLines(padNaarBoekenFile).Count()];
            log("Aantal lijntjes: " + File.ReadLines(padNaarBoekenFile).Count());
            int j = 0; //j = linecount
            try
            {
                StreamReader r = new StreamReader(padNaarBoekenFile);
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    //Read the file...
                    string[] array = line.Split('\t');
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i] = array[i].Trim();
                    }
                    if (array.Length == 6 && checkData(array))
                    {
                        commandos[j++] = "INSERT INTO Boek VALUES (" + j.ToString() + ", '" + array[1].Replace("'", "''") + "', '" + array[0].Replace("'", "''") + "', " + array[3].Replace(",", ".") + ", '" + array[2].Replace("'", "''").ToUpper() + "', '" + array[5].Replace("'", "''").ToUpper() + "')";
                    }
                }
                r.Close();
            }
            catch (Exception e)
            {
                log(e.Message);
                success = false;
            }
            connectie.Open();
            foreach (string s in commandos) {
                if (s != null && s.Length > 0) { //We willen geen lege string uitvoeren als query
                    log(s);
                    try
                    {
                        command = new SqlCommand(s, connectie);
                        command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        log(e.Message + "\r\n");
                    } //And continue with the other queries, like nothing happened.
                }                      
            }
            connectie.Close();
            success = true;
            
            return success;
        }

        private bool checkData(string[] array)
        {
            bool validData = true;
            //array[0]: Titel moet langer zijn dan 0 karakters
            //array[1]: ISBN = Maximaal 13 chars en minimaal 1
            //array[2]: Categorie, langer dan 0 chars
            if (array[0].Length < 1 || array[1].Length < 1 || array[1].Length >= 14 || array[2].Length < 1 || array[3].Length < 1 || array[5].Length < 1) {
                validData = false;
            } 
            return validData;
        }

        public bool fillStatusen(string padNaarStatusenFile)
        {
            bool success = false;
            try {
                command = new SqlCommand("insert into Status values (1, 'nog te starten'), (2, 'in constructie'), (3, 'afgewerkt')", connectie);
                connectie.Open();
                command.ExecuteNonQuery();
                connectie.Close();
                success = true;
            } catch(Exception e)
            {
                log(e.Message);
                success = false;
            }
            return success;            
        }
    }
}
