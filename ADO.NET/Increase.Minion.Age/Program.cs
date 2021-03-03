using Microsoft.Data.SqlClient;
using System;
using System.Linq;

namespace Increase.Minion.Age
{
    class Program
    {
        static void Main(string[] args)
        {
            string connection = @"Server = .; Database = MinionsDB; Trusted_Connection=True";

            int[] id = Console.ReadLine().Split().Select(int.Parse).ToArray();
            string range = $"{string.Join(", ", id)}";

            using SqlConnection sqlConnection = new SqlConnection(connection);
            sqlConnection.Open();

            foreach (var item in id)
            {
                string updateAgeAndNameQuery = @"UPDATE Minions
                            SET Name = UPPER(LEFT(Name, 1)) + SUBSTRING(Name, 2, LEN(Name)), 
                                Age += 1
                                WHERE Id = @Id";
                using SqlCommand updateQuery = new SqlCommand(updateAgeAndNameQuery, sqlConnection);
                updateQuery.Parameters.AddWithValue("@Id", item);
                updateQuery.ExecuteNonQuery();
            }

            string listAllMimionsQuery = @"SELECT Name, Age FROM Minions";
            using SqlCommand listMinionsCommand = new SqlCommand(listAllMimionsQuery, sqlConnection);
            using SqlDataReader reader = listMinionsCommand.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"{reader[0] } { reader[1]}");
            }
        }
    }
}
