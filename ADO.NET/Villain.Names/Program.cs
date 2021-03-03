using Microsoft.Data.SqlClient;
using System;

namespace Villain.Names
{
    class Program
    {
        static void Main(string[] args)
        {
            string connection = @"Server = .; Database = MinionsDB; Trusted_Connection=True";

            using SqlConnection sqlConnection = new SqlConnection(connection);
            sqlConnection.Open();

            string vilianNamesText = @"SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount  
                                        FROM Villains AS v 
                                        JOIN MinionsVillains AS mv ON v.Id = mv.VillainId 
                                        GROUP BY v.Id, v.Name 
                                        HAVING COUNT(mv.VillainId) > 3 
                                        ORDER BY COUNT(mv.VillainId)";

            using SqlCommand villiansQuery = new SqlCommand(vilianNamesText, sqlConnection);
            using SqlDataReader reader = villiansQuery.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"{reader[0]} - {reader[1]}");
            }
        }
    }
}
