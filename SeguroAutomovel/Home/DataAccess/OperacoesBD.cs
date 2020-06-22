using Newtonsoft.Json;
using SeguroAutomovel.Home.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace SeguroAutomovel.Home.DataAccess
{
    public class OperacoesBD
    {
        public static string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["BDStrConn"].ConnectionString;

        public static double MARGEM_SEGURANCA = 0.03;

        public static double LUCRO = 0.05;

        public string baseUrl = "http://localhost:3000";  // Mock JsonServer


        public Proponente buscarProponente(string cpfSegurado)
        {
            // Mock Json Server - Proponentes
            using (var client = new HttpClient())
            {

                HttpResponseMessage response = client.GetAsync(baseUrl + string.Format("/proponentes?cpf={0}", cpfSegurado)).Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(" - Erro: Não foi possível recuperar os proponentes do serviço.");
                }

                string resultJSON = response.Content.ReadAsStringAsync().Result;

                List<Proponente> listaProponentes = JsonConvert.DeserializeObject<Proponente[]>(resultJSON).ToList();

                return listaProponentes.FirstOrDefault(x => x.Cpf == cpfSegurado);
            }
        }


        public Apolice calcularSeguroAuto(string cpfSegurado)
        {
            try
            {
                Proponente proponente = buscarProponente(cpfSegurado);

                if (proponente == null)
                {
                    throw new Exception(" - CPF não encontrado na base");
                }

                decimal valorVeiculo = proponente.ValorVeiculo;

                decimal taxaRisco = (valorVeiculo * 5) / (2 * valorVeiculo);

                decimal premioRisco = taxaRisco / 100 * valorVeiculo;

                decimal premioPuro = premioRisco * (decimal)(1 + MARGEM_SEGURANCA);

                decimal premioComl = Math.Round((decimal)(1 + LUCRO) * premioPuro, 2);

                decimal valorSeguro = premioComl;

                Apolice apolice = new Apolice()
                {
                    Id = 0,
                    Nome = proponente.Nome,
                    Cpf = proponente.Cpf,
                    DataNascimento = proponente.DataNascimento,
                    DataSeguro = DateTime.Today,
                    InicioVigencia = DateTime.Today.AddDays(7),
                    FinalVigencia = DateTime.Today.AddDays(7 + 365),
                    CepPernoite = proponente.CepPernoite,
                    Placa = proponente.Placa,
                    Modelo = proponente.Modelo,
                    ValorIS = proponente.ValorVeiculo,
                    PremioTotal = valorSeguro,
                    Status = "A"
                };

                return apolice;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - Erro método calcularSeguroAuto");
            }
        }

        public Relatorio RelatorioMediasSeguroAuto(DateTime dataInicio, DateTime dataFinal)
        {
            try
            {
                Relatorio relatorio = new Relatorio();

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "select count(*) as 'QtdRegistros', AVG(ValorIS) as 'MediaValorIS', AVG(PremioTotal) as 'MediaPremioTotal'" +
                            "from Apolice where DataSeguro between @DataInicio and @DataFinal";
                        command.Parameters.AddWithValue("@DataInicio", dataInicio);
                        command.Parameters.AddWithValue("@DataFinal", dataFinal);

                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            relatorio.QtdRegistros = Convert.ToInt32(reader["QtdRegistros"]);

                            if (relatorio.QtdRegistros > 0)
                            {
                                relatorio.Descricao = "RELATÓRIO DE MÉDIAS DOS SEGUROS DE AUTOMÓVEL";
                                relatorio.DataHoraExtracao = DateTime.Now;
                                relatorio.DataInicio = dataInicio;
                                relatorio.DataFinal = dataFinal;
                                relatorio.MediaValorIS = Math.Round(Convert.ToDecimal(reader["MediaValorIS"]), 2);
                                relatorio.MediaPremioTotal = Math.Round(Convert.ToDecimal(reader["MediaPremioTotal"]), 2);

                            }
                            else
                            {
                                throw new Exception(" - Relatório não encontrado para o período informado.");
                            }
                        }
                        else
                        {
                            throw new Exception(" - Relatório não encontrado para o período informado.");
                        }
                    }

                    connection.Close();
                }

                return relatorio;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - Erro método RelatorioMediasSeguroAuto");
            }
        }

        public Apolice PesquisarSeguroAuto(int id)
        {
            try
            {
                Apolice apolice = new Apolice();

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "Select Id, Nome, Cpf, DataNascimento, DataSeguro, InicioVigencia, FinalVigencia, CepPernoite, Placa, Modelo, ValorIS, PremioTotal, Status " +
                                              " From Apolice Where Id = @Id";
                        command.Parameters.AddWithValue("@Id", id);

                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            apolice.Id = Convert.ToInt32(reader["Id"]);
                            apolice.Nome = reader["Nome"].ToString();
                            apolice.Cpf = reader["Cpf"].ToString();
                            apolice.DataNascimento = Convert.ToDateTime(reader["DataNascimento"]);
                            apolice.DataSeguro = Convert.ToDateTime(reader["DataSeguro"]);
                            apolice.InicioVigencia = Convert.ToDateTime(reader["InicioVigencia"]);
                            apolice.FinalVigencia = Convert.ToDateTime(reader["FinalVigencia"]);
                            apolice.CepPernoite = reader["CepPernoite"].ToString();
                            apolice.Placa = reader["Placa"].ToString();
                            apolice.Modelo = reader["Modelo"].ToString();
                            apolice.ValorIS = Convert.ToDecimal(reader["ValorIS"]);
                            apolice.PremioTotal = Convert.ToDecimal(reader["PremioTotal"]);
                            apolice.Status = reader["Status"].ToString();
                        }
                        else
                        {
                            throw new Exception(" - Seguro não encontrado para o Id informado ");
                        }
                    }

                    connection.Close();
                }

                return apolice;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - Erro método PesquisarSeguroAuto");
            }
        }

        public List<Apolice> PesquisarSeguroAuto(DateTime dataInicio, DateTime dataFinal)
        {
            try
            {
                List<Apolice> listApolices = new List<Apolice>();

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "Select Id, Nome, Cpf, DataNascimento, DataSeguro, InicioVigencia, FinalVigencia, CepPernoite, Placa, Modelo, ValorIS, PremioTotal, Status " +
                                              " from Apolice where DataSeguro between @DataInicio and @DataFinal Order by Id";
                        command.Parameters.AddWithValue("@DataInicio", dataInicio);
                        command.Parameters.AddWithValue("@DataFinal", dataFinal);

                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            Apolice apolice = new Apolice()
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nome = reader["Nome"].ToString(),
                                Cpf = reader["Cpf"].ToString(),
                                DataNascimento = Convert.ToDateTime(reader["DataNascimento"]),
                                DataSeguro = Convert.ToDateTime(reader["DataSeguro"]),
                                InicioVigencia = Convert.ToDateTime(reader["InicioVigencia"]),
                                FinalVigencia = Convert.ToDateTime(reader["FinalVigencia"]),
                                CepPernoite = reader["CepPernoite"].ToString(),
                                Placa = reader["Placa"].ToString(),
                                Modelo = reader["Modelo"].ToString(),
                                ValorIS = Convert.ToDecimal(reader["ValorIS"]),
                                PremioTotal = Convert.ToDecimal(reader["PremioTotal"]),
                                Status = reader["Status"].ToString()
                            };

                            listApolices.Add(apolice);
                        }
                    }

                    connection.Close();
                }

                if (listApolices.Count() > 0)
                    return listApolices;

                throw new Exception(" - Seguros não encontrados para o período informado ");

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - Erro método PesquisarSeguroAuto");
            }
        }

        public List<Apolice> PesquisarSeguroAuto(string cpf)
        {
            try
            {
                List<Apolice> listApolices = new List<Apolice>();

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "Select Id, Nome, Cpf, DataNascimento, DataSeguro, InicioVigencia, FinalVigencia, CepPernoite, Placa, Modelo, ValorIS, PremioTotal, Status " +
                                              " from Apolice Where Cpf = @Cpf Order by Id";
                        command.Parameters.AddWithValue("@Cpf", cpf);

                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            Apolice apolice = new Apolice()
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nome = reader["Nome"].ToString(),
                                Cpf = reader["Cpf"].ToString(),
                                DataNascimento = Convert.ToDateTime(reader["DataNascimento"]),
                                DataSeguro = Convert.ToDateTime(reader["DataSeguro"]),
                                InicioVigencia = Convert.ToDateTime(reader["InicioVigencia"]),
                                FinalVigencia = Convert.ToDateTime(reader["FinalVigencia"]),
                                CepPernoite = reader["CepPernoite"].ToString(),
                                Placa = reader["Placa"].ToString(),
                                Modelo = reader["Modelo"].ToString(),
                                ValorIS = Convert.ToDecimal(reader["ValorIS"]),
                                PremioTotal = Convert.ToDecimal(reader["PremioTotal"]),
                                Status = reader["Status"].ToString()
                            };

                            listApolices.Add(apolice);
                        }
                    }

                    connection.Close();
                }

                if (listApolices.Count() > 0)
                    return listApolices;

                throw new Exception(" - Seguros não encontrados para o cpf informado ");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - Erro método PesquisarSeguroAuto");
            }
        }

        public Apolice gravarSeguroAuto(Apolice apolice)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "Insert Into Apolice(Nome, Cpf, DataNascimento, DataSeguro, InicioVigencia, FinalVigencia, CepPernoite, Placa, Modelo, ValorIS, PremioTotal, Status) " +
                                                          "Values (@Nome, @Cpf, @DataNascimento, @DataSeguro, @InicioVigencia, @FinalVigencia, @CepPernoite, @Placa, @Modelo, @ValorIS, @PremioTotal, @Status) ";

                        command.Parameters.AddWithValue("Nome", apolice.Nome);
                        command.Parameters.AddWithValue("Cpf", apolice.Cpf);
                        command.Parameters.AddWithValue("DataNascimento", apolice.DataNascimento);
                        command.Parameters.AddWithValue("DataSeguro", apolice.DataSeguro);
                        command.Parameters.AddWithValue("InicioVigencia", apolice.InicioVigencia);
                        command.Parameters.AddWithValue("FinalVigencia", apolice.FinalVigencia);
                        command.Parameters.AddWithValue("CepPernoite", apolice.CepPernoite);
                        command.Parameters.AddWithValue("Placa", apolice.Placa);
                        command.Parameters.AddWithValue("Modelo", apolice.Modelo);
                        command.Parameters.AddWithValue("ValorIS", apolice.ValorIS);
                        command.Parameters.AddWithValue("PremioTotal", apolice.PremioTotal);
                        command.Parameters.AddWithValue("Status", apolice.Status);

                        if (command.ExecuteNonQuery() > 0)
                        {
                            command.Connection = connection;
                            command.CommandText = "Select @@IDENTITY AS 'Identity'";
                            command.Parameters.Clear();

                            SqlDataReader reader = command.ExecuteReader();

                            if (reader.Read())
                            {
                                apolice.Id = reader["Identity"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Identity"]);
                            }
                            else
                            {
                                throw new Exception(" - Erro método gravarSeguroAuto - Select ");
                            }
                        }
                        else
                        {
                            throw new Exception(" - Erro método gravarSeguroAuto - Insert ");
                        }
                    }

                    connection.Close();
                }

                return apolice;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - Erro método gravarSeguroAuto");
            }
        }
    }
}