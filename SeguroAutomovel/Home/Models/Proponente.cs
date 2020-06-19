using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SeguroAutomovel.Home.Models
{
    public class Proponente
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public DateTime DataNascimento { get; set; }
        public string CepPernoite { get; set; }
        public string Placa { get; set; }
        public string Modelo { get; set; }
        public decimal ValorVeiculo { get; set; }
    }

    public class Documento
    {
        public string Cpf { get; set; }
        public string Rg { get; set; }
        public string Habilitacao { get; set; }
    }
}