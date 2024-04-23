﻿using System.Net.Http.Json;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using Microsoft.Data.SqlClient;

namespace myJsonClinet
{
    internal class Program
    {
        static async Task Main(string[] args)
        {





            //string userApi = "https://jsonplaceholder.typicode.com/users/";
            //HttpClient client = new HttpClient();

            //var result = await client.GetAsync(userApi);
            //var content = await result.Content.ReadAsStringAsync();

            //HttpRequestMessage htprm = new HttpRequestMessage(HttpMethod.Head,userApi);
            //var resulthead = await client.SendAsync(htprm);

            //Console.WriteLine("Result from Json api :"+ resulthead.IsSuccessStatusCode);
            //Console.WriteLine("Result from Json api :"+result.StatusCode);
            //Console.WriteLine(content);



            // Create an instance of HttpClient
            using (var client = new HttpClient())
            {
                //set url address 
                client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/users/");

                // Create an HttpRequestMessage instance
                var request = new HttpRequestMessage(HttpMethod.Get, client.BaseAddress);

                // Add headers to HttpRequestMessage if needed
                request.Headers.Add("Accept", "application/json");

                try
                {
                    // Send the request asynchronously
                    using (HttpResponseMessage response = await client.SendAsync(request))
                    {
                        // Ensure we got a successful response
                        if (!response.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"Error: {response.StatusCode}");
                            return;
                        }

                        // Read the response content and process it
                        string content = await response.Content.ReadAsStringAsync();

                        //Console.WriteLine(content);

                        List<userData>? ud = await response.Content.ReadFromJsonAsync<List<userData>>();

                        if (ud != null)
                        {
                            //foreach (var item in ud) { Console.WriteLine(value: $"Json:{item.username},{item.phone},{item.company.name},{item.address.city},{item.address.zipcode}"); }
                            try
                            {
                                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

                                builder.DataSource = "wd1";
                                builder.UserID = "sa";
                                builder.Password = "monopoly0987";
                                builder.InitialCatalog = "apidb";
                                builder.Encrypt = false;

                                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                                {
                                    Console.WriteLine("\nQuery data example:");
                                    Console.WriteLine("=========================================\n");


                                    connection.Open();
                                    String sqlquery = @"IF NOT EXISTS (Select * from [dbo].[users] where [id] = @id) 
                                                        BEGIN INSERT INTO [dbo].[users]
                                                                                ([id]
                                                                                ,[username]
                                                                                ,[phone]
                                                                                ,[companyname]
                                                                                ,[addresscity]
                                                                                ,[addresszipcode])
                                                                                values (@id, @username, @phone, @companyname, @addresscity, @addresszipcode) END 
                                                        ELSE BEGIN UPDATE [dbo].[users] SET [id]=@id, [username]=@username, [phone]=@phone, [companyname]=@companyname,[addresscity]=@addresscity,[addresszipcode]=@addresszipcode WHERE [id]=@id END";

                                    foreach (var item in ud)
                                    {
                                        using (SqlCommand command = new SqlCommand(sqlquery, connection))
                                        {
                                            command.Parameters.AddWithValue("@id", item.id);
                                            command.Parameters.AddWithValue("@username", item.username);
                                            command.Parameters.AddWithValue("@phone", item.phone);
                                            command.Parameters.AddWithValue("@companyname", item.company.name);
                                            command.Parameters.AddWithValue("@addresscity", item.address.city);
                                            command.Parameters.AddWithValue("@addresszipcode", item.address.zipcode);
                                                                                       
                                            command.ExecuteNonQuery();
                                        }
                                        
                                    }

                                }
                            }
                            catch (SqlException e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                            Console.WriteLine("\nDone. Press enter.");
                            Console.ReadLine();


                        }

                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request exception: {e.Message}");
                }
            }


            Console.WriteLine("finish");


            
        }
    }
}

