using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Change_Town_Names_Casing
{
    public class Program
    {
        static void Main(string[] args)
        {
            string connection = @"Server = .; Database = MinionsDB; Trusted_Connection=True";

            using SqlConnection sqlConnection = new SqlConnection(connection);
            sqlConnection.Open();
            string countryInput = Console.ReadLine();
            string townsToUpperQuery = @"UPDATE Towns
                                    SET Name = UPPER(Name)
                                    WHERE CountryCode = 
                                    (SELECT c.Id FROM Countries AS c WHERE c.Name =                         @countryName)";

            

            using SqlCommand upTownsCommand = new SqlCommand(townsToUpperQuery, sqlConnection);
            upTownsCommand.Parameters.AddWithValue("@countryName", countryInput);
            int countOfTowns = upTownsCommand.ExecuteNonQuery();

            if (countOfTowns == 0)
            {
                Console.WriteLine("No town names were affected.");
            }
            else
            {
                Console.WriteLine($"{countOfTowns} town names were affected.");

                string changedTownsQuery = @"SELECT t.Name 
                                        FROM Towns as t
                                        JOIN Countries AS c ON c.Id = t.CountryCode
                                        WHERE c.Name = @countryName";
                using SqlCommand changedTownsCommand = new SqlCommand(changedTownsQuery, sqlConnection);
                changedTownsCommand.Parameters.AddWithValue("@countryName", countryInput);
                using SqlDataReader reader = changedTownsCommand.ExecuteReader();

                List<string> towns = new List<string>();

                while (reader.Read())
                {
                    towns.Add((string)reader[0]);
                }
                Console.WriteLine($"[{string.Join(", ", towns)}]");
            }
            
        }
    }
}
